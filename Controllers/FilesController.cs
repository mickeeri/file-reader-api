using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace file_reader_api.Controllers
{
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
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
        public void Post([FromBody]string value)
        {
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
