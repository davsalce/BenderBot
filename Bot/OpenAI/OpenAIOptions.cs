namespace Bot.OpenAI
{
    public class OpenAIOptions
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public SeriesRecommendationOptions? SeriesRecommendations { get; set; }
        public class SeriesRecommendationOptions
        {
            public int Max_tokens { get; set; }
            public int Temperature { get; set; }
            public float Frequency_penalty { get; set; }
            public int Presence_penalty { get; set; }
            public float Top_p { get; set; }
            public int Best_of { get; set; }
            public string[] Stop { get; set; }
        }
    }

}
