
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TextToAudio.Services
{
    public class AzureTtsService
    {
        private readonly IHttpClientFactory _factory;
        private readonly AzureConfig _cfg;

        public AzureTtsService(IHttpClientFactory factory, AzureConfig cfg)
        {
            _factory = factory;
            _cfg = cfg;
        }

        public async Task<byte[]> SynthesizeAsync(string text, string voice = "alloy", string format = "mp3")
        {
            var client = _factory.CreateClient();

            // Azure OpenAI TTS endpoint (deployment in URL)
            //var url = "https://mdnaz-mi97k3aj-eastus2.cognitiveservices.azure.com/openai/deployments/gpt-4o-mini-tts/audio/speech?api-version=2025-03-01-preview";
            var url = _cfg.endpoint;
            //$"{_cfg.endpoint}/openai/deployments/{_cfg.ttsDeployment}/audio/speech?api-version={_cfg.apiVersionTts}";

            var payload = new
            {
                model = "gpt-4o-mini-tts",
                input = text,
                voice = voice
                // आवश्यकता अनुसार output_format जोड़ सकते हैं:
                // output_format = format
            };

            var json = JsonSerializer.Serialize(payload);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            // IMPORTANT: Ocp-Apim-Subscription-Key (Bearer नहीं)
            req.Headers.Add("api-key", _cfg.apiKey);

            var res = await client.SendAsync(req);

            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"TTS failed: {(int)res.StatusCode} {res.ReasonPhrase} - {err}");
            }

            return await res.Content.ReadAsByteArrayAsync();
        }
    }
}
