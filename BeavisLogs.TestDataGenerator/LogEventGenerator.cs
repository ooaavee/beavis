using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NLipsum.Core;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.AzureTableStorage;
using Serilog.Sinks.AzureTableStorage.KeyGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BeavisLogs.TestDataGenerator
{
    public class LogEventGenerator
    {
        private int _counter;
        private readonly Random _rnd = new Random();
        private readonly SerilogAzureTableStorageOptions _options;

        private static readonly LogLevel[] Levels = {
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Information,
            LogLevel.Warning,
            LogLevel.Error,
            LogLevel.Critical
        };

        public LogEventGenerator(IOptions<SerilogAzureTableStorageOptions> options)
        {
            _options = options.Value;
        }

        public void GenerateLogEvents()
        {
            _options.Sinks.Select(sink =>
            {
                var thread = new Thread(Start);
                thread.Start(sink);
                return thread;
            }).ToList().ForEach(t => t.Join());
        }

        private void Start(object obj)
        {
            var sink = (SerilogAzureTableStorageOptions.Sink) obj;
            var index = _options.Sinks.IndexOf(sink);
            var lipsum = GetLipsum(index);
            Run(new Handle {Sink = sink, Lipsum = lipsum});
        }

        private void Run(Handle handle)
        {
            // generate random number of log events
            var rnd = new Random();
            var count = rnd.Next(1, 500);

            var entities = new List<LogEventEntity>();

            for (var i = 0; i < count; i++)
            {
                var entity = CreateLogEvent(handle);
                entities.Add(entity);

                var millis = rnd.Next(5, 300);
                Thread.Sleep(millis);

                Interlocked.Increment(ref _counter);
            }

            SaveLogEntities(entities, handle);
        }
       
        private LogEventEntity CreateLogEvent(Handle handle)
        {            
            var now = DateTimeOffset.UtcNow;
            var text = GetRandomSentence(handle);          
            var mt = new MessageTemplate(new[] { new TextToken(text) });

            // get random log level
            var level = Levels[_rnd.Next(0, Levels.Length)];

            LogEvent e;

            switch (level)
            {
                case LogLevel.Trace:
                    e = new LogEvent(now, LogEventLevel.Verbose, null, mt, new List<LogEventProperty>());
                    break;

                case LogLevel.Debug:
                    e = new LogEvent(now, LogEventLevel.Debug, null, mt, new List<LogEventProperty>());
                    break;

                case LogLevel.Information:
                    e = new LogEvent(now, LogEventLevel.Information, null, mt, new List<LogEventProperty>());
                    break;
                case LogLevel.Warning:
                    e = new LogEvent(now, LogEventLevel.Warning, null, mt, new List<LogEventProperty>());
                    break;

                case LogLevel.Error:
                    e = new LogEvent(now, LogEventLevel.Error, GenerateRandomException(handle), mt, new List<LogEventProperty>());

                    break;
                case LogLevel.Critical:
                    e = new LogEvent(now, LogEventLevel.Fatal, GenerateRandomException(handle), mt, new List<LogEventProperty>());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var partitionKey = handle.DefaultKeyGenerator.GeneratePartitionKey(e);
            var rowKey = handle.DefaultKeyGenerator.GenerateRowKey(e);

            var entity = new LogEventEntity(e, CultureInfo.InvariantCulture, partitionKey, rowKey);
            return entity;
        }

        private Exception GenerateRandomException(Handle handle)
        {
            var message = GetRandomSentence(handle);

            Exception innerException = null;
            if (_rnd.Next(0, 2) == 1)
            {
                innerException = GenerateRandomException(handle);
            }

            try
            {
                // get random exception
                switch (_rnd.Next(0, 3))
                {
                    case 0:
                        if (innerException != null)
                        {
                            throw new InvalidOperationException(message, innerException);
                        }
                        else
                        {
                            throw new InvalidOperationException(message);
                        }

                    case 1:
                        if (innerException != null)
                        {
                            throw new DataException(message, innerException);
                        }
                        else
                        {
                            throw new DataException(message);
                        }

                    case 2:
                        if (innerException != null)
                        {
                            throw new ArgumentOutOfRangeException(message, innerException);
                        }
                        else
                        {
                            throw new DataException(message);
                        }
                }
            }
            catch (Exception e)
            {
                return e;
            }

            throw new InvalidOperationException("fuck");
        }

        private static string GetLipsum(int index)
        {
            var lipsums = new List<string>
            {
                Lipsums.ChildHarold,
                Lipsums.Decameron,
                Lipsums.RobinsonoKruso,
                Lipsums.TheRaven,
                Lipsums.Faust,
                Lipsums.InDerFremde,
                Lipsums.LeBateauIvre,
                Lipsums.LeMasque,
                Lipsums.LoremIpsum,
                Lipsums.NagyonFaj,
                Lipsums.Omagyar,
                Lipsums.TierrayLuna
            };

            var counter = 0;

            while (true)
            {
                foreach (var t in lipsums)
                {
                    if (counter == index)
                    {
                        return t;
                    }
                    counter++;
                }
            }
        }

        private static string GetRandomSentence(Handle handle)
        {
            string FirstLetterToUpper(string str)
            {
                if (str.Length > 1)
                {
                    return char.ToUpper(str[0]) + str.Substring(1);
                }
                return str.ToUpper();
            }

            var text = LipsumGenerator.Generate(1, Features.Sentences, null, handle.Lipsum);
            text = FirstLetterToUpper(text);
            text = text + ".";
            return text;
        }
         
        private void SaveLogEntities(List<LogEventEntity> entities, Handle handle)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(handle.Sink.ConnectionString);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(handle.Sink.TableName);
            table.CreateIfNotExistsAsync().Wait();
          
            foreach (LogEventEntity entity in entities)
            {
                table.ExecuteAsync(TableOperation.Insert(entity)).Wait();
            }
        }

        private class Handle
        {
            public SerilogAzureTableStorageOptions.Sink Sink { get; set; }
            public string Lipsum { get; set; }
            public DefaultKeyGenerator DefaultKeyGenerator { get; set; } = new DefaultKeyGenerator();
        }
    }
}
