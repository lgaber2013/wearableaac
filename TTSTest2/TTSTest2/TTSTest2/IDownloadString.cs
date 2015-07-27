using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Description:
 * 
 * This is the 'downloading stuff from the internet' interface. It is the first part of the requirements for using a 
 * DependencyService method, which we need to use because somee code needs to be written specific to a platform.
 * Thus, this serves as the general guidelines and requirements for a platform's use of this interface.
 * 
 * This interface's methods are targeted specifically for implementing the grabbing of button press information from the internet
 * in order to convert it into a WebDataItem object array wich aids in easier analysis.
 * 
 * */

namespace TTSTest2
{
    public interface IDownloadString
    {
        string DownloadAsString(string url);
        byte[] DownloadAsByteArr(string url);
    }
}