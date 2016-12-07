using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GIS
{
    public class FileLister
    {
        public string StartPath;

        public FileLister(string StartPath)
        {
            StartPath = StartPath.Trim();
            if (StartPath.EndsWith("\\"))
                StartPath = StartPath.Substring(0, StartPath.Length - 1);
            if (!Directory.Exists(StartPath))
                throw new Exception("Path " + StartPath + " could not be found.");
            this.StartPath = StartPath;
        }


        public List<string> FindFiles(string SearchPattern, bool Recursive = true)
        {
            List<string> FileList = new List<string>(20);
            DirectoryInfo D = new DirectoryInfo(StartPath);
            FileInfo [] FileInfor = null;
            if (Recursive)
                 FileInfor = D.GetFiles(SearchPattern, SearchOption.AllDirectories);
            else
                FileInfor = D.GetFiles(SearchPattern, SearchOption.TopDirectoryOnly);
            foreach (FileInfo Info in FileInfor)
                FileList.Add(Info.FullName);
            return FileList;
        }

        public int CountFiles(string SearchPattern, bool Recursive = true) 
        {
            List<string> FileList = FindFiles(SearchPattern, Recursive);
            int Count = FileList.Count;
            FileList = null;
            return Count;
        }


        public List<string> FindSubDirs(bool Recursive = true)
        {
            List<string> DirList = new List<string>(20);
            DirectoryInfo[] SubDir = null;
            DirectoryInfo D = new DirectoryInfo(StartPath);
            if (Recursive)
                SubDir = D.GetDirectories("*", SearchOption.AllDirectories);
            else
                SubDir = D.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (DirectoryInfo Dir in SubDir)
                DirList.Add(Dir.FullName);
            return DirList;
        }


        public void Cleanup()
        {
            StartPath = "ERROR";
        }


        public static void Cleanup(FileLister Lister)
        {
            Lister.Cleanup();
        }

        ~FileLister()
        {
            Cleanup();
        }
    }
}
