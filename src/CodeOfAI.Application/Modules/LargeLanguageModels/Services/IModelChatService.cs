using CodeOfAI.Application.Modules.LargeLanguageModels.Dtos;
using CodeOfAI.Core.StandardOptions;

namespace CodeOfAI.Application.Modules.LargeLanguageModels.Services
{
    public interface IModelChatService
    {
        /// <summary>
        /// 发送消息并接收流式响应
        /// </summary>
        /// <param name="request">对话请求</param>
        /// <param name="onChunkReceived">接收到数据块时的回调</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task SendMessageStreamAsync(
            ChatRequest request,
            Action<ChatResponse> onChunkReceived = null,
            CancellationToken cancellationToken = default);


    }
}
