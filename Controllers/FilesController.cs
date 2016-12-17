using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using FileReaderAPI.Models;

namespace FileReaderAPI.Controllers
{
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private IHostingEnvironment _environment;

        public FilesController(IHostingEnvironment environment)
        {
            _environment = environment;
        }        

        // GET api/files
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "file1", "file2" };
        }

        // GET api/files/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/files
        [HttpPost]
        public async Task<IActionResult> UploadFiles()
        {
            // Get the files from the request form. 
            var files = Request.Form.Files;

            // Specify path where to save file. 
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            var fileResults = new List<TextFile>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Save file to uploads folder
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))                    
                    {
                        await file.CopyToAsync(fileStream);
                    }               
                     
                    // Add file to list of files 
                    fileResults.Add(new TextFile { 
                        Name = file.FileName,
                        Type = file.ContentType,
                        Length = file.Length,
                    });
                }
            }            

            return Json(fileResults);
        }

        // PUT api/files/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/files/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
