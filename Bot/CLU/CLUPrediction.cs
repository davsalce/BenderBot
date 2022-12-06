using System.Text.Json.Serialization;

namespace Bot.CLU
{
    public class CLUPrediction
    {
        [JsonPropertyName("topIntent")]
        public string TopIntent { get; set; }

        [JsonPropertyName("projectKind")]
        public string ProjectKind { get; set; }
        [JsonPropertyName("intents")]
        public Intent[] Intents { get; set; }
        [JsonPropertyName("entities")]
        public Entity[] Entities { get; set; }

        public class Intent
        {
            [JsonPropertyName("category")]
            public string Category { get; set; }
            [JsonPropertyName("confidenceScore")]
            public float ConfidenceScore { get; set; }
        }

        public class Entity
        {
            [JsonPropertyName("category")]
            public string Category { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("offset")]
            public int Offset { get; set; }

            [JsonPropertyName("length")]
            public int Length { get; set; }

            [JsonPropertyName("confidenceScore")]
            public int ConfidenceScore { get; set; }

            [JsonPropertyName("resolutions")]
            public Resolution[] Resolutions { get; set; }

            [JsonPropertyName("extraInformation")]
            public Extrainformation[] ExtraInformation { get; set; }
        }

        public class Resolution
        {
            [JsonPropertyName("resolutionKind")]
            public string ResolutionKind { get; set; }

            [JsonPropertyName("numberKind")]
            public string NumberKind { get; set; }

            [JsonPropertyName("relativeTo")]
            public string RelativeTo { get; set; }

            [JsonPropertyName("begin")]
            public DateTime? Begin { get; set; }

            [JsonPropertyName("end")]
            public DateTime? End { get; set; }

            [JsonPropertyName("value")]
            [JsonConverter(typeof(ValueCustomJsonConverter))]
            public object Value { get; set; }

            [JsonPropertyName("dateTimeSubKind")]
            public string DateTimeSubKind { get; set; }

            [JsonPropertyName("timex")]
            public string Timex { get; set; }
        }

        public class Extrainformation
        {
            [JsonPropertyName("extraInformationKind")]
            public string ExtraInformationKind { get; set; }

            [JsonPropertyName("value")]
            public string Value { get; set; }

            [JsonPropertyName("key")]
            public string Key { get; set; }

        }
    }
}
