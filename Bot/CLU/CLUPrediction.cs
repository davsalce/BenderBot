using System.Text.Json.Serialization;

namespace Bot.CLU
{
    public class CLUPrediction
    {
        [JsonPropertyName("topIntent")]
        public string TopIntent { get; set; }
        public string ProjectKind { get; set; }
        public Intent[] Intents { get; set; }
        public Entity[] Entities { get; set; }

        public class Intent
        {
            public string Category { get; set; }
            public float ConfidenceScore { get; set; }
        }

        public class Entity
        {
            public string Category { get; set; }
            public string Text { get; set; }
            public int Offset { get; set; }
            public int Length { get; set; }
            public int ConfidenceScore { get; set; }
            public Resolution[] Resolutions { get; set; }
            public Extrainformation[] ExtraInformation { get; set; }
        }

        public class Resolution
        {
            public string ResolutionKind { get; set; }
            public string NumberKind { get; set; }
            public string RelativeTo { get; set; }
            public DateTime? Begin { get; set; }
            public DateTime? End { get; set; }
            public object Value { get; set; }
            public string dateTimeSubKind { get; set; }
            public string timex { get; set; }
            public string numberKind { get; set; }
        }

        public class Extrainformation
        {
            public string ExtraInformationKind { get; set; }
            public string Value { get; set; }
        }
    }
}
