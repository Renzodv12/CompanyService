using CompanyService.Core.Entities;
using CompanyService.Core.Models.Task;

namespace CompanyService.Core.Utils
{
    public static class CommentHelper
    {
        public static List<CommentDto> BuildNestedComments(
            IEnumerable<TaskComment> allComments,
            Guid? parentId = null)
        {
            var comments = allComments
                .Where(c => c.ParentCommentId == parentId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id.ToString(),
                    Author = new AuthorDto
                    {
                        Id = c.AuthorId.ToString(),
                        Name = c.AuthorName,
                        Username = c.AuthorUsername,
                        Avatar = c.AuthorAvatar
                    },
                    CreatedAt = c.CreatedAt,
                    Content = c.Content,
                    Comments = BuildNestedComments(allComments, c.Id) // Recursión para replies
                })
                .ToList();

            return comments;
        }
    }

}
