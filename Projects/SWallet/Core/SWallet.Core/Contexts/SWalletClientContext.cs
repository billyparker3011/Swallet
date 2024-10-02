using HnMicro.Framework.Contexts;
using HnMicro.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using SWallet.Core.Configs;
using SWallet.Core.Consts;
using SWallet.Core.Models.Clients;

namespace SWallet.Core.Contexts
{
    public class SWalletClientContext : BaseClientContext, ISWalletClientContext
    {
        public SWalletClientContext(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public ClientOfManagerModel Manager
        {
            get
            {
                if (HttpContextAccessor.HttpContext == null)
                    throw new HnMicroException("HttpContext cannot be NULL.");

                var claimManagerId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.ManagerClaimConfig.ManagerId);
                if (claimManagerId == null || string.IsNullOrEmpty(claimManagerId.Value) || !long.TryParse(claimManagerId.Value, out long managerId) || managerId <= 0L)
                    throw new HnMicroException("Cannot parse managerId.");

                var claimMasterId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.MasterId);
                if (claimMasterId == null || string.IsNullOrEmpty(claimMasterId.Value) || !long.TryParse(claimMasterId.Value, out long masterId))
                    throw new HnMicroException("Cannot parse MasterId.");

                var claimSupermasterId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.SupermasterId);
                if (claimSupermasterId == null || string.IsNullOrEmpty(claimSupermasterId.Value) || !long.TryParse(claimSupermasterId.Value, out long supermasterId))
                    throw new HnMicroException("Cannot parse SupermasterId.");

                var claimParentId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.ManagerClaimConfig.ParentId);
                if (claimParentId == null || string.IsNullOrEmpty(claimParentId.Value) || !long.TryParse(claimParentId.Value, out long parentId))
                    throw new HnMicroException("Cannot parse ParentId.");

                var claimUserName = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Username);
                if (claimUserName == null || string.IsNullOrEmpty(claimUserName.Value))
                    throw new HnMicroException("Cannot parse UserName.");

                var claimRole = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.RoleId);
                if (claimRole == null || string.IsNullOrEmpty(claimRole.Value) || !int.TryParse(claimRole.Value, out int roleId))
                    throw new HnMicroException("Cannot parse Role.");

                var claimManagerRole = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.ManagerClaimConfig.ManagerRole);
                if (claimManagerRole == null || string.IsNullOrEmpty(claimManagerRole.Value) || !int.TryParse(claimManagerRole.Value, out int managerRole))
                    throw new HnMicroException("Cannot parse ManagerRole.");

                //var claimPermissions = HttpContextAccessor.HttpContext.User.Claims.Where(f => f.Type == ClaimConfigs.ManagerClaimConfig.Permissions).Select(f => f.Value).ToList();
                //if (claimPermissions.Count == 0)
                //    throw new HnMicroException("Cannot parse Permissions.");

                var claimHash = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Hash);
                if (claimHash == null || string.IsNullOrEmpty(claimHash.Value))
                    throw new HnMicroException("Cannot parse Hash.");

                return new ClientOfManagerModel
                {
                    ManagerId = managerId,
                    UserName = claimUserName.Value,
                    RoleId = roleId,
                    ParentId = parentId,
                    MasterId = masterId,
                    SupermasterId = supermasterId,
                    ManagerRole = managerRole,
                    //Permissions = claimPermissions,
                    Hash = claimHash.Value
                };
            }
        }

        public ClientOfCustomerModel Customer
        {
            get
            {
                if (HttpContextAccessor.HttpContext == null)
                    throw new HnMicroException("HttpContext cannot be NULL.");

                var claimCustomerId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.CustomerClaimConfig.CustomerId);
                if (claimCustomerId == null || string.IsNullOrEmpty(claimCustomerId.Value) || !long.TryParse(claimCustomerId.Value, out long customerId) || customerId == 0L)
                    throw new HnMicroException("Cannot parse CustomerId.");

                var claimUserName = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Username);
                if (claimUserName == null || string.IsNullOrEmpty(claimUserName.Value))
                    throw new HnMicroException("Cannot parse UserName.");

                var claimRole = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.RoleId);
                if (claimRole == null || string.IsNullOrEmpty(claimRole.Value) || !int.TryParse(claimRole.Value, out int roleId))
                    throw new HnMicroException("Cannot parse Role.");

                var claimHash = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Hash);
                if (claimHash == null || string.IsNullOrEmpty(claimHash.Value))
                    throw new HnMicroException("Cannot parse Hash.");

                var claimFirstName = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.CustomerClaimConfig.FirstName);
                var claimLastName = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.CustomerClaimConfig.LastName);
                var claimIsAffiliate = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.CustomerClaimConfig.IsAffiliate);
                var isAffiliate = false;
                if (claimIsAffiliate != null && claimIsAffiliate.Value == "true") isAffiliate = true;
                var claimEmail = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.CustomerClaimConfig.Email);
                var claimTelegram = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.CustomerClaimConfig.Telegram);

                var claimSupermaster = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.SupermasterId);
                if (claimSupermaster == null || string.IsNullOrEmpty(claimSupermaster.Value) || !long.TryParse(claimSupermaster.Value, out long supermasterId))
                    supermasterId = 0L;

                var claimMaster = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.MasterId);
                if (claimMaster == null || string.IsNullOrEmpty(claimMaster.Value) || !long.TryParse(claimMaster.Value, out long masterId))
                    masterId = 0L;

                var claimAgent = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.AgentId);
                if (claimAgent == null || string.IsNullOrEmpty(claimAgent.Value) || !long.TryParse(claimAgent.Value, out long agentId))
                    agentId = 0L;

                return new ClientOfCustomerModel
                {
                    CustomerId = customerId,
                    UserName = claimUserName.Value,
                    RoleId = roleId,
                    Hash = claimHash.Value,
                    FirstName = claimFirstName != null ? claimFirstName.Value : string.Empty,
                    LastName = claimLastName != null ? claimLastName.Value : string.Empty,
                    IsAffiliate = isAffiliate,
                    Email = claimEmail != null ? claimEmail.Value : string.Empty,
                    Telegram = claimTelegram != null ? claimTelegram.Value : string.Empty,
                    SupermasterId = supermasterId,
                    MasterId = masterId,
                    AgentId = agentId
                };
            }
        }

        public void ValidationPrepareToken()
        {
            Microsoft.Extensions.Primitives.StringValues sToken;
            if (!HttpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out sToken)) throw new ForbiddenException();
            if (sToken.ToString() != OtherConsts.PrepareToken) throw new ForbiddenException();
        }
    }
}
