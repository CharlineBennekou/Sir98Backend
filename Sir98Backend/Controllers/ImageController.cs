using Microsoft.AspNetCore.Mvc;

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
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
    }
}