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
 * This is the ChatItem database class. This is where your database's methods and constructors go.
 * Notice that the IChatSQLite interface is used to instantiate the database. You can see it's implementation 
 * in the ChatSQLite _Android and _iOS files.
 * 
 * */


namespace TTSTest2.Data
{
    public class ChatItemDatabase
    {
        static object locker = new object();

        SQLiteConnection database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        public ChatItemDatabase()
            //constructor
            //post: ChatItem database created
        {
            database = DependencyService.Get<IChatSQLite>().GetConnection();
            // create the tables
            database.CreateTable<ChatItem>();
        }

        public IEnumerable<ChatItem> GetItems()
            //post: returns all the ChatItems currently stored in the database
            //(in the form of a list of ChatItems)
        {
            lock (locker)
            {
                return (from i in database.Table<ChatItem>() select i).ToList();
            }
        }

        public ChatItem GetItem(int id)
            //pre: int id is supposedly an id of an existing item in your database.
            //post: returns the ChatItem from the database with this id.
        {
            lock (locker)
            {
                return database.Table<ChatItem>().FirstOrDefault(x => x.ID == id);
            }
        }

        public int Size()
            //post: returns the number of elements stored in the database
        {
            lock (locker)
            {
                return database.Table<ChatItem>().ToArray().Length;
            }
        }

        public ChatItem[] GetArray()
            //post: returns all the ChatItems currently stored in the database
            //(in the form of an array of ChatItems)
        {
            lock (locker)
            {
                return database.Table<ChatItem>().ToArray();
            }
        }

        public int SaveItem(ChatItem item)
            //pre: ChatItem item is a ChatItem that you want to save in your database.
            //post: returns the item's new id in the database (probably).
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
                return database.Delete<ChatItem>(id);
            }
        }

        public void DeleteAllItems()
            //post: deletes all the items currently in the database.
        {
            lock (locker)
            {
                ChatItem[] arr = GetArray();
                int[] ids = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    ids[i] = arr[i].ID;
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    database.Delete<ChatItem>(ids[i]);
                }
            }
        }

        public void DeleteExcessItems()
            //post: if there were more than 100 items in your database before this method
            //was called, it will delete the oldest entries from the database until
            //there are only 100 left in it.
        {
            List<ChatItem> mylist = GetArray().ToList<ChatItem>();
            while (mylist.Count > 100)
            {
                DeleteItem(mylist[0].ID);
                mylist.Remove(mylist[0]);
            }
        }

    }
}