using CompanyService.Core.Feature.Querys.Security;
using CompanyService.Core.Interfaces;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace CompanyService.Core.Feature.Handler.Security
{
    public class IsTokenRevokedHandler : IRequestHandler<IsTokenRevokedQuery, bool>
    {
        private readonly ILogger<IsTokenRevokedHandler> _logger;
        private readonly IRedisService _redisService;

        public IsTokenRevokedHandler(ILogger<IsTokenRevokedHandler> logger, IRedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
        }

        public async Task<bool> Handle(IsTokenRevokedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar si el token está en la lista de tokens revocados en Redis
                var revokedKey = $"revoked_token:{request.Jti}";
                var isRevoked = await _redisService.KeyExistsAsync(revokedKey);
                
                return isRevoked;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si token {Jti} está revocado", request.Jti);
                // En caso de error, por seguridad, consideramos el token como no revocado
                // para no bloquear usuarios legítimos
                return false;
            }
        }
    }
}
