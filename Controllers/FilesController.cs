using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using FileReaderAPI.Models;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Net;

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

                    char[] delimiters = { ' ', '.', ',', ';', '\'', '-', ':', '!', '?', '(', ')', '<', '>', '=', '*', '/', '[', ']', '{', '}', '\\', '"', '\r', '\n' };                    
                
                    // Split text and create new object ordered by highest word count. 
                    var results = source.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Length > 3)
                                                .GroupBy(x => x)
                                                .Select(x => new { Count = x.Count(), Word = x.Key })
                                                .OrderByDescending(x => x.Count);

                    // Extract the word count of the first object. 
                    int highestWordCount = results.First().Count;
                    
                    // Select the elements with that word count. 
                    var elementsWithHighestWordCount = results.Where(x => x.Count == highestWordCount);

                    string processedContent = source;
                    List<string> mostCommonWords = new List<string>();

                    foreach (var element in elementsWithHighestWordCount)
                    {
                        // Surround the common word with foo and bar. 
                        processedContent = processedContent.Replace(element.Word, "foo" + element.Word + "bar");

                        // Add the word to list of most common word. 
                        mostCommonWords.Add(element.Word);
                    }

                    Result result = new Result { FileName = name, ProcessedContent = processedContent, MostCommonWords = mostCommonWords };

                    return Json(result);  
                }
                               
            }            
            catch (FileNotFoundException)
            {
                return NotFound();
            }    
            catch (Exception)
            {
                return new StatusCodeResult(500);
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
