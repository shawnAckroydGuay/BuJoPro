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
        private readonly IAgendaCreator _bulletJournalCreator;

        public BuJoProController(ILogger<BuJoProController> logger, IAgendaCreator bulletJournalCreator)
        {
            _logger = logger;
            _bulletJournalCreator = bulletJournalCreator;
        }

        [HttpGet("GetAlpaga")]
        public IActionResult CreerAgendaVersionAlpaga([FromQuery] int firstMonth, [FromQuery] int monthCount = 6)
        {
            var result = _bulletJournalCreator.CreerLePlanificateurEnPdf(firstMonth);
            
            Response.Headers.Add("Content-Type", "application/pdf");
            // Response.Headers.Add("Content-Disposition", "inline; filename=bulletjournal.pdf");

            return File(result, "application/pdf", "agenda-alpaga.pdf");
        }
    }
}