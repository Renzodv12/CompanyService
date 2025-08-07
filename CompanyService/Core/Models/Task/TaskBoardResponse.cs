namespace CompanyService.Core.Models.Task
{
    public class TaskBoardResponse
    {
        public List<ColumnDto> Columns { get; set; } = new();
        public List<TaskDto> Tasks { get; set; } = new();
    }
}
