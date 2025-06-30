using FluentValidation;
using UMS.Models.Employee;

namespace UMS.Validations.Account.Employee;

public class EmployeeRegisterValidation : AbstractValidator<AddEmployee>
{
    public EmployeeRegisterValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(@"^[a-zA-Z][a-zA-Z0-9.]*@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$").WithMessage("Email is invalid.");
        
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .Matches(@"^[a-zA-Z][a-zA-Z0-9.]*$").WithMessage("UserName is invalid.");
        
        RuleFor(x=>x.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .Matches(@"[A-Za-z ]+$").WithMessage("FullName is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8)
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[^a-zA-Z0-9]).*$").WithMessage("Password is invalid.");

        RuleFor(x => x.DesignationId)
            .NotEmpty().WithMessage("DesignationId is required.");
    }
}