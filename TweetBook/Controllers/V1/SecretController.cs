using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TweetBook.Filter;

namespace TweetBook.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretController : ControllerBase
    {

        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("I have no secret");
        }
    }
}