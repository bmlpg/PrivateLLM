using OutSystems.ExternalLibraries.SDK;

namespace PrivateLLM
{
    [OSInterface(Name = "PrivateLLM", Description= "Secure, free, and fully local AI for ODC.", IconResourceName = "PrivateLLM.resources.privatellm_logo.png")]
    public interface IPrivateLLM
    {
        [OSAction (ReturnName = "Response")]
        public string Call(
            string SystemPrompt,
            string UserPrompt,
            [OSParameter(Description = "URL of the model file in the \"huggingface.co\" CDN. Default: \"https://huggingface.co/bartowski/Qwen2.5-0.5B-Instruct-GGUF/resolve/main/Qwen2.5-0.5B-Instruct-Q4_K_S.gguf\"")]
            string? ModelFileURL = null);
    }
}