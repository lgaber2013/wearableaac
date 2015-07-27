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
 * This is the TodoListPage class. This serves as the page you can view and add (see toolbar) new todoitems 
 * (functions) for a buttonitem from.
 * you can navigate to it from a ButtonItemPage.
 * 
 * */

namespace TTSTest2.Views
{
    public class TodoListPage : ContentPage
    {
        ListView listView;
        string Mac;

        public TodoListPage(string buttonMac, string buttonAlias)
            //constructor
            //pre: string buttonMac is the previous page's button mac, string buttonAlias is the previous page's button name
            //post: the button mac is passed so all your todo items made on this page can be related to the button,
            //and alias was passed so the title can say "buttonname's functions".
        {
            Mac = buttonMac;
            Title = buttonAlias + " Functions";

            listView = new ListView();

            listView.ItemTemplate = new DataTemplate
                    (typeof(TodoItemCell));

            listView.ItemSelected += (sender, e) =>
            {
                var todoItem = (TodoItem)e.SelectedItem;
                if (todoItem.isTTS == true)
                {
                    var todoPage = new TodoItemPage(buttonMac);
                    todoPage.BindingContext = todoItem;
                    ((App)App.Current).ResumeAtTodoId = todoItem.ID;
                    Debug.WriteLine("setting ResumeAtTodoId = " + todoItem.ID);
                    Navigation.PushAsync(todoPage);
                }
                else
                {
                    var todoAudioPage = new TodoItemAudioPage(buttonMac);
                    todoAudioPage.BindingContext = todoItem;
                    if (todoItem.Name == "" || todoItem.Name == null)
                    {
                        todoItem.Name = "test";
                    }
                    ((App)App.Current).ResumeAtTodoId = todoItem.ID;
                    Debug.WriteLine("setting ResumeAtTodoId = " + todoItem.ID);

                    Navigation.PushAsync(todoAudioPage);
                }
                //////used to be just this:
                ////var todoPage = new TodoItemPage(buttonMac); 
                ////todoPage.BindingContext = todoItem;
                ////((App)App.Current).ResumeAtTodoId = todoItem.ID;
                ////Debug.WriteLine("setting ResumeAtTodoId = " + todoItem.ID);
                ////Navigation.PushAsync(todoPage);
            };


            var layout = new StackLayout();
            if (Device.OS == TargetPlatform.WinPhone)
            { // WinPhone doesn't have the title showing
                layout.Children.Add(new Label
                {
                    Text = buttonAlias + " Functions",
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                });
            }
            layout.Children.Add(listView);
            layout.VerticalOptions = LayoutOptions.FillAndExpand;
            Content = layout;


            #region toolbar
            ToolbarItem tbi = null;
            if (Device.OS == TargetPlatform.iOS)
            {
                tbi = new ToolbarItem("+", null, async () =>  //trying it as an async, but hey it doesn't always turn out awesome so we'll see i guess...
                {
                    var audioORTTS = await DisplayAlert("Create new button function", "Do you want to create a function from an audio recording or from text to speech?", "Audio", "TTS");

                    if (audioORTTS == false)//text to speech = false result
                    {
                        var todoItem = new TodoItem();
                        todoItem.Pitch = 50.0;
                        todoItem.Speed = 50.0;
                        var todoPage = new TodoItemPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                        todoPage.BindingContext = todoItem;
                        await Navigation.PushAsync(todoPage);
                    }
                    else//audio = true
                    {
                        var todoItem = new TodoItem();
                        var todoAudioPage = new TodoItemAudioPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                        if (todoItem.Name == "" || todoItem.Name == null)
                        {
                            todoItem.Name = "test";
                        }
                        todoAudioPage.BindingContext = todoItem;
                        await Navigation.PushAsync(todoAudioPage);
                    }
                    ////////was just this:
                    //////var todoItem = new TodoItem();
                    ////////MyButton.ButtonTodoList.Add(todoItem); //where???? where does this go???? does the todo page have to add it itself there???
                    //////var todoPage = new TodoItemPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                    //////todoPage.BindingContext = todoItem;
                    //////Navigation.PushAsync(todoPage);

                }, 0, 0);
            }
            if (Device.OS == TargetPlatform.Android)
            {    
                tbi = new ToolbarItem("+", "plus", async () =>  //trying it as an async, but hey it doesn't always turn out awesome so we'll see i guess...
                {// BUG: Android doesn't support the icon being null
                var audioORTTS = await DisplayAlert("Create new button function", "Do you want to create a function from an audio recording or from text to speech?", "Audio", "TTS");

                if (audioORTTS == false)//text to speech = false
                {
                    var todoItem = new TodoItem();
                    todoItem.Pitch = 50.0;
                    todoItem.Speed = 50.0; 
                    var todoPage = new TodoItemPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                    todoPage.BindingContext = todoItem;
                    await Navigation.PushAsync(todoPage);
                }
                else//audio = true
                {
                    var todoItem = new TodoItem();
                    var todoAudioPage = new TodoItemAudioPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                    if (todoItem.Name == "" || todoItem.Name == null)
                    {
                        todoItem.Name = "test";
                    }
                    todoAudioPage.BindingContext = todoItem;
                    await Navigation.PushAsync(todoAudioPage);
                }
                ////////was just this:
                //////var todoItem = new TodoItem();
                ////////MyButton.ButtonTodoList.Add(todoItem); //where???? where does this go???? does the todo page have to add it itself there???
                //////var todoPage = new TodoItemPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                //////todoPage.BindingContext = todoItem;
                //////Navigation.PushAsync(todoPage);
                }, 0, 0);

            }
            if (Device.OS == TargetPlatform.WinPhone)
            {
                tbi = new ToolbarItem("+", null, async () =>  //trying it as an async, but hey it doesn't always turn out awesome so we'll see i guess...
                {// BUG: Android doesn't support the icon being null
                    var audioORTTS = await DisplayAlert("Create new button function", "Do you want to create a function from an audio recording or from text to speech?", "Audio", "TTS");

                    if (audioORTTS == false)//text to speech = false
                    {
                        var todoItem = new TodoItem();
                        todoItem.Pitch = 50.0;
                        todoItem.Speed = 50.0; 
                        var todoPage = new TodoItemPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                        todoPage.BindingContext = todoItem;
                        await Navigation.PushAsync(todoPage);
                    }
                    else//audio = true
                    {
                        var todoItem = new TodoItem();
                        var todoAudioPage = new TodoItemAudioPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                        if (todoItem.Name == "" || todoItem.Name == null)
                        {
                            todoItem.Name = "test";
                        }
                        todoAudioPage.BindingContext = todoItem;
                        await Navigation.PushAsync(todoAudioPage);
                    }
                    ////////was just this:
                    //////var todoItem = new TodoItem();
                    ////////MyButton.ButtonTodoList.Add(todoItem); //where???? where does this go???? does the todo page have to add it itself there???
                    //////var todoPage = new TodoItemPage(buttonMac); //how do you keep it changed again?? var? pass by reference is what I want...
                    //////todoPage.BindingContext = todoItem;
                    //////Navigation.PushAsync(todoPage);
                }, 0, 0);
            }

            ToolbarItems.Add(tbi);

            if (Device.OS == TargetPlatform.iOS) //this is how you do it!!! ugh I feel dumb. -u-
            {
                var tbi2 = new ToolbarItem("?", null, () =>       //is a bug, read above haven't set up iOS yet, come back in a sec
                {                                                   //it actually doesn't like todos
                    //var todos = App.Database.GetItemsNotDone();
                    var tospeak = "";
                    //foreach (var t in todos)
                    //    tospeak += t.Name + " ";
                    if (tospeak == "") tospeak = "there are no tasks to do";

                    DependencyService.Get<ITextToSpeech>().Speak(tospeak, 1.0f, 1.0f);
                }, 0, 0);
                ToolbarItems.Add(tbi2);
            }
            #endregion
        }

        protected override void OnAppearing()
            //post: does normal on appearing stuff, and sets the listview's items source to all todo items with this button mac.
        {
            base.OnAppearing();
            // reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtTodoId = -1;

            listView.ItemsSource = App.tDatabase.GetItems(Mac);
        }


    }
}