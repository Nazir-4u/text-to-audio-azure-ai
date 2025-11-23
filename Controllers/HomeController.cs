
using Microsoft.AspNetCore.Mvc;
using TextToAudio.Services;

namespace TextToAudio.Controllers
{
    public class HomeController : Controller
    {
        private readonly AzureTtsService _tts;

        public HomeController(AzureTtsService tts)
        {
            _tts = tts;
        }

        // GET: /
        public IActionResult Index()
        {
            return View();
        }

        public record TtsRequest(string text, string? voice, string? format);

        // POST: /Home/Speak
        [HttpPost]
        [IgnoreAntiforgeryToken] // fetch से कॉल करने के लिए आसान; चाहो तो anti-forgery implement कर सकते हो
        public async Task<IActionResult> Speak([FromBody] TtsRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.text))
                return BadRequest(new { error = "text is required" });

            var voice = req.voice ?? "alloy";
            var format = (req.format ?? "mp3").ToLowerInvariant();

            var audioBytes = await _tts.SynthesizeAsync(req.text, voice, format);

            var contentType = format switch
            {
                "wav" => "audio/wav",
                "ogg" => "audio/ogg",
                _ => "audio/mpeg"
            };

            // Inline playback के लिए filename omit करें
            return File(audioBytes, contentType);
        }
    }
}
