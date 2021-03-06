using System;

namespace INotificator.Common.Models
{
    public class Options
    {
        public string ApiKey { get; set; }

        public long ChatId { get; set; }

        public DnsConfig Dns { get; set; }

        public AvitoConfig Avito { get; set; }

        public OnlinetradeConfig Onlinetrade { get; set; }

        public HpoolConfig Hpool { get; set; }

        public ToMinersConfig ToMiners { get; set; }

        public ComputerUniverseConfig ComputerUniverse { get; set; }

        public class DnsConfig
        {
            public bool IsEnabled { get; set; }
            public string Url { get; set; }
            public string Configurator { get; set; }
        }

        public class AvitoConfig
        {
            public string Url { get; set; }
            public Watcher[] Watchers { get; set; }

            public class Watcher
            {
                public string Name { get; set; }

                public bool IsEnabled { get; set; }

                public bool DisableNotification { get; set; }

                public string Path { get; set; }
            }
        }

        public class OnlinetradeConfig
        {
            public string Url { get; set; }
            public Watcher[] Watchers { get; set; }

            public class Watcher
            {
                public string Name { get; set; }

                public bool IsEnabled { get; set; }

                public string Path { get; set; }
            }
        }

        public class HpoolConfig
        {
            public bool IsEnabled { get; set; }

            public string Url { get; set; }

            public string TargetCapacity { get; set; }
        }

        public class ComputerUniverseConfig
        {
            public bool IsEnabled { get; set; }

            public string Url { get; set; }
        }

        public class ToMinersConfig
        {
            public bool IsEnabled { get; set; }

            public string Url { get; set; }

            public int ApiVersion { get; set; }
        }
    }
}