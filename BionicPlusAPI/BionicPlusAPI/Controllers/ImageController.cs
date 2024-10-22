using BionicPlusAPI.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BionicPlusAPI.Controllers
{
    [ApiController]
    [Route("api/Images")]
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Image required");
            }

            var imageInfo = await _imageService.UploadImageAsync(imageFile.FileName, imageFile.OpenReadStream());

            return Ok(imageInfo);
        }
    }
}
