using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.ReservationDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApiContext _context;
        public ReservationsController(IMapper mapper, ApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult ReservationList()
        {
            var values = _context.Reservations.ToList();
            if (values == null || values.Count == 0)
            {
                return NotFound("Rezervasyon Bulunamadı.");
            }
            return Ok(_mapper.Map<List<ResultReservationDto>>(values));
        }

        [HttpPost]
        public IActionResult CreateReservation(CreateReservationDto createReservationDto)
        {
            var value = _mapper.Map<Reservation>(createReservationDto);
            _context.Reservations.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteReservation(int id)
        {
            var value = _context.Reservations.Find(id);
            if (value == null)
            {
                return NotFound("Rezervasyon Bulunamadı.");
            }
            _context.Reservations.Remove(value);
            _context.SaveChanges();
            return Ok("İşlem başarıyla tamamlandı");
        }

        [HttpGet("{id}")]
        public IActionResult GetReservation(int id)
        {
            var value = _context.Reservations.Find(id);
            if (value == null)
            {
                return NotFound("Rezervasyon Bulunamadı.");
            }
            return Ok(_mapper.Map<GetByIdReservationDto>(value));
        }

        [HttpPut]
        public IActionResult UpdateReservation(UpdateReservationDto updateReservationDto)
        {
            var value = _mapper.Map<Reservation>(updateReservationDto);
            _context.Reservations.Update(value);
            _context.SaveChanges();
            return Ok("Güncelleme işlemi başarılı");
        }
    }
}
