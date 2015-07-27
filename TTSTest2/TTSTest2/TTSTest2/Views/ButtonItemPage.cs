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

/*
 * Description:
 * 
 * This is the ButtonItemPage class. This serves as the page you create and edit or delete a ButtonItem on.
 * You are given your button's mac address to enter and you can name it.
 * The button is confirmed as yours when you press it while the page is listening for it. 
 * you can navigate to it from the starting page (the ButtonListPage)
 * you can navigate to the list of this ButtonItem's functions (todotitems) once it has been confirmed.
 * 
 * */

namespace TTSTest2.Views
{
    public class ButtonItemPage : ContentPage
    {
        bool confirm;           //only turned to true and save is ok'd when the confirmation of the mac address is successfull
        int unixTimestamp1;
        string lastSavedName;

        Label myOut = new Label //a testing label
        {
            Text = "will Be replaced"
        };

        EntryCell macAddr = new EntryCell
        {
            Label = "Button MAC:"
        };

        EntryCell buttonName = new EntryCell
        {
            Label = "Button Name:"
        };

        public ButtonItemPage(bool approved)
        //constructor
        {
            confirm = approved;

            this.SetBinding(ContentPage.TitleProperty, "ButtonAlias"); //the button edit page is called this.

            NavigationPage.SetHasNavigationBar(this, true);

            macAddr.SetBinding(EntryCell.TextProperty, "ButtonMac");

            Button confirmID = new Button
            {
                Text = "Confirm Address and Name"
            };
            confirmID.Clicked += async (sender, e) =>
            //pre: the confirmID button has been clicked
            //post: The physical button you are attempting to add's existence will either be confirmed or denied
            //by asking you to press it while a popup is open; calls ConfirmButton().
            {
                unixTimestamp1 = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 6, 0, 0))).TotalSeconds; //THE OFFSET IS 6 HOURS for Boulder and Denver
                var answer = await DisplayAlert("Confirming Mac Address", "First, please make sure the button is on and connected to the internet. Then press it once and close this notification by pressing \"OK\".", "OK", "Cancel");
                Debug.WriteLine("running");
                Debug.WriteLine("Answer: " + answer); // writes true or false

                myOut.Text = "t1=" + unixTimestamp1;// + " t2 =" + unixTimestamp2;

