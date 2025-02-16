using System.Threading.Tasks;
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
        private static readonly string[] AllowedMimeTypesForCover = new[]
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp",
            "image/webp"
        };

        public BuJoProController(ILogger<BuJoProController> logger, IAgendaCreator bulletJournalCreator)
        {
            _logger = logger;
            _bulletJournalCreator = bulletJournalCreator;
        }

        [HttpPost("GetAlpaga")]
        public async Task<IActionResult> CreerAgendaVersionAlpaga(
         [FromQuery] int firstMonth,
         [FromQuery]string title,
         IFormFile imageCouverture,
         [FromQuery] int monthCount = 6)
        {
            // Vérifier le type MIME du fichier
            if (!AllowedMimeTypesForCover.Contains(imageCouverture.ContentType))
                return BadRequest("Type de fichier non autorisé. Seules les images sont acceptées.");

            var imageCouvertureStream = ConvertFileToByteArray(imageCouverture);

            var result = _bulletJournalCreator.CreerLePlanificateurEnPdf(firstMonth, title, imageCouvertureStream);
            
            Response.Headers.Add("Content-Type", "application/pdf");

            return File(result, "application/pdf", "agenda-alpaga.pdf");
        }

        private byte[] ConvertFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}