using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace voice
{
    class Program
    {
        //string cuid = "00-FF-E9-AC-EB-9A";//可以随便写
        static string serverURL = "http://vop.baidu.com/server_api";
        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = "XOsO2fM2vp8Ge9uryKMI30BX";
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = "BAMtDFQMb84RS2kM9eht2lThR1Nqg7vm";

        private static String getAccessToken()
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            String result_final = "";
            Console.WriteLine(result);
            if (result.Length != 0)
            {
                String[] indexs = result.Split(',');
                foreach (String index in indexs)
                {
                    String[] _indexs = index.Split('\"');
                    if (_indexs[2] == ":")
                    {
                        result_final = _indexs[3];
                        break;
                    }
                }
            }
            return result_final;
        }

        private static string Post(string audioFilePath,String token)
        {
            serverURL += "?lan=zh&cuid=00-FF-E9-AC-EB-9A&token=" + token;
            FileStream fs = new FileStream(audioFilePath, FileMode.Open);
            byte[] voice = new byte[fs.Length];
            fs.Read(voice, 0, voice.Length);
            fs.Close();
            fs.Dispose();

            HttpWebRequest request = null;

            Uri uri = new Uri(serverURL);
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = 5000;
            request.Method = "POST";
            request.ContentType = "audio/wav; rate=16000";
            request.ContentLength = voice.Length;
            try
            {
                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(voice, 0, voice.Length);
                    writeStream.Close();
                    writeStream.Dispose();
                }
            }
            catch
            {
                return null;
            }
            string result = string.Empty;
            string result_final = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string line = string.Empty;
                        StringBuilder sb = new StringBuilder();
                        while (!readStream.EndOfStream)
                        {
                            line = readStream.ReadLine();
                            sb.Append(line);
                            sb.Append("\r");
                        }
                        readStream.Close();
                        readStream.Dispose();
                        result = sb.ToString();
                        string[] indexs = result.Split(',');
                        foreach (string index in indexs)
                        {
                            string[] _indexs = index.Split('"');
                            if (_indexs[2] == ":[")
                                result_final = _indexs[3];
                        }
                    }
                    responseStream.Close();
                    responseStream.Dispose();
                }
                response.Close();
            }
            return result_final;
        }
        
        static void Main(string[] args)
        {
            string audioFilePath = "C:/Users/admin/Desktop/16k.wav";
            string resuilt = Post(audioFilePath,getAccessToken());
            Console.WriteLine(resuilt);
            Console.ReadLine();

        }
    }
}
