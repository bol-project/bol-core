using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(p => p).NotEmpty().WithMessage("Person object cannot be empty.");
            RuleFor(p => p.Nin).NotEmpty().WithMessage("National Identification Number cannot be empty.");
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty.");
            RuleFor(p => p.Surname).NotEmpty().WithMessage("Surname cannot be empty.");
            RuleFor(p => p.Combination).NotEmpty().WithMessage("2 digit combination cannot be empty.");
            RuleFor(p => p.Country).NotEmpty().WithMessage("Country cannot be empty.");

            RuleFor(p => p.Name).Must(HasCapitalLetters).WithMessage("Name must consist of capital letters A-Z.");
            RuleFor(p => p.Surname).Must(HasCapitalLetters).WithMessage("Surname must consist of capital letters A-Z.");
            RuleFor(p => p.MiddleName)
                .Must(HasCapitalLetters)
                .When(p => !string.IsNullOrEmpty(p.MiddleName))
                .WithMessage("Middle Name must consist of capital letters A-Z.");


        }

        private bool HasCapitalLetters(string input)
        {
            return true;
        }
    }
}
