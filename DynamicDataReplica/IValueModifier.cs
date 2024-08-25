namespace DynamicDataReplica
{
    public interface IValueModifier
    {
        bool TryUpdateValue(string propertyPath, object? value, out object? modifiedValue);
    }
}
