using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.AboutDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public AboutsController(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult AboutList()
        {
            var values = _context.Abouts.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateAbout(CreateAboutDto createAboutDto)
        {
            var value = _mapper.Map<About>(createAboutDto);
            _context.Abouts.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _context.Abouts.Remove(value);
                _context.SaveChanges();
                return Ok("Hakkımızda silme işlemi başarılı");
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(value);
            }
        }

        [HttpPut]
        public IActionResult UpdateAbout(UpdateAboutDto updateAboutDto)
        {
            var value = _context.Abouts.Find(updateAboutDto.AboutId);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(updateAboutDto, value);
                _context.SaveChanges();
                return Ok("Hakkımızda güncelleme işlemi başarılı");
            }
        }
    }
}
