using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Event;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Event;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Event
{
    public class GetUserEventsQueryHandler : IRequestHandler<GetUserEventsQuery, List<EventDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserEventsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EventDto>> Handle(GetUserEventsQuery request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                return new List<EventDto>();

            // Obtener eventos donde el usuario es asistente
            var userAttendances = await _unitOfWork.Repository<EventAttendee>()
                .WhereAsync(ea => ea.UserId == Guid.Parse(request.UserId));

            var eventIds = userAttendances.Select(ua => ua.EventId).ToList();

            var events = await _unitOfWork.Repository<CompanyService.Core.Entities.Event>()
                .WhereAsync(e => e.CompanyId == request.CompanyId && e.IsActive && eventIds.Contains(e.Id));

            var query = events.AsQueryable();

            // Filtrar por fechas si se proporcionan
            if (request.StartDate.HasValue)
                query = query.Where(e => e.End >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(e => e.Start <= request.EndDate.Value);

            var filteredEventIds = query.Select(e => e.Id).ToList();
            var allAttendees = await _unitOfWork.Repository<EventAttendee>()
                .WhereAsync(ea => filteredEventIds.Contains(ea.EventId));

            return query.OrderBy(e => e.Start).Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Start = e.Start,
                End = e.End,
                AllDay = e.AllDay,
                Priority = e.Priority,
                IsActive = e.IsActive,
                Attendees = allAttendees.Where(a => a.EventId == e.Id).Select(a => new EventAttendeeDto
                {
                    UserId = a.UserId,
                    Status = a.Status,
                    IsOrganizer = a.IsOrganizer
                }).ToList()
            }).ToList();
        }
    }
}
