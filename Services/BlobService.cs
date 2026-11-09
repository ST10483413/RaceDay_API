using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace RaceDay_API.Services
{
    public class BlobService
    {
        private readonly IWebHostEnvironment _environment;

        public BlobService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            //  Define the uploads folder
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            //  Create the folder if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            //  Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //  Return the URL (CHANGE PORT IF YOUR API IS NOT ON 7203)
            return $"https://localhost:7203/uploads/{fileName}";
        }
    }
}