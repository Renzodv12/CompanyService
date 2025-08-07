namespace CompanyService.Core.Models.Task
{
    public class CommentDto
    {
        public string Id { get; set; }
        public AuthorDto Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }

}
