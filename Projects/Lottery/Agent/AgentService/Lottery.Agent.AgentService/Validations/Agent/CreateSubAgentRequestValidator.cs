using FluentValidation;
using Lottery.Agent.AgentService.Requests.Agent;
using Lottery.Core.Localizations;

namespace Lottery.Agent.AgentService.Validations.Agent
{
    public class CreateSubAgentRequestValidator : AbstractValidator<CreateSubAgentRequest>
    {
        public CreateSubAgentRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage(Messages.Agent.UserNameIsRequired)
                .Matches(@"^\S+$")
                .WithMessage(Messages.Agent.UserNameNotContainsWhiteSpace)
                .Matches(@"[A-Z\d]")
                .WithMessage(Messages.Agent.UserNameDoesNotContainSpecialCharacters)
                .MinimumLength(2)
                .WithMessage(Messages.Agent.UserNameMustHaveAtLeastTwoCharacters);
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(Messages.Agent.PasswordIsRequired);
        }
    }
}
