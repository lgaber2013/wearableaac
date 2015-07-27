using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

/*
 * Description:
 * 
 * This is the WebDataItem class. This serves as the outline for the type of object you derive from the json 
 * string you download frrom the internet whenever you are listening for buttons to be pressed. This object is equivalent to 
 * one entry in the database found at http://www.cs.colorado.edu/~shaunkane/buttons/buttons.php?action=read.
 * 
 * EX: {"rowid":"1","buttonID":"58b89d34fe18","unixTime":"1437478668","datetime":"2015-07-21 11:37:48"}
 * 
 * related classes: IDownloadString interface and the classes that implement it and methods that use it
 * 
 * */

namespace TTSTest2.Models
{
    [DataContract]
    public class WebDataItem
    {
        [DataMember]
        public int rowid { get; set; }  //gets and sets a rowid.
        [DataMember]
        public string buttonID { get; set; }
        [DataMember]
        public int unixTime { get; set; }
        [DataMember]
        public DateTime datetime { get; set; }

        //NOTE: THE SPECIAL WORDS IN BRACKETS^ INDICATE THIS CLASS' CONNECTION TO THE JSON FORMAT CONVERSION HAPPENING ELSEWHERE IN THE CODE.

        public WebDataItem()
        {

        }

        public WebDataItem(int r, string b, int u, DateTime d)
        {
            rowid = r;
            buttonID = b;
            unixTime = u;
            datetime = d;
        }
    }
}
