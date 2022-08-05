using Azure;
using Azure.AI.Language.Conversations;
using Azure.AI.Language.QuestionAnswering;
using Bot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IBot, BenderBot>();
builder.Services.AddTransient<IBotFrameworkHttpAdapter, CloudAdapter>();
builder.Services.AddSingleton<QuestionAnsweringClient>(servicesProvider =>
{
    QuestionAnsweringClientOptions questionAnsweringClientOptions = new QuestionAnsweringClientOptions()
    {
        DefaultLanguage = "es-ES"
    };
    Uri uri = new Uri("https://ts-bot-language.cognitiveservices.azure.com");
    AzureKeyCredential azureKeyCredential = new AzureKeyCredential("08f8aae73b7a4049a0ba8f58187c3c67");
    return new QuestionAnsweringClient(uri, azureKeyCredential, questionAnsweringClientOptions);
});
builder.Services.AddSingleton<QuestionAnsweringProject>(servicesProvider =>
{
    return new QuestionAnsweringProject("ts-bot-customQuestionAnswering", "production");

});
builder.Services.AddSingleton<ConversationAnalysisClient>(servicesProvider =>
{ 
    Uri endpoint = new Uri("https://ts-bot-language.cognitiveservices.azure.com");
    AzureKeyCredential credential = new AzureKeyCredential("08f8aae73b7a4049a0ba8f58187c3c67"); 
    return new ConversationAnalysisClient(endpoint, credential);

});
WebApplication? app = builder.Build();

app.MapGet("/",() =>"Hello World!");
app.MapPost("api/messages", async context => 
{
    IBot bot = context.RequestServices.GetRequiredService<IBot>();
    var adapter = context.RequestServices.GetRequiredService<IBotFrameworkHttpAdapter>();
    await adapter.ProcessAsync(context.Request, context.Response, bot);

});

app.Run();
