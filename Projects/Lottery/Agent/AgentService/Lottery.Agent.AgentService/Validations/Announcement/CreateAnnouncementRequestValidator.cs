using FluentValidation;
using Lottery.Agent.AgentService.Requests.Announcement;
using Lottery.Core.Localizations;

namespace Lottery.Agent.AgentService.Validations.Announcement
{
    public class CreateAnnouncementRequestValidator : AbstractValidator<CreateAnnouncementRequest>
    {
        public CreateAnnouncementRequestValidator() 
        {
            RuleFor(x => x.Level).NotNull().WithMessage(Messages.Announcement.AnnouncementLevelIsRequired);
        }
    }
}
