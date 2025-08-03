using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Event;
using CompanyService.Core.Feature.Querys.Event;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Event;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Event
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Validar fechas
            if (request.Start >= request.End)
                throw new DefaultException("La fecha de inicio debe ser anterior a la fecha de fin.");

            var eventEntity = new CompanyService.Core.Entities.Event
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Start = request.Start,
                End = request.End,
                AllDay = request.AllDay,
                Priority = request.Priority,
                CompanyId = request.CompanyId,
                CreatedBy = Guid.Parse(request.UserId),
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Event>().AddAsync(eventEntity);

            // Agregar asistentes
          //  foreach (var attendeeUserId in request.AttendeeUserIds)
            //{
                var attendee = new EventAttendee
                {
                    Id = Guid.NewGuid(),
                    EventId = eventEntity.Id,
                    //UserId = attendeeUserId,
                    //IsOrganizer = attendeeUserId == Guid.Parse(request.UserId),
                    //Status = attendeeUserId == Guid.Parse(request.UserId)
                    UserId = Guid.Parse(request.UserId),
                    IsOrganizer = true,
                    Status = true
                        ? Core.Enums.AttendeeStatus.Accepted
                        : Core.Enums.AttendeeStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<EventAttendee>().AddAsync(attendee);
           // }

            await _unitOfWork.SaveChangesAsync();
            return eventEntity.Id;
        }
    }

 


   
}