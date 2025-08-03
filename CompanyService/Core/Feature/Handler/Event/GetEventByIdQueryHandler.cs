using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Event;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Event;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Event
{
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventDto?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                return null;

            var eventEntity = await _unitOfWork.Repository<CompanyService.Core.Entities.Event>()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.CompanyId == request.CompanyId);

            if (eventEntity == null)
                return null;

            var attendees = await _unitOfWork.Repository<EventAttendee>()
                .WhereAsync(ea => ea.EventId == request.Id);

            return new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                Start = eventEntity.Start,
                End = eventEntity.End,
                AllDay = eventEntity.AllDay,
                Priority = eventEntity.Priority,
                IsActive = eventEntity.IsActive,
                Attendees = attendees.Select(a => new EventAttendeeDto
                {
                    UserId = a.UserId,
                    Status = a.Status,
                    IsOrganizer = a.IsOrganizer
                }).ToList()
            };
        }
    }

}
