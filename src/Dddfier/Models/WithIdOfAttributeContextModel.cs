namespace Dddfier.Models;

internal sealed record WithIdOfAttributeContextModel(
    string Namespace, string ClassName, string PropertyName, string TypeName)
{
    public string Namespace { get; } = Namespace;
    public string ClassName { get; } = ClassName;
    public string PropertyName { get; } = PropertyName;
    public string TypeName { get; } = TypeName;
}
