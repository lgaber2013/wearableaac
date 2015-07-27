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
 * This is the TodoItemAudioPage class. This serves as the page you create and edit or delete Audio recording-based TodoItems on. 
 * you can navigate to it from a ButtonItem's TodoListPage.
 * 
 * */

namespace TTSTest2.Views
{
    public class TodoItemAudioPage : ContentPage
    {
        string prevName = "";  //keeps track of file name changes

        Dictionary<string, int> stringToTrigger = new Dictionary<string, int> //the dictionary for our trigger picker
        {
            {"1 Button Press", 1},       { "2 Button Presses", 2 },         
            { "3 Button Presses", 3 },       { "4 Button Presses", 4 },         
            //{ "5 Button Presses", 5 },     { "6 Button Presses", 6 },
        };

        Button speakButton = new Button { Text = "Play Recording", IsEnabled = false };
        Button startRecordButton = new Button { Text = "Start Recording", IsEnabled = true };
        Button stopRecordButton = new Button { Text = "Stop Recording", IsEnabled = false };

        EntryCell nameEntry = new EntryCell
        {
            Label = "Audio File Name:",
            Text = "test" //this needs to have a default name.
        };



        public TodoItemAudioPage(string buttonMac)
            //constructor
            //pre: string buttonMac is the button mac of the button we navigated to the todo list page from.
            //post: whatever function is created, it's ButtonMac property is equivalent to string buttonMac
            //(done so we can keep track of what functions belong to what buttons.)
        {


            this.SetBinding(ContentPage.TitleProperty, "Name");

            NavigationPage.SetHasNavigationBar(this, true);
            nameEntry.SetBinding(EntryCell.TextProperty, "Name");
            
            // Create label for displaying picked trigger
            Label chosenTrigger = new Label
            {
                Text = "",
                FontSize = 15, //Device.GetNamedSize(NamedSize.Default, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            chosenTrigger.SetBinding(Label.TextProperty, "Notes");



            Picker picker = new Picker
            {
                Title = "Trigger",
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            foreach (string triggerName in stringToTrigger.Keys) //adding the trigger dictionary list
            {
                picker.Items.Add(triggerName);
            }

            picker.SelectedIndexChanged += (sender, args) =>
                //pre: the selected index of the trigger picker has been changed
                //post: it updates the trigger/Notes for the current todoitem/function we're working on
            {
                if (picker.SelectedIndex == -1)
                {
                    chosenTrigger.Text = "";
                }
                else
                {
                    string triggerName = picker.Items[picker.SelectedIndex];
                    chosenTrigger.Text = triggerName;
                }

                var todoItem = (TodoItem)BindingContext;
                todoItem.Notes = chosenTrigger.Text;
            };

            

            var doneEntrySwitch = new SwitchCell
            {
                Text = "Function is: "
            };
            doneEntrySwitch.SetBinding(SwitchCell.OnProperty, "Done");



            var saveButton = new Button { Text = "Save" };  
            saveButton.Clicked += (sender, e) =>
            //pre: the save button has been clicked
            //post: it will attempt to save the button function as a TodoItem object in the database if it has legitamate values.
            //it will also return you to the TodoList page if it is saved successfully.
            {
                System.Diagnostics.Debug.WriteLine("made it in to save button...");
                //first see if you need to overwrite the name...
                if (prevName != nameEntry.Text)
                {
                    bool result = DependencyService.Get<IAudioRecording>().overwriteCurrFile(prevName, nameEntry.Text);
                    if (result == true)
                    {
                        //continue with checks
                        if (chosenTrigger.Text != "" && chosenTrigger.Text != null)
                        {
                            var todoItem = (TodoItem)BindingContext;
                            todoItem.ButtonMac = buttonMac;
                            todoItem.isTTS = false;
                            //^I added this one line. its an essential line.
                            App.tDatabase.SaveItem(todoItem);

                            if (doneEntrySwitch.On)
                            {
                                if (noExistingTrigger(buttonMac, chosenTrigger.Text) == true)
                                {
                                    this.Navigation.PopAsync();
                                }
                                else
                                {
                                    App.tDatabase.DeleteItem(todoItem.ID);
                                    DisplayAlert("Error Saving", "A function programmed on this button already posesses ownership of the currently selected trigger and is currently turned on! Please change your trigger selection and try again, or turn off the function.", "OK");
                                }
                            }
                            else
                            {
                                this.Navigation.PopAsync();
                            }
                        }
                        else
                        {
                            if (doneEntrySwitch.On)
                            {
                                DisplayAlert("Error Saving", "This function is on but has no specified trigger. Please enter a trigger and try again, or turn off the function.", "OK");
                            }
                            else
                            {
                                var todoItem = (TodoItem)BindingContext;
                                todoItem.ButtonMac = buttonMac;
                                todoItem.isTTS = false;
                                App.tDatabase.SaveItem(todoItem);
                                this.Navigation.PopAsync();
                            }
                        }
                    }
                    else
                    {
                        DisplayAlert("Error Saving", "This function has an invalid audio file name (for some reason) and cannot be saved as given. Please change the file name and try again. Suggestions: no spaces, no names starting with numbers, avoid using characters that are not numbers or letters.", "OK");
                    }
                }
                else
                {
                    if (chosenTrigger.Text != "" && chosenTrigger.Text != null)
                    {
                        var todoItem = (TodoItem)BindingContext;
                        todoItem.ButtonMac = buttonMac;
                        todoItem.isTTS = false;
                        App.tDatabase.SaveItem(todoItem);

                        if (doneEntrySwitch.On)
                        {
                            if (noExistingTrigger(buttonMac, chosenTrigger.Text) == true)
                            {
                                this.Navigation.PopAsync();
                            }
                            else
                            {
                                App.tDatabase.DeleteItem(todoItem.ID);
                                DisplayAlert("Error Saving", "A function programmed on this button already posesses ownership of the currently selected trigger and is currently turned on! Please change your trigger selection and try again, or turn off the function.", "OK");
                            }
                        }
                        else
                        {
                            this.Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        if (doneEntrySwitch.On)
                        {
                            DisplayAlert("Error Saving", "This function is on but has no specified trigger. Please enter a trigger and try again, or turn off the function.", "OK");
                        }
                        else
                        {
                            var todoItem = (TodoItem)BindingContext;
                            todoItem.ButtonMac = buttonMac;
                            todoItem.isTTS = false;
                            App.tDatabase.SaveItem(todoItem);
                            this.Navigation.PopAsync();
                        }
                    }
                }
                
            };

            var deleteButton = new Button { Text = "Delete" };
            deleteButton.Clicked += (sender, e) =>
            //pre: the delete button has been clicked
            //post: it will delete any of this function's/todoitem's information in the database.
            //it will also return you to the TodoList page.
            {
                var todoItem = (TodoItem)BindingContext;
                todoItem.ButtonMac = buttonMac;
                todoItem.isTTS = false;
                DependencyService.Get<IAudioRecording>().deleteFile();
                App.tDatabase.DeleteItem(todoItem.ID);
                this.Navigation.PopAsync();
            };

            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.Clicked += (sender, e) =>
            //pre: the cancel button has been clicked
            //post: it will return you to the TodoList page without changing anything.
            {
                var todoItem = (TodoItem)BindingContext;
                todoItem.ButtonMac = buttonMac;
                todoItem.isTTS = false;
                this.Navigation.PopAsync();
            };

            startRecordButton.Clicked += (sender, e) =>
                //pre: the start recording button has been clicked
                //post: the media recorder will start recording any audio it picks up from the phone's microphone.
            {
                var todoItem = (TodoItem)BindingContext;
                stopRecordButton.IsEnabled = !stopRecordButton.IsEnabled;
                startRecordButton.IsEnabled = !startRecordButton.IsEnabled;
                DependencyService.Get<IAudioRecording>().startRecording();
            };

            
            stopRecordButton.Clicked += (sender, e) =>
            //pre: the stop recording button has been clicked
            //post: the media recorder will stop recording any audio it picks up from the phone's microphone;
            //I think it also saves the file on your sd card.
            {
                var todoItem = (TodoItem)BindingContext;
                stopRecordButton.IsEnabled = !stopRecordButton.IsEnabled;
                DependencyService.Get<IAudioRecording>().stopRecording();
                speakButton.IsEnabled = true;
            };

            speakButton.Clicked += (sender, e) =>
            //pre: the play recording button has been clicked
            //post: the media player will play the last recorded audio related to this todoitem.
            {
                var todoItem = (TodoItem)BindingContext;
                speakButton.IsEnabled = false;
                DependencyService.Get<IAudioRecording>().playRecording();
                speakButton.IsEnabled = true;
                startRecordButton.IsEnabled = true;
            };


            TableView tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("TableView Title")
                {
                    new TableSection("AUDIO RECORDING SETTINGS")
                    {
                        nameEntry,
                        new ViewCell
                        {
                            View = startRecordButton
                        },
                        new ViewCell
                        {
                            View = stopRecordButton
                        },
                        new ViewCell
                        {
                            View = speakButton
                        },
                        new ViewCell
                        {
                            View = picker
                        },
                        new ViewCell
                        {
                            View = chosenTrigger
                        },
                       
                        doneEntrySwitch, 
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
            //post: does normal onappearing jobs then checks if there is already an existing audio file on the sd card
            //with the name in the name entry, and if there is then the play recording /speak button is enabled
        {
            base.OnAppearing();

            bool mystuf = DependencyService.Get<IAudioRecording>().newFileInitialzed(nameEntry.Text);
            if (mystuf == false)
            {
                speakButton.IsEnabled = true;
            }
            prevName = nameEntry.Text;
        }

        //other methods section
        public bool noExistingTrigger(string mac, string trigger)
            //pre: string mac is the mac address passed to the page, and string trigger is the currently 
            //set trigger for this todoitem/function.
            //post: returns true if there is no existing functionn that already has this as their mac 
            //address, on status and trigger- false otherwise.
        {
            int count = 0;
            TodoItem[] myitems = App.tDatabase.GetArray(mac);
            if (myitems.Length > 0)
            {
                if (myitems[0] != null)
                {
                    for (int i = 0; i < myitems.Length; i++)
                    {
                        if (myitems[i].Done == true && myitems[i].Notes == trigger)
                        {
                            count++;
                        }
                    }
                    if(count >= 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


    }
}
