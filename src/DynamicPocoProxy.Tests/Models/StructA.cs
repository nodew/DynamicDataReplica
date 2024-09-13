namespace DynamicPocoProxy.Tests.Models;

internal readonly struct StructA(int propertyA, StructB propertyB)
{
    public int PropertyA { get; } = propertyA;

    public StructB PropertyB { get; } = propertyB;
}
