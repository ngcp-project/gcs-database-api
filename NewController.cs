using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gcs-database-api
{
    [Route("[controller]")]
    public class NewController : Controller
    {
private readonly ILogger<NewController> _logger;
public NewController(ILogger<NewController> logger)
        {
 _logger = logger;
        }

        public IActionResult Index()
        {
    return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}