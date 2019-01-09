using BeavisCli;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeavisLogs.Drivers.Renderers;

namespace BeavisLogs.Drivers
{
    public class LogEventSlot
    {
        private readonly Dictionary<string, SubSlot> _subs = new Dictionary<string, SubSlot>();
        private readonly List<SlotEnumerator> _enumerators = new List<SlotEnumerator>();
        private int _eventCount;

        public LogEventSlot(LogEventSlotProperties properties)
        {
            Properties = properties;

            foreach (DataSourceInfo source in properties.Sources)
            {
                _subs.Add(source.Id, new SubSlot());
                _enumerators.Add(new SlotEnumerator(this, source));
            }
        }

        public LogEventSlotProperties Properties { get; }

        public bool IsLimitReached { get; private set; }

        public bool Serve(HttpContext context, Response response)
        {
            LogEventOutputRenderer renderer = Properties.Renderer;

            bool more = false;

            SlotEnumerator enumerator = NextEnumerator();

            if (enumerator != null)
            {
                DataSourceInfo source = enumerator.Source;

                if (!enumerator.IsServingStarted)
                {
                    renderer.OnServingStarted(source, context, response);
                }
               
                more = enumerator.Pop(renderer.MaxBlockSize, out ILogEvent[] events, out Exception[] exceptions);

                if (!more)
                {
                    SlotEnumerator next = NextEnumerator();
                    if (next != null && !IsLimitReached)
                    {
                        more = true;
                    }
                }

                if (events.Any())
                {
                    renderer.Output(source, events, context, response);
                }

                if (exceptions.Any())
                {
                    renderer.Output(source, exceptions, context, response);
                }
            }

            if (!more)
            {
                if (IsLimitReached)
                {
                    renderer.OnLimitReached(context, response);
                }

                renderer.OnPollingCompleted(context, response);
            }

            return more;
        }

        public void OnFound(DataSourceInfo source, ILogEvent[] events)
        {
            SubSlot sub = Sub(source);

            lock (sub.Lock)
            {
                if (sub.IsCompleted)
                {
                    throw new InvalidOperationException("Already completed.");
                }

                foreach (ILogEvent e in events)
                {
                    if (Properties.Limit > 0 && _eventCount >= Properties.Limit)
                    {
                        IsLimitReached = true;
                        break;
                    }

                    sub.Events.Add(e);
                    sub.EventCount++;

                    Interlocked.Increment(ref _eventCount);
                }
            }
        }

        public void OnErrorOccurred(DataSourceInfo source, Exception ex)
        {
            SubSlot sub = Sub(source);

            lock (sub.Lock)
            {
                if (sub.IsCompleted)
                {
                    throw new InvalidOperationException("Already completed.");
                }

                sub.Exceptions.Add(ex);
            }
        }

        public void OnQueryCompleted(DataSourceInfo source)
        {
            SubSlot sub = Sub(source);

            lock (sub.Lock)
            {
                sub.IsCompleted = true;
            }
        }

        public bool IsCompleted(DataSourceInfo source)
        {
            return Sub(source).IsCompleted;
        }

        public bool IsReadyToServe()
        {
            var count = _subs.Count;

            if (count == 1)
            {
                return true;
            }

            if (count > 1)
            {
                if (IsLimitReached || IsAllCompleted())
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsAllCompleted()
        {
            foreach (var sub in _subs.Values)
            {
                lock (sub.Lock)
                {
                    if (!sub.IsCompleted)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private SubSlot Sub(DataSourceInfo source)
        {
            if (!_subs.TryGetValue(source.Id, out var value))
            {
                throw new InvalidOperationException($"Unable to find a sub-slot for the data source '{source.Id}'.");
            }
            return value;
        }

        private SlotEnumerator NextEnumerator()
        {
            foreach (var enumerator in _enumerators)
            {
                if (enumerator.IsAllServed)
                {
                    continue;
                }
                return enumerator;
            }
            return null;
        }

        private class SubSlot
        {
            public readonly object Lock = new object();
            public readonly List<ILogEvent> Events = new List<ILogEvent>();
            public readonly List<Exception> Exceptions = new List<Exception>();
            public bool IsCompleted;
            public int EventCount;
        }

        private class SlotEnumerator
        {
            private readonly LogEventSlot _slot;

            public SlotEnumerator(LogEventSlot slot, DataSourceInfo source)
            {
                _slot = slot;
                Source = source;
            }

            public bool IsAllServed { get; private set; }

            public bool IsServingStarted { get; private set; }

            public DataSourceInfo Source { get; }

            public bool Pop(int maxPopCount, out ILogEvent[] events, out Exception[] exceptions)
            {
                SubSlot sub = _slot.Sub(Source);

                IsServingStarted = true;

                bool more = true;

                lock (sub.Lock)
                {
                    exceptions = sub.Exceptions.ToArray();
                    sub.Exceptions.Clear();

                    int eventCount = sub.Events.Count;

                    int popCount = maxPopCount <= eventCount ? maxPopCount : eventCount;

                    events = new ILogEvent[popCount];

                    if (popCount > 0)
                    {
                        sub.Events.CopyTo(0, events, 0, popCount);
                        sub.Events.RemoveRange(0, popCount);
                    }

                    if (sub.IsCompleted && !sub.Events.Any())
                    {
                        more = false;
                        IsAllServed = true;
                    }
                }

                return more;
            }
        }
    }
}