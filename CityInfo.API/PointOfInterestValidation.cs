using CityInfo.API.Models;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace CityInfo.API
{
    public class PointOfInterestValidation : AbstractValidator<PointOfInterestForCreationDto>
    {
        public PointOfInterestValidation()
        {
            RuleFor(x => x.Name).Custom((name, validationContext) =>
            {
                if (Regex.IsMatch(name, ".[!@#]")) 
                {
                    validationContext.AddFailure("Invalid character in name");
                }
            });
        }
    }
}
