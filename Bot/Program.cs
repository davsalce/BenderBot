using Bot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IBot, BenderBot>();
builder.Services.AddTransient<IBotFrameworkHttpAdapter, CloudAdapter>();
WebApplication? app = builder.Build();

app.MapGet("/",() =>"Hello World!");
app.MapPost("api/messages", async context => 
{
    IBot bot = context.RequestServices.GetRequiredService<IBot>();
    var adapter = context.RequestServices.GetRequiredService<IBotFrameworkHttpAdapter>();
    await adapter.ProcessAsync(context.Request, context.Response, bot);

});

app.Run();
