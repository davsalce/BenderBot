using Azure;
using Azure.AI.Language.Conversations;
using Azure.AI.Language.QuestionAnswering;
using Bot;
using Bot.Bots;
using Bot.CLU;
using Bot.CQA;
using Bot.Dialogs;
using Bot.Dialogs.ChangeLanguage;
using Bot.Dialogs.MarkEpisodeAsWatched;
using Bot.DirectLine;
using Bot.IntentHandlers;
using Bot.Middleware;
using Bot.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using MockSeries;
using System.Net.Http.Headers;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IBot, BenderBot>();
builder.Services.AddTransient<IBotFrameworkHttpAdapter, BenderAdapter>();

// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

builder.Services.AddSingleton<QuestionAnsweringClient>(servicesProvider =>
{
    CQAOptions CQAOptions = builder.Configuration
            .GetSection("CustomQuestionAnswering")
            .Get<CQAOptions>();

    QuestionAnsweringClientOptions questionAnsweringClientOptions = new QuestionAnsweringClientOptions()
    {
        DefaultLanguage = "es-ES"
    };
    Uri uri = new Uri(CQAOptions.CustomQuestionAnsweringClient.Endpoint);
    AzureKeyCredential azureKeyCredential = new AzureKeyCredential(CQAOptions.CustomQuestionAnsweringClient.Credential);
    return new QuestionAnsweringClient(uri, azureKeyCredential, questionAnsweringClientOptions);
});
builder.Services.AddSingleton<QuestionAnsweringProject>(servicesProvider =>
{
    CQAOptions CQAOptions = builder.Configuration.GetSection("CustomQuestionAnswering").Get<CQAOptions>();

    return new QuestionAnsweringProject(CQAOptions.QuestionAnsweringProject.ProyectName, CQAOptions.QuestionAnsweringProject.DeploymentName);

});
builder.Services.AddSingleton<ConversationAnalysisClient>(servicesProvider =>
{
    CLUOptions CLUOptions = builder.Configuration.GetSection("ConversationLanguageUnderstandingClient").Get<CLUOptions>();
    Uri endpoint = new Uri(CLUOptions.Endpoint);
    AzureKeyCredential credential = new AzureKeyCredential(CLUOptions.Credential); 
    return new ConversationAnalysisClient(endpoint, credential);
});
builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.Services.AddSingleton<ConversationState>();
builder.Services.AddSingleton<CLUMiddleware>();
builder.Services.AddSingleton<CQADialog>();
builder.Services.AddSingleton<TrendingDialog>();
builder.Services.AddSingleton<SeriesClient>();
builder.Services.AddTransient<MarkAsWatchedRootDialog>();
builder.Services.AddSingleton<PendingEpisodesDialog>();
builder.Services.AddSingleton<RecomendSeriesDialog>();
builder.Services.AddSingleton<MarkSeasonAsWatchedDialog>();
builder.Services.AddSingleton<MarkEpisodeAsWatchedDialog>();
builder.Services.AddSingleton<GetSeriesNameDialog>();
builder.Services.AddSingleton<GetSeasonDialog>();
builder.Services.AddSingleton<GetEpisodeDialog>();
builder.Services.AddTransient<ChangeLanguageDialog>();
builder.Services.AddSingleton<LanguageMiddleware>();
builder.Services.AddSingleton<GetLanguageDialog>();
builder.Services.AddSingleton<DialogHelper>();

builder.Services.AddTransient<IIntentHandler, MarkEpisodeAsWatchedIntentHandler>();
builder.Services.AddTransient<IIntentHandler, ChangeLanguageIntentHandler>();
builder.Services.AddTransient<IIntentHandler, PendingEpisodeIntentHandler>();
builder.Services.AddTransient<IIntentHandler, RecomendSeriesIntentHandler>();
builder.Services.AddTransient<IIntentHandler, TrendingIntentHander>();
builder.Services.AddTransient<IIntentHandler, CQAIntentHandler>();

builder.Services.AddHttpClient<DirectLineClient>(client =>
{
    DirectLineOptions directLineOptions = builder.Configuration.GetSection(nameof(DirectLineOptions)).Get<DirectLineOptions>();
    client.BaseAddress = new Uri(directLineOptions.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", directLineOptions.SecretKey);
});

builder.Services.AddOpenAIClient(builder.Configuration);

WebApplication? app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/",() =>"Hello World!");
app.MapPost("api/messages", async context => 
{
    IBot bot = context.RequestServices.GetRequiredService<IBot>();
    var adapter = context.RequestServices.GetRequiredService<IBotFrameworkHttpAdapter>();
    await adapter.ProcessAsync(context.Request, context.Response, bot);

});
app.MapGet("api/directline/generateToken", 
    async (DirectLineClient directLineClient) =>
{
    return await directLineClient.GenerateTokenAsync();
});
app.MapGet("api/directline/reconnect/{conversationId}", 
    async ([FromRoute] string conversationId,
            [FromQuery] int watermark, 
            [FromServices] DirectLineClient directLineClient) =>
{
    return await directLineClient.Reconnect(conversationId, watermark);
});

app.Run();