using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using TTSTest2.Models;
using System.IO;
using System.Runtime.Serialization.Json;
using TTSTest2.Data;

/*
 * Description:
 * 
 * This is the ListeningPage class. This serves as the page you listen for real-time use of button presses;
 * The application only listens for button pressees and plays corresponding functions while this page is open.
 * you can navigate to it from the starting page (the ButtonListPage)
 * 
 * */

namespace TTSTest2.Views
{
    class ListeningPage : ContentPage
    {
        Label state = new Label { };

        bool mytrigger;
        int unixTimestamp1;
        bool justSpoke = false;

        public ListeningPage()
            //constructor
        {
            Label header = new Label
            {
                Text = "Listening for button presses...",
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            

            ActivityIndicator activityIndicator = new ActivityIndicator
            {
                Color = Device.OnPlatform(Color.Black, Color.Default, Color.Default),
                IsRunning = true,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    state,
                    activityIndicator
                }
            };
        }

        

        protected override void OnAppearing()
            //post: does all the normal stuff that needs to be done on its appearence,
            //then starts listening forbutton presses and if the right trigger is pressed it producees audio.
            //major methods used are anyON(), Device.StartTimer and ListenModeInitiated().
        {
            base.OnAppearing();

            if (anyON() == false)
            {
                state.Text = "No Functions of registered buttons are on";
            }
            else
            {
                state.Text = "At least one function of a registered button is on; while this page is open, you are listening to the database";
                mytrigger = true;

                if (mytrigger == true)
                {
                    Debug.WriteLine("page is still open");
                    Device.StartTimer(new TimeSpan(0, 0, 2), () =>  //5 secis just too long...whay can't we build in a listenthat just says f any button is pressed once start listening for a command for the next 5 seconds instead of listening for a command for 5 seconds forever... ///not quite... also why does it fail after 3-5 goes? try.... 5 but it did work at 1...
                    {
                        ListenModeInitiated();
                        if (mytrigger == true)  //this is the switch, yes?? //quits looping onec nothing's on...
                        {
                            Debug.WriteLine("page is still open");
                            return true;
                        }
                        else //its true or false
                        {
                            DependencyService.Get<IDownloadString>().DownloadAsString("http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=clear");
                            Debug.WriteLine("page has been closed, and db has been cleared");
                            return false;
                        }
                    });
                }
            }
        }

        protected override void OnDisappearing()
            //post: does all the normal on disappearing work, then
            //halts the listening loop routine
        {
            base.OnDisappearing();
            mytrigger = false;
        }


