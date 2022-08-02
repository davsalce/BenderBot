using Azure;
using Azure.AI.Language.QuestionAnswering;
using Bot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IBot, BenderBot>();
builder.Services.AddTransient<IBotFrameworkHttpAdapter, CloudAdapter>();
builder.Services.AddSingleton<QuestionAnsweringClient>(builder =>
{
    Uri uri = new Uri("https://ts-bot-language.cognitiveservices.azure.com");
    AzureKeyCredential azureKeyCredential = new AzureKeyCredential("08f8aae73b7a4049a0ba8f58187c3c67");
    return new QuestionAnsweringClient(uri, azureKeyCredential);
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
