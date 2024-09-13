namespace DynamicPocoProxy.Tests.Models;

internal readonly struct StructB(int alpha, string beta)
{
    public int PropertyAlpha { get; } = alpha;

    public string PropertyBeta { get; } = beta;
}
