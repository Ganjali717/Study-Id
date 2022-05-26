using System.Collections.Specialized;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;

namespace StudyId.WebRequestManager
{
    public abstract class WebRequestManager
    {
        public string proxy = string.Empty;
        protected HttpWebRequest GenerateBaseWebRequest(string link, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = method;
            if (method == "GET")
            {
                request.Referer = link;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.AllowAutoRedirect = false;
                request.Headers.Add("Accept-Language", "ru,en-US;q=0.7,en;q=0.3");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36";
                request.ServicePoint.Expect100Continue = false;
                request.KeepAlive = true;
            }
            else
            {
                request.Accept = "Accept: */*";
                request.Referer = link;
                request.KeepAlive = true;
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                request.Headers.Add("Upgrade-Insecure-Requests: 1");
                request.Headers.Add("Cache-Control: max-age=0");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.ServicePoint.Expect100Continue = false;
                request.KeepAlive = true;
            }
            if (!string.IsNullOrEmpty(proxy))
            {
                request.Proxy = new WebProxy(proxy);
            }
            return request;
        }
        protected string GetResponceForRequest(HttpWebRequest request)
        {
            try
            {
                var responce = (HttpWebResponse)request.GetResponse();
                var responceStream = responce.GetResponseStream();
                if (responceStream == null) throw new Exception("ResponceStream empty");
                if (responce.Headers.Get("Content-Encoding") != null && responce.Headers.Get("Content-Encoding") == "br")
                {
                    using (var decompress = new BrotliStream(responceStream, CompressionMode.Decompress))
                    {
                        using (var reader = new StreamReader(decompress, Encoding.UTF8))
                        {
                            var responceData = reader.ReadToEnd();
                            return responceData;
                        }
                    }
                }
                else if (responce.Headers.Get("Content-Encoding") != null && responce.Headers.Get("Content-Encoding") == "gzip")
                {
                    using (var decompress = new GZipStream(responceStream, CompressionMode.Decompress))
                    {
                        using (var reader = new StreamReader(decompress, Encoding.UTF8))
                        {
                            var responceData = reader.ReadToEnd();
                            return responceData;
                        }
                    }
                }
                else if (responce.Headers.Get("Content-Encoding") != null && responce.Headers.Get("Content-Encoding") == "deflate")
                {
                    using (var decompress = new DeflateStream(responceStream, CompressionMode.Decompress))
                    {
                        using (var reader = new StreamReader(decompress, Encoding.UTF8))
                        {
                            var responceData = reader.ReadToEnd();
                            return responceData;
                        }
                    }
                }
                else
                {
                    using (var data = responce.GetResponseStream())
                    {
                        var reader = new StreamReader(data);
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return string.Empty;
        }
        protected void WriteDataToRequest(HttpWebRequest requestInfo, NameValueCollection collection)
        {
            var dateResult = GetRequestData(collection);
            var requestData = Encoding.UTF8.GetBytes(dateResult);
            WriteDataToRequest(requestInfo, requestData);
        }
        protected void WriteDataToRequest(HttpWebRequest requestInfo, byte[] requestData)
        {
            if (requestData.Length > 0)
            {
                requestInfo.ContentLength = requestData.Length;
                var stream = requestInfo.GetRequestStream();
                stream.Write(requestData, 0, requestData.Length);
                stream.Close();
            }
            else
            {
                requestInfo.ContentLength = 0;
            }
        }
        internal string GetRequestData(NameValueCollection data)
        {
            var splitter = string.Empty;
            var builder = new StringBuilder();
            foreach (var line in data.AllKeys)
            {
                builder.Append(splitter);
                builder.Append(HttpUtility.UrlEncode(line));
                builder.Append("=");
                builder.Append(HttpUtility.UrlEncode(data[line]));
                splitter = "&";
            }
            return builder.ToString();
        }


    }
}
