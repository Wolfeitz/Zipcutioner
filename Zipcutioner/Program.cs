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
            string oldVal = "http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash".ToLower();
            string newVal = "https://get.adobe.com/flashplayer/".ToLower();
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
                //do we have to pass through a path to the directories or is that part of the 'name'?
                var dirs = Directory.EnumerateDirectories(folderLoc);
                var files = Directory.EnumerateFiles(folderLoc, "*." + ext, SearchOption.AllDirectories);

                //this will ponly get the count of files in the top level directory
                //Console.WriteLine("{0} total files found.", files.Count().ToString());
                int i = 0;

                //process all zip files in current directory
                i = DelveZips(files, modFileNm, oldVal, newVal, i);
                i = parseDirectory(dirs, modFileNm, oldVal, newVal, ext, i);
                
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

                i = RepString(currentFile, modFileNm, oldVal, newVal, i);
            }
            return i;
        }

        static int RepString(String zipFileNm, string modFileNm, string oldVal, string newVal, int i)
        {
            //https://stackoverflow.com/questions/46810169/overwrite-contents-of-ziparchiveentry
            using (var archive = ZipFile.Open(zipFileNm, ZipArchiveMode.Update))
            {
                StringBuilder document;
                //this seems to be seaching by fullname wich contains the path in the zip file and not simply the name
                //var entry = archive.GetEntry(modFileNm);  //entry contents "foobar123"
                //if (entry == null) { return i; }


                //ZipArchiveEntry entry = archive.GetEntry("ExistingFile.txt");
                //using (StreamWriter writer = new StreamWriter(entry.Open()))
                //{
                //    writer.BaseStream.Seek(0, SeekOrigin.End);
                //    writer.WriteLine("append line to file");
                //}
                //entry.LastWriteTime = DateTimeOffset.UtcNow.LocalDateTime;


                foreach (var fi in archive.Entries)
                {
                    //this assumes only one of this file then exits.  :/
                    if (fi.Name.ToLower() == modFileNm) 
                    {



                        //StringBuilder document;
                        //var entry = archive.GetEntry("foo.txt");//entry contents "foobar123"
                        using (StreamReader reader = new StreamReader(fi.Open()))
                        {
                            document = new StringBuilder(reader.ReadToEnd());
                        }

                        document.Replace("http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash", newVal);

                        using (StreamWriter writer = new StreamWriter(fi.Open()))
                        {
                            writer.Write(document.ToString()); //entry contents "baz123123", expected "baz123"
                            writer.Flush();
                        }




                        //using (StreamReader reader = new StreamReader(fi.Open()))
                        //{
                        //    document = new StringBuilder(reader.ReadToEnd());
                        //}

                        ////fi.Delete();
                        ////var entry = archive.CreateEntry(modFileNm);
                        ////document.Replace(oldVal.ToLower(), newVal);


                        //StreamWriter writer = new StreamWriter(entryStream);
                        //writer.WriteLine("Updated line.");
                        //writer.Flush();

                        //using (StreamWriter writer = new StreamWriter(entry.Open()))
                        //{
                        //    writer.Write(document);
                        //}
                        i++;
                        return i;
                    }
                }

               return i;
            }
        }
    }
}