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

                    var result = GetAirportNames(translationOnly.Trim());

                    if (result.id > 0)
                    {
                        resp.FromId = result.id;
                        resp.Success = true;
                        resp.Message = "Please, state your destination";
                    }
                    else if (result.id == -1) 
                    {
                        resp.FromId = result.id;
                        resp.Success = false;
                        resp.Message = result.Message;
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

        private (long id, string Message) GetAirportNames(string names)
        {
            try
            {
                string[] wordList = names.Split(' ');

                Airport airport = new Airport();

                bool validAirport = false;
                bool validGo = false;
                bool validWill = true;


                if (wordList.Count() > 1)
                {
                    foreach (var item in wordList)
                    {
                        if (!validAirport)
                        {
                            airport = _context.Airports.Where(x => x.Name.Trim().ToLower() == item.Trim().ToLower()).FirstOrDefault();
                            if (airport != null)
                            {
                                validAirport = true;
                            }
                        }

                        if (!validGo && item.Trim().ToLower() == "go")
                        {
                            validGo = true;
                        }

                        if (validWill && (item.Trim().ToLower() == "no" || item.Trim().ToLower() == "not"))
                        {
                            validWill = false;
                        }
                    }
                    if (validWill)
                    {

                        if (validGo)
                        {
                            if (validAirport)
                            {
                                return (airport.AirportID, "valid");
                            }
                            else 
                            {
                                return (0, "Invalid Airport");
                            }
                        }
                        else 
                        {
                            return (-1, "Irrevelant Information, I guess!");
                        }
                    }
                    else
                    {
                        return (-1, "Not Going Anywhere!");
                    }
                }
                else
                {
                    airport = _context.Airports.Where(x => x.Name.Trim().ToLower() == names.Trim().ToLower()).FirstOrDefault();

                    if (airport != null)
                    {
                        return (airport.AirportID, "Valid");
                    }
                    else
                    {
                        return (0, "Invalid Airport");
                    }

                }
            }
            catch (Exception ex)
            {
                return (-1, ex.Message);
            }
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