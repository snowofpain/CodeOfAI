using CodeOfAI.Application.Modules.LargeLanguageModels.Dtos;
using CodeOfAI.Core.Defination;
using CodeOfAI.Core.StandardOptions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeOfAI.Application.Modules.LargeLanguageModels.Services
{
    
    public class DeepseekModelChatService : IModelChatService,ITransient
    {
        private readonly ILogger<DeepseekModelChatService> _logger;
        private readonly string _modelName;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public DeepseekModelChatService(
            ILogger<DeepseekModelChatService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;

            _modelName = "deepseek-chat";
            _baseUrl = "https://api.deepseek.com/chat/completions";

            //创建请求
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(3);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(CommonDef.UserAgent);
        }


        public async Task SendMessageStreamAsync(
            ChatRequest request, 
            Action<ChatResponse> onChunkReceived = null, 
            CancellationToken cancellationToken = default)
        {
            using var httpRequest = CreateHttpRequest(request.Message);
            using var response = await SendRequestAsync(httpRequest, cancellationToken);
            await ProcessResponseStreamAsync(response, onChunkReceived, cancellationToken);
        }

        /// <summary>
        /// 创建请求信息
        /// </summary>
        private HttpRequestMessage CreateHttpRequest(string message)
        {
            var request = new ChatMessages
            {
                Model = _modelName,
                Stream = true
            };
            request.Messages.Add(new ChatMessage
            {
                Content = message,
                Role = MessageType.USER
            });

            return CreateHttpRequestV0(request);
        }

        /// <summary>
        /// 创建请求信息
        /// </summary>
        private HttpRequestMessage CreateHttpRequestV0(ChatMessages chatMessage)
        {
            var jsonContent = JsonSerializer.Serialize(chatMessage, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, CommonDef.ContentType)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            return httpRequest;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Log.Error($"Api Request Failed, Status Code:{response.StatusCode}, ErrorContent: {errorContent}");
                throw Oops.Oh<DeepseekModelChatService>($"调用API异常：{errorContent}");
            }
            return response;
        }

        /// <summary>
        /// 处理大模型返回的流式请求
        /// </summary>
        private async Task ProcessResponseStreamAsync(HttpResponseMessage response, Action<ChatResponse> onChunkReceived, CancellationToken cancellationToken)
        {
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line == null) break;
                if (!line.StartsWith("data:")) continue;

                //提取Json内容
                var jsonData = line.AsSpan(6).Trim();
                if (jsonData.Equals("[DONE]".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    var streamResponse = JsonSerializer.Deserialize<ChatResponse>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (streamResponse?.Choices?.Count > 0)
                    {
                        var content = streamResponse.Choices[0].Delta?.Content;
                        if (!string.IsNullOrEmpty(content))
                            onChunkReceived?.Invoke(streamResponse);
                    }

                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Can not Parse Json Object:{jsonData},Error Description:{ex.ToString()}");
                    throw Oops.Oh($"Json解析失败，原因：{ex.ToString()}");
                }

                _logger.LogInformation($"Json Data:{jsonData}");
            }
        }
    }
}
