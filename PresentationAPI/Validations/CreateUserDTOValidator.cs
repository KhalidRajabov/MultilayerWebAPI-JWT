using AuthServer.Core.DTOs;
using FluentValidation;

namespace PresentationAPI.Validations
{
    public class CreateUserDTOValidator:AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidator()
        {
            RuleFor(x=>x.Email).NotEmpty().WithMessage("Email required").EmailAddress().WithMessage("Email is not valid");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
        }
    }
}
