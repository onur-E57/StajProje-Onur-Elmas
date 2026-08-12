using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Entities;
using System.Reflection;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChefsController : ControllerBase
    {
        private readonly ApiContext _context;
        public ChefsController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ChefsList()
        {
            var values = _context.Chefs.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateChef(Chef chef)
        {
            _context.Chefs.Add(chef);
            _context.SaveChanges();
            return Ok(chef);
        }

        [HttpDelete]
        public IActionResult DeleteChef(int id)
        {
            var value = _context.Chefs.Find(id);
            if(value == null)
            {
                return NotFound();
            }else
            {
                _context.Chefs.Remove(value);
                _context.SaveChanges();
                return Ok("Şef başarılı bir şekilde kaldırılmıştır");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdChefList(int id)
        {
            var value = _context.Chefs.Find(id);
            if(value == null)
            {
                return NotFound();
            }else
            {
                return Ok(value);
            }
        }

        [HttpPut]
        public IActionResult UpdateChef(Chef chef)
        {
            var value = _context.Chefs.Find(chef.ChefId);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                value.NameSurname = chef.NameSurname;
                value.Title = chef.Title;
                value.Description = chef.Description;
                value.ImageUrl = chef.ImageUrl;
                _context.SaveChanges();
                return Ok("Şef başarılı bir şekilde güncellenmiştir");
            }
        }
    }
}
