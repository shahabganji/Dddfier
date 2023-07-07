using Dddfier.GeneratedCode;
using Dddfier.GeneratedCode.Attributes;

namespace Dddfier.Console.AggregateIdExample;

[WithIdOf<Guid>(PropertyName = "CustomerId", SetterModifier = SetterModifier.Protected)]
public partial class Customer
{

    public Customer(Guid id)
    {
        this.CustomerId = (CustomerId)id;
    }

    public Guid ToGuid() => CustomerId;
    
    public override string ToString() => this.CustomerId.ToString();
}
