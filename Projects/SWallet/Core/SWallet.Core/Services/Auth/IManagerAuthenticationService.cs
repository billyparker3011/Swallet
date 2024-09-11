﻿using HnMicro.Core.Scopes;
using HnMicro.Framework.Models;
using SWallet.Core.Models.Auth;

namespace SWallet.Core.Services.Auth
{
    public interface IManagerAuthenticationService : IScopedDependency
    {
        Task<JwtToken> Auth(AuthModel model);
    }
}