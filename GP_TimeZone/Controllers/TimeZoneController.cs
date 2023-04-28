using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GP_TimeZone.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimeZoneController : ControllerBase
    {
        protected readonly string URLBASE = "https://www.timeapi.io";

        protected readonly ILogger<Domain.DTimeZone> _logger;

        public TimeZoneController(ILogger<Domain.DTimeZone> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IList<Domain.BTimeZone>> GetAsync()
        {
            List<Domain.BTimeZone> listBTimeZone = new List<Domain.BTimeZone>();

            string url = $"{URLBASE}/api/TimeZone/AvailableTimeZones";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var stringJson = await response.Content.ReadAsStringAsync();
                List<string>? timeZones = JsonConvert.DeserializeObject<List<string>>(stringJson);

                List<Domain.DTimeZone> returnList = new List<Domain.DTimeZone>();
                if (timeZones != null)
                {
                    foreach (var item in timeZones)
                    {
                        returnList.Add(new Domain.DTimeZone(item));
                    }
                }

                foreach (var item in returnList)
                {
                    if (item != null && item.HasZone && item.Zone != null)
                    {
                        listBTimeZone.Add(new Domain.BTimeZone(item.Zone));
                    }
                }
            }
            //Error with Etc
            //https://www.timeapi.io/api/Time/current/zone?timeZone=Etc/GMT+0
            return listBTimeZone.Where(c=>c.Area.Name!="Etc").ToList();            
        }

        /// <summary>
        /// Example : https://www.timeapi.io/api/Time/current/zone?timeZone=Europe/Amsterdam
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/{controller}/GetTime")]
        public async Task<Domain.BTime> GetTime(string timeZone)
        {
            Domain.BTime returnValue = new Domain.BTime();


            string url = $"{URLBASE}/api/Time/current/zone?timeZone={timeZone}";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var stringJson = await response.Content.ReadAsStringAsync();
                Domain.DTime? dTime = JsonConvert.DeserializeObject<Domain.DTime>(stringJson);

                if (dTime != null)
                {
                    returnValue.dateTime = dTime.dateTime;
                }
            }
            return returnValue;
        }
    }
}