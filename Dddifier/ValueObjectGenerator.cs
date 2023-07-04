namespace Dddifier;


public sealed class ValueObjectGenerator
{
}

[WithIdOf<int>(PropertyName = "CustomerId", SetterModifier = SetterModifier.Private)]
public partial class Customer
{
}



public enum SetterModifier
{
    Public,
    Private,
    Protected,
}

public class WithIdOfAttribute<T> : Attribute
{
    public string PropertyName { get; set; } = "Id";
    public SetterModifier SetterModifier { get; set; } = SetterModifier.Private;
}

public sealed partial class CustomerId
{
    private readonly int _value;

    private CustomerId(int id)
    {
        _value = id;
    }

    public static implicit operator int(CustomerId id) => id._value;
    public static explicit operator CustomerId(int id) => new(id);
}

public partial class Customer
{

    public CustomerId CustomerId { get; private set; } = default!;
}
