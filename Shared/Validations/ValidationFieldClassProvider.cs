using Microsoft.AspNetCore.Components.Forms;

namespace UVGramWeb.Shared.Validations;

public class ValidationFieldClassProvider : FieldCssClassProvider
{
    private readonly string _validClass;
    private readonly string _errorClass;

    public ValidationFieldClassProvider(string validClass, string errorClass)
    {
        _validClass = validClass;
        _errorClass = errorClass;
    }

    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var hasMessages = !editContext.GetValidationMessages(fieldIdentifier).Any();

        return hasMessages ? _validClass : _errorClass;
    }
}