namespace Bot.OpenAI
{
    // Define a class for OpenAI response
    public class OpenAIResponse
    {
        // A property for the id value
        public string Id { get; set; }
        // A property for the object value
        public string Object { get; set; }
        // A property for the created value
        public long Created { get; set; }
        // A property for the model value
        public string Model { get; set; }
        // A property for the choices value
        public List<Choice> Choices { get; set; }
        // A property for the usage value
        public Usage Usage { get; set; }
    }

    // Define a class for Choice 
    public class Choice
    {
        // A property for the text value 
        public string Text { get; set; }
        // A property for the index value 
        public int Index { get; set; }
        // A property for the finish_reason value 
        public string FinishReason { get; set; }
        // A property for the logprobs value 
        public object Logprobs { get; set; }
    }

    // Define a class for Usage 
    public class Usage
    {
        // A property for the completion_tokens value 
        public int CompletionTokens { get; set; }
        // A property for the prompt_tokens value 
        public int PromptTokens { get; set; }
        // A property for the total_tokens value 
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
