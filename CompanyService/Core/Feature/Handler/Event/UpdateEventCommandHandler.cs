using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Event;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Event
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener el evento
            var eventEntity = await _unitOfWork.Repository<CompanyService.Core.Entities.Event>()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.CompanyId == request.CompanyId);

            if (eventEntity == null)
                throw new DefaultException("Evento no encontrado.");

            // Verificar que el usuario puede editar el evento (organizador)
            var organizerAttendee = await _unitOfWork.Repository<EventAttendee>()
                .FirstOrDefaultAsync(ea => ea.EventId == request.Id && ea.UserId == Guid.Parse(request.UserId) && ea.IsOrganizer);

            if (organizerAttendee == null)
                throw new DefaultException("Solo el organizador puede editar el evento.");

            // Validar fechas
            if (request.Start >= request.End)
                throw new DefaultException("La fecha de inicio debe ser anterior a la fecha de fin.");

            // Actualizar evento
            eventEntity.Title = request.Title;
            eventEntity.Description = request.Description;
            eventEntity.Start = request.Start;
            eventEntity.End = request.End;
            eventEntity.AllDay = request.AllDay;
            eventEntity.Priority = request.Priority;
            eventEntity.IsActive = request.IsActive;
            eventEntity.LastModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CompanyService.Core.Entities.Event>().Update(eventEntity);

            // Actualizar asistentes - eliminar los existentes y agregar los nuevos
            var existingAttendees = await _unitOfWork.Repository<EventAttendee>()
                .WhereAsync(ea => ea.EventId == request.Id);

            foreach (var attendee in existingAttendees)
            {
                _unitOfWork.Repository<EventAttendee>().Remove(attendee);
            }

            // Agregar nuevos asistentes
            foreach (var attendeeUserId in request.AttendeeUserIds)
            {
                var attendee = new EventAttendee
                {
                    Id = Guid.NewGuid(),
                    EventId = eventEntity.Id,
                    UserId = attendeeUserId,
                    IsOrganizer = attendeeUserId == Guid.Parse(request.UserId),
                    Status = attendeeUserId == Guid.Parse(request.UserId)
                        ? Core.Enums.AttendeeStatus.Accepted
                        : Core.Enums.AttendeeStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<EventAttendee>().AddAsync(attendee);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

}
