using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

/*
 * Description:
 * 
 * This is the ButtonItem class. This serves as the outline for the type of object you store in your ButtonItem database,
 * and the database won't store complicated types like Colors or Arrays so you should avoid their use. This object is equivalent to
 * one entry in the ButtonItem database. the ID is the unique primary key that is automatically incremented.
 * 
 * */


namespace TTSTest2.Models
{
    public class ButtonItem
    {
        public ButtonItem ()
		{
		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; } //The ID is always the primary key, and is called the same thing for all the objects stored in a database.
        public string ButtonMac { get; set; } //we will store a button mac address here!
        public string ButtonAlias { get; set; } //we will store a button name here!

        public bool confirmed { get; set; } //stores if the user has confirmed the button or not. 
        //I don't know if we still need this. I know we're using it currently so it shouldn't be removed just yet.
    }
}