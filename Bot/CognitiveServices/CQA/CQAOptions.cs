namespace Bot.CognitiveServices.CQA
{
    public class CQAOptions
    {
        public Customquestionansweringclient CustomQuestionAnsweringClient { get; set; }
        public Questionansweringproject QuestionAnsweringProject { get; set; }
    }

    public class Customquestionansweringclient
    {
        public string Endpoint { get; set; }
        public string Credential { get; set; }
    }

    public class Questionansweringproject
    {
        public string ProyectName { get; set; }
        public string DeploymentName { get; set; }
    }

}
