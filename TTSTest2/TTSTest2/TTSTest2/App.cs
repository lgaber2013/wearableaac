using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Xamarin.Forms;
using TTSTest2.Data;
using TTSTest2.Views;

/*
 * Description:
 * 
 * This is the App class. this is what runs the show in the most broadest sense; it is where the databases are instantiated,
 * And where the initial page you start the app on is determined.
 * 
 * */

namespace TTSTest2
{
    public class App : Application
    {
        static TodoItemDatabase todoDatabase;
        static ButtonItemDatabase buttonDatabase;
        static ChatItemDatabase chatDatabase;

        public App()
        {
            ////I added these here for convienience.
            //App.tDatabase.DeleteAllItems();
            //App.bDatabase.DeleteAllItems();
            //App.cDatabase.DeleteAllItems();
            var mainNav = new NavigationPage(new ButtonListPage());

            MainPage = mainNav;
        }

        public static TodoItemDatabase tDatabase
        {
            get
            {
                if (todoDatabase == null)
                {
                    todoDatabase = new TodoItemDatabase();
                }
                return todoDatabase;
            }
        }

        public static ButtonItemDatabase bDatabase
        {
            get
            {
                if (buttonDatabase == null)
                {
                    buttonDatabase = new ButtonItemDatabase();
                }
                return buttonDatabase;
            }
        }

        public static ChatItemDatabase cDatabase
        {
            get
            {
                if (chatDatabase == null)
                {
                    chatDatabase = new ChatItemDatabase();
                }
                return chatDatabase;
            }
        }

        public int ResumeAtTodoId { get; set; }

        protected override void OnStart()
        {
            Debug.WriteLine("OnStart");

            // always re-set when the app starts
            // users expect this (usually)
            //			Properties ["ResumeAtTodoId"] = "";
            if (Properties.ContainsKey("ResumeAtTodoId"))
            {
                var rati = Properties["ResumeAtTodoId"].ToString();
                Debug.WriteLine("   rati=" + rati);
                if (!String.IsNullOrEmpty(rati))
                {
                    Debug.WriteLine("   rati = " + rati);
                    ResumeAtTodoId = int.Parse(rati);

                    if (ResumeAtTodoId >= 0)
                    {
                        var mainButtonPage = new ButtonListPage(); //was going to shit to a todo item page.
                        mainButtonPage.BindingContext = bDatabase.GetItem(ResumeAtTodoId); //u know...

                        MainPage.Navigation.PushAsync(
                            mainButtonPage,
                            false); // no animation
                    }
                }
            }
        }
        protected override void OnSleep()
        {
            Debug.WriteLine("OnSleep saving ResumeAtTodoId = " + ResumeAtTodoId);
            // the app should keep updating this value, to
            // keep the "state" in case of a sleep/resume
            Properties["ResumeAtTodoId"] = ResumeAtTodoId;
        }
        protected override void OnResume()
        {
            Debug.WriteLine("OnResume");
            //			if (Properties.ContainsKey ("ResumeAtTodoId")) {
            //				var rati = Properties ["ResumeAtTodoId"].ToString();
            //				Debug.WriteLine ("   rati="+rati);
            //				if (!String.IsNullOrEmpty (rati)) {
            //					Debug.WriteLine ("   rati = " + rati);
            //					ResumeAtTodoId = int.Parse (rati);
            //
            //					if (ResumeAtTodoId >= 0) {
            //						var todoPage = new TodoItemPage ();
            //						todoPage.BindingContext = Database.GetItem (ResumeAtTodoId);
            //
            //						MainPage.Navigation.PushAsync (
            //							todoPage,
            //							false); // no animation
            //					}
            //				}
            //			}
        }
    }
}
