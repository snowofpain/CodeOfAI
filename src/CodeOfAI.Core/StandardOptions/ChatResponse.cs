using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeOfAI.Core.StandardOptions
{
    public class ChatResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new();
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("delta")]
        public Delta Delta { get; set; } = new();

        [JsonPropertyName("message")]
        public ToolModelMessage Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Delta
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("reasoning_content")]
        public string ReasoningContent { get; set; }
    }

    public class ToolModelMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } = null;

        [JsonPropertyName("tool_calls")]
        public List<ToolCallMessage> ToolCalls { get; set; } = new();
    }

    public class ToolCallMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public FunctionMessage Function { get; set; }
    }

    public class FunctionMessage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("arguments")]
        public string Arguments { get; set; }
    }
}
