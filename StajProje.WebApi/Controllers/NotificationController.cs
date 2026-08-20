using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ApiContext _context;
        public NotificationsController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult NotificationList()
        {
            var values = _context.Notifications.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return Ok("Bildirim ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteNotification(int id)
        {
            var value = _context.Notifications.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _context.Notifications.Remove(value);
                _context.SaveChanges();
                return Ok("Bildirim silme işlemi başarılı");
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetNotification(int id)
        {
            var value = _context.Notifications.Find(id);
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
        public IActionResult UpdateNotification(Notification notification)
        {
            _context.Notifications.Update(notification);
            _context.SaveChanges();
            return Ok("Bildirim güncelleme işlemi başarılı");
        }
    }
}
