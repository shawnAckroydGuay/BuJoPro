using BuJoProApplicationLogic.BuJoCreator;
using Microsoft.AspNetCore.Mvc;

namespace BuJoProApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMonthPlanningCreator _monthPlanningCreator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMonthPlanningCreator monthPlanningCreator)
        {
            _logger = logger;
            _monthPlanningCreator = monthPlanningCreator;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetBuJoPDFV1")]
        public IActionResult CreateBulletJournal()
        {
            var bujoPDF = _monthPlanningCreator.CreateSixMonths();

            Response.Headers.Add("Content-Type", "application/pdf");
            Response.Headers.Add("Content-Disposition", "inline; filename=bulletjournal.pdf");

            return File(bujoPDF, "application/pdf");
        }

        [HttpGet("GetBuJoPDFV2")]
        public IActionResult CreateBulletJournalV2()
        {
            var bujoPDFBytes = _monthPlanningCreator.CreateSixMonths();
            var bujoPDFBase64 = Convert.ToBase64String(bujoPDFBytes);

            return Ok(new { BuJoPDFBase64 = bujoPDFBase64 });
        }
    }
}