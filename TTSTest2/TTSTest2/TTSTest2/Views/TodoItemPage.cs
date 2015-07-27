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
 * This is the TodoItemPage class. This serves as the page you create and edit or delete TTS-based TodoItems on. 
 * You can even specify the speed and pitch you want to hear the text in.
 * you can navigate to it from a ButtonItem's TodoListPage.
 * 
 * */

namespace TTSTest2.Views
{
    public class TodoItemPage : ContentPage
    {
        double pitch; //is the tts number scale representation of the pitch value
        double speed; //is the tts number scale representation of the speed value

        Label pitchLabel = new Label
        {
            Text = "\tPitch value is 50.0", //changed to initiallize to 50.... i've tried... but for some reason it won't?
            FontSize = 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        Label speedLabel = new Label
        {
            Text = "\tSpeed value is 50.0",
            FontSize = 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        Slider pitchSlider = new Slider
        {
            Minimum = 0,
            Maximum = 100,
            VerticalOptions = LayoutOptions.Center,
            Value = 50 //change to initiallize to 50 (here is what's not working with that problem.)
        };

        Slider speedSlider = new Slider
        {
            Minimum = 0,
            Maximum = 100,
            VerticalOptions = LayoutOptions.Center,
            Value = 50, //change to initiallize to 50 (here is what's not working with that problem.)
        };

        Dictionary<string, int> stringToTrigger = new Dictionary<string, int> //the dictionary for our trigger picker
        {
            {"1 Button Press", 1},       { "2 Button Presses", 2 },         
            { "3 Button Presses", 3 },       { "4 Button Presses", 4 },         
            //{ "5 Button Presses", 5 },     { "6 Button Presses", 6 },
        };


        public TodoItemPage(string buttonMac)
        //constructor
        //pre: string buttonMac is the button mac of the button we navigated to the todo list page from.
        //post: whatever function is created, it's ButtonMac property is equivalent to string buttonMac
        //(done so we can keep track of what functions belong to what buttons.)
        {
            this.SetBinding(ContentPage.TitleProperty, "Name");

            NavigationPage.SetHasNavigationBar(this, true);

            var nameEntry = new EntryCell
            {
                Label = "Text:"
            };
            nameEntry.SetBinding(EntryCell.TextProperty, "Name");

            
            // Create label for displaying picked trigger
            Label chosenTrigger = new Label
            {
                Text = "",
                FontSize = 15,
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
                    chosenTrigger.Text = triggerName; //was stringToTrigger[triggerName].ToString(); (just a number)
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
                if (chosenTrigger.Text != "" && chosenTrigger.Text != null)
                {
                    var todoItem = (TodoItem)BindingContext;
                    todoItem.ButtonMac = buttonMac;
                    todoItem.isTTS = true;
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
                        todoItem.isTTS = true;
                        App.tDatabase.SaveItem(todoItem);
                        this.Navigation.PopAsync();
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
                this.Navigation.PopAsync();
            };


            var speakButton = new Button { Text = "Speak" };
            speakButton.Clicked += (sender, e) =>
            //pre: the speak button has been clicked
            //post: the phone will play the entered text related to this todoitem using tts.
            {
                var todoItem = (TodoItem)BindingContext;
                DependencyService.Get<ITextToSpeech>().Speak(todoItem.Name/* + ", Trigger is " + todoItem.Notes*/, pitch, speed);
            };


            pitchSlider.ValueChanged += OnSliderValueChanged1;
            speedSlider.ValueChanged += OnSliderValueChanged2;
            //
            pitchSlider.SetBinding(Slider.ValueProperty, "Pitch");
            speedSlider.SetBinding(Slider.ValueProperty, "Speed");

            TableView tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("TableView Title")
                {
                    new TableSection("TEXT TO SPEECH SETTINGS")
                    {
                        nameEntry,
                        new TextCell
                        {
                            Text = "Pitch",
                            Detail = "Controls how high or low the text reads out",
                        },
                        new ViewCell
                        {
                            View = pitchSlider
                        },
                        new ViewCell
                        {
                            View = pitchLabel
                        },
                        new TextCell
                        {
                            Text = "Speed",
                            Detail = "Controls how fast or slow the text reads out",
                        },
                        new ViewCell
                        {
                            View = speedSlider
                        },
                        new ViewCell
                        {
                            View = speedLabel
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


        //other methods section
        void OnSliderValueChanged1(object sender, ValueChangedEventArgs e)
        //pre: this todoitem's current pitch value reflects the number scale used for its slider (0-100)
        //post: a pitch value is set converted to the number scale of the tts parameter range required 
        //(whose practical values range from almost zero to 2)
        {
            //this conversion math below only applies if the device is an android, 
            //because the scale for things having to do with iOS are different.
            var todoItem = (TodoItem)BindingContext;
            todoItem.Pitch = e.NewValue;

            pitchLabel.Text = String.Format("\tPitch value is {0:F1}", e.NewValue);
            pitch = e.NewValue;
            if (pitch == 0.0f)
            {
                pitch = 0.01f;
            }
            else
            {
                pitch = pitch / 50.0f;
            }
        }

        void OnSliderValueChanged2(object sender, ValueChangedEventArgs e)
        //pre: this todoitem's current speed value reflects the number scale used for its slider (0-100)
        //post: a speed value is set converted to the number scale of the tts parameter range required 
        //(whose practical values range from almost zero to 5)
        {
            var todoItem = (TodoItem)BindingContext;
            todoItem.Speed = e.NewValue;

            speedLabel.Text = String.Format("\tSpeed value is {0:F1}", e.NewValue);
            speed = e.NewValue;
            if (speed == 0.0f)
            {
                speed = 0.01f;
            }
            else if (speed <= 50.0f)
            {
                speed = speed / 50.0f;
            }
            else
            {
                speed = (speed / 12.5f) - 3.0f;
            }
        }


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