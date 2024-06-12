using Lottery.Core.Enums;

namespace Lottery.Core.Models.Player
{
    public class PlayerModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public Role RoleId { get; set; }
        public UserState State { get; set; }
        public UserState? ParentState { get; set; }
        public bool Lock { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
