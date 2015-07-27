using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using TTSTest2;
using Xamarin.Forms;
using TTSTest2.iOS;
using System.IO;

/*
 * Description:
 * 
 * This is the ChatSQLite_iOS class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the iOS platform's implementation of the IChatSQLite interface.
 * 
 * */

[assembly: Dependency (typeof (ChatSQLite_iOS))]
namespace TTSTest2.iOS
{
    public class ChatSQLite_iOS : IChatSQLite
    {
        public ChatSQLite_iOS()
            //constructor
        {
        }

        #region ISQLite implementation
        public SQLite.SQLiteConnection GetConnection()
            //post: creates/starts working with a chatitem database saved under the file name ChatSQLite.db3
        {
            var sqliteFilename = "ChatSQLite.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);

            // This is where we copy in the prepopulated database
            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                File.Copy(sqliteFilename, path);
            }

            var conn = new SQLite.SQLiteConnection(path);

            // Return the database connection 
            return conn;
        }
        #endregion
    }
}