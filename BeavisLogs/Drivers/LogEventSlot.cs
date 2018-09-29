using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeavisCli;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;

namespace BeavisLogs.Drivers
{
    public class LogEventSlot
    {
        private readonly Dictionary<string, SubSlot> _subs = new Dictionary<string, SubSlot>();
        private readonly int _limit;

        private int _eventCount = 0;

        private readonly List<SlotEnumerator> _enumerators = new List<SlotEnumerator>();

        public LogEventSlot(IEnumerable<DataSourceInfo> sources, int limit = 0)
        {
            _limit = limit;

            Key = KeyUtil.GenerateKey();

            foreach (var source in sources)
            {
                _subs.Add(source.Id, new SubSlot());
                _enumerators.Add(new SlotEnumerator(this, source));
            }
        }

        public IEnumerable<SlotEnumerator> Enumerators
        {
            get
            {
                if (IsReadyToServe())
                {
                    foreach (var enumerator in _enumerators)
                    {
                        if (!enumerator.IsAllServed)
                        {
                            yield return enumerator;
                        }
                    }
                }
            }
        }

        public bool IsLimitReached { get; private set; }

        public string Key { get; }

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
                    if (_limit > 0 && _eventCount >= _limit)
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
            // - jos vain 1 sub-slot, niin on heti
            // - jos > 1 sub-slotia, niin kaikki pitää olla completed tai limitReached

            var count = _subs.Count;

            if (count == 1)
            {
                return true;
            }

            if (count > 1)
            {
                if (IsLimitReached)
                {
                    return true;
                }

                foreach (var sub in _subs.Values)
                {
                    if (!sub.IsCompleted)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private SubSlot Sub(DataSourceInfo source)
        {
            if (!_subs.TryGetValue(source.Id, out var value))
            {
                throw new InvalidOperationException($"Unable to find a sub-slot for the data source '{source.Id}'.");
            }
            return value;
        }

        private class SubSlot
        {
            public readonly object Lock = new object();
            public readonly List<ILogEvent> Events = new List<ILogEvent>();
            public readonly List<Exception> Exceptions = new List<Exception>();
            public bool IsCompleted;
            public int EventCount;
        }

        public class SlotEnumerator
        {
            private readonly LogEventSlot _slot;

            public SlotEnumerator(LogEventSlot slot, DataSourceInfo source)
            {
                _slot = slot;
                Source = source;
            }

            public bool IsAllServed { get; }

            public DataSourceInfo Source { get; }

            public ILogEvent[] Pop(int maxCount, out Exception[] exceptions, out bool pending)
            {
                SubSlot sub = _slot.Sub(Source);

                pending = true;

                ILogEvent[] events;

                lock (sub.Lock)
                {
                    exceptions = sub.Exceptions.ToArray();
                    sub.Exceptions.Clear();
             
                    int eventCount = sub.Events.Count;

                    int returnCount = maxCount <= eventCount ? maxCount : eventCount;

                    events = new ILogEvent[returnCount];

                    if (returnCount > 0)
                    {
                        sub.Events.CopyTo(0, events, 0, returnCount);

                        sub.Events.RemoveRange(0, returnCount);
                    }

                    if (sub.IsCompleted)
                    {
                        if (!sub.Events.Any())
                        {
                            pending = false;
                        }
                    }
                }

                return events;
            }
        }

    }
}
