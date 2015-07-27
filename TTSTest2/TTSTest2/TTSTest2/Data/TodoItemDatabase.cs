using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using TTSTest2.Models; //adding this fixed todoitem highlighted in red in below code; happened here and in todoitem page and todolistpage

/*
 * Description:
 * 
 * This is the TodoItem database class. This is where your database's methods and constructors go.
 * Notice that the ITodoSQLite interface is used to instantiate the database. You can see it's implementation 
 * in the TodoSQLite _Android and _iOS files.
 * 
 * */

namespace TTSTest2.Data
{
    public class TodoItemDatabase
    {
        static object locker = new object();

        SQLiteConnection database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        public TodoItemDatabase()
            //constructor
            //post: TodoItem database created
        {
            database = DependencyService.Get<ITodoSQLite>().GetConnection();
            // create the tables
            database.CreateTable<TodoItem>();
        }

        public IEnumerable<TodoItem> GetItems()
            //post: returns all the TodoItems currently stored in the database
            //(in the form of a list of TodoItems)
        {
            lock (locker)
            {
                return (from i in database.Table<TodoItem>() select i).ToList();
            }
        }


        public IEnumerable<TodoItem> GetItems(string mac)
            //pre: string mac is the button mac of an existing button
            //post: returns all the TodoItems currently stored in the database that 
            //have this mac address (in the form of a list of TodoItems)
        {
            lock (locker)
            {
                List<TodoItem> list = new List<TodoItem>();
                TodoItem[] arr = GetArray();
                for (int i = 0; i < Size(); i++)
                {
                    if (arr[i].ButtonMac == mac)
                    {
                        list.Add(arr[i]);
                    }
                }
                return list;
            }
        }

        public IEnumerable<TodoItem> GetItemsNotDone() 
            //we never use this, it was just here.
            //post: returns a list of TodoItems that are not marked as done
        {
            lock (locker)
            {
                return database.Query<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
            }
        }

        public TodoItem GetItem(int id)
            //pre: int id is supposedly an id of an existing item in your database.
            //post: returns the TodoItem from the database with this id.
        {
            lock (locker)
            {
                return database.Table<TodoItem>().FirstOrDefault(x => x.ID == id);
            }
        }

        //I added the following methods
        public int Size()
            //post: returns the number of elements stored in the database
        {
            lock (locker)
            {
                return database.Table<TodoItem>().ToArray().Length;
            }
        }

        public TodoItem[] GetArray()
            //post: returns all the TodoItems currently stored in the database
            //(in the form of an array of TodoItems)
        {
            lock (locker)
            {
                return database.Table<TodoItem>().ToArray();
            }
        }

        public TodoItem[] GetArray(string mac)
            //post: returns all the TodoItems currently stored in the database 
            //with this button mac address. (in the form of an array of TodoItems)
        {
            lock (locker)
            {
                TodoItem[] arr = GetArray();
                int count = 0;
                for (int i = 0; i < Size(); i++)
                {
                    if (arr[i].ButtonMac == mac)//gets all the todo items with a mac value equivalent to our button's value.
                    {
                        count++;
                    }
                }

                if (count != 0)
                {
                    TodoItem[] macArr = new TodoItem[count];
                    int count2 = 0;
                    for (int i = 0; i < Size(); i++)
                    {
                        if (arr[i].ButtonMac == mac)//gets all the todo items with a mac value equivalent to our button's value.
                        {
                            macArr[count2] = arr[i];
                            count2++;
                        }
                    }

                    return macArr;
                }
                else
                {
                    TodoItem[] macArr = new TodoItem[1];
                    return macArr;
                }

            }
        }

        public int SaveItem(TodoItem item)
            //pre: TodoItem item is a TodoItem that you want to save in your database.
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
                DependencyService.Get<IAudioRecording>().deleteFile(GetItem(id).Name);
                return database.Delete<TodoItem>(id);
            }
        }

        public void DeleteAllItems()
            //post: deletes all the items currently in the database.
        {
            lock (locker)
            {
                TodoItem[] arr = GetArray();
                if (arr != null)//idk if that's a good solution...
                {
                    int[] ids = new int[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ids[i] = arr[i].ID;
                    }

                    for (int i = 0; i < arr.Length; i++)
                    {
                        DependencyService.Get<IAudioRecording>().deleteFile(GetItem(ids[i]).Name);
                        database.Delete<TodoItem>(ids[i]);
                    }
                }
            }
        }

        public void DeleteAllItems(string mac)
            //post: deletes all the items currently in the database 
            //with this button mac address.
        {
            lock (locker)
            {
                TodoItem[] arr = GetArray(mac);
                if (arr[0] != null)
                {
                    int[] ids = new int[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ids[i] = arr[i].ID;
                    }

                    for (int i = 0; i < arr.Length; i++)
                    {
                        DependencyService.Get<IAudioRecording>().deleteFile(GetItem(ids[i]).Name); 
                        //if its not found then no fuss is kicked
                        //but if it is its deleted...
                        database.Delete<TodoItem>(ids[i]);
                    }
                }
            }
        }


    }
}