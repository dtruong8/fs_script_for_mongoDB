using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace files_to_mongo
{
    class Program
    {
        // Steps to run C# Console Applications into Command Program
        // Step 1: Open "Developer Command Prompt" (note: This is not the same as Command Prompt)
        // Step 2: Change directory in Developer Command Prompt to point to path of the script.
        // Step 3 (Optional): If script is not compiled or code has been updated it must be compiled by typing "CSC scriptName.CS".
        // Step 4: Run script by typing "scriptName"
    
        static void Main(string[] args) // driver class
        {
            int total_files;
            int result;
            int cnt = 0;
            string[] fileset = readDirectory();
            total_files = fileset.Length;
            
            for(int i = 0; i < fileset.Length; i++) 
            {
                
                result = mongodb_upload(fileset[i]); // upload file to mongoDB
                 
                if(result != 1) // catching errors
                {
                    Console.WriteLine("There was an issue uploading " + fileset[i]);
                }
                else
                {
                    Console.WriteLine("Successfully uploaded " + fileset[i]);
                    cnt++; // count how many successful upload(s)
                }
            }            
        }
         // retrieves files name
        static string[] readDirectory() 
        { 
            string filename;
            int cnt = 0;
            int error_cnt = 0;
            int result;

            string[] files = Directory.GetFiles(@"Z:\Saad\Mongodb\upload\photos\test", "*.pdf"); //insert pdf files from folder into array
            if(files.Length <=0) // no files found
            {
                Console.WriteLine("No files found in directory");
            }
            else
            {
                for(int i = 0; i < files.Length; i++)
                {
                    filename = Path.GetFileName(files[i]);
                    result = mongodb_upload(filename); // upload file details to webservice one at a time
                    if(result == 1)
                    {
                        cnt++; //count all successful uploads
                    }
                    else
                    {
                        error_cnt++;
                    }
                }   
            }
            Console.WriteLine(cnt);
            Console.WriteLine("This is 0: " + files[0]);
            return files;
        }

        static int mongodb_upload(string file)
        {
            int result = 1; // 1 means successful upload
            string substring_ID;
            string substring_fn;
            string service_url;

            if(!String.IsNullOrEmpty(file))
            {
                var regex = @"\((.*)\)(.*)"; //regular expression that splits file name into two groups (ID, filename)
                Match match = Regex.Match(file, regex);
                if(match.Success)
                {

                    substring_ID = match.Groups[1].ToString(); // group 1
                    substring_fn = match.Groups[2].ToString(); // group 2
                    Console.WriteLine("ID: " + substring_ID + " & File name = " + substring_fn);
                    service_url = "http://localhost:52381/upload/upload?file_id="+ substring_ID + "&filename=" + substring_fn; //this is the web service to upload mongoDB
                    WebRequest request = WebRequest.Create(service_url);
                    request.Method = "GET";
                    WebResponse response = request.GetResponse();
                    result = Int32.Parse(response.ToString());
                } 
                else
                {
                    result = 0;
                }
           
            }
            return result;
        }

    }
}
