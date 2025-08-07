// CompanyService/Core/Utils/CommentHelper.cs
using CompanyService.Core.Entities;
using CompanyService.Core.Models.Task;

namespace CompanyService.Core.Utils
{
    public static class CommentHelper
    {
        public static List<CommentDto> BuildNestedComments(IEnumerable<TaskComment> comments)
        {
            if (comments == null || !comments.Any())
                return new List<CommentDto>();

            var commentList = comments.ToList();

            // Primero, obtener todos los comentarios padre (sin ParentCommentId)
            var parentComments = commentList
                .Where(c => c.ParentCommentId == null)
                .OrderBy(c => c.CreatedAt)
                .ToList();

            var result = new List<CommentDto>();

            foreach (var parentComment in parentComments)
            {
                var commentDto = new CommentDto
                {
                    Id = parentComment.Id.ToString(),
                    Author = new AuthorDto
                    {
                        Id = parentComment.AuthorId.ToString(),
                        Name = parentComment.AuthorName,
                        Username = parentComment.AuthorUsername,
                        Avatar = parentComment.AuthorAvatar
                    },
                    CreatedAt = parentComment.CreatedAt,
                    Content = parentComment.Content,
                    Comments = GetReplies(parentComment.Id, commentList)
                };

                result.Add(commentDto);
            }

            return result;
        }

        private static List<CommentDto> GetReplies(Guid parentCommentId, List<TaskComment> allComments)
        {
            var replies = allComments
                .Where(c => c.ParentCommentId == parentCommentId)
                .OrderBy(c => c.CreatedAt)
                .ToList();

            var replyDtos = new List<CommentDto>();

            foreach (var reply in replies)
            {
                var replyDto = new CommentDto
                {
                    Id = reply.Id.ToString(),
                    Author = new AuthorDto
                    {
                        Id = reply.AuthorId.ToString(),
                        Name = reply.AuthorName,
                        Username = reply.AuthorUsername,
                        Avatar = reply.AuthorAvatar
                    },
                    CreatedAt = reply.CreatedAt,
                    Content = reply.Content,
                    Comments = GetReplies(reply.Id, allComments) // Recursivo para replies de replies
                };

                replyDtos.Add(replyDto);
            }

            return replyDtos;
        }
    }
}