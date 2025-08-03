using CompanyService.Core.Feature.Commands.Event;
using CompanyService.Core.Feature.Querys.Event;
using CompanyService.Core.Models.Event;
using CompanyService.WebApi.Extensions;
using FluentValidation;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class EventEndpoints
    {
        public static IEndpointRouteBuilder MapEventEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies/{companyId:guid}/events", CreateEvent)
                .WithName("CreateEvent")
                .WithTags("Events")
                .RequireAuthorization()
                .Accepts<CreateEventRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/events", GetEvents)
                .WithName("GetEvents")
                .WithTags("Events")
                .RequireAuthorization()
                .Produces<List<EventDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/events/{eventId:guid}", GetEvent)
                .WithName("GetEvent")
                .WithTags("Events")
                .RequireAuthorization()
                .Produces<EventDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/events/{eventId:guid}", UpdateEvent)
                .WithName("UpdateEvent")
                .WithTags("Events")
                .RequireAuthorization()
                .Accepts<UpdateEventRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/events/{eventId:guid}", DeleteEvent)
                .WithName("DeleteEvent")
                .WithTags("Events")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/events/{eventId:guid}/attendee-status", UpdateAttendeeStatus)
                .WithName("UpdateAttendeeStatus")
                .WithTags("Events")
                .RequireAuthorization()
                .Accepts<UpdateAttendeeStatusRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/users/{userId:guid}/events", GetUserEvents)
                .WithName("GetUserEvents")
                .WithTags("Events")
                .RequireAuthorization()
                .Produces<List<EventDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateEvent(
            Guid companyId,
            CreateEventRequest request,
            HttpContext httpContext,
            IValidator<CreateEventRequest> validator,
            ISender mediator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateEventCommand
            {
                Title = request.Title,
                Description = request.Description,
                Start = request.Start,
                End = request.End,
                AllDay = request.AllDay,
                Priority = request.Priority,
                AttendeeUserIds = request.AttendeeUserIds,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var eventId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/events/{eventId}", new { Id = eventId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetEvents(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetEventsQuery
            {
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate,
                UserId = claims.UserId
            };

            try
            {
                var events = await mediator.Send(query);
                return Results.Ok(events);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetEvent(
            Guid companyId,
            Guid eventId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetEventByIdQuery
            {
                Id = eventId,
                CompanyId = companyId,
                UserId = claims.UserId
            };

            try
            {
                var eventDto = await mediator.Send(query);
                return eventDto is null ? Results.NotFound() : Results.Ok(eventDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateEvent(
            Guid companyId,
            Guid eventId,
            UpdateEventRequest request,
            HttpContext httpContext,
            IValidator<UpdateEventRequest> validator,
            ISender mediator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateEventCommand
            {
                Id = eventId,
                Title = request.Title,
                Description = request.Description,
                Start = request.Start,
                End = request.End,
                AllDay = request.AllDay,
                Priority = request.Priority,
                IsActive = request.IsActive,
                AttendeeUserIds = request.AttendeeUserIds,
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

        private static async Task<IResult> DeleteEvent(
            Guid companyId,
            Guid eventId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteEventCommand
            {
                Id = eventId,
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

        private static async Task<IResult> UpdateAttendeeStatus(
            Guid companyId,
            Guid eventId,
            UpdateAttendeeStatusRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateAttendeeStatusCommand
            {
                EventId = eventId,
                Status = request.Status,
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

        private static async Task<IResult> GetUserEvents(
            Guid companyId,
            Guid userId,
            HttpContext httpContext,
            ISender mediator,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetUserEventsQuery
            {
                CompanyId = companyId,
                UserId = userId.ToString(),
                StartDate = startDate,
                EndDate = endDate
            };

            try
            {
                var events = await mediator.Send(query);
                return Results.Ok(events);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}