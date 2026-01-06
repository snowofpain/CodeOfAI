//{
//    "model":"deepseek-chat",
//	"messages":
//		[

//            {
//        "role":"user",
//				"content":"Here is Your Content"

//            },
//			{
//        ...

//            }
//		],
//	"stream":true
//}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeOfAI.Core.StandardOptions
{
    public class ChatMessages
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = true;
    }

    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
