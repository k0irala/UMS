using FluentValidation;
using UMS.Models;

namespace UMS.Validations.Account.Manager
{
    public class ManagerRegisterValidator : AbstractValidator<ManagerRegisterModel>
    {
        public ManagerRegisterValidator() 
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name Cannot Be Empty")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Full Name Cannot Contain Number and Special Characters");
            RuleFor(x => x.UserName)
               .NotEmpty().WithMessage("UserName Cannot Be Empty");

            RuleFor(x => x.Password)
                .MinimumLength(6).MaximumLength(12)
                .NotEmpty().WithMessage("Password Cannot be Empty");
            RuleFor(x => x.DesignationId)
                .NotEmpty().WithMessage("Please Select a Designation");
        }
    }
}
