namespace DotNetAI.Core.Models;

public class ChatModel
{
    public record ChatRequest(string Message);
    public record ChatResponse(string Reply, string Provider);
}
