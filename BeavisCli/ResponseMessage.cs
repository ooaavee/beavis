﻿using Newtonsoft.Json;

namespace BeavisCli
{
    /// <summary>
    /// Response message
    /// </summary>
    public class ResponseMessage
    {
        [JsonProperty("text")]
        public virtual string Text { get; set; }

        [JsonProperty("type")]
        public virtual string Type { get; set; }

        public static ResponseMessage Error(string text)
        {
            return new ResponseMessage
            {
                Text = text ?? string.Empty,
                Type = ResponseMessageTypes.Error.ToString()
            };
        }

        public static ResponseMessage Plain(string text)
        {
            return new ResponseMessage
            {
                Text = text ?? string.Empty,
                Type = ResponseMessageTypes.Plain.ToString()
            };
        }

        public static ResponseMessage Success(string text)
        {
            return new ResponseMessage
            {
                Text = text ?? string.Empty,
                Type = ResponseMessageTypes.Success.ToString()
            };
        }
    }
}
