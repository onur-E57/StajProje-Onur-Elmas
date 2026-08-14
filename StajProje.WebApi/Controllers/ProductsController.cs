using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.ProductDtos;
using StajProje.WebApi.Entities;
using FluentValidation;
using AutoMapper;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<Product> _validator;

        public ProductsController(ApiContext context, IMapper mapper, IValidator<Product> validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public IActionResult ProductList()
        {
            var values = _context.Products.ToList();
            if (values == null || values.Count == 0)
            {
                return NotFound("Ürün bulunamadı.");
            }
            return Ok(_mapper.Map<List<ResultProductDto>>(values));
        }

        [HttpPost]
        public IActionResult CreateProduct(CreateProductDto createProductDto)
        {
            var validationResult = _validator.Validate(_mapper.Map<Product>(createProductDto));

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var value = _mapper.Map<Product>(createProductDto);
            _context.Products.Add(value);
            _context.SaveChanges();
            return Ok("Ürün başarıyla eklendi.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var value = _context.Products.Find(id);
            if (value == null)
            {
                return NotFound("Ürün bulunamadı.");
            }
            _context.Products.Remove(value);
            _context.SaveChanges();
            return Ok("Ürün başarıyla silindi.");
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var value = _context.Products.Find(id);
            if (value == null)
            {
                return NotFound("Ürün bulunamadı.");
            }
            return Ok(_mapper.Map<ResultProductDto>(value));
        }

        [HttpPut]
        public IActionResult UpdateProduct(UpdateProductDto updateProductDto)
        {
            var value = _validator.Validate(_mapper.Map<Product>(updateProductDto));
            if (!value.IsValid)
            {
                return BadRequest(value.Errors.Select(x => x.ErrorMessage));
            }
            _context.Products.Update(_mapper.Map<Product>(updateProductDto));
            _context.SaveChanges();
            return Ok("Ürün başarıyla güncellendi.");
        }
    }
}
