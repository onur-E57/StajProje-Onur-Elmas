using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.MessageDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApiContext _context;
        public MessagesController(IMapper mapper, ApiContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult MessageList()
        {
            var values = _context.Messages.ToList();
            if(values == null || values.Count == 0)
            {
                return NotFound("No messages found.");
            }
            return Ok(_mapper.Map<List<ResultMessageDto>>(values));
        }

        [HttpPost] 
        public IActionResult CreateMessage(CreateMessageDto createMessageDto)
        {
            var value = _mapper.Map<Message>(createMessageDto);
            _context.Messages.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteMessage(int id)
        {
            var value = _context.Messages.Find(id);
            if(value == null)
            {
                return NotFound("Message not found.");
            }
            _context.Messages.Remove(value);
            _context.SaveChanges();
            return Ok("İşlem başarıyla tamamlandı");
        }

        [HttpGet("{id}")]
        public IActionResult GetMessage(int id)
        {
            var value = _context.Messages.Find(id);
            if(value == null)
            {
                return NotFound("Message not found.");
            }
            return Ok(_mapper.Map<GetByIdMessageDto>(value));
        }

        [HttpPut]
        public IActionResult UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var value = _mapper.Map<Message>(updateMessageDto);
            _context.Messages.Update(value);
            _context.SaveChanges();
            return Ok("Güncelleme işlemi başarılı");
        }

        [HttpGet("MessageListByIsReadFalse")]
        public IActionResult MessageListByIsReadFalse()
        {
            var values = _context.Messages.Where(x=>x.IsRead==false).ToList();
            return Ok(values);
        }
    }
}
