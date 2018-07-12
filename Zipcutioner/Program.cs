using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Zipcutioner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            string modFileNm = "SCORM_utilities.js";
            string oldVal = "http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash".ToLower();
            string newVal = "https://get.adobe.com/flashplayer/".ToLower();
            
            try
            {
                //var files = from file in Directory.EnumerateFiles(@"c:\temp", "*.zip", SearchOption.AllDirectories)
                //           from line in File.ReadLines(file)
                //           where line.Contains("Microsoft")
                //          select new

                var files = Directory.EnumerateFiles(@"c:\temp", "*.zip", SearchOption.AllDirectories);

                foreach (string currentFile in files)
                {
                    //string fileName = currentFile.Substring(sourceDirectory.Length + 1);
                    //Directory.Move(currentFile, Path.Combine(archiveDirectory, fileName));
                    RepString(currentFile, modFileNm, oldVal, newVal);
                }

                
                Console.WriteLine("{0} files found.", files.Count().ToString());
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

        static void RepString(String zipFileNm, string modFileNm, string oldVal, string newVal)
        {

            //https://stackoverflow.com/questions/46810169/overwrite-contents-of-ziparchiveentry
            using (var archive = ZipFile.Open(zipFileNm, ZipArchiveMode.Update))
            {
                StringBuilder document;
                var entry = archive.GetEntry(modFileNm);//entry contents "foobar123"
                if (entry == null) { return; }
                using (StreamReader reader = new StreamReader(entry.Open()))
                {
                    document = new StringBuilder(reader.ReadToEnd());
                }

                entry.Delete();
                entry = archive.CreateEntry(modFileNm);
                document.Replace(oldVal, newVal); 

                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    writer.Write(document);
                }
            }
        }
    }
}

