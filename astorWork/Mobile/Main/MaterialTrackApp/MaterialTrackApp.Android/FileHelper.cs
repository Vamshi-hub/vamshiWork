using System;
using System.IO;
using Xamarin.Forms;
using MaterialTrackApp.Interface;
using MaterialTrackApp.Droid;

[assembly: Dependency(typeof(FileHelper))]
namespace MaterialTrackApp.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}