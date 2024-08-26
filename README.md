# DynamicPocoProxy

## Overview

The `DynamicPocoProxy` library provides a dynamic way to replicate and manipulate objects in .NET.

It allows for deep and shallow cloning of objects, dynamic property access, and custom value modification through the `IValueModifier` interface. Additionally, it includes a custom JSON converter for seamless serialization and deserialization of `DynamicDataReplica` objects.

## Comparison

### DispatchProxy

`DispatchProxy` is a .NET class that provides a way to create proxy instances that can intercept method calls. It is part of the .NET framework and is used to create dynamic proxies for interfaces.

- It creates proxies for interfaces by subclassing `DispatchProxy`.
- It's .NET built in proxy solution.
- It can only intercepts method calls on interfaces.

### Castle.Core

`Castle.Core` is a popular library that provides dynamic proxy capabilities, among other features. It allows for the creation of proxy objects that can intercept method calls and property accesses.

- It uses `ProxyGenerator` to create proxy objects. It supports interface and class proxies.
- It provides extensive interception capabilities through `IInterceptor`
- It can proxy virtual properties only.

### DynamicPocoProxy

- It can proxy and modify read-only properties.
- It can only proxy properties, not able to proxy methods.
- It leverage the `dynamic` capability of .NET, you can't cast the `replica` back to original type.

## Usage example

### Basic Usage

```csharp
var target = new { Name = "John", Age = 30 };
dynamic replica = DynamicDataReplica.DeepClone(target);

Console.WriteLine(replica.Name); // Outputs: John
Console.WriteLine(replica.Age);  // Outputs: 30
```

### Accessing nested properties

```csharp
var target = new { Name = "John", Address = new { City = "New York", Zip = "10001" } };
dynamic replica = DynamicDataReplica.DeepClone(target, true);

Console.WriteLine(replica.Address.City); // Outputs: New York
Console.WriteLine(replica.Address.Zip);  // Outputs: 10001
```

### Updating the property value with Modifier

```csharp
public class CustomModifier : IValueModifier
{
    public bool TryUpdateValue(string propertyPath, object originalValue, out object? modifiedValue)
    {
        if (propertyPath == "Age")
        {
            modifiedValue = (int)originalValue + 1;
            return true;
        }
        modifiedValue = null;
        return false;
    }
}

var target = new { Name = "John", Age = 30 };
dynamic replica = new DynamicDataReplica(target, true, new CustomModifier());

Console.WriteLine(replica.Name); // Outputs: John
Console.WriteLine(replica.Age);  // Outputs: 31
```
