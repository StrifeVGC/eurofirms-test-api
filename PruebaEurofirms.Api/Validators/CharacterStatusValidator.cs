using FluentValidation;

/// <summary>
/// The fluent validator for Character Status. Validates that the status is one of the allowed values in the Enum.
/// </summary>
public class CharacterStatusValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterStatusValidator"/> class.
    /// </summary>
    public CharacterStatusValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .Must(BeAValidStatus)
            .WithMessage("Status must be one of: Alive, Dead, Unknown");
    }

    /// <summary>
    /// Checks if the provided status is a valid enum value.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    private bool BeAValidStatus(string status)
    {
        return Enum.TryParse<CharacterStatus>(status, ignoreCase: true, out _);
    }
}
