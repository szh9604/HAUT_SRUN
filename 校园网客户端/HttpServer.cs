using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 校园网客户端
{
    class HttpServer : Form
    {
        // 网络连接
        public static string RequestWebAPI(string url, string sendData,String method)
        {
            string backMsg = "";
            try
            {
                System.Net.WebRequest httpRquest = System.Net.HttpWebRequest.Create(url);
                httpRquest.Timeout = 5000;
                httpRquest.Method = method;
                //这行代码很关键，不设置ContentType将导致后台参数获取不到值
                httpRquest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(sendData);
                //httpRquest.ContentLength = dataArray.Length;
                if (method.Equals("POST"))
                {
                    System.IO.Stream requestStream = null;
                    if (string.IsNullOrWhiteSpace(sendData) == false)
                    {
                        requestStream = httpRquest.GetRequestStream();
                        requestStream.Write(dataArray, 0, dataArray.Length);
                        requestStream.Close();
                    }
                    requestStream.Dispose();
                }
                System.Net.WebResponse response = httpRquest.GetResponse();
                System.IO.Stream responseStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, System.Text.Encoding.UTF8);
                backMsg = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                
                responseStream.Close();
                responseStream.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("糟糕，网络连接出现了一点点失误");
            }
            return backMsg;
        }
    }
}
