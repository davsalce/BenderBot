namespace Bot.Bot.Channels.DirectLine
{
    public class DirectLineActivitiesResult
    {
            public List<Activity> activities { get; set; }
            public string watermark { get; set; }

        public class Activity
        {
            public string type { get; set; }
            public string id { get; set; }
            public DateTime timestamp { get; set; }
            public string serviceUrl { get; set; }
            public string channelId { get; set; }
            public From from { get; set; }
            public Conversation conversation { get; set; }
            public string locale { get; set; }
            public List<Entity> entities { get; set; }
            public Channeldata channelData { get; set; }
            public Value value { get; set; }
            public string name { get; set; }
            public string text { get; set; }
            public string inputHint { get; set; }
        }

        public class From
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Conversation
        {
            public string id { get; set; }
        }

        public class Channeldata
        {
            public string clientActivityID { get; set; }
            public DateTime clientTimestamp { get; set; }
        }

        public class Value
        {
            public string locale { get; set; }
        }

        public class Entity
        {
            public string type { get; set; }
            public bool requiresBotState { get; set; }
            public bool supportsListening { get; set; }
            public bool supportsTts { get; set; }
        }

    }
}
