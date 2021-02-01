using Microsoft.AspNetCore.Mvc;

namespace RetailInMotion.WebApi.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class HealthCheckController : Controller
    {
        public string Index()
        {
            return "ok";
        }
    }
}
