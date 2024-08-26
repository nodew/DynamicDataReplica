namespace DynamicPocoProxy;

/// <summary>
/// Represents a value modifier that can be used to modify the value of <see cref="DynamicDataReplica"/> objects.
/// </summary>
public interface IValueModifier
{
    bool TryUpdateValue(string propertyPath, object? value, out object? modifiedValue);
}
