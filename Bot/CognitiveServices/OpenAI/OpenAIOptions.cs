namespace Bot.CognitiveServices.OpenAI
{
    public class OpenAIOptions
    {
        public string Credential { get; set; }
        public string Endpoint { get; set; }
        public CompletionParametersOptions? CompletionParameters { get; set; }
        public class CompletionParametersOptions
        {
            public int Max_tokens { get; set; } = 100;
            public float Temperature { get; set; } = 0.5F;
            public float Frequency_penalty { get; set; } = 0.5F;
            public float Presence_penalty { get; set; } = 0.5F;
			public float Top_p { get; set; } = 0.5F;
            public int Best_of { get; set; } = 1;
            public string[] Stop { get; set; }
        }
    }

}
