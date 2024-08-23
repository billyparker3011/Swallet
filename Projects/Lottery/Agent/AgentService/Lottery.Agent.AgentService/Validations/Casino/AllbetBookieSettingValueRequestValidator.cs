using FluentValidation;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Agent.AgentService.Validations.Casino
{
    public class AllbetBookieSettingValueRequestValidator : AbstractValidator<AllbetBookieSettingValue>
    {
        public AllbetBookieSettingValueRequestValidator()
        {
            RuleFor(item => item.Agent).NotNull();
            RuleFor(item => item.ApiURL).NotNull();
            RuleFor(item => item.ContentType).NotNull();
            RuleFor(item => item.OperatorId).NotNull();
            RuleFor(item => item.AllbetApiKey).NotNull();
            RuleFor(item => item.PartnerApiKey).NotNull();
            RuleFor(item => item.Suffix).NotNull();
        }
    }
}