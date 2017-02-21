/*
 *Code from:  https://github.com/dougbrion/kinect-sdk
 * 
 */
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.IO.Ports;
//Allows for Sleep()
using System.Threading;
//Allows for OSC
using Ventuz.OSC;

namespace WPFKinectSDK18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var sensorStatus = new KinectSensorChooser();

            sensorStatus.KinectChanged += KinectSensorChooserKinectChanged;

            kinectChooser.KinectSensorChooser = sensorStatus;
            sensorStatus.Start();

        }

        private void KinectSensorChooserKinectChanged(object sender, KinectChangedEventArgs e)
        {
            
            if (sensor != null)
                sensor.SkeletonFrameReady -= KinectSkeletonFrameReady;

            sensor = e.NewSensor;

            if (sensor == null)
                return;

            switch (Convert.ToString(e.NewSensor.Status))
            {
                case "Connected": KinectStatus.Content = "Connected";
                    break;
                case "Disconnected": KinectStatus.Content = "Disconnected";
                    break;
                case "Error": KinectStatus.Content = "Error";
                    break;
                case "NotReady": KinectStatus.Content = "Not Ready";
                    break;
                case "NotPowered": KinectStatus.Content = "Not Powered";
                    break;
                case "Initializing": KinectStatus.Content = "Initialising";
                    break;
                default: KinectStatus.Content = "Undefined";
                    break;
            }

            sensor.SkeletonStream.Enable(); 
            sensor.SkeletonFrameReady += KinectSkeletonFrameReady;

        }


        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var skeletons = new Skeleton[0];

            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length == 0)
            {
                return;
            }

            var skel = skeletons.FirstOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);
            if (skel == null)
            {
                return;
            }

            var rightHand = skel.Joints[JointType.WristRight];
            XValueRight.Text = rightHand.Position.X.ToString(CultureInfo.InvariantCulture);
            YValueRight.Text = rightHand.Position.Y.ToString(CultureInfo.InvariantCulture);
            ZValueRight.Text = rightHand.Position.Z.ToString(CultureInfo.InvariantCulture);

            var leftHand = skel.Joints[JointType.WristLeft];
            XValueLeft.Text = leftHand.Position.X.ToString(CultureInfo.InvariantCulture);
            YValueLeft.Text = leftHand.Position.Y.ToString(CultureInfo.InvariantCulture);
            ZValueLeft.Text = leftHand.Position.Z.ToString(CultureInfo.InvariantCulture);

            var centreHip = skel.Joints[JointType.HipCenter];

            /*
             *Below this is where the code edit was made.
             * *******************
            */
            //creates the 
            var manager = new OscManager();

            //Sets up the IP address and Port to which the bundle is sending to. 
            //Edit DestIP to the laptop your communicating with.
            manager.DestIP= "192.168.0.26";
            manager.DestPort = 12000;

            //Creates the message and bundle. This is what will send the data.
            OscElement message;
            OscBundle bundle;


            if (leftHand.Position.Y > .2)
            {
                //Lower octave
                message = new OscElement("/octave", 0);
                bundle = new OscBundle();
                bundle.AddElement(message);
                manager.Send(bundle);
            }
            else if ((leftHand.Position.Y > .2) && (leftHand.Position.Y < .45))
            {
                //Mid octave
                message = new OscElement("/octave", 1);
                bundle = new OscBundle();
                bundle.AddElement(message);
                manager.Send(bundle);
            }
            else if (leftHand.Position.Y > .50)
            {
                //High octave
                message = new OscElement("/octave", 2);
                bundle = new OscBundle();
                bundle.AddElement(message);
                manager.Send(bundle);
            }
            /********
             * This ends where the code was edited
             */

            if (rightHand.Position.Y > 0.3)
            {
                RightRaised.Text = "Raised";
            }
            else if (leftHand.Position.Y > 0.3)
            {
                LeftRaised.Text = "Raised";
            }
            else
            {
                LeftRaised.Text = "Lowered";
                RightRaised.Text = "Lowered";
            }

            //Pauses system so not too many signals are being sent.
            Thread.Sleep(10);
        }
    }
}