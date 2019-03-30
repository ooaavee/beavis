using System;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandBehaviourAttribute : Attribute
    {
        private readonly bool _isVisibleForHelp;
        private readonly bool _isTabCompletionEnabled;

        public CommandBehaviourAttribute(): this(true, true)
        {
        }

        public CommandBehaviourAttribute(bool isVisibleForIsVisibleForHelp, bool isTabCompletionEnabledCompletionEnabled)
        {
            _isVisibleForHelp = isVisibleForIsVisibleForHelp;
            _isTabCompletionEnabled = isTabCompletionEnabledCompletionEnabled;
        }

        public virtual bool IsVisibleForHelp(ICommand cmd, HttpContext context)
        {
            return _isVisibleForHelp;
        }

        public virtual bool IsTabCompletionEnabled(ICommand cmd, HttpContext context)
        {
            return _isTabCompletionEnabled;
        }
    }
}