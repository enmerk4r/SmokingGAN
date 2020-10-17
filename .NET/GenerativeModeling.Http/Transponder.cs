using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace GenerativeModeling.Http
{
    public static class Transponder
    {
        public static string Post(string route, object body)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);
                request.ContentType = "application/json";
                request.Method = "POST";
                string json = JsonConvert.SerializeObject(body);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                return null;
            }
           
        }
    }
}
