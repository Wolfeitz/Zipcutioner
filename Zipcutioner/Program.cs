using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;


namespace Zipcutioner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            string modFileNm = "SCORM_utilities.js".ToLower();
            string oldVal = "http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash";
            string newVal = "https://get.adobe.com/flashplayer/";
            string ext = "zip";
            //string folderLoc = "/Volumes/Macintosh HD/Users/robert.wolfe/temp"; //args[0];  //@"c:\temp";
            string folderLoc = args[0];

            //foreach (string vArg in args)
            //{
            //   Console.WriteLine($"Arg: {vArg}");
            //}
            //Console.WriteLine($"folderLoc: {folderLoc}");
            try
            {
                //var dirs = Directory.EnumerateDirectories(folderLoc);
                //this will get all the zip files from files and folders but not from zips
                var files = Directory.EnumerateFiles(folderLoc, "*." + ext, SearchOption.AllDirectories);

                //this will ponly get the count of files in the top level directory
                //Console.WriteLine("{0} total files found.", files.Count().ToString());
                int i = 0;

                //process all zip files in current directory
                i = DelveZips(files, modFileNm, oldVal, newVal, i);
                //i = parseDirectory(dirs, modFileNm, oldVal, newVal, ext, i);
                
                Console.WriteLine("{0} files modified.", i.ToString());
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }
          
        }

        static int parseDirectory(IEnumerable<string> dirs, string modFileNm, string oldVal, string newVal, string ext, int i)
        {
            foreach(string directory in dirs)
            {
                var ndirs = Directory.EnumerateDirectories(directory);
                //these will be zip files which can contain files, directories, or more zips
                var files = Directory.EnumerateFiles(directory, "*." + ext, SearchOption.AllDirectories);
                i = DelveZips(files, modFileNm, oldVal, newVal, i);
                i = parseDirectory(ndirs, modFileNm, oldVal, newVal, ext, i);

            }
            return i;
        }

        static int DelveZips(IEnumerable<string> files, string modFileNm, string oldVal, string newVal, int i)
        {
            foreach (string currentFile in files)
            {
                //string fileName = currentFile.Substring(sourceDirectory.Length + 1);
                //Directory.Move(currentFile, Path.Combine(archiveDirectory, fileName));

                //https://stackoverflow.com/questions/46810169/overwrite-contents-of-ziparchiveentry
                using (var archive = ZipFile.Open(currentFile, ZipArchiveMode.Update))
                {
                    StringBuilder document;

                    foreach (var fi in archive.Entries)
                    {
                        //this assumes only one of this file then exits.  :/
                        if (fi.Name.ToLower() == modFileNm)
                        {
                            using (StreamReader reader = new StreamReader(fi.Open()))
                            {
                                document = new StringBuilder(reader.ReadToEnd());
                            }

                            document.Replace(oldVal, newVal);

                            using (StreamWriter writer = new StreamWriter(fi.Open()))
                            {
                                writer.Write(document.ToString()); //entry contents "baz123123", expected "baz123"
                                writer.Flush();
                            }

                            i++;
                        }

                    }
                }
            }
            return i;
        }
    }
}