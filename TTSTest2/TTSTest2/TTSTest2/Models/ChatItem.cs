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
 * This is the ChatItem class. This serves as the outline for the type of object you store in your ChatItem database,
 * and the database won't store complicated types like Colors or Arrays so you should avoid their use. This object is equivalent to 
 * one entry in the ChatItem database. the ID is the unique primary key that is automatically incremented.
 * 
 * */

namespace TTSTest2.Models
{
    public class ChatItem
    {
        public ChatItem ()
		{
		}

		[PrimaryKey, AutoIncrement]
		public int ID { get; set; } //Id is the primary key
		public string Segment { get; set; } //is the Text you are text to speaking 
    }
}