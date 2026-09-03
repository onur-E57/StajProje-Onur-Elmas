using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.ImageDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public ImagesController(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult ImageList()
        {
            var values = _context.Images.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateImage(CreateImageDto createImageDto)
        {
            var value = _mapper.Map<Image>(createImageDto);
            _context.Images.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteImage(int id)
        {
            var value = _context.Images.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _context.Images.Remove(value);
                _context.SaveChanges();
                return Ok("Görsel silme işlemi başarılı");
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetImage(int id)
        {
            var value = _context.Images.Find(id);
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
        public IActionResult UpdateImage(UpdateImageDto updateImageDto)
        {
            var value = _context.Images.Find(updateImageDto.ImageId);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(updateImageDto, value);
                _context.SaveChanges();
                return Ok("Görsel güncelleme işlemi başarılı");
            }
        }
    }
}
