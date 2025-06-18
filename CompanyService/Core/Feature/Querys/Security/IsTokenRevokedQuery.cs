﻿using MediatR;

namespace CompanyService.Core.Feature.Querys.Security
{
    public record IsTokenRevokedQuery(string Jti) : IRequest<bool>;
}
