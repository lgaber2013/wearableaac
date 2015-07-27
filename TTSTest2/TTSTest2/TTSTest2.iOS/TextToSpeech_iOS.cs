using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using AVFoundation;
using Xamarin.Forms;
using TTSTest2;
using TTSTest2.iOS;

/*
 * Description:
 * 
 * This is the TextToSpeech_iOS class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the iOS platform's implementation of the ITextToSpeech interface.
 * 
 * */

[assembly: Dependency (typeof (TextToSpeech_iOS))]
namespace TTSTest2.iOS
{
    public class TextToSpeech_iOS : ITextToSpeech  //need its own identical interface or does it just fugure it out later?? figures it out
    {
        public TextToSpeech_iOS() 
            //constructor
        {
        }

        /// <summary>
        /// Speak example from: 
        /// http://blog.xamarin.com/make-your-ios-7-app-speak/
        /// </summary>
        public void Speak(string text, double pitch, double speed)
            //pre: all parameters are tts parameters
            //post: the string text is spoken using tts at the given pitch and speed values.
        {
            var speechSynthesizer = new AVSpeechSynthesizer();

            var speechUtterance = new AVSpeechUtterance(text)
            {
                //needs a little work to get right... so our speed scale is .01 to 1, then 1 to 5. 
                //their 1 (maybe not a 1) is = max speech rate / 4... //not completed
                Rate = AVSpeechUtterance.MaximumSpeechRate / 4,
                Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
                Volume = 0.5f,
                PitchMultiplier = (float)pitch //1.0f //might b ok to directly replace with our pitch value
            };

            speechSynthesizer.SpeakUtterance(speechUtterance);
            System.Diagnostics.Debug.WriteLine("spoke " + speechUtterance.SpeechString + ", speech rate = " + speechUtterance.Rate);
            //I want to see this when it can be done so I can update the code here!!
        }

    }
}