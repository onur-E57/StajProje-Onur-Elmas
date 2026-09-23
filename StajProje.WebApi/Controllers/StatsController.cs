using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ApiContext _context;

        public StatsController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet("ProductCount")]
        public IActionResult ProductCount()
        {
            var value = _context.Products.Count();
            return Ok(value);
        }
        [HttpGet("ReservationCount")]
        public IActionResult ReservationCount()
        {
            var value = _context.Reservations.Count();
            return Ok(value);
        }
        [HttpGet("ChefCount")]
        public IActionResult ChefCount()
        {
            var value = _context.Chefs.Count();
            return Ok(value);
        }
        [HttpGet("MessageCount")]
        public IActionResult MessageCount()
        {
            var value = _context.Messages.Count();
            return Ok(value);
        }
    }
}
