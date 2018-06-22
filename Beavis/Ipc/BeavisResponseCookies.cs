using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Beavis.Ipc
{
    public class BeavisResponseCookies : IResponseCookies
    {
        public const int ActionTypeAppend1 = 1;
        public const int ActionTypeAppend2 = 2;
        public const int ActionTypeDelete1 = 3;
        public const int ActionTypeDelete2 = 4;

        public List<Action> Actions { get; set; } = new List<Action>();

        public void Append(string key, string value)
        {
            Actions.Add(new Action
            {
                ActionType = ActionTypeAppend1,
                Key = key,
                Value = value
            });
        }

        public void Append(string key, string value, CookieOptions options)
        {
            Actions.Add(new Action
            {
                ActionType = ActionTypeAppend2,
                Key = key,
                Value = value,
                Options = options
            });
        }

        public void Delete(string key)
        {
            Actions.Add(new Action
            {
                ActionType = ActionTypeDelete1,
                Key = key
            });
        }

        public void Delete(string key, CookieOptions options)
        {
            Actions.Add(new Action
            {
                ActionType = ActionTypeDelete2,
                Key = key,
                Options = options
            });
        }

        public sealed class Action
        {
            public int ActionType { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public CookieOptions Options { get; set; }
        }
    }
}