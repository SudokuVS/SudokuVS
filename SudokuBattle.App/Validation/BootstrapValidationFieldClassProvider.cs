using Microsoft.AspNetCore.Components.Forms;

namespace SudokuBattle.App.Validation;

public class BootstrapValidationFieldClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        if (!editContext.IsModified(fieldIdentifier))
        {
            return "";
        }

        bool isValid = editContext.IsValid(fieldIdentifier);

        // Blazor vs. Bootstrap:
        // isvalid = is-valid
        // isinvalid = is-invalid

        return isValid ? "is-valid" : "is-invalid";
    }
}
