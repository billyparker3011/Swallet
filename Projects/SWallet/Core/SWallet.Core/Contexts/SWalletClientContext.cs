using HnMicro.Framework.Contexts;
using HnMicro.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using SWallet.Core.Configs;
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

                var claimPermissions = HttpContextAccessor.HttpContext.User.Claims.Where(f => f.Type == ClaimConfigs.ManagerClaimConfig.Permissions).Select(f => f.Value).ToList();
                if (claimPermissions.Count == 0)
                    throw new HnMicroException("Cannot parse Permissions.");

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
                    Permissions = claimPermissions,
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

                return new ClientOfCustomerModel
                {
                    CustomerId = customerId,
                    UserName = claimUserName.Value,
                    RoleId = roleId,
                    Hash = claimHash.Value
                };
            }
        }
    }
}
