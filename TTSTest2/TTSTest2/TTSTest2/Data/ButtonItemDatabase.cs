using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using TTSTest2.Models;


/*
 * Description:
 * 
 * This is the ButtonItem database class. This is where your database's methods and constructors go.
 * Notice that the IButtonSQLite interface is used to instantiate the database. You can see it's implementation 
 * in the ButtonSQLite _Android and _iOS files.
 * 
 * Also notice that you can call methods to other databases within this class;
 * EX: App.tDatabase.DeleteAllItems(); in the DeleteAllItems() method.
 * */


namespace TTSTest2.Data
{
    public class ButtonItemDatabase
    {
        static object locker = new object();

        SQLiteConnection database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        public ButtonItemDatabase()
            //constructor
            //post: ButtonItem database created
        {
            database = DependencyService.Get<IButtonSQLite>().GetConnection();
            // create the tables
            database.CreateTable<ButtonItem>();
        }


        public IEnumerable<ButtonItem> GetItems()
            //post: returns all the ButtonItems currently stored in the database
            //(in the form of a list of ButtonItems)
        {
            lock (locker)
            {
                return (from i in database.Table<ButtonItem>() select i).ToList();
            }
        }


        public ButtonItem GetItem(int id)
            //pre: int id is supposedly an id of an existing item in your database.
            //post: returns the ButtonItem from the database with this id.
        {
            lock (locker)
            {
                return database.Table<ButtonItem>().FirstOrDefault(x => x.ID == id);
            }
        }


        public int Size()
            //post: returns the number of elements stored in the database
        {
            lock (locker)
            {
                return database.Table<ButtonItem>().ToArray().Length;
            }
        }


        public ButtonItem[] GetArray()
            //post: returns all the ButtonItems currently stored in the database
            //(in the form of an array of ButtonItems)
        {
            lock (locker)
            {
                return database.Table<ButtonItem>().ToArray();
            }
        }


        public int SaveItem(ButtonItem item)
            //pre: ButtonItem item is a ButtonItem that you want to save in your database.
            //post: returns the item's new id in the database.
        {
            lock (locker)
            {
                if (item.ID != 0)
                {
                    database.Update(item);
                    return item.ID;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }


        public int DeleteItem(int id)
            //pre: int id is supposedly an id of an existing item in your database.
            //post: deletes the item in your database with the given id.
        {
            lock (locker)
            {
                //before this in the regular code for the delete button 
                //you delete all the todoitems with this button mac. should you move it to here? maybe...
                //SEE MARKED PLACES ON BUTTON ITEM PAGE

                //deleting all the todo items with this mac as well...
                App.tDatabase.DeleteAllItems(GetItem(id).ButtonMac);

                return database.Delete<ButtonItem>(id);
            }
        }


        public void DeleteAllItems()
            //post: deletes all the items currently in the database.
        {
            lock (locker)
            {
                //deleting all the todo items as well...
                App.tDatabase.DeleteAllItems();

                ButtonItem[] arr = GetArray();
                int[] ids = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    ids[i] = arr[i].ID;
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    database.Delete<ButtonItem>(ids[i]);
                }
            }
        }


    }
}