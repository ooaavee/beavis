using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Beavis.Ipc
{
    public static class BeavisProtocol
    {
        public static string CreateRequestMessage(HttpRequest request)
        {
            var model = CreateRequestModel(request);
            var json = JsonConvert.SerializeObject(model, Formatting.None);
            var base64 = Base64Encode(json);
            return base64;
        }

        public static string CreateResponseMessage(BeavisHttpResponse response, BeavisProtocolResponseStatus status)
        {
            string responseMessage;

            if (status == BeavisProtocolResponseStatus.Failed)
            {
                responseMessage = ((int)status).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                var model = CreateResponseModel(response);
                var json = JsonConvert.SerializeObject(model, Formatting.None);
                var base64 = Base64Encode(json);
                responseMessage = $"{((int)status).ToString(CultureInfo.InvariantCulture)}{base64}";
            }

            return responseMessage;
        }

        public static HttpRequestModel CreateRequestModel(string requestMessage)
        {
            string json = Base64Decode(requestMessage);
            HttpRequestModel model = JsonConvert.DeserializeObject<HttpRequestModel>(json);
            return model;
        }

        public static HttpResponseModel CreateResponseModel(string responseMessage, out BeavisProtocolResponseStatus status)
        {
            HttpResponseModel model = null;

            status = GetResponseStatus(responseMessage);

            if (status == BeavisProtocolResponseStatus.Succeed)
            {
                var payload = GetResponsePayload(responseMessage);
                var json = Base64Decode(payload);
                model = JsonConvert.DeserializeObject<HttpResponseModel>(json);
            }

            return model;
        }

        private static BeavisProtocolResponseStatus GetResponseStatus(string responseMessage)
        {
            var s = responseMessage.Substring(0, 1);
            var i = Int32.Parse(s);
            var v = (BeavisProtocolResponseStatus) i;
            return v;
        }

        private static string GetResponsePayload(string responseMessage)
        {
            var payload = responseMessage.Substring(1);
            return payload;
        }

        private static string Base64Encode(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        private static string Base64Decode(string encoded)
        {
            var bytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(bytes);
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

            var stream = (MemoryStream)response.Body;
            model.Body = stream.ToArray();
            stream.Dispose();

            model.ContentLength = response.ContentLength;
            model.ContentType = response.ContentType;
            model.Cookies = (BeavisResponseCookies)response.Cookies;
            model.HasRedirect = response.HasRedirect;
            model.RedirectLocation = response.RedirectLocation;
            model.IsRedirectPermanent = response.IsRedirectPermanent;

            return model;
        }
    }
}