                if (answer == false)
                {
                    return;
                }
                else
                {
                    //grab data from the internet and print it in label, then see if you can parse it.
                    if (ConfirmButton() == true)
                    {
                        await DisplayAlert("Success", "This mac address is valid and the button was pressed within the window of time given.", "OK");
                        //need to check for when people try to enter 2 of the same button and when the button names are the same.
                        if (buttonName.Text != null || buttonName.Text != "")
                        {
                            confirm = true;
                        }
                    }
                    else
                    {
                        confirm = false;
                        await DisplayAlert("Not Confirmed", "The mac address given was not valid. Please try again. Make sure to enter the address correctly and press the button while the popup is open.", "OK");
                    }
                }
            };

            //buttonName.PropertyChanged += buttonName_PropertyChanged;  
            //macAddr.PropertyChanged += macAddr_PropertyChanged; 
            //NOTE: ^these lines makes you confirm and resave every time, I don't like it

            buttonName.SetBinding(EntryCell.TextProperty, "ButtonAlias");

            var saveButton = new Button { Text = "Save" };
            saveButton.Clicked += (sender, e) =>
            //pre: the save button has been clicked
            //post: it will attempt to save the Button's ButtonItem information in the database if it has legitamate values.
            //it will also return you to the ButtonList page if it is saved successfully.
            {
                if (confirm == true)
                {
                    var buttonItem = (ButtonItem)BindingContext;
                    buttonItem.confirmed = true;
                    App.bDatabase.SaveItem(buttonItem);
                    this.BindingContext = buttonItem;
                    if (noExistingButton(macAddr.Text) == true)
                    {
                        this.Navigation.PopAsync();
                    }
                    else
                    {
                        //App.tDatabase.DeleteAllItems(buttonItem.ButtonMac); //they wouldn't have items??? this is the line that should be moved probably...
                        App.bDatabase.DeleteItem(buttonItem.ID);
                        DisplayAlert("Error Saving", "The button with mac address " + macAddr.Text + " already exists! Duplicates are not allowed.", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error Saving", "please check that you have confirmed the mac address and given the button a name.", "OK");
                }
            };

            var functionsButton = new Button { Text = "Edit Functions" };
            functionsButton.Clicked += (sender, e) =>
            //pre: the edit functions button has been clicked
            //post: it will attempt to save the Button's ButtonItem information in the database if it has legitamate values.
            //it will also send you to the TodoListPage for this button if it is saved successfully.
            {
                if (confirm == true)
                {
                    var buttonItem = (ButtonItem)BindingContext;
                    buttonItem.confirmed = true;
                    App.bDatabase.SaveItem(buttonItem);
                    this.BindingContext = buttonItem;
                    if (noExistingButton(macAddr.Text) == true)
                    {
                        var todoList = new TodoListPage(buttonItem.ButtonMac, buttonItem.ButtonAlias); //The todo list for a button needs the button's MAC.
                        this.Navigation.PushAsync(todoList);
                    }
                    else
                    {
                        //App.tDatabase.DeleteAllItems(buttonItem.ButtonMac); //they wouldn't have items??? this is the line that should be moved probably...
                        App.bDatabase.DeleteItem(buttonItem.ID);
                        DisplayAlert("Error Saving", "The button with mac address " + macAddr.Text + " already exists! Duplicates are not allowed.", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error Saving", "please check that you have confirmed the mac address and given the button a name.", "OK");
                }
            };

            var deleteButton = new Button { Text = "Delete" };
            deleteButton.Clicked += (sender, e) =>
            //pre: the delete button has been clicked
            //post: it will delete any of the button's ButtonItem information in the database.
            //it will also return you to the ButtonList page.
            {
                var buttonItem = (ButtonItem)BindingContext;
                //App.tDatabase.DeleteAllItems(buttonItem.ButtonMac);  //they wouldn't have items??? this is the line that should be moved probably...
                App.bDatabase.DeleteItem(buttonItem.ID);
                this.Navigation.PopAsync();
            };

            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.Clicked += (sender, e) =>
            //pre: the cancel button has been clicked
            //post: it will return you to the ButtonList page without changing anything.
            {
                var buttonItem = (ButtonItem)BindingContext;
                this.Navigation.PopAsync();
            };

            TableView tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("TableView Title")
                {
                    new TableSection("TEXT TO SPEECH SETTINGS")
                    {
                        macAddr,
                        buttonName,
                        new ViewCell
                        {
                            View = confirmID
                        },
                        //new ViewCell
                        //{
                        //    View = myOut
                        //},
                        new ViewCell
                        {
                            View = functionsButton
                        },
                        new ViewCell
                        {
                            View = saveButton
                        },
                        new ViewCell
                        {
                            View = deleteButton
                        },
                        new ViewCell
                        {
                            View = cancelButton
                        },
                    }
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    tableView
                }
            };

        }

        protected override void OnAppearing()
            //post: when the page first appears, the 'lastSavedName' variable is set up
            //and the regular stuff that's done on appearing happens too.
        {
            base.OnAppearing();
            lastSavedName = buttonName.Text;
        }


        //private void buttonName_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    confirm = false;
        //}

        //private void macAddr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    confirm = false;
        //}



        public bool ConfirmButton()
            //post: will get the web data, then compare it and then set the confirm bool to true or 
            //false. then whether that's true you need to do pop-up stuff.
        {
            WebDataItem[] myItems = getWebData();

            if (myItems.Length > 0 && myItems[0] != null && myItems[0].rowid != 0) //means that the array is actually filled with something.
            {
                for (int i = 0; i < myItems.Length; i++)
                {
                    //if (myItems[i].unixTime > unixTimestamp1)//this is already done in the get web data method.
                    //{
                    if (myItems[i].buttonID == macAddr.Text)
                    {
                        return true;
                    }
                    //}
                }
            }
            return false;
        }

        private WebDataItem[] getWebData()
            //post: grabs the current list of data from the web and converts it into an array of WebDataItems 
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
            else
            {
                string jaysons = Encoding.UTF8.GetString(jayson, 0, jayson.Length);
                string str = jaysons;
                Debug.WriteLine(jaysons);//for checking stuff is thistoo much???
                List<WebDataItem> deserWebDataItems = new List<WebDataItem>();
                Char[] myChars = new Char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

                if (str != "[]")
                {
                    while (str != "]")
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

                        if (mynum >= unixTimestamp1)
                        {
                            //Debug.WriteLine("applicable!!!!! timestamp of this item is = " + mynum);
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
                            //loop until there are no more
                        }
                        //Debug.WriteLine("NOT   applicable!!!!! timestamp of this item is = " + mynum);
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

        public bool noExistingButton(string mac)
            //pre: string mac is the mac address currently entered on the page
            //post: returns true if there is no existing button that already has this as their mac address, false otherwise.
        {
            int count = 0;
            ButtonItem[] myitems = App.bDatabase.GetArray();
            if (myitems.Length > 0 && myitems[0] != null)
            {
                for (int i = 0; i < myitems.Length; i++)
                {
                    if (myitems[i].ButtonMac == mac)
                    {
                        count++;
                    }
                }
                if (count >= 2)
                {
                    return false;
                }
            }
            return true;
        }





    }
}


//OLD GETWEBDATA() METHOD CONTENTS
//////there is an error that happens where thie index comes back negative and its caused by the internet not connecting on your phone
//////but I don' know how to catch it yet, please add debug messages in this method if it happens again... so we can catch it
////byte[] jayson = DependencyService.Get<IDownloadString>().DownloadAsByteArr("http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=read");

////if (jayson[0] == 0)
////{
////    DisplayAlert("App Error Alert", "Your phone is not connected to the internet. Try again later.", "OK");

////    //want this...
////    WebDataItem[] deserWebDataItems = new WebDataItem[1];

////        deserWebDataItems[0] = new WebDataItem();
////    return deserWebDataItems;
////}
////else
////{

////    string jaysons = Encoding.UTF8.GetString(jayson, 0, jayson.Length);
////    //System.Console.WriteLine(jaysons);

////    //get one of the stuffs...
////    string str = jaysons;
////    int count = 0;
////    int first;
////    int last;
////    string str2;
////    //This search returns the substring between two strings, so  
////    //the first index is moved to the character just after the first string. 

////    //jaysonstring final form example...
////    string jaysonfinal = "";

////    if (str != "[]")
////    {
////        while (str != "]")  //could all just be in here, replacing stuf later.... except for the counting thing being so important... 
////        {
////            first = str.IndexOf("{") + 1;
////            last = str.IndexOf("}");
////            str2 = str.Substring(first - 1, last - first + 2);
////            jaysonfinal = str2;
////            //System.Console.WriteLine(str2);
////            //System.Console.WriteLine();
////            str = str.Substring(last + 1);
////            //System.Console.WriteLine(str);
////            //System.Console.WriteLine();
////            count++;
////        }
////        //loop until there are no more stuffs 
////    }
////    Char[] myChars = new Char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
////    string str3 = jaysons;
////    string[] myData = new string[count];
////    if (str3 != "[]")
////    {
////        for (int i = 0; i < count; i++)
////        {
////            first = str3.IndexOf("{") + 1;
////            last = str3.IndexOf("}");
////            str2 = str3.Substring(first - 1, last - first + 2);
////            myData[i] = str2;
////            int start = myData[i].LastIndexOf("datetime") + "datetime".Length;
////            myData[i] = myData[i].Insert(start + 3, @"\/" + "Date(");


////            int end = myData[i].LastIndexOfAny(myChars) + 1;

////            myData[i] = myData[i].Insert(end, ")" + @"\/");
////            //System.Console.WriteLine(myData[i]);


////            //System.Console.WriteLine(str2);
////            str3 = str3.Substring(last + 1);
////        }
////    }

////    //want this...
////    WebDataItem[] deserWebDataItems = new WebDataItem[count];

////    for (int i = 0; i < count; i++)
////    {
////        deserWebDataItems[i] = new WebDataItem();
////    }


////    for (int i = 0; i < count; i++)
////    {

////        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(myData[i])); //json is a json string.. cool!! //was jaysons
////        DataContractJsonSerializer ser1 = new DataContractJsonSerializer(deserWebDataItems[i].GetType());
////        deserWebDataItems[i] = ser1.ReadObject(ms) as WebDataItem;
////        //ms. .Close(); am I ruined if I leave this out?? i think it can be replaecd possibly
////    }

////    return deserWebDataItems;
////}