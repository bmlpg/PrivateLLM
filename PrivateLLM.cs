using System.Diagnostics;
using LLama;
using LLama.Common;
using Microsoft.Extensions.Logging;

namespace PrivateLLM
{
    public class PrivateLLM : IPrivateLLM
    {
        private readonly ILogger _logger;

        public PrivateLLM(ILogger logger)
        {
            _logger = logger;

            LLama.Native.NativeLibraryConfig.All.WithLogCallback((level, message) =>
            {
                bool isError = level == LLama.Native.LLamaLogLevel.Error;
                bool isWarning = level == LLama.Native.LLamaLogLevel.Warning;

                if (!isError && !isWarning) return;

                using var activity = Activity.Current?.Source.StartActivity("PrivateLLM.NativeLog");

                if (isError)
                {
                    _logger.LogError($"LL# NATIVE ERROR: {message}");
                    activity?.SetStatus(ActivityStatusCode.Error, message);
                }
                else
                {
                    _logger.LogWarning($"LL# NATIVE WARNING: {message}");
                }
            });
        }
        public string Call(string SystemPrompt, string UserPrompt, string? ModelFileURL = null)
        {
            if (string.IsNullOrWhiteSpace(ModelFileURL))
            {
                ModelFileURL = "https://huggingface.co/bartowski/Qwen2.5-0.5B-Instruct-GGUF/resolve/main/Qwen2.5-0.5B-Instruct-Q4_K_S.gguf";
            }

            SetupModel(ModelFileURL);

            return Task.Run(async () =>
            {
                var modelPath = Path.Combine(Path.GetTempPath(), "model.gguf");
                var parameters = new ModelParams(modelPath)
                {
                    ContextSize = 1024,
                    GpuLayerCount = 0
                };

                using (var weights = LLamaWeights.LoadFromFile(parameters))
                {
                    var executor = new StatelessExecutor(weights, parameters);

                    string fullPrompt = $"<|im_start|>system\n{SystemPrompt}<|im_end|><|im_start|>user\n{UserPrompt}<|im_end|>\n<|im_start|>assistant\n";

                    var inferenceParams = new InferenceParams()
                    {
                        MaxTokens = 256,
                        AntiPrompts = new[] { "<|im_end|>" }
                    };

                    var responseBuilder = new System.Text.StringBuilder();

                    await foreach (var token in executor.InferAsync(fullPrompt, inferenceParams))
                    {
                        responseBuilder.Append(token);
                    }

                    return responseBuilder.ToString().Trim();
                }
            }).GetAwaiter().GetResult();
        }

        private void SetupModel(string modelFileURL)
        {
            string filePath = Path.Combine(Path.GetTempPath(), "model.gguf");

            if (File.Exists(filePath)) return;

            _logger.LogInformation("Starting atomic model download...");

            string tempFilePath = filePath + ".tmp";

            try
            {
                HttpClient client = new HttpClient();

                using var request = new HttpRequestMessage(HttpMethod.Get, modelFileURL);
                request.Headers.Add("User-Agent", "OutSystems-PrivateLLM-Plugin-ODC");

                using var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                using (var networkStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    networkStream.CopyTo(fileStream);
                }

                if (File.Exists(filePath)) File.Delete(filePath);
                File.Move(tempFilePath, filePath);

                _logger.LogInformation("Model download complete and verified.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Download failed: {ex.Message}");
                // Cleanup temp file on failure so the next attempt starts fresh
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                throw;
            }
        }
    }
}
