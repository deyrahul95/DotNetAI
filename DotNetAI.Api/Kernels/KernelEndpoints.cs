using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using static DotNetAI.Core.Models.ChatModel;


namespace DotNetAI.Api.Kernels;

public static class KernelEndpoints
{
    public static void MapKernelEndpoints(this WebApplication app)
    {
        app.MapPost("/api/kernel/ask", async (
            [FromBody] ChatRequest request,
            Kernel kernel,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            logger.LogInformation("Executing ask request. Request: {@Request}", request);

            if (string.IsNullOrEmpty(request.Message))
            {
                logger.LogInformation("Empty request.");
                return Results.BadRequest("Message Can't be null or empty!");
            }

            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var result = await kernel.InvokePromptAsync(
                promptTemplate: request.Message,
                arguments: new KernelArguments(settings),
                cancellationToken: ct);

            logger.LogInformation(
                "Received response from Kernel. Function: {@Function}, Metadata: {@Metadata}",
                result.Function.Description,
                JsonSerializer.Serialize(result.Metadata));

            var response = new ChatResponse(
                Reply: result.ToString(),
                Provider: "Semantic Kernel");

            logger.LogInformation("Ask request executed. Response: {@Response}", response);

            return Results.Ok(response);
        })
        .WithTags("Kernel")
        .WithName("AskKernel")
        .Accepts<ChatRequest>("application/json")
        .Produces<ChatResponse>(StatusCodes.Status200OK, "application/json")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Ask any task to kernel")
        .WithSummary("Ask question for kernel");
    }
}
