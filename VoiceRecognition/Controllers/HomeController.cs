using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VoiceRecognition.Models;

namespace VoiceRecognition.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            VoiceSearchViewModel viewModel = new VoiceSearchViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(VoiceSearchViewModel viewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(viewModel.voiceRendered) && !string.IsNullOrWhiteSpace(viewModel.voiceRendered))
                {

                    viewModel.voiceRendered = await GetTranslation(viewModel.voiceRendered);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View(viewModel);
        }

        private async Task<string> GetTranslation(string voiceRendered)
        {
            string result = voiceRendered;
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://google-translate1.p.rapidapi.com/language/translate/v2"),
                    Headers =
                            {
                                { "x-rapidapi-host", "google-translate1.p.rapidapi.com" },
                                { "x-rapidapi-key", "76e0e1276amsh5d6fedb81e5764ep1b91f8jsncae7d6facf78" },
                            },
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                { "q", voiceRendered },
                                { "target", "en" },
                                { "source", "bn" },
                            }),
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(body);
                    Translation Translation = Newtonsoft.Json.JsonConvert.DeserializeObject<Translation>(body);
                    result = Translation.translatedText;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Test()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}