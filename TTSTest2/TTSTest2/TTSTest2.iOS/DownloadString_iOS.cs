using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;
using TTSTest2.iOS;
using System.Net;

/*
 * Description:
 * 
 * This is the DownloadString_iOS class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the iOS platform's implementation of the IDownloadString interface.
 * 
 * */

[assembly: Dependency (typeof (DownloadString_iOS))]

namespace TTSTest2.iOS
{
    class DownloadString_iOS : IDownloadString
    {
        public DownloadString_iOS () {} //constructor

        public string DownloadAsString(string url)
        //pre: string url is a url you want to grab data from.
        //post: returns a string with the downloaded data 
        //Comment: I don't really think we are using this method anywhere anymore.
        {
            //System.Threading.Thread.Sleep(5000);
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }

        public byte[] DownloadAsByteArr(string url)
        //pre: string url is the url of a website whose contents you'd like to download.
        //post: returns a byte array representing json data from the url.
        //you are downloading a byte array, but its converted into data you care about later.
        {
            byte[] stuff;
            WebClient client = new WebClient();
            stuff = client.DownloadData(url);
            //}
            //catch (Java.Lang.Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
            //}
            //catch (System.Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
            //}

            return stuff;
        }

        //public void DownloadAsByteArrAsync(string url)
        //{
        //    WebClient client = new WebClient();
        //    Uri myuri = new Uri(url);
        //    //try
        //    //{
        //        client.DownloadDataAsync(myuri);
        //   // }
        //    //catch (Java.Lang.Exception e)
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
        //    //    stuff = new byte[] { 0 };
        //    //}
        //    //catch (System.Exception e)
        //    //{
        //    //    System.Diagnostics.Debug.WriteLine(e.ToString() + "\n" + e.Message);
        //    //    stuff = new byte[] { 0 };
        //    //}
        //}

    }
}
