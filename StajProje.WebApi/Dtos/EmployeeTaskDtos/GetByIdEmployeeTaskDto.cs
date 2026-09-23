namespace StajProje.WebApi.Dtos.EmployeeTaskDtos
{
    public class GetByIdEmployeeTaskDto
    {
        public int EmployeeTaskId { get; set; }
        public string TaskName { get; set; }
        public byte TaskStatusValue { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
    }
}
