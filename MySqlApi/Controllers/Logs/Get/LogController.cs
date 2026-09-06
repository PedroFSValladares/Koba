using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MySqlApi.Controllers.Logs.Get
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : Controller
    {
        [HttpGet]
        public IActionResult Get(){
            return Ok("Hi");
        }
    }
}