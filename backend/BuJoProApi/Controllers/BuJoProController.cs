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
            private readonly IAlpagaAgendaCreator _foldedBulletJournalCreator;
            private readonly SpiralAlpagaAgendaCreator _spiralBulletJournalCreator;
            private static readonly string[] AllowedMimeTypesForCover = new[]
            {
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/bmp",
                "image/webp"
            };

            public BuJoProController(ILogger<BuJoProController> logger,
            IAlpagaAgendaCreator foldedBulletJournalCreator,
            SpiralAlpagaAgendaCreator spiralAlpagaAgendaCreator)
            {
                _logger = logger;
                _foldedBulletJournalCreator = foldedBulletJournalCreator;
                _spiralBulletJournalCreator = spiralAlpagaAgendaCreator;
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

                var result = _foldedBulletJournalCreator.CreerLePlanificateurEnPdf(firstMonth, title, imageCouvertureStream);
                
                Response.Headers.Add("Content-Type", "application/pdf");

                return File(result, "application/pdf", "agenda-alpaga.pdf");
            }

            [HttpPost("GetSpiralAlpaga")]
            public async Task<IActionResult> CreerAgendaVersionAlpagaSpiral(
            [FromQuery] int firstMonth,
            [FromQuery]string title,
            IFormFile imageCouverture,
            [FromQuery] int monthCount = 12)
            {
                // Vérifier le type MIME du fichier
                if (!AllowedMimeTypesForCover.Contains(imageCouverture.ContentType))
                    return BadRequest("Type de fichier non autorisé. Seules les images sont acceptées.");

                var imageCouvertureStream = ConvertFileToByteArray(imageCouverture);

                var result = _spiralBulletJournalCreator.CreerLePlanificateurEnPdf(firstMonth, title, imageCouvertureStream, monthCount);
                
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