using HnMicro.Framework.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HnMicro.Framework.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route(RouteHelper.BaseRoute.V1.BaseRoute)]
    public abstract class HnControllerBase : ControllerBase
    {

    }
}
