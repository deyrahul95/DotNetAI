using System.ClientModel;
using DotNetAI.Api.Chat;
using DotNetAI.Api.Kernels;
using DotNetAI.Core.Plugins;
using Microsoft.SemanticKernel;
using OpenAI;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new OpenAIClient(
        new ApiKeyCredential(config["OpenAI:ApiKey"]!),
        new OpenAIClientOptions
        {
            Endpoint = new Uri(config["OpenAI:Endpoint"]!)
        });
});

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var kb = Kernel.CreateBuilder();
    kb.AddOpenAIChatCompletion(
        modelId: config["OpenAI:ModelName"]!,
        apiKey: config["OpenAI:ApiKey"]!,
        endpoint: new Uri(config["OpenAI:Endpoint"]!));

    var kernel = kb.Build();
    kernel.Plugins.AddFromType<DevToolPlugins>();
    return kernel;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapChatEndpoints();
app.MapKernelEndpoints();

app.UseHttpsRedirection();

app.Run();