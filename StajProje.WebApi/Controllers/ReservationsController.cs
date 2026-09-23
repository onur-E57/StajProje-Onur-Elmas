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

        [HttpGet("GetTotalReservationCount")]
        public IActionResult GetTotalReservationCount()
        {
            var values = _context.Reservations.Count();
            return Ok(values);
        }

        [HttpGet("GetTotalCustomerCount")]
        public IActionResult GetTotalCustomerCount()
        {
            var values = _context.Reservations.Sum(x => x.CountofPeople);
            return Ok(values);
        }

        [HttpGet("GetPendingReservationsCount")]
        public IActionResult GetPendingReservationsCount()
        {
            var values = _context.Reservations.Where(x => x.ReservationStatus == "Beklemede").Count();
            return Ok(values);
        }

        [HttpGet("GetApprovedReservationsCount")]
        public IActionResult GetApprovedReservationsCount()
        {
            var values = _context.Reservations.Where(x => x.ReservationStatus == "Onaylandı").Count();
            return Ok(values);
        }

        [HttpGet("GetReservationStats")]
        public IActionResult GetReservationStats()
        {
            DateTime today = DateTime.Today;
            DateTime fourMonthsAgo = today.AddMonths(-3);

            // 1. SQL tarafında sadece gruplama ve veri çekme
            var rawData = _context.Reservations
                .Where(r => r.ReservationDate >= fourMonthsAgo)
                .GroupBy(r => new { r.ReservationDate.Year, r.ReservationDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Approved = g.Count(x => x.ReservationStatus == "Onaylandı"),
                    Pending = g.Count(x => x.ReservationStatus == "Beklemede"),
                    Canceled = g.Count(x => x.ReservationStatus == "İptal Edildi")
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList(); // Burada SQL biter, veriler RAM’e alınır

            // 2. Bellekte DTO'ya mapleme + tarih formatlama
            var result = rawData.Select(x => new ReservationChartDto
            {
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                Approved = x.Approved,
                Pending = x.Pending,
                Canceled = x.Canceled
            }).ToList();

            return Ok(result);
        }
    }
}
