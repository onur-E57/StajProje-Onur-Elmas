using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajProje.WebApi.Context;
using StajProje.WebApi.Dtos.EmployeeTaskDtos;
using StajProje.WebApi.Dtos.ProductDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTasksController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;
        public EmployeeTasksController(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult EmployeeTaskList()
        {
            var values = _context.EmployeeTasks.ToList();
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateEmployeeTask(CreateEmployeeTaskDto createEmployeeTaskDto)
        {
            var value = _mapper.Map<EmployeeTask>(createEmployeeTaskDto);
            _context.EmployeeTasks.Add(value);
            _context.SaveChanges();
            return Ok("Ekleme işlemi başarılı");
        }

        [HttpDelete]
        public IActionResult DeleteEmployeeTask(int id)
        {
            var value = _context.EmployeeTasks.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _context.EmployeeTasks.Remove(value);
                _context.SaveChanges();
                return Ok("Çalışan görevi silme işlemi başarılı");
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeTask(int id)
        {
            var value = _context.EmployeeTasks.Find(id);
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
        public IActionResult UpdateEmployeeTask(UpdateEmployeeTaskDto updateEmployeeTaskDto)
        {
            var value = _context.EmployeeTasks.Find(updateEmployeeTaskDto.EmployeeTaskId);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(updateEmployeeTaskDto, value);
                _context.SaveChanges();
                return Ok("Çalışan görevi güncelleme işlemi başarılı");
            }
        }
    }
}
