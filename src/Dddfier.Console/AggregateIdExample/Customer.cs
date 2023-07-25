using Dddfier.GeneratedCode;
using Dddfier.GeneratedCode.Attributes;

namespace Dddfier.Console.AggregateIdExample;

[WithIdOf<double>(PropertyName = "CustomerId", SetterModifier = SetterModifier.Protected)]
public partial class Customer
{
    public override string ToString() => this.CustomerId.ToString();

    // void Foo()
    // {
    //     var customerId = (CustomerId)Guid.NewGuid();
    //     Guid customerIdGuid = CustomerId;
    // }
}


// public class Product
// {
//     public int Id { get; init; }
// }
//
// public class Voucher
// {
//     public Guid VoucherId { get; init; }
// }
 
