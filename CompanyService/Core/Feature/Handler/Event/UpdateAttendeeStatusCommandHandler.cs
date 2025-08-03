using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Event;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Event
{
    public class UpdateAttendeeStatusCommandHandler : IRequestHandler<UpdateAttendeeStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAttendeeStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateAttendeeStatusCommand request, CancellationToken cancellationToken)
        {
            // Validar acceso a la empresa
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            // Obtener el asistente
            var attendee = await _unitOfWork.Repository<EventAttendee>()
                .FirstOrDefaultAsync(ea => ea.EventId == request.EventId && ea.UserId == Guid.Parse(request.UserId));

            if (attendee == null)
                throw new DefaultException("No estás invitado a este evento.");

            // Actualizar estado
            attendee.Status = request.Status;
            _unitOfWork.Repository<EventAttendee>().Update(attendee);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
