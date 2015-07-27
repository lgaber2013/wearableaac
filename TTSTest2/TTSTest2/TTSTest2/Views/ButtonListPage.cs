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
 * This is the ButtonListPage class. This serves as the first/main activity page, and it is where you navigate to everything from
 * at this point in time. It is where you can navigate (see toolbar) to add a new button, listen for button presses, and open the chat page.
 * It is the default page displayed upon opening the applcation.
 * 
 * */

namespace TTSTest2.Views
{
    public class ButtonListPage : ContentPage
    {
        ListView listView;

        public ButtonListPage()
            //constructor
        {
            Title = "Button List";
            listView = new ListView();

            listView.ItemTemplate = new DataTemplate
                    (typeof(ButtonItemCell));
            listView.ItemSelected += (sender, e) =>
                //pre: a list view item has been selected
                //post: you navigate to the buttonitempage for this buttonitem.
            {
                var buttonItem = (ButtonItem)e.SelectedItem;
                var buttonPage = new ButtonItemPage(buttonItem.confirmed);
                buttonPage.BindingContext = buttonItem;
                ((App)App.Current).ResumeAtTodoId = buttonItem.ID;
                Debug.WriteLine("setting ResumeAtTodoId = " + buttonItem.ID);
                Navigation.PushAsync(buttonPage);
            };


            var layout = new StackLayout();
            if (Device.OS == TargetPlatform.WinPhone)
            { // WinPhone doesn't have the title showing
                layout.Children.Add(new Label
                {
                    Text = "Button List",
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                });
            }
            layout.Children.Add(listView);
            layout.VerticalOptions = LayoutOptions.FillAndExpand;
            Content = layout;

            
            #region toolbar
            ToolbarItem tbi = null;
            ToolbarItem tbi3 = null;
            ToolbarItem tbi4 = null;
            if (Device.OS == TargetPlatform.iOS)
            {
                tbi = new ToolbarItem("+", null, () =>
                {
                    var buttonItem = new ButtonItem();
                    var buttonPage = new ButtonItemPage(false);
                    buttonPage.BindingContext = buttonItem;
                    Navigation.PushAsync(buttonPage);
                }, 0, 0);

                tbi3 = new ToolbarItem("Listen", null, () =>
                    {
                        var lPage = new ListeningPage();
                        Navigation.PushAsync(lPage);
                    }, 0, 0);

                tbi4 = new ToolbarItem("Chat", null, () =>
                {
                    ChatItem chatItem = new ChatItem();
                    var cPage = new ChatPage();
                    cPage.BindingContext = chatItem;
                    Navigation.PushAsync(cPage);

                }, 0, 0);
            }
            if (Device.OS == TargetPlatform.Android)
            {                                               // BUG: Android doesn't support the icon being null
                tbi = new ToolbarItem("+", "plus", () =>
                {
                    var buttonItem = new ButtonItem();
                    var buttonPage = new ButtonItemPage(false);
                    buttonPage.BindingContext = buttonItem;
                    Navigation.PushAsync(buttonPage);
                }, 0, 0);

                tbi3 = new ToolbarItem("Listen", "playy", () =>
                {
                    var lPage = new ListeningPage();
                    Navigation.PushAsync(lPage);
                }, 0, 0);

                tbi4 = new ToolbarItem("Chat", "chat", ()  =>
                {
                    ChatItem chatItem = new ChatItem();
                    var cPage = new ChatPage();
                    cPage.BindingContext = chatItem;
                    Navigation.PushAsync(cPage);

                }, 0, 0);
            }
            if (Device.OS == TargetPlatform.WinPhone)
            {
                tbi = new ToolbarItem("Add", "add.png", () =>
                {
                    var buttonItem = new ButtonItem();
                    var buttonPage = new ButtonItemPage(false);
                    buttonPage.BindingContext = buttonItem;
                    Navigation.PushAsync(buttonPage);
                }, 0, 0);

                tbi3 = new ToolbarItem("Listen", "playy.png", () =>
                {
                    var lPage = new ListeningPage();
                    Navigation.PushAsync(lPage);
                    
                }, 0, 0);

                tbi4 = new ToolbarItem("Chat", "chat.png", () =>
                {
                    ChatItem chatItem = new ChatItem();
                    var cPage = new ChatPage();
                    cPage.BindingContext = chatItem;
                    Navigation.PushAsync(cPage);

                }, 0, 0);
            }

            ToolbarItems.Add(tbi);
            ToolbarItems.Add(tbi3);
            ToolbarItems.Add(tbi4);

            if (Device.OS == TargetPlatform.iOS)
            {
                var tbi2 = new ToolbarItem("?", null, () =>
                {
                    var tospeak = "";
                    if (tospeak == "") tospeak = "there are no tasks to do";
                    DependencyService.Get<ITextToSpeech>().Speak(tospeak, 1.0f, 1.0f);
                }, 0, 0);
                ToolbarItems.Add(tbi2);
            }
            #endregion 
        }

        protected override void OnAppearing()
            //post: does normal on appearing stuff, and sets the listview's items source.
        {
            base.OnAppearing(); // reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtTodoId = -1;  //is this a problem? saying resume on a todo id when this is a button?
            listView.ItemsSource = App.bDatabase.GetItems();
        }

    }
}