using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.CategoryDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult CategoryList()
        {
            var values = _context.Categories.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var value = _mapper.Map<Category>(createCategoryDto);
            _context.Categories.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            var value = _context.Categories.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _context.Categories.Remove(value);
                _context.SaveChanges();
                return Ok("Kategori silme işlemi başarılı");
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var value = _context.Categories.Find(id);
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
        public IActionResult UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var value = _context.Categories.Find(updateCategoryDto.CategoryId);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(updateCategoryDto, value);
                _context.SaveChanges();
                return Ok("Kategori güncelleme işlemi başarılı");
            }
        }
    }
}
