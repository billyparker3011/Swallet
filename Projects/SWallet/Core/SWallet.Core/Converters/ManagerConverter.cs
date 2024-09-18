using HnMicro.Core.Helpers;
using SWallet.Core.Enums;
using SWallet.Core.Models.Manager;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class ManagerConverter
    {
        public static ManagerModel ToManagerModel(this Manager manager)
        {
            return new ManagerModel
            {
                FullName = manager.FullName,
                ManagerCode = manager.ManagerCode,
                ManagerId = manager.ManagerId,
                ManagerRole = manager.ManagerRole,
                MasterId = manager.MasterId,
                ParentId = manager.ParentId,
                RoleId = manager.RoleId,
                State = manager.State.ToEnum<ManagerState>(),
                SupermasterId = manager.SupermasterId,
                Username = manager.Username
            };
        }

        public static ManagerSessionModel ToManagerSessionModel(this ManagerSession session)
        {
            return new ManagerSessionModel
            {
                Hash = session.Hash,
                IpAddress = session.IpAddress,
                LatestDoingTime = session.LatestDoingTime,
                ManagerId = session.ManagerId,
                Platform = session.Platform,
                State = session.State.ToEnum<SessionState>(),
                UserAgent = session.UserAgent
            };
        }
    }
}