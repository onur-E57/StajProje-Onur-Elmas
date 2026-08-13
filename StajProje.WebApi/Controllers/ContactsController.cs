using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.ContactDtos;
using StajProje.WebApi.Entities;


namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApiContext _context;
        
        public ContactsController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ContactsList()
        {
            var contacts = _context.Contacts.ToList();
            return Ok(contacts);
        }

        [HttpPost]
        public IActionResult CreateContact(CreateContactDto createContactDto)
        {
            Contact contact = new Contact();
            contact.Email = createContactDto.Email;
            contact.Phone = createContactDto.Phone;
            contact.Address = createContactDto.Address;
            contact.MapLocation = createContactDto.MapLocation;
            contact.OpenHours = createContactDto.OpenHours;
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteContact (int id)
        {
            var value = _context.Contacts.Find(id);
            _context.Contacts.Remove(value);
            _context.SaveChanges();
            return Ok("Silme işlemi başarılı");
        }

        [HttpGet("{id}")]
        public IActionResult GetContact(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [HttpPut]
        public IActionResult UpdateContact(UpdateContactDto updateContactDto)
        {
            Contact contact = new Contact();
            contact.Email = updateContactDto.Email;
            contact.Address = updateContactDto.Address;
            contact.Phone = updateContactDto.Phone;
            contact.OpenHours = updateContactDto.OpenHours;
            contact.MapLocation = updateContactDto.MapLocation;
            contact.ContactId = updateContactDto.ContactId;
            _context.Contacts.Update(contact);
            _context.SaveChanges();
            return Ok("Güncelleme işlemi başarılı");
        }
    }
}
