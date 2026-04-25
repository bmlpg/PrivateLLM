using OutSystems.ExternalLibraries.SDK;

namespace PrivateLLM
{
    [OSInterface(Name = "PrivateLLM", Description= "Experimental free and fully local AI for ODC.", IconResourceName = "PrivateLLM.resources.privatellm_logo.png")]
    public interface IPrivateLLM
    {
        [OSAction (ReturnName = "Response")]
        public Response Call(
            string SystemPrompt,
            string UserPrompt,
            [OSParameter(Description = "Lower (0.0 - 0.2) for precise data extraction. Higher (0.7+) for creative summaries.\nDefault: 0.1")]
            float Temperature = 0.1f,
            [OSParameter(Description = "Controls the maximum length of the response. Increase for long summaries; decrease to save processing time.\nDefault: 256")]
            int MaxTokens = 256,
            [OSParameter(Description = "Limits the model to a 'nucleus' of high-probability words. Lower (e.g., 0.5) to make the output more focused and predictable.\nDefault: 0.9")]
            float TopP = 0.9f,
            [OSParameter(Description = "URL of the model file in the \"huggingface.co\" CDN. Default: \"https://huggingface.co/bartowski/Qwen2.5-0.5B-Instruct-GGUF/resolve/main/Qwen2.5-0.5B-Instruct-Q4_K_S.gguf\"")]
            string ModelFileURL = "",
            [OSParameter(Description = "Some models require a FREE Hugging Face access token to be fetched.")]
            string HFAccessToken = "",
            int ContextSize = 1024,
            int Threads = 1
        );

        [OSAction(ReturnName = "Responses")]
        public List<Response> BulkCall(
            List<Request> Requests,
            [OSParameter(Description = "URL of the model file in the \"huggingface.co\" CDN. Default: \"https://huggingface.co/bartowski/Qwen2.5-0.5B-Instruct-GGUF/resolve/main/Qwen2.5-0.5B-Instruct-Q4_K_S.gguf\"")]
            string ModelFileURL = "",
            [OSParameter(Description = "Some models require a FREE Hugging Face access token to be fetched.")]
            string HFAccessToken = "",
            int ContextSize = 1024,
            int Threads = 1
        );

        [OSAction(Description = "Dummy action to be used to keep the container alive, thus preventing \"cold-starts\".")]
        public void Ping();
    }
}