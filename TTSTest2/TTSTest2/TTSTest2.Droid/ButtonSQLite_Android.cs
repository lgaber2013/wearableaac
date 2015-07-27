using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using TTSTest2;
using Xamarin.Forms;
using TTSTest2.Droid;
using System.IO;
using TTSTest2.Data;

/*
 * Description:
 * 
 * This is the ButtonSQLite_Android class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the android platform's implementation of the IButtonSQLite interface.
 * 
 * */

[assembly: Dependency (typeof (ButtonSQLite_Android))]
namespace TTSTest2.Droid
{
    public class ButtonSQLite_Android : IButtonSQLite
	{
		public ButtonSQLite_Android ()
            //constructor
		{
		}

		#region ISQLite implementation
		public SQLite.SQLiteConnection GetConnection ()
            //post: creates/starts working with a buttonitem database saved under the file name ButtonSQLite.db3
		{
			var sqliteFilename = "ButtonSQLite.db3";
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal); // Documents folder
			var path = Path.Combine(documentsPath, sqliteFilename);

			// This is where we copy in the prepopulated database
			Console.WriteLine (path);
			if (!File.Exists(path))
			{
				var s = Forms.Context.Resources.OpenRawResource(Resource.Raw.ButtonSQLite);  // RESOURCE NAME ###

				// create a write stream
				FileStream writeStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
				// write to the stream
				ReadWriteStream(s, writeStream);
			}

			var conn = new SQLite.SQLiteConnection(path);

			// Return the database connection 
			return conn;
		}
		#endregion

		/// <summary>
		/// helper method to get the database out of /raw/ and into the user filesystem
		/// </summary>
		void ReadWriteStream(Stream readStream, Stream writeStream)
		{
			int Length = 256;
			Byte[] buffer = new Byte[Length];
			int bytesRead = readStream.Read(buffer, 0, Length);
			// write the required bytes
			while (bytesRead > 0)
			{
				writeStream.Write(buffer, 0, bytesRead);
				bytesRead = readStream.Read(buffer, 0, Length);
			}
			readStream.Close();
			writeStream.Close();
		}
	}
}