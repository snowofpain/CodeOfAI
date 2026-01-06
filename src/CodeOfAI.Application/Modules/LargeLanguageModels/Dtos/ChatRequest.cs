namespace CodeOfAI.Application.Modules.LargeLanguageModels.Dtos
{
    public class ChatRequest
    {
        /// <summary>
        /// 用户输入的消息
        /// </summary>
        [Required(AllowEmptyStrings = false,ErrorMessage ="消息不可为空")]
        public string Message { get; set; }
    }
}
