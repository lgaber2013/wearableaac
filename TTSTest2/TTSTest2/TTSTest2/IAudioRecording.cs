using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description:
 * 
 * This is the 'audio recording' interface. It is the first part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the general guidelines and requirements for a platform's use of this interface.
 * 
 * This interface's methods are targeted specifically for implementing the recording and playback and management of audio files.
 * 
 * */

namespace TTSTest2
{
    public interface IAudioRecording
    {
        //specific to page 
        bool newFileInitialzed(string filename);
        void startRecording();
        void stopRecording();
        void playRecording();
        bool overwriteCurrFile(string oldName, string newName);
        void deleteFile();

        //more general use functions
        void deleteFile(string filename);
        void buttonClick(string filename);
    }
}
