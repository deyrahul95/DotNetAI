using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using static DotNetAI.Core.Models.ChatModel;

namespace DotNetAI.Api.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        app.MapPost("/api/chat", async (
            [FromBody] ChatRequest request,
            OpenAIClient azureClient,
            IConfiguration configuration,
            ILogger<Program> logger,
            CancellationToken ct) =>
            {
                logger.LogInformation("Executing chat request. Request: {@Request}", request);

                if (string.IsNullOrEmpty(request.Message))
                {
                    logger.LogInformation("Empty request.");
                    return Results.BadRequest("Message Can't be null or empty!");
                }

                var chatClient = azureClient
                    .GetChatClient(configuration["OpenAI:ModelName"]);
                logger.LogInformation("OpenAI chat client configured.");

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("You are a helpful .NET experience developer assistant."),
                    new UserChatMessage(request.Message)
                };

                var result = await chatClient.CompleteChatAsync(
                    messages: messages,
                    cancellationToken: ct);
                logger.LogInformation(
                    "Received response from OpenAI. Model: {@Model}, Usage: {@Usage}",
                    result.Value.Model,
                    JsonSerializer.Serialize(result.Value.Usage));

                var response = new ChatResponse(
                    Reply: result.Value.Content[0].Text,
                    Provider: "OpenAI");

                logger.LogInformation("Chat request executed. Response: {@Response}", response);

                return Results.Ok(response);
            })
            .WithTags("Chat")
            .WithName("PostChat")
            .Accepts<ChatRequest>("application/json")
            .Produces<ChatResponse>(StatusCodes.Status200OK, "application/json")
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/json")
            .WithDescription("Chat with Open AI")
            .WithSummary("Post Chat to Open AI");
    }
}
