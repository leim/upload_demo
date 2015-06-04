using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace uploadDemo
{
    class Program
    {
        //boundary理论上应该是随机的一个串
        private static string boundary = "-----------------------------525258000";
        //文件名
        private static string filename;
        static void Main(string[] args)
        {
            //本地文件，在bin/debug目录下
            filename = "dnode.zip";
            string s = doUpload();
            Console.WriteLine(s);
            Console.ReadLine();

            //{"statusCode":900,"filePath":"upload/20150605/79eb0374641e6d3417ecd6f41a8e4ea5.zip"}
            //结果是一个json格式，statusCode是错误码，900代表成功，filePath就是上传之后的文件路径，将filePath的值取出，前面拼接http://myse.aliapp.com/
            //即可得到完整的文件url：http://myse.aliapp.com/upload/20150605/79eb0374641e6d3417ecd6f41a8e4ea5.zip   
        }
        public static string doUpload()
        {
            //读取文件
            FileStream fs = File.OpenRead(filename);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);


            //构造请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://myse.aliapp.com/upload");

            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Host = "myse.aliapp.com";

            //上传文件流
            try
            {
                string head = "--" + boundary + "\r\nContent-Disposition:form-data;name=\"upload\";filename=\"" + filename + "\"\r\n\r\n";
                string foot = "\r\n--" + boundary + "--\r\n";

                byte[] byteHead = Encoding.UTF8.GetBytes(head);
                byte[] byteFoot = Encoding.UTF8.GetBytes(foot);

                request.ContentLength = byteHead.Length + byteFoot.Length + buffer.Length;

                var postStream = request.GetRequestStream();

                postStream.Write(byteHead, 0, byteHead.Length);
                postStream.Write(buffer, 0, buffer.Length);
                postStream.Write(byteFoot, 0, byteFoot.Length);

                postStream.Close();

                var response = request.GetResponse();
                string resStr = "";

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        resStr = reader.ReadToEnd();
                    }
                }

                return resStr;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }
    }
}
