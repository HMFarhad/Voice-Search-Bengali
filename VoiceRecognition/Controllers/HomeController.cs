using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
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
        public async Task<ActionResult> FlightSearch(long fromId, long toId, DateTime? date)
        {
            if (fromId > 0 && toId > 0)
            {
                if (date != null && date != DateTime.MinValue && date > DateTime.Now)
                {
                    List<Flight> Flights = new List<Flight>();

                    try
                    {
                        List<Airline> airlines = _context.Airlines.ToList();

                        string from = _context.Airports.Where(x => x.AirportID == fromId).FirstOrDefault().Name;
                        string to = _context.Airports.Where(x => x.AirportID == toId).FirstOrDefault().Name;

                        int id = 1;
                        Random r = new Random();
                        if (date.Value.Date == DateTime.Today)
                        {
                            foreach (var item in airlines)
                            {
                                int rInt = r.Next(0, 1000);
                                double price = (fromId * toId * 1000) + rInt;
                                Flight flight1800 = new Flight(id, item.Name, price, "18.00 (GMT +06.00)", from, to);
                                Flights.Add(flight1800);

                                rInt = r.Next(0, 1000);
                                price = (fromId * toId * 1000) + rInt;
                                Flight flight2000 = new Flight(id++, item.Name, price, "20.00 (GMT +06.00)", from, to);
                                Flights.Add(flight2000);
                                id++;
                            }
                        }
                        else
                        {
                            foreach (var item in airlines)
                            {
                                int rInt = r.Next(0, 1000);

                                double price = (fromId * toId * 1000) + rInt;
                                Flight flight1000 = new Flight(id, item.Name, price, "10.00 (GMT +06.00)", from, to);
                                Flights.Add(flight1000);

                                rInt = r.Next(0, 1000);
                                price = (fromId * toId * 1000) + rInt;
                                Flight flight0800 = new Flight(id++, item.Name, price, "08.00 (GMT +06.00)", from, to);
                                Flights.Add(flight0800);

                                price = (fromId * toId * 1000) + rInt;
                                Flight flight1800 = new Flight(id, item.Name, price, "18.00 (GMT +06.00)", from, to);
                                Flights.Add(flight1800);

                                rInt = r.Next(0, 1000);
                                price = (fromId * toId * 1000) + rInt;
                                Flight flight2000 = new Flight(id++, item.Name, price, "20.00 (GMT +06.00)", from, to);
                                Flights.Add(flight2000);
                                id++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    return View(Flights.OrderBy(x => x.Price));
                }
                else
                {
                    ViewBag.Message = "Please provide date (later than today)!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Message = "Please provide deputure and destination!";
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public async Task<JsonResult> GetDepurture(string rawData)
        {
            SyncResponse resp = new SyncResponse();
            resp.Success = false;

            try
            {
                if (!string.IsNullOrEmpty(rawData) && !string.IsNullOrWhiteSpace(rawData))
                {
                    var translation = await GetTranslation(rawData);

                    if (translation.success)
                    {
                        string names = translation.result;

                        string[] wordList;

                        Airport airport1 = new Airport();
                        Airport airport2 = new Airport();

                        if (names.Contains(' '))
                        {
                            wordList = names.Split(' ');
                        }
                        else if (names.Contains('-'))
                        {
                            wordList = names.Split('-');
                        }
                        else
                        {
                            wordList = new string[0];
                        }

                        if (wordList.Count() > 0)
                        {
                            var multiResult = GetMultiResult(translation.result.Trim());
                            resp.Message = multiResult.Message;

                            if (multiResult.success)
                            {
                                resp.Success = true;
                                if (multiResult.date != null && multiResult.date != DateTime.MinValue)
                                {
                                    resp.DepurtureDate = multiResult.date.Value;
                                }
                                resp.FromId = multiResult.fromId;
                                resp.ToId = multiResult.toId;
                                
                                return Json(resp);

                            }
                            else
                            {
                                return Json(resp);
                            }
                        }

                        var result = GetAirportName(translation.result.Trim());

                        if (result.success)
                        {
                            resp.FromId = result.Id;
                            resp.Success = true;
                            resp.Message = "success";
                        }
                        else
                        {
                            resp.Message = result.Message;
                        }
                    }
                    else
                    {
                        resp.Message = "Translation limit on API has expired or invalid!! " + translation.result;
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

        [HttpPost]
        public async Task<JsonResult> GetDestination(string rawData)
        {
            SyncResponse resp = new SyncResponse();
            resp.Success = false;

            try
            {
                if (!string.IsNullOrEmpty(rawData) && !string.IsNullOrWhiteSpace(rawData))
                {
                    var translation = await GetTranslation(rawData);

                    if (translation.success)
                    {
                        var result = GetAirportName(translation.result.Trim());

                        if (result.success)
                        {
                            resp.ToId = result.Id;
                            resp.Success = true;
                            resp.Message = "Success";
                        }
                        else
                        {
                            resp.Message = result.Message;
                        }
                    }
                    else
                    {
                        resp.Message = "Translation limit on API has expired or invalid!! " + translation.result;
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

        [HttpPost]
        public async Task<JsonResult> GetDate(string rawData)
        {
            SyncResponse resp = new SyncResponse();
            resp.Success = false;

            try
            {
                if (!string.IsNullOrEmpty(rawData) && !string.IsNullOrWhiteSpace(rawData))
                {
                    var translation = await GetTranslation(rawData);

                    if (translation.success)
                    {
                        string dateString = translation.result;
                        try
                        {
                            DateTime convertedDate = DateTime.Parse(dateString);
                            if (convertedDate != null && convertedDate != DateTime.MinValue)
                            {
                                resp.DepurtureDate = convertedDate;
                                resp.Success = true;
                                return Json(resp);
                            }
                        }
                        catch (Exception ex)
                        {
                            resp.Message = ex.Message;
                        }
                    }
                    else
                    {
                        resp.Message = "Translation limit on API has expired or invalid!! " + translation.result;
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

        private (bool success, long Id, string Message) GetAirportName(string names)
        {
            Airport airport = new Airport();

            airport = _context.Airports.Where(x => x.Name.Trim().ToLower() == names.Trim().ToLower()).FirstOrDefault();

            if (airport != null && airport.AirportID > 0)
            {
                return (true,  airport.AirportID, "");
            }
            else
            {
                return (false, 0, "Invalid name or we don't operate there yet!");
            }
        }

        private (bool success, long fromId, long toId,DateTime? date, string Message) GetMultiResult(string names)
        {
            try
            {

                string[] wordList;

                Airport airport1 = new Airport();
                Airport airport2 = new Airport();

                bool validAirport1 = false;
                bool validAirport2 = false;
                bool validGo = false;
                bool validWill = true;

                if (names.Contains(' '))
                {
                    wordList = names.Split(' ');
                    validGo = true;
                }
                else if (names.Contains('-'))
                {
                    wordList = names.Split('-');
                    validGo = true;
                }
                else
                {
                    wordList = new string[0];
                }

                if (wordList.Count() > 1)
                {
                    try
                    {
                        DateTime convertedDate = DateTime.Parse(names);
                        if (convertedDate != null && convertedDate != DateTime.MinValue)
                        {
                            return (true, 0,0,convertedDate,"");
                        }
                    }
                    catch (Exception)
                    {
                        foreach (var item in wordList)
                        {
                            if (!validAirport1)
                            {
                                airport1 = _context.Airports.Where(x => x.Name.Trim().ToLower() == item.Trim().ToLower()).FirstOrDefault();
                                if (airport1 != null)
                                {
                                    validAirport1 = true;
                                    continue;
                                }
                            }

                            if (!validAirport2)
                            {
                                airport2 = _context.Airports.Where(x => x.Name.Trim().ToLower() == item.Trim().ToLower()).FirstOrDefault();
                                if (airport2 != null)
                                {
                                    validAirport2 = true;
                                    continue;
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
                                if (validAirport1)
                                {
                                    if (validAirport2)
                                    {
                                        return (true, airport1.AirportID, airport2.AirportID, null, "Please, state depurture date!");
                                    }
                                    else
                                    {
                                        return (true, airport1.AirportID, 0,null, "Please, state destination!");
                                    }
                                }
                                else
                                {
                                    return (false, 0, 0,null, "Invalid Airport!");
                                }
                            }
                            else
                            {
                                return (false, 0, 0, null, "Irrevelant Information!");
                            }
                        }
                        else
                        {
                            return (false, 0, 0,null, "Not Going Anywhere!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false,0,0,null, ex.Message);
            }
            return (false, 0, 0, null, "INVALID!");
        }

        private async Task<(bool success, string result)> GetTranslation(string voiceRendered)
        {
            string result = voiceRendered;
            try
            {
                string apiKey = WebConfigurationManager.AppSettings["TranslateKey"].ToString();
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://google-translate1.p.rapidapi.com/language/translate/v2"),
                    Headers =
                            {
                                { "x-rapidapi-host", "google-translate1.p.rapidapi.com" },
                                { "x-rapidapi-key", apiKey },
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
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(body);
                        Root root = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(body);
                        result = root.data.translations.FirstOrDefault().translatedText;
                        return (true, result);
                    }
                    else
                    {
                        return (false, response.ReasonPhrase);
                    }

                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return (false,result);
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