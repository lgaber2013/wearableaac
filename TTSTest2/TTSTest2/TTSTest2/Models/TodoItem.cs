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
 * This is the TodoItem class. This serves as the outline for the type of object you store in your TodoItem database,
 * and the database won't store complicated types like Colors or Arrays so you should avoid their use. This object is equivalent to
 * one entry in the TodoItem database. the ID is the unique primary key that is automatically incremented.
 * 
 * */

namespace TTSTest2.Models
{
    public class TodoItem
    {
        public TodoItem ()
		{
		}

		[PrimaryKey, AutoIncrement]
        public int ID { get; set; } //ID is the primary key
		public string Name { get; set; } //Name is either the name of your audio file or the string converted with TTS.
		public string Notes { get; set; } //Notes is the trigger for this item represented as a string.\
		public bool Done { get; set; } //Done sets whether or not the device is listening for the trigger for the item. Like an on/off.

        public double Pitch { get; set; } //Pitch represents the double value for pitch for the voice.
        public double Speed { get; set; } //Speed represents the double value for speech rate for the voice. 
        public string ButtonMac { get; set; } //we will store a button mac address here! the button mac is the button that owns this todo item.
        public bool isTTS { get; set; } //isTTS stores if this particular todo item is a tts or not (if not, then its an audio file recording).

        //NOTE: BECAUSE THIS REPRESENTS THE BASIS FOR A DATABASE ENTRY, YOU CANNOT ADD SPECIAL TYPES LIKE COLOR OR ARRAYS; 
        //IF YOU NEED TO DO THIS, CONSIDER STORING THEM AS A BASIC DATATYPE THEN CONVERTING WHAT EVER IS IN THAT TYPE TO WHAT YOU ACTUALLY WANT.
    }
}