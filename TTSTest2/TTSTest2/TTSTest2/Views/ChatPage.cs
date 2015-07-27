using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using TTSTest2.Models;
using System.IO;

/*
 * Description:
 * 
 * This is the ChatPage class. This serves as the page you create and replay ChatItems on. It is meant to be 
 * used in conversations that need more specific language, but also provides a way for you to repeat or reuse phrases.
 * you can navigate to it from the starting page (the ButtonListPage)
 * 
 * */

namespace TTSTest2.Views
{
    class ChatPage : ContentPage
    {
        List<string> mychats = new List<string>();
        ListView listView; 

        public ChatPage()
            //constructor
        {
            listView = new ListView();

            listView.ItemTemplate = new DataTemplate
                    (typeof(ChatItemCell));

            listView.ItemSelected += (sender, e) =>
                //pre: a list item has been selected
                //post: the text of the chat item it represents is spoken out loud using text to speech.
            {
                var chatItem = (ChatItem)e.SelectedItem;
                DependencyService.Get<ITextToSpeech>().Speak(chatItem.Segment, 1.0, 1.0);
            };

            NavigationPage.SetHasNavigationBar(this, true);

            var nameEntry = new EntryCell
            {
                Label = "Text:"
            };
            nameEntry.SetBinding(EntryCell.TextProperty, "Segment");

            var saveButton = new Button { Text = "Save" };
            saveButton.Clicked += (sender, e) =>
                //pre: the save button has been clicked
                //post: the chat item is saved and added to the listview
            {
                var chatItem = (ChatItem)BindingContext;
                App.cDatabase.SaveItem(chatItem);
                this.BindingContext = new ChatItem();
                listView.ItemsSource = App.cDatabase.GetItems();
            };

            var speakButton = new Button { Text = "Speak" };
            speakButton.Clicked += (sender, e) =>
                //pre: the speak button has been clicked
                //post: the chat item is saved and added to the listview and
                //the text of the chat item it represents is spoken out loud using text to speech.
            {
                var chatItem = (ChatItem)BindingContext;
                DependencyService.Get<ITextToSpeech>().Speak(chatItem.Segment, 1.0, 1.0);
                App.cDatabase.SaveItem(chatItem);
                this.BindingContext = new ChatItem(); //this makes it able to add several instead of just one
                listView.ItemsSource = App.cDatabase.GetItems();
            };

            TableView tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("TableView Title")
                {
                    new TableSection("CHAT TEXT ENTRY WINDOW")
                    {
                        new TextCell
                        {
                            Text = "Chat",
                            Detail = " just enter what you want to say"
                        },
                        nameEntry,
                        new ViewCell
                        {
                            View = speakButton
                        }, 
                        new ViewCell
                        {
                            View = saveButton
                        },
                    }
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    listView,
                    tableView,
                }
            };
        }

        protected override void OnAppearing()
            //post: does normal on appearing stuff, and sets the listview's items source.
        {
            base.OnAppearing();
            listView.ItemsSource = App.cDatabase.GetItems();
        }

        protected override void OnDisappearing()
            //post: does normal on disappearing stuff, and deletes all excess items (> 100) created starting with the oldest
            //(done to control the space the data takes up)
        {
            base.OnDisappearing();
            App.cDatabase.DeleteExcessItems();
        }


    }
}




        

       