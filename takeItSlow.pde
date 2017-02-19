import oscP5.*;
import netP5.*;
  
OscP5 oscP5;
NetAddress blockyTalky;

int input=0;

// so that we can have the same interval for both received messages
boolean sent=false;

void setup() {
  oscP5 = new OscP5(this,12000);
  // set this to the IP of your BlockyTalky
  blockyTalky = new NetAddress("192.168.2.182",8675);
}

/* send an OSC message for every 50 wekinator inputs, so we don't overload the BlockyTalky.
   before we added this filter program it kept crashing the synth and we had to restart 
   the raspberry pi. which is not good, so let's slow it down.
   
   we also had trouble getting wekinator to communicate directly with BlockyTalky; there
   was lots of lag and it was a bit irregular. for some reason turning the OSC message into
   new OSC messages remedies this.
   
   the three individual messages is just because we didn't feel like dealing with lists
   within BlockyTalky. */
   
void oscEvent(OscMessage theOscMessage) {
  input++;
  if (input >= 40 && theOscMessage.checkAddrPattern("/wek/outputs")==true){
    OscMessage note = new OscMessage("/note");
    note.add(theOscMessage.get(0).floatValue());
    oscP5.send(note, blockyTalky);
    
    OscMessage occidental = new OscMessage("/occidental");
    occidental.add(theOscMessage.get(2).floatValue());
    oscP5.send(occidental, blockyTalky);
    
    sent = true;
    input = 0;
  }
  
  if (sent == true && theOscMessage.checkAddrPattern("/octave")==true){
    oscP5.send(theOscMessage, blockyTalky);
    sent = false;
  }
}