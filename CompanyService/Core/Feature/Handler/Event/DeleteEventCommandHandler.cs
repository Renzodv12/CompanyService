using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Event;
using CompanyService.Core.Feature.Querys.Event;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Event;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Event
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
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

            // Verificar que el usuario puede eliminar el evento (organizador)
            var organizerAttendee = await _unitOfWork.Repository<EventAttendee>()
                .FirstOrDefaultAsync(ea => ea.EventId == request.Id && ea.UserId == Guid.Parse(request.UserId) && ea.IsOrganizer);

            if (organizerAttendee == null)
                throw new DefaultException("Solo el organizador puede eliminar el evento.");

            // Eliminar asistentes primero
            var attendees = await _unitOfWork.Repository<EventAttendee>()
                .WhereAsync(ea => ea.EventId == request.Id);

            foreach (var attendee in attendees)
            {
                _unitOfWork.Repository<EventAttendee>().Remove(attendee);
            }

            // Eliminar evento
            _unitOfWork.Repository<CompanyService.Core.Entities.Event>().Remove(eventEntity);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }

   
}