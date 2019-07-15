using Newtonsoft.Json;
using System;
using System.Net;

namespace ISC.IDRDownloader
{
    public class MyWebClient : WebClient
    {
        public MyWebClient(string baseAddress)
        {
            BaseAddress = baseAddress;
        }

        public MyWebClient(string baseAddress, string userName, string password)
        {
            BaseAddress = baseAddress;
            BearerToken = GetBearerToken(userName, password);
        }

        public MyWebClient(string baseAddress, string bearerToken)
        {
            BaseAddress = baseAddress;
            BearerToken = bearerToken;
        }

        public string BearerToken { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var web = base.GetWebRequest(uri);

            web.Timeout = 20 * 60 * 1000;

            return web;
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <typeparam name="TResponse">The type of the attribute response.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="onComplete">The configuration complete.</param>
        /// <param name="onError">The configuration error.</param>
        public TResponse Get<TResponse>(string url)
        {
            AddSecurityHeaders();

            var response = DownloadString(new Uri(url));

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(response);
            }
            catch (Exception ex)
            {
                throw new FormatException(response, ex);
            }
        }

        public TResponse Post<TResponse>(string url, object jsonData)
        {
            AddSecurityHeaders();

            //byte[] data = Encoding.UTF8.GetBytes(Helpers.ConversionHelper.ToJsonString(jsonData));
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
            var response = UploadString(new Uri(url), "POST", data);

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(response);
            }
            catch (Exception ex)
            {
                throw new FormatException(response, ex);
            }
        }

        public TResponse Delete<TResponse>(string url, object jsonData)
        {
            AddSecurityHeaders();

            string data = string.Empty;

            if (jsonData != null)
            {
                data = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
            }

            var response = UploadString(new Uri(url), "DELETE", data);

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(response);
            }
            catch (Exception ex)
            {
                throw new FormatException(response, ex);
            }
        }

        public byte[] GetFile(string url)
        {
            AddSecurityHeaders();
            return this.DownloadData(url);
        }

        public void AddSecurityHeaders()
        {
            Headers.Clear();
            Headers.Add(HttpRequestHeader.ContentType, "application/json");
            Headers.Add("Authorization", $"Bearer {BearerToken}");

            Encoding = System.Text.Encoding.UTF8;
        }

        public string GetBearerToken(string userName, string password)
        {
            string bearer = string.Empty;
            string data = string.Format($"grant_type=password&username={userName}&password={password}");

            string url = $"token";

            string response = UploadString(url, data);

            if (!string.IsNullOrEmpty(response))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(response);

                bearer = o["access_token"].ToString();
            }

            return bearer;
        }
    }
}
