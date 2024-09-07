using FluentValidation;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Agent.AgentService.Validations.Agent
{
    public class BookieSettingValidator : AbstractValidator<AllbetBookieSettingValue>
    {
        public BookieSettingValidator()
        {
            RuleFor(item => item.Agent).NotNull();
            RuleFor(item => item.ApiURL).NotNull();
            RuleFor(item => item.ContentType).NotNull();
            RuleFor(item => item.OperatorId).NotNull();
            RuleFor(item => item.AllbetApiKey).NotNull();
            RuleFor(item => item.PartnerApiKey).NotNull();
            RuleFor(item => item.Suffix).NotNull();
            RuleFor(item => item.AllowScanByRange).NotNull();
            RuleFor(item => item.IsMaintainance).NotNull();
            RuleFor(item => item.FromScanByRange).NotNull().When(c=>c.AllowScanByRange).LessThan(c => c.ToScanByRange);
            RuleFor(item => item.ToScanByRange).NotNull().When(c=>c.AllowScanByRange);
            RuleFor(item => item.FromMaintainance).NotNull().When(c=>c.IsMaintainance).LessThan(c => c.ToMaintainance);
            RuleFor(item => item.ToScanByRange).NotNull().When(c=>c.IsMaintainance);
        }
    }
}
