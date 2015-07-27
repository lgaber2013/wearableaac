//works! idk about the spontaneous restarts tho. needs investigation so it can stop doing that... really only started happening when I used the delays

/* Modified by lauren gaber
 *  This sketch sends data via HTTP GET requests to http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=add&id= (then the mac address of this board)
 *  which adds a json format table entry to the page which you can see by going to the url: http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=read 
 *
 *  You need to eventually change it so it can connect with any wifi network!!!!!
 *
 */

#include <ESP8266WiFi.h>

//#define ESP8266_LED 5

unsigned long time1 = 0;
unsigned long time2 = 0;

const char* ssid     = "Sikuli";//this will need to change eventually... I'm sad, how do we get these???
const char* password = "labsikuli"; 

const int btnPin = 2;//13;  //button is attached to pin 13, and is wired up based on example 5 from arduino uno kit
bool wasPressed = false;

String urlEnding = "/~shaunkane/buttons/buttons.php?action=add&id=";

//http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=add&id=button1 I'm imaginingg the ID = macaddr_0 to infinity... 

const char* host = "www.cs.colorado.edu";
byte mac[6];

void MACK()
{
  WiFi.macAddress(mac);
          Serial.print("MAC: ");
          Serial.print(mac[5],HEX);
          Serial.print(":");
          Serial.print(mac[4],HEX);
          Serial.print(":");
          Serial.print(mac[3],HEX);
          Serial.print(":");
          Serial.print(mac[2],HEX);
          Serial.print(":");
          Serial.print(mac[1],HEX);
          Serial.print(":");
          Serial.println(mac[0],HEX);
}

void addMacString()
{
  Serial.println("Updating my url with the mac address of my board:");
  
  //only one button from the board.... so why don't it just record it eveery time. 
  //and then the number of times it ppears will be the translation key. yeahhh... I like it.
  for(int k = 5; k > -1; k--) //num of things in mac
  {
    urlEnding += String(mac[k],HEX);
  }
  Serial.println(urlEnding);
}



void setup() {
  //ESP.wdtDisable();
  Serial.begin(115200);
  delay(10);
  //pinMode(ESP8266_LED, OUTPUT);
  pinMode(btnPin, INPUT); //button pin is an input on pin 13
  
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(500);  //required & essential delay
    Serial.print(".");
  }
  
  MACK();
  addMacString();  //append this board's mac address to the urlending!
  Serial.println("");
  Serial.println("WiFi connected");  
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  
  
}

int value = 0;
        
void loop() {
  
  int btnState;
  btnState = digitalRead(btnPin);
  
  //Serial.println("we have made it past the digital read");
  
  if(btnState == LOW) //if button is pushed
  {
    if(wasPressed == false)//and it wasn't pushed before
    {
      
      time1 = millis();
      if(time1-time2 > 250)
      {
        wasPressed = true; //say that it hs been pushed, then do the big something once in the same code block
        Serial.println("the red button has been pressed");
        //START OF CONNECTING CODE
      
        ///////////////////////////////////////////////////////////////////////////////////////////////////delay(5000);
        ++value;
      
      
        //Serial.println("we have made it past the delay");
      
      
        Serial.print("connecting to ");
        Serial.println(host);
  
        // Use WiFiClient class to create TCP connections
        WiFiClient client;
        const int httpPort = 80;
        if (!client.connect(host, httpPort)) {
          Serial.println("connection failed");
          return;
        }
    
        // We now create a URI for the request
        String url = urlEnding; 
        Serial.print("Requesting URL: ");
        Serial.println(url);
  
        // This will send the request to the server
        client.print(String("GET ") + url + " HTTP/1.1\r\n" +
                 "Host: " + host + "\r\n" + 
                 "Connection: close\r\n\r\n");
        delay(10); //I'm pretty sure this is the reverb delay, and this is awefulll
  
        // Read all the lines of the reply from server and print them to Serial
        while(client.available()){
          String line = client.readStringUntil('\r');   //oh hey maybe I can get informationfrom here!
          Serial.print(line);
      }
  
      //Serial.println();
      Serial.println("closing connection");
  
      //END OF CONNECTING CODE 
      //Serial.println("going to yield");
      yield(); //hmmmmmm
      //Serial.println("finished yielding");
      }
      time2 = time1;
    }
    else if(wasPressed == true)//the button was down/pushed before now. my hope is that it doesn't do it a million times because it reading it so fast
    {
      return;
    }
  }
  else //once the button is no longer being pushed, set was pressed baack to false. this means it is done being pressed once.
  {
    wasPressed = false;
  }//end of low/high test
  
}//end of loop
