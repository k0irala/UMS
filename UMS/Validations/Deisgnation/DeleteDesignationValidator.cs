using FluentValidation;
using UMS.Models;

namespace UMS.Validations.Deisgnation
{
    public class DeleteDesignationValidator : AbstractValidator<int>
    {
        public DeleteDesignationValidator() 
        {
            RuleFor(id => id)
                .NotEmpty().WithMessage("Id Cannot be Empty");
        }
    }
}
