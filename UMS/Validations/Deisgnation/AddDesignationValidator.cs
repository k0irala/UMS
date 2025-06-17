using FluentValidation;
using UMS.Models.Designation;

namespace UMS.Validations.Deisgnation
{
    public class AddDesignationValidator : AbstractValidator<AddDesignationModel>
    {
        public AddDesignationValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Designation Name Is Required")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Designation Name should not contain any letters!");
        }
    }
}
