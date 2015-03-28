using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;


namespace client
{

    public class HttpHandler
    {

        public string addPost(string id,string text,string key)
        {
            string surl;
            surl = "http://localhost:81/add.php";
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["id"] = id;
                values["text"] = text;
                values["key"] = key;
                var response = client.UploadValues(surl, values);

                var responseString = Encoding.Default.GetString(response);
                return responseString; 
            }

            
        }

        public string getPost(string id)
        {
            string address = string.Format(
   "http://localhost:81/get.php?id={0}",
   Uri.EscapeDataString(id));
            string text;
            using (WebClient client = new WebClient())
            {
                text = client.DownloadString(address);
            }


            if (text == "Error - Not found")
            {
                return null;
            }
            else
            {
                return text;
            }
        }
    }
}
