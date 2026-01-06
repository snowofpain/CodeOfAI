namespace CodeOfAI.Application.Modules.LargeLanguageModels.Dtos
{
    public class ChatRequest
    {
        /// <summary>
        /// 调用模型的APIKey
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage ="ApiKey不可为空")]
        public string ApiKey { get; set; }

        /// <summary>
        /// 用户输入的消息
        /// </summary>
        [Required(AllowEmptyStrings = false,ErrorMessage ="消息不可为空")]
        public string Message { get; set; }
    }
}
