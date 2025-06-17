using FluentValidation;
using UMS.Models.Designation;

namespace UMS.Validations.Deisgnation
{
    public class UpdateDesignationValidator :AbstractValidator<UpdateDesignationModel>
    {
        public UpdateDesignationValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id Cannot be Empty");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name Cannot Be Empty")
                .Matches(@"^[a-zA-Z\\s]+$").WithMessage("Name should only contain letters");
        }
    }
}
