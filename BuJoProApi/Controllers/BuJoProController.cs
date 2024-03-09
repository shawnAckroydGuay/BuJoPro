using BuJoProApplicationLogic.BuJoCreator;
using Microsoft.AspNetCore.Mvc;

namespace BuJoProApi.Controllers
{
    /// <summary>
    /// Represents a controller for weather forecast operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BuJoProController : ControllerBase
    {   
        private readonly ILogger<BuJoProController> _logger;
        private readonly IMonthPlanningCreator _monthPlanningCreator;

        public BuJoProController(ILogger<BuJoProController> logger, IMonthPlanningCreator monthPlanningCreator)
        {
            _logger = logger;
            _monthPlanningCreator = monthPlanningCreator;
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