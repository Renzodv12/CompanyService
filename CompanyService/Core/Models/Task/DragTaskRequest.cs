namespace CompanyService.Core.Models.Task
{
    public class DragTaskRequest
    {
        public string TaskId { get; set; }
        public string SourceColumnId { get; set; }
        public string TargetColumnId { get; set; }
        public int NewPosition { get; set; }
        public string TargetTaskId { get; set; } // Para insertar antes/después de otra tarea
    }
}
