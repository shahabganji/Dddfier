//using Dddfier.GeneratedCode;
//using Dddfier.GeneratedCode.Attributes;

using Dddfier.GeneratedCode;
using Dddfier.GeneratedCode.Attributes;

namespace Dddfier.Console.AggregateIdExample;

[WithIdOf<Guid>(PropertyName = "CustomerId", SetterModifier = SetterModifier.Protected)]
public partial class Customer
{
    public override string ToString() => this.CustomerId.ToString();
}


public class Product
{
    public int Id { get; init; }
}

public class Voucher
{
    public required Customer Id { get; set; }
    public Guid VoucherId { get; init; }
}

