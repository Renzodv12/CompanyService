using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Feature.Querys.Task;
using CompanyService.Core.Models.Task;
using CompanyService.WebApi.Extensions;
using MediatR;
namespace CompanyService.WebApi.Endpoints
{
    public static class TaskEndpoints
    {
        public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            // Board endpoints
            app.MapGet("/companies/{companyId:guid}/tasks/board", GetTaskBoard)
                .WithName("GetTaskBoard")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Produces<TaskBoardResponse>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Task CRUD endpoints
            app.MapPost("/companies/{companyId:guid}/tasks", CreateTask)
                .WithName("CreateTask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<CreateTaskRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/tasks/{taskId:guid}", GetTask)
                .WithName("GetTask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Produces<TaskDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/tasks/{taskId:guid}", UpdateTask)
                .WithName("UpdateTask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<UpdateTaskRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/tasks/{taskId:guid}", DeleteTask)
                .WithName("DeleteTask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Drag and drop
            app.MapPost("/companies/{companyId:guid}/tasks/drag", DragTask)
                .WithName("DragTask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<DragTaskRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Column CRUD endpoints
            app.MapPost("/companies/{companyId:guid}/task-columns", CreateColumn)
                .WithName("CreateTaskColumn")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<CreateColumnRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/task-columns/{columnId:guid}", UpdateColumn)
                .WithName("UpdateTaskColumn")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<UpdateColumnRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/task-columns/{columnId:guid}", DeleteColumn)
                .WithName("DeleteTaskColumn")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/task-columns/{columnId:guid}/clear", ClearColumn)
                .WithName("ClearTaskColumn")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Comments
            app.MapPost("/companies/{companyId:guid}/tasks/{taskId:guid}/comments", AddComment)
                .WithName("AddTaskComment")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<AddCommentRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            // Reply to comment
            app.MapPost("/companies/{companyId:guid}/tasks/{taskId:guid}/comments/{commentId:guid}/reply", AddCommentReply)
                .WithName("AddCommentReply")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<AddCommentRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            // Subtasks
            app.MapPost("/companies/{companyId:guid}/tasks/{taskId:guid}/subtasks", CreateSubtask)
                .WithName("CreateSubtask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<CreateSubtaskRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/tasks/{taskId:guid}/subtasks/{subtaskId:guid}", UpdateSubtask)
                .WithName("UpdateSubtask")
                .WithTags("Tasks")
                .RequireAuthorization()
                .Accepts<UpdateSubtaskRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();
            app.MapPost("/companies/{companyId:guid}/tasks/initialize", InitializeDefaultColumns)
    .WithName("InitializeDefaultTaskColumns")
    .WithTags("Tasks")
    .RequireAuthorization()
    .Produces(StatusCodes.Status200OK)
    .WithOpenApi();
            return app;
        }
        private static async Task<IResult> InitializeDefaultColumns(
    Guid companyId,
    HttpContext httpContext//,
   /* ITaskSeederService taskSeederService*/)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
              //  await taskSeederService.CreateDefaultColumnsForCompanyAsync(companyId);
                return Results.Ok(new { message = "Default columns created successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
        private static async Task<IResult> GetTaskBoard(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetTaskBoardQuery
            {
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var board = await mediator.Send(query);
                return Results.Ok(board);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateTask(
            Guid companyId,
            CreateTaskRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateTaskCommand
            {
                Title = request.Title,
                Description = request.Description,
                ColumnId = Guid.Parse(request.ColumnId),
                DueDate = request.DueDate,
                Labels = request.Labels,
                AssigneeUserIds = request.AssigneeUserIds,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var taskId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/tasks/{taskId}", new { Id = taskId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetTask(
            Guid companyId,
            Guid taskId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetTaskByIdQuery
            {
                Id = taskId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var task = await mediator.Send(query);
                return task is null ? Results.NoContent() : Results.Ok(task);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateTask(
            Guid companyId,
            Guid taskId,
            UpdateTaskRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateTaskCommand
            {
                Id = taskId,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Labels = request.Labels,
                AssigneeUserIds = request.AssigneeUserIds,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteTask(
            Guid companyId,
            Guid taskId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteTaskCommand
            {
                Id = taskId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DragTask(
            Guid companyId,
            DragTaskRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DragTaskCommand
            {
                TaskId = Guid.Parse(request.TaskId),
                SourceColumnId = Guid.Parse(request.SourceColumnId),
                TargetColumnId = Guid.Parse(request.TargetColumnId),
                NewPosition = request.NewPosition,
                TargetTaskId = !string.IsNullOrEmpty(request.TargetTaskId) ? Guid.Parse(request.TargetTaskId) : null,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateColumn(
            Guid companyId,
            CreateColumnRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateColumnCommand
            {
                Name = request.Name,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var columnId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/task-columns/{columnId}", new { Id = columnId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateColumn(
            Guid companyId,
            Guid columnId,
            UpdateColumnRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateColumnCommand
            {
                Id = columnId,
                Name = request.Name,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteColumn(
            Guid companyId,
            Guid columnId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteColumnCommand
            {
                Id = columnId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> ClearColumn(
            Guid companyId,
            Guid columnId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new ClearColumnCommand
            {
                Id = columnId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> AddComment(
            Guid companyId,
            Guid taskId,
            AddCommentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new AddCommentCommand
            {
                TaskId = taskId,
                Content = request.Content,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var commentId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/tasks/{taskId}/comments/{commentId}", new { Id = commentId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateSubtask(
            Guid companyId,
            Guid taskId,
            CreateSubtaskRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateSubtaskCommand
            {
                TaskId = taskId,
                Title = request.Title,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var subtaskId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/tasks/{taskId}/subtasks/{subtaskId}", new { Id = subtaskId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateSubtask(
            Guid companyId,
            Guid taskId,
            Guid subtaskId,
            UpdateSubtaskRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateSubtaskCommand
            {
                Id = subtaskId,
                Title = request.Title,
                Done = request.Done,
                TaskId = taskId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
        private static async Task<IResult> AddCommentReply(
     Guid companyId,
     Guid taskId,
     Guid commentId,
     AddCommentRequest request,
     HttpContext httpContext,
     ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new AddCommentReplyCommand
            {
                TaskId = taskId,
                ParentCommentId = commentId,
                Content = request.Content,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var replyId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/tasks/{taskId}/comments/{replyId}", new { Id = replyId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
 }