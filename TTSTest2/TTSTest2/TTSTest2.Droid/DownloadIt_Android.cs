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

using Xamarin.Forms;
using System.Net;
using TTSTest2.Droid;
using Java.Lang;

//NOTE: DO NOT CHANGE THE NAME OF THIS FILE TO 'DOWNLOADSTRING_ANDROID', I DON'T KNOW WHY Exactly BUT IT MESSES UP SOME STUFF
//AND ITS OFTEN TO HARD TO FIGURE OUT WHAT. Just leave the name alone.

/*
 * Description:
 * 
 * This is the DownloadIt_Android class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the android platform's implementation of the IDownloadString interface.
 * 
 * */

[assembly: Dependency(typeof(DownloadIt_Android))]
namespace TTSTest2.Droid
{
    public class DownloadIt_Android : IDownloadString
    {
        public DownloadIt_Android()
            //constructor
        {
        }

        public string DownloadAsString(string url)
            //pre: string url is a url you want to grab data from.
            //post: returns a string with the downloaded data 
            //Comment: I don't really think we are using this method anywhere anymore.
        {
            string stuff = "0";
            WebClient client = new WebClient();

            try
            {
                stuff = client.DownloadString(url);
            }
            catch(Java.Lang.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
                stuff = "-1";
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
                stuff = "-1";
            }
            //System.Diagnostics.Debug.WriteLine(stuff);

            return stuff;
        }

        public byte[] DownloadAsByteArr(string url) 
            //pre: string url is the url of a website whose contents you'd like to download.
            //post: returns a byte array representing json data from the url.
            //you are downloading a byte array, but its converted into data you care about later.
        {
            byte[] stuff;
            WebClient client = new WebClient();

            try
            {
                stuff = client.DownloadData(url);
            }
            catch (Java.Lang.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
                stuff = new byte[] { 0 };
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
                stuff = new byte[] { 0 };
            }

            return stuff;
        }

        
    }
}