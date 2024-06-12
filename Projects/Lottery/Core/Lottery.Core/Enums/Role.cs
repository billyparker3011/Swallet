using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Enums
{
    public enum Role
    {
        [Display(Name = "Company")]
        Company = 0,

        [Display(Name = "Supermaster")]
        Supermaster = 1,

        [Display(Name = "Master")]
        Master = 2,

        [Display(Name = "Agent")]
        Agent = 3,

        [Display(Name = "Player")]
        Player = 4
    }
}