        bool anyON()
            //post: returns true if any of the functions on the confirmed buttons are turned on, false otherwise.
        {
            ButtonItem[] mybuttons = App.bDatabase.GetArray();
            if (mybuttons != null)
            {
                for (int i = 0; i < mybuttons.Length; i++) //need this button's todo item list
                {//ref a todo item>??
                    if (mybuttons[i].confirmed == true) //checking if any of them equal true, or are turned on
                    {
                        TodoItem[] mybuttonstodos = App.tDatabase.GetArray(mybuttons[i].ButtonMac);//.bDatabase.GetArray();
                        if (mybuttonstodos[0] != null)
                        {
                            for (int k = 0; k < mybuttonstodos.Length; k++)
                            {
                                if (mybuttonstodos[k].Done == true)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        void ListenModeInitiated()
            //post: determines if a trigger for a turned on function was entered by the user,
            //and if it was it calls onIRLButtonClicked()
        {
            ButtonItem[] mybuttons = App.bDatabase.GetArray();
            WebDataItem[] myItems = getWebData();

            if (myItems.Length > 0 && myItems[0].rowid != 0) //a row id equivalent to zero is invalid and indicates a null or empty item
            {
                for (int i = 0; i < mybuttons.Length; i++) //need this button's todo item list
                {
                    if (mybuttons[i].confirmed == true) //checking if any of them are turned on
                    {
                        TodoItem[] mybuttonstodos = App.tDatabase.GetArray(mybuttons[i].ButtonMac);
                        for (int k = 0; k < mybuttonstodos.Length; k++)
                        {
                            int timespressed = enteringTrigger(mybuttons[i].ButtonMac, myItems);

                            if (timespressed != -1 && mybuttonstodos[k].Done == true && Convert.ToInt32(mybuttonstodos[k].Notes.Substring(0, 1)) == timespressed)
                            {
                                Debug.WriteLine("We spoke this round"); 
                                justSpoke = true; //we definitely know that it spoke
                                onIRLButtonClicked(mybuttonstodos[k]);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("We did not speak this round");
                justSpoke = false;
            }

        }




        public int enteringTrigger(string buttonMac, WebDataItem[] myItems)
            //pre: string buttonMac is an existing button's mac address, WebDataItem[] myItems is an array of web 
            //data collected from the internet.
            //post: returns the count of how many times the button with this mac address was pressed. 
            //zero times is an invalid answer so that returns the invalid key too.
        {
            int count = 0;
            for (int i = 0; i < myItems.Length; i++)
            {
                if (myItems[i].buttonID == buttonMac)
                {
                    count++;
                }
            }
            if (count == 0)
            {
                Debug.WriteLine("num times pressed: none");
                return -1;
            }
            else
            {
                Debug.WriteLine("num times pressed: " + count);
                return count;
            }
        }


        private WebDataItem[] getWebData()
            //post: collects web data from th internet in the form of a json byte array/string and 
            //converts it into an array of WebDataItems; the items returned are the ones that were entered in the last 2-4 seconds.
        {
            unixTimestamp1 = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 6, 0, 0))).TotalSeconds; //THE OFFSET IS 6 HOURS
            Debug.WriteLine("unixtimestamp1 = " + unixTimestamp1);

            byte[] jayson = DependencyService.Get<IDownloadString>().DownloadAsByteArr("http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=read");

            if (jayson[0] == 0)
            {
                DisplayAlert("App Error Alert", "Your phone is not connected to the internet. Try again later.", "OK");
                WebDataItem[] deserWebDataItems = new WebDataItem[1];
                deserWebDataItems[0] = new WebDataItem();
                return deserWebDataItems;
            }
            //////    //
            //////else if(Encoding.UTF8.GetString(jayson, 0, jayson.Length).Length > 30000)//has approximately over 300 entries in the db... 
            //////    //wwil probably have to remove this...
            //////{
            //////    DependencyService.Get<IDownloadString>().DownloadAsString("http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=clear"); 
            //////    //clears out the db
            //////    WebDataItem[] deserWebDataItems = new WebDataItem[1];
            //////    deserWebDataItems[i] = new WebDataItem();
            //////    return deserWebDataItems;
            //////}
            //////    //
            else
            {
                string jaysons = Encoding.UTF8.GetString(jayson, 0, jayson.Length);
                string str = jaysons;
                Debug.WriteLine(jaysons); //prints what we're grabbing
                List<WebDataItem> deserWebDataItems = new List<WebDataItem>();
                Char[] myChars = new Char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

                if (str != "[]")
                {
                    while (str != "]")  //could all just be in here, replacing stuf later.... except for the counting thing being so important... 
                    {
                        int first = str.IndexOf("{") + 1;
                        int last = str.IndexOf("}");
                        string str2 = str.Substring(first - 1, last - first + 2);
                        str = str.Substring(last + 1);

                        int mytime = str2.IndexOf("unixTime") + "unixTime".Length + 3;
                        string str4 = str2.Substring(mytime, 10);
                        int mynum = 0;
                        try
                        {
                            mynum = Convert.ToInt32(str4);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(str4 + " and exception was:" + e.ToString());
                        }
                        
                        if (justSpoke == true)
                        {
                            if (mynum > unixTimestamp1)//needs a wider window on occasion, so the just spoke variable helps manage that.
                            {
                                //continue with collecting data for this one
                                first = str2.LastIndexOf("datetime") + "datetime".Length;
                                str2 = str2.Insert(first + 3, @"\/" + "Date(");
                                last = str2.LastIndexOfAny(myChars) + 1;
                                str2 = str2.Insert(last, ")" + @"\/");

                                WebDataItem myItem = new WebDataItem();
                                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str2));
                                DataContractJsonSerializer ser1 = new DataContractJsonSerializer(myItem.GetType());
                                myItem = ser1.ReadObject(ms) as WebDataItem;
                                deserWebDataItems.Add(myItem);
                                //ms. .Close(); am I ruined if I leave this out?? i think it can be replaecd possibly
                                //loop until there are no more
                            }
                        }
                        else //if just spoke is false, look for anything that happend in the last 4 seconds by -2 from unixTimestamp1
                        {
                            if (mynum > (unixTimestamp1 - 2))//it needs to be a pretty wide window, it caused really bad response times without it 
                            {
                                //continue with collecting data for this one
                                first = str2.LastIndexOf("datetime") + "datetime".Length;
                                str2 = str2.Insert(first + 3, @"\/" + "Date(");
                                last = str2.LastIndexOfAny(myChars) + 1;
                                str2 = str2.Insert(last, ")" + @"\/");

                                WebDataItem myItem = new WebDataItem();
                                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str2));
                                DataContractJsonSerializer ser1 = new DataContractJsonSerializer(myItem.GetType());
                                myItem = ser1.ReadObject(ms) as WebDataItem;
                                deserWebDataItems.Add(myItem);
                                //ms. .Close(); am I ruined if I leave this out?? i think it can be replaecd possibly
                                //loop until there are no more
                            }
                        }
                        
                    }
                }

                if (deserWebDataItems.Count > 0)
                {
                    return deserWebDataItems.ToArray();  //successful path end
                }
                else //(none of the grabbed data is from 2 seconds ago)
                {
                    WebDataItem[] deserWebDataItemsfail = new WebDataItem[1];
                    deserWebDataItemsfail[0] = new WebDataItem();
                    return deserWebDataItemsfail;
                }
            }
        }


        double ConvertPitch(TodoItem item)
            //pre: TodoItem item is a todo item with a pitch value reflecting the number scale used for its slider (0-100)
            //post: a pitch value is returned converted to the number scale of the tts parameter range required 
            //(whose practical values range from almost zero to 2)
        {
            double pich = item.Pitch;
            if (pich == 0.0f)
            {
                pich = 0.01f;
            }
            else
            {
                pich = pich / 50.0f; //should initially = 50 = 1, and if it = 100, then pitch = 2.
            }
            return pich;
        }

        double ConvertSpeed(TodoItem item)
            //pre: TodoItem item is a todo item with a speed value reflecting the number scale used for its slider (0-100)
            //post: a speed value is returned converted to the number scale of the tts parameter range required 
            //(whose practical values range from almost zero to 5)
        {
            double sped = item.Speed;
            if (sped == 0.0f)
            {
                sped = 0.01f;
            }
            else if (sped <= 50.0f)
            {
                sped = sped / 50.0f;
            }
            else
            {
                sped = (sped / 12.5f) - 3.0f; //if it = 50, then the actual value = 1. and if it = 100, then it actually = 5.
            }
            return sped;
        }

        void onIRLButtonClicked(TodoItem selectedItem)
            //pre: TodoItem selectedItem is a function whose trigger has been activatd by button presses by the user.
            //post: it plays the audio file or text to speech out loud.
        {
            if (selectedItem.isTTS == true)
            {
                double pitch = ConvertPitch(selectedItem); //converts slider value of pitch to actual value of pitch
                double speed = ConvertSpeed(selectedItem); //converts slider value of speed to actual value of speed
                DependencyService.Get<ITextToSpeech>().Speak(selectedItem.Name/* + ", Trigger is " + selectedItem.Notes*/, pitch, speed);
            }
            else
            {
                DependencyService.Get<IAudioRecording>().buttonClick(selectedItem.Name);
            }
        }
        

    }
}
