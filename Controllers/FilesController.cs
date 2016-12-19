using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using FileReaderAPI.Models;
using System.Linq;
using System;

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

            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Create directory if it doesn't exists. 
            System.IO.Directory.CreateDirectory(uploadFolder);            

            // Get all the files from the uploads folder. 
            var fileNames = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "uploads")).Select(Path.GetFileName);

            foreach (var name in fileNames)
            {
                textFiles.Add(new TextFile { Name = name });
            }
                                
            return textFiles;
        }

        // GET api/files/5
        [HttpGet("{fileName}")]
        public async Task<IActionResult> ReadFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);       

                using (StreamReader reader = System.IO.File.OpenText(filePath))                    
                {
                    string source = await reader.ReadToEndAsync();
                
                    return Json(TextProcesser.ReplaceMostCommonWords(fileName, source));  
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
            // Get the files from the request as form object. 
            var files = Request.Form.Files;

            string[] allowedFileTypes = { "text/markdown", "application/rtf", "text/plain" };

            foreach (var file in files)
            {
                // Return bad request if one of the files does not have allowed MIME-type. 
                if (Array.IndexOf(allowedFileTypes, file.ContentType) == -1)
                {
                    return BadRequest();
                }
            }

            // Specify path where to save file. 
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            var uploadedFiles = new List<TextFile>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Save file to uploads folder
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))                    
                    {
                        await file.CopyToAsync(fileStream);
                    }               
                     
                    // Add the uploaded file to list of uploaded files. 
                    uploadedFiles.Add(new TextFile { Name = file.FileName });
                }
            }            

            return Json(uploadedFiles);
        }

        // DELETE api/files/filename.txt
        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);   
                System.IO.File.Delete(filePath);  
                return NoContent();   
            }
            catch (FileNotFoundException)
            {                
                return NotFound();
            }
           
        }
    }
}
