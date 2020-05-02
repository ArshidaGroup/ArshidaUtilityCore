using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace ArshidaCoreUtility
{
    public enum httpVerb
    {
        GET, POST, PUT, DELETE
    }
    public class RestClient
    {
        public string endPoint { get; set; }
        public httpVerb httpMethod { get; set; }

        public RestClient(string v)
        {
            endPoint = string.Empty;
            httpMethod = httpVerb.GET;
        }
        public  string makeRequest()
        {
            string strResponseValue = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = httpMethod.ToString();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new NotImplementedException();
                }
                // Process the rewsponse stream ... (could be JSON, XML or HTML etc ...)

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();
                        }//End of StreamReader
                    }
                }
                // End of using ResponseStream 

            }//End of using response

            return strResponseValue;
        }
    }
}
