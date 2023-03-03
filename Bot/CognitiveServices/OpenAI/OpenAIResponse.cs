namespace Bot.CognitiveServices.OpenAI
{
    public class OpenAIResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
        public int Index { get; set; }
        public string FinishReason { get; set; }
        public object Logprobs { get; set; }
    }

    public class Usage
    {
        public int CompletionTokens { get; set; }
        public int PromptTokens { get; set; }
        public int TotalTokens { get; set; }
    }

    public class Urls
    {
        public string Netflix { get; set; }
        public string HBO { get; set; }
        public string AmazonPrime { get; set; }
        public string Movistar { get; set; }
        public string Sky { get; set; }
        public string RakutenTV { get; set; }
        public string AppleTV { get; set; }
        public string GooglePlay { get; set; }
        public string Youtube { get; set; }
        public string Filmin { get; set; }
        public string Disney { get; set; }
    }
}
