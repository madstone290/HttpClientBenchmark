using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpClientBenchmark.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace HttpClientBenchmark.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        [Route("long")]
        public FileStreamResult GetLong()
        {
            var fileStream = System.IO.File.Open("people-long.json", System.IO.FileMode.Open);
            return File(fileStream, "application/json");
        }

        [Route("middle")]
        public FileStreamResult GetMiddle()
        {
            var fileStream = System.IO.File.Open("people-middle.json", System.IO.FileMode.Open);
            return File(fileStream, "application/json");
        }

        [Route("short")]
        public FileStreamResult GetShort()
        {
            var fileStream = System.IO.File.Open("people-short.json", System.IO.FileMode.Open);
            return File(fileStream, "application/json");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Person value)
        {
        }
    }
}
