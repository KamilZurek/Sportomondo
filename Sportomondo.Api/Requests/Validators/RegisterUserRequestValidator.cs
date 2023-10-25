using FluentValidation;
using Sportomondo.Api.Context;
using System;

namespace Sportomondo.Api.Requests.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator(SportomondoDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .Custom((email, context) =>
                    {
                        var emailAlreadyExist = dbContext.Users.Any(u => u.Email == email);

                        if (emailAlreadyExist)
                        {
                            context.AddFailure("Email", "This email is already taken.");
                        }
                    });

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .GreaterThan(new DateTime(1900, 01, 01))
                .LessThan(DateTime.Now);

            RuleFor(x => x.Weight)
                .GreaterThan(0m);

            RuleFor(x => x.Password)
                .MinimumLength(5)
                .Equal(x => x.ConfirmPassword);
        }
    }
}
