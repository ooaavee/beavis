﻿using Newtonsoft.Json;

namespace BeavisCli
{
    /// <summary>
    /// This is the base class for all response messages.
    /// </summary>
    public abstract class ResponseMessage
    {
        [JsonProperty("text")]
        public virtual string Text { get; set; }

        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    /// <summary>
    /// Information message
    /// </summary>
    public sealed class SuccessMessage : ResponseMessage
    {
        public SuccessMessage(string text)
        {
            Text = text;
        }

        public override string Type => "success";
    }

    /// <summary>
    /// Error message
    /// </summary>
    public sealed class ErrorMessage : ResponseMessage
    {
        public ErrorMessage(string text)
        {
            Text = text;
        }

        public override string Type => "error";
    }

    /// <summary>
    /// Information message
    /// </summary>
    public sealed class InformationMessage : ResponseMessage
    {
        public InformationMessage(string text)
        {
            Text = text;
        }
        public override string Type => "information";
    }

}
