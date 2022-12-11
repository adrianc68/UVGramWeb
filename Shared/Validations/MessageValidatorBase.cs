using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace UVGramWeb.Shared.Validations;

public class MessageValidatorBase<TValue> : ComponentBase, IDisposable
{
    protected FieldIdentifier _fieldIdentifier;
    protected EventHandler<ValidationStateChangedEventArgs> _stateChangedHandler;
    [CascadingParameter]
    protected EditContext EditContext { get; set; }
    [Parameter]
    public Expression<Func<TValue>> For { get; set; }

    protected IEnumerable<string> ValidationMessages =>
        EditContext.GetValidationMessages(_fieldIdentifier);

    protected override void OnInitialized()
    {
        _fieldIdentifier = FieldIdentifier.Create(For);
        EditContext.OnValidationStateChanged += _stateChangedHandler;
    }

    public void Dispose()
    {
        EditContext.OnValidationStateChanged -= _stateChangedHandler;
    }
}