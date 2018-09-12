using BeavisLogs.Models.Logs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BeavisLogs.Drivers.Serilog.AzureTableStorage
{
    public sealed class LogEventTableEntity : TableEntity, IDictionary<string, EntityProperty>, ILogEvent
    {
        private IDictionary<string, EntityProperty> _properties;

        public LogEventTableEntity()
        {
            _properties = new Dictionary<string, EntityProperty>();
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            _properties = properties;
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return _properties;
        }

        public void Add(string key, EntityProperty value)
        {
            _properties.Add(key, value);
        }

        public void Add(string key, bool value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, byte[] value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, DateTime? value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, DateTimeOffset? value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, double value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, Guid value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, int value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, long value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public void Add(string key, string value)
        {
            _properties.Add(key, new EntityProperty(value));
        }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        public ICollection<string> Keys => _properties.Keys;

        public bool Remove(string key)
        {
            return _properties.Remove(key);
        }

        public bool TryGetValue(string key, out EntityProperty value)
        {
            return _properties.TryGetValue(key, out value);
        }

        public ICollection<EntityProperty> Values => _properties.Values;

        public EntityProperty this[string key]
        {
            get => _properties[key];
            set => _properties[key] = value;
        }

        public void Add(KeyValuePair<string, EntityProperty> item)
        {
            _properties.Add(item);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Contains(KeyValuePair<string, EntityProperty> item)
        {
            return _properties.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, EntityProperty>[] array, int arrayIndex)
        {
            _properties.CopyTo(array, arrayIndex);
        }

        public int Count => _properties.Count;

        public bool IsReadOnly => _properties.IsReadOnly;

        public bool Remove(KeyValuePair<string, EntityProperty> item)
        {
            return _properties.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, EntityProperty>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        #region ILogEvent

        private IDictionary<string, object> _values;

        DateTimeOffset ILogEvent.Timestamp => (DateTimeOffset)_values[nameof(ILogEvent.Timestamp)];

        Microsoft.Extensions.Logging.LogLevel ILogEvent.Level => (Microsoft.Extensions.Logging.LogLevel)_values[nameof(ILogEvent.Level)];

        string ILogEvent.Message => (string)_values[nameof(ILogEvent.Message)];

        string ILogEvent.Exception => (string)_values[nameof(ILogEvent.Exception)];

        Dictionary<string, object> ILogEvent.Properties => (Dictionary<string, object>)_values[nameof(ILogEvent.Properties)];

        void ILogEvent.ReadLogEvent(IDictionary<string, object> values)
        {
            _values = values;
        }

        #endregion
    }
}
