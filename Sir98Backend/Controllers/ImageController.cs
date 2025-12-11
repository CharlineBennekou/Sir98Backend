using Microsoft.AspNetCore.Mvc;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly Dictionary<string, string> _extensionAndContentType = new() {
            { ".png", "image/png" },
            { ".jpg", "image/jpeg"},
            { ".jpeg", "image/jpeg" },
            { ".jfif",  "image/jpeg" },
            { ".pjpeg", "image/jpeg" },
            { ".pjp", "image/jpeg" },
            { ".svg", "image/svg+xml" },
            { ".tif", "image/tiff" },
            { ".avif", "image/avif" }
        };
        private readonly string _imageDirectory = Path.Join(Environment.CurrentDirectory, "Images");
        

        /// <summary>
        /// Controller for requesting images
        /// </summary>
        /// <param name="image">Must be filename and extension like this: Mette.jpg</param>
        /// <returns></returns>
        [HttpGet("{image}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public IActionResult GetImage(string image)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string path = Path.Join(workingDirectory, "Images", $"{image}");
            string directory = Path.Join(workingDirectory, "Images");
            if (Directory.Exists(directory) && System.IO.File.Exists(path))
            {
                string extension = Path.GetExtension(path);
                Byte[] b = System.IO.File.ReadAllBytes(path);
                switch (extension)
                {
                    case ".png":
                        return File(b, "image/png");
                    case ".jpg":
                    case ".jpeg":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        return File(b, "image/jpeg");
                    case ".svg":
                        return File(b, "image/svg+xml");
                    case ".tif":
                        return File(b, "image/tiff");
                    case ".avif":
                        return File(b, "image/avif");
                    default:
                        return StatusCode(415, "The requested file type is not supported");
                }
            }
            else
            {
                return NotFound($"Image file not found at {directory} {path}");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post(IEnumerable<IFormFile> images)
        {
            if(images is null || images is default(IEnumerable<IFormFile>) || images.Count() < 1)
            {
                return BadRequest();
            }
            if(images.Count() > 1)
            {
                return BadRequest("Too many images");
            }
            var image = images.First();
            if (image is null || image is default(IFormFile))
            {
                return BadRequest("No image file included");
            }
            var parts = image.FileName.Split(".");
            var fileExtension = $".{parts.Last()}";
            if(
                _extensionAndContentType.ContainsKey(fileExtension) == false || 
                _extensionAndContentType[fileExtension] != image.ContentType
                )
            {
                return StatusCode(415, "The uploaded file type is not supported");
            }

            if(Directory.Exists(_imageDirectory) == false)
            {
                Directory.CreateDirectory(_imageDirectory);
            }
            string newFileName = Guid.NewGuid().ToString();
            string imagePath = Path.Join(_imageDirectory, $"{newFileName}{fileExtension}");
            if(System.IO.File.Exists(imagePath))
            {
                return StatusCode(500, "No space for new image, too many images on server");
            }
            using var fileStream = new FileStream(imagePath, FileMode.Create);
            image.CopyToAsync(fileStream);
            return Ok($"{newFileName}{fileExtension}");
        }
    }
}