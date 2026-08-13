using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.FeatureDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApiContext _context;
        public FeaturesController(IMapper mapper, ApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult FeatureList()
        {
            var values = _context.Features.ToList();
            return Ok(_mapper.Map<List<ResultFeatureDto>>(values));
        }

        [HttpPost]
        public IActionResult CreateFeature(CreateFeatureDto createFeatureDto)
        {
            var value = _mapper.Map<Feature>(createFeatureDto);
            _context.Features.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");

        }

        [HttpDelete]
        public IActionResult DeleteFeature(int id)
        {
            var value = _context.Features.Find(id);
            if(value == null)
            {
                return NotFound("Veri bulunamadı.");
            }
            _context.Features.Remove(value);
            _context.SaveChanges();
            return Ok("İşlem başarıyla tamamlandı");
        }

        [HttpGet("{id}")]
        public IActionResult GetFeature(int id)
        {
            var value = _context.Features.Find(id);
            if(value == null)
            {
                return NotFound("Veri bulunamadı.");
            }
            return Ok(_mapper.Map<GetByIdFeatureDto>(value));
        }

        [HttpPut]
        public IActionResult UpdateFeature(UpdateFeatureDto updateFeatureDto)
        {
            var value = _mapper.Map<Feature>(updateFeatureDto);
            _context.Features.Update(value);
            _context.SaveChanges();
            return Ok("Güncelleme işlemi başarılı");

        }

    }
}
