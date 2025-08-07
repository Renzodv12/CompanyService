using CompanyService.Core.Entities;
using CompanyService.Core.Exceptions;
using CompanyService.Core.Feature.Commands.Task;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateColumnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                throw new DefaultException("No tienes acceso a esta empresa.");

            var column = await _unitOfWork.Repository<TaskColumn>()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId);

            if (column == null)
                throw new DefaultException("Columna no encontrada.");

            column.Name = request.Name;
            _unitOfWork.Repository<TaskColumn>().Update(column);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
