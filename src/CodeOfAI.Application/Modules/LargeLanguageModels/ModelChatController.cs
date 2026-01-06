using CodeOfAI.Application.Modules.LargeLanguageModels.Dtos;
using CodeOfAI.Application.Modules.LargeLanguageModels.Services;
using Furion.JsonSerialization;
using Microsoft.Extensions.Logging;

namespace CodeOfAI.Application.Modules.LargeLanguageModels
{
    [ApiController]
    [Route("/api/model")]
    public class ModelChatController :ControllerBase
    {

        private IModelChatService _chatService;
        private IJsonSerializerProvider _jsonSerializer;
        private ILogger<ModelChatController> _logger;

        public ModelChatController(
            IModelChatService chatService,
            IJsonSerializerProvider jsonSerializer,
            ILogger<ModelChatController> logger)
        {
            _chatService = chatService;
            _jsonSerializer = jsonSerializer;
            _logger = logger;
        }


        /// <summary>
        /// 调用SSE流式对话
        /// </summary>
        [HttpPost("stream-chat/v1")]
        public async Task StreamChat([FromBody] ChatRequest request)
        {
            //Sse流式响应
            Response.ContentType = "text/event-stream";
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            Response.Headers.Append("X-Accel-Buffering", "no"); // 禁用Nginx缓冲

            //设置CancellationToken
            var cancellationToken = HttpContext.RequestAborted;

            try
            {
                await _chatService.SendMessageStreamAsync(
                    request,
                    async chunk =>
                    {
                        await WriteSseEventAsync(chunk, cancellationToken);
                    },
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("用户取消了对话");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Sse Resolve:{ex.ToString()}");
            }

        }


        /// <summary>
        /// SSE数据传输
        /// </summary>
        private async Task WriteSseEventAsync(object data, CancellationToken cancellationToken)
        {
            var sseData = $"data: {_jsonSerializer.Serialize(data)}\n\n";
            await Response.WriteAsync(sseData, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
