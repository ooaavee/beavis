using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Beavis.Ipc
{
    public static class BeavisProtocol
    {

        public static string CreateRequestMessage(HttpRequest request)
        {
            HttpRequestModel model = CreateRequestModel(request);
            string json = JsonConvert.SerializeObject(model, Formatting.None);
            string base64 = Base64Encode(json);
            return base64;
        }

        public static string CreateResponseMessage(BeavisHttpResponse response)
        {
            HttpResponseModel model = CreateResponseModel(response);
            string json = JsonConvert.SerializeObject(model, Formatting.None);
            string base64 = Base64Encode(json);
            return base64;
        }

        public static HttpRequestModel CreateRequestModel(string requestMessage)
        {
            string json = Base64Decode(requestMessage);
            HttpRequestModel model = JsonConvert.DeserializeObject<HttpRequestModel>(json);
            return model;
        }

        public static HttpResponseModel CreateResponseModel(string responseMessage)
        {
            string json = Base64Decode(responseMessage);
            HttpResponseModel model = JsonConvert.DeserializeObject<HttpResponseModel>(json);
            return model;
        }

        private static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
 
        private static HttpRequestModel CreateRequestModel(HttpRequest request)
        {   
            var model = new HttpRequestModel();

            model.Method = request.Method;
            model.Scheme = request.Scheme;
            model.IsHttps = request.IsHttps;
            model.Host = request.Host.ToString();
            model.PathBase = request.PathBase.ToString();
            model.Path = request.GetSubPath();
            model.QueryString = request.QueryString.ToString();

            foreach (var item in request.Query)
            {
                model.Query[item.Key] = item.Value.ToArray();
            }

            model.Protocol = request.Protocol;

            foreach (var item in request.Headers)
            {
                model.Headers[item.Key] = item.Value.ToArray();
            }

            foreach (var item in request.Cookies)
            {
                model.Cookies.Add(new List<string> { item.Key, item.Value }.ToArray());
            }

            model.ContentLength = request.ContentLength;
            model.ContentType = request.ContentType;

            var buffer = new byte[1024];
            using (var stream = new MemoryStream())
            {
                int readBytes;
                while ((readBytes = request.Body.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, readBytes);
                }
                model.Body = stream.ToArray();
            }

            model.HasFormContentType = request.HasFormContentType;

            if (request.HasFormContentType)
            {
                throw new NotImplementedException("Form content-type is not supported.");
            }

            return model;
        }

        private static HttpResponseModel CreateResponseModel(BeavisHttpResponse response)
        {
            var model = new HttpResponseModel();

            model.StatusCode = response.StatusCode;

            foreach (var item in response.Headers)
            {
                model.Headers[item.Key] = item.Value.ToArray();
            }

            var stream = (MemoryStream) response.Body;
            model.Body = stream.ToArray();
            stream.Dispose();

            model.ContentLength = response.ContentLength;
            model.ContentType = response.ContentType;
            model.Cookies = (BeavisResponseCookies) response.Cookies;          
            model.HasRedirect = response.HasRedirect;
            model.RedirectLocation = response.RedirectLocation;
            model.IsRedirectPermanent = response.IsRedirectPermanent;

            return model;
        }      
    }
}
