using System;
using System.Text.RegularExpressions;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using FluentValidation;

namespace Bol.Core.Validators;

public class CompanyValidator : AbstractValidator<Company>, ICompanyValidator
{
    private readonly Regex _capitalLettersOrNumbers = new Regex(@"^[A-Z0-9]+$");
    private readonly Regex _capitalLettersWithOneSpace = new Regex(@"^[A-Z0-9]+(?: [A-Z0-9]+)*$");

    public CompanyValidator(ICountryCodeService countryCodeService)
    {
        RuleFor(p => p.Combination)
            .NotEmpty()
            .WithMessage("1 digit combination cannot be empty.")
            .Length(1)
            .WithMessage($"Combination must be exactly 1 digits.")
            .Must(_capitalLettersOrNumbers.IsMatch)
            .WithMessage("Combination must be an English Uppercase letter or Number.");
        
        RuleFor(p => p.Title)
            .NotEmpty()
            .WithMessage("Company Title cannot be empty.")
            .Must(_capitalLettersWithOneSpace.IsMatch)
            .WithMessage("Title must be multiple words with English Uppercase letters or Numbers and only one space in between.");
        
        RuleFor(p => p.VatNumber)
            .NotEmpty()
            .WithMessage("VAT Number cannot be empty.")
            .Must(_capitalLettersOrNumbers.IsMatch)
            .WithMessage("VAT Number must have English Uppercase letters or Numbers.");
        
        RuleFor(p => p.Country)
            .NotEmpty()
            .WithMessage("Country cannot be empty.")
            .Must(c => countryCodeService.IsValidCode(c.Alpha3))
            .WithMessage("Country Code is not valid.");
        
        RuleFor(p => p.IncorporationDate)
            .LessThan(DateTime.Now)
            .WithMessage("Incorporation Date must be on a past moment of time.");
    }
}

public interface ICompanyValidator : IValidator<Company> { }
