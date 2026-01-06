using CodeOfAI.Application.Modules.LargeLanguageModels.Dtos;
using CodeOfAI.Core.StandardOptions;

namespace CodeOfAI.Application.Modules.LargeLanguageModels.Services
{
    public abstract class ChatServiceBase
    {
        public abstract Task SendMessageStreamAsync(ChatRequest request, Action<ChatResponse> onChunkReceived = null, CancellationToken cancellationToken = default);

    }
}
