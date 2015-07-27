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

using System.IO;
using Android.Media;

/*
 * Description:
 * 
 * This is the AudioRecording_Android class. It is the second part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the android platform's implementation of the IAudioRecording interface.
 * 
 * */

[assembly: Dependency(typeof(AudioRecording_Android))]
namespace TTSTest2.Droid
{
    public class AudioRecording_Android : IAudioRecording
    {
        MediaRecorder _recorder = new MediaRecorder();
        MediaPlayer _player = new MediaPlayer();
        string filen = "";
        string path = "";

        public AudioRecording_Android() 
            //constructor
        {

        }

        public bool newFileInitialzed(string filename)
            //pre: string filename is the last enterd or set 'audio file name' text on the todoitemaudiopage that calls this method.
            //(this is called before anything actually happens, in the onappearing method.)
            //post: a new file is made with this name if required, if not nothing new happens because the file exists.
            //true is returned when a new file is initialized, false if not.
        {
            filen = filename;
            path = "/sdcard/" + filen + ".3gpp";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, path);

            if (System.IO.File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine("file already exists");
                return false;
            }
            else
            {
                System.IO.File.Create(filePath).Dispose(); //if this isn't disposed an error happens.
                System.Diagnostics.Debug.WriteLine("created a file");
                return true; //means a brand new thing was created
            }
        }

        public void startRecording()
            //post: starts recording any audio input from the phone's microphone.
        {
            System.Diagnostics.Debug.WriteLine("path = " + path);

            try
            {
                _recorder.SetAudioSource(AudioSource.Mic);
            }
            catch (Java.Lang.RuntimeException e)
            {
                System.Diagnostics.Debug.WriteLine("WHAT IS WRONG: " + e.Message);
            }
            _recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            _recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            _recorder.SetOutputFile(path);
            _recorder.Prepare();
            _recorder.Start();
        }

        public void stopRecording()
            //post: stops recording any audio input from the phone's microphone.
        {
            _recorder.Stop();
            _recorder.Reset();
        }


        public void playRecording()
            //post: plays the recording saved at path.
        {
            _player.SetDataSource(path);
            //_player.SetVolume(100, 100); //no effect
            _player.Prepare();
            _player.Start();
            while (_player.IsPlaying) 
            { }

            //then reset things
            _recorder = new MediaRecorder();
            _player = new MediaPlayer();
            _player.Reset();
        }

        public bool overwriteCurrFile(string oldName, string newName)
            //pre: string oldName is the old audio file name, string newName is the new audio file name desired
            //post: the current audio file being worked on is saved under a different name.
            //Comment: old name is going to be whatever it was initialized as, and new name is going to be whatever's in there 
            //but before you call this you need to check if they are equivalent to eachother...
        {
            if (oldName == filen && newName != null && newName != "")
            {
                //the old name has to equal what was initialized at the beginning, 
                //but we also have to update filen here to equal new name
                string path1 = "/sdcard/" + filen + ".3gpp";
                var documentsPath1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath1 = Path.Combine(documentsPath1, path1);

                string path2 = "/sdcard/" + newName + ".3gpp";
                var documentsPath2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath2 = Path.Combine(documentsPath2, path2);

                if (System.IO.File.Exists(filePath1) && !System.IO.File.Exists(filePath2))
                {
                    try
                    {
                        System.IO.File.Move(filePath1, filePath2);
                        filen = newName;
                        path = "/sdcard/" + newName + ".3gpp";
                        return true;
                    }
                    catch (System.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("the probobobolem is: " + e.Message);
                        return false;
                    }
                }
            }
            return false;
        }


        public void deleteFile()  
            //post: deletes the current adio file being worked on
            //comment: called when you decide to delete an audio todo.
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, path);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }


        //most general use

        public void buttonClick(string filename) 
            //pre: string file name is supposedly the name of an audio file saved on the phone's sd card and recorded using the app.
            //post: play sthe audio file with this filename (no existance checks because those already happened)
            //comment: plays any file, called while listening for button presses
        {
            _player.SetDataSource("/sdcard/" + filename + ".3gpp");
            //_player.SetVolume(100, 100); //no effect
            _player.Prepare();
            _player.Start();
            while (_player.IsPlaying)
            { }

            //then reset things
            _recorder = new MediaRecorder();
            _player = new MediaPlayer();
            _player.Reset();
        }


        public void deleteFile(string filename) 
            //pre: string filename is the name of a file you want to delete
            //post: if the file exists it is deleted.
            //Comment: called when you do a wipe of all todoitems
        {
            string path1 = "/sdcard/" + filename + ".3gpp";
            var documentsPath1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath1 = Path.Combine(documentsPath1, path1);

            if (System.IO.File.Exists(filePath1))
            {
                System.IO.File.Delete(filePath1);
            }
        }

    }
}