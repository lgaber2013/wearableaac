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

using Android.Speech.Tts;
using Xamarin.Forms;
using Java.Lang;
using TTSTest2;
using TTSTest2.Droid;
using System.IO;

/*
 * Description:
 * 
 * This is the TextToSpeech_Android class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the android platform's implementation of the ITextToSpeech interface.
 * 
 * */

[assembly: Dependency(typeof(TextToSpeech_Android))]
namespace TTSTest2.Droid
{
    public class TextToSpeech_Android : Java.Lang.Object, ITextToSpeech, TextToSpeech.IOnInitListener
    {
        TextToSpeech speaker;
        string toSpeak;
        public TextToSpeech_Android()
            //constructor
        {
        }

        public void Speak(string text, double pitch, double speed)
            //pre: all parameters are tts parameters
            //post: the string text is spoken using tts at the given pitch and speed values.
        {
            var c = Forms.Context;
            toSpeak = text;
            if (speaker == null)
                speaker = new TextToSpeech(c, this);
            else
            {
                //these 2 lines added
                speaker.SetPitch((float)pitch);
                speaker.SetSpeechRate((float)speed);

                var p = new Dictionary<string, string>();
                speaker.Speak(toSpeak, QueueMode.Flush, p);
            }
        }

        #region IOnInitListener implementation
        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                System.Diagnostics.Debug.WriteLine("spoke");
                var p = new Dictionary<string, string>();
                speaker.Speak(toSpeak, QueueMode.Flush, p);
            }
            else
                System.Diagnostics.Debug.WriteLine("was quiet");
        }
        #endregion


    }
}