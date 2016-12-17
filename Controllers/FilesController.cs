using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using FileReaderAPI.Models;
using System.Linq;
using System.Text;

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
        public IEnumerable<TextFile> Get()
        {           
            var textFiles = new List<TextFile>();

            // Get all the files from the uploads folder. 
            var fileNames = Directory.GetFiles(_environment.WebRootPath + "/uploads").Select(Path.GetFileName);

            foreach (var name in fileNames)
            {
                textFiles.Add(new TextFile { Name = name });
            }
                                
            return textFiles;
        }

        // GET api/files/5
        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "uploads", name);       

                string source = "";

                using (StreamReader reader = System.IO.File.OpenText(filePath))                    
                {
                    source = await reader.ReadToEndAsync();

                    var textFile = new TextFile { Name = name, Content = source };

                    return Json(textFile);  
                }
                               
            }            
            catch (FileNotFoundException)
            {
                return NotFound();
            }            
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
