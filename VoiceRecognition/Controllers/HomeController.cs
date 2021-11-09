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
        VoiceSearchDBContext _context = new VoiceSearchDBContext();
        public ActionResult Index()
        {

            VoiceSearchViewModel viewModel = new VoiceSearchViewModel();
            List<Airport> airports = _context.Airports.ToList();

            ViewBag.Airports = new SelectList(airports, "AirportID", "Name");

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

        [HttpPost]
        public async Task<JsonResult> SyncSpeech(string rawData)
        {
            SyncResponse resp = new SyncResponse();
            resp.Success = false;

            try
            {
                if (!string.IsNullOrEmpty(rawData) && !string.IsNullOrWhiteSpace(rawData))
                {
                    string translationOnly = await GetTranslation(rawData);

                    long id = GetAirportNames(translationOnly.Trim());

                    if (id>0)
                    {
                        resp.FromId = id;
                        resp.Success = true;
                        resp.Message = "Please, state your destination";
                    }
                    else
                    {
                        resp.Message = "Sorry, either stated place is not valid or we don't operate at the stated place!";
                    }

                }
                else
                {
                    resp.Message = "No Search Data!";
                    return Json(resp);
                }

                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        private long GetAirportNames(string names)
        {
            try
            {
                Airport airport = _context.Airports.Where(x => x.Name.Trim().ToLower() == names.Trim().ToLower()).FirstOrDefault();
                if (airport != null)
                {
                    return airport.AirportID;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
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
                    Root root = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(body);
                    result = root.data.translations.FirstOrDefault().translatedText;
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