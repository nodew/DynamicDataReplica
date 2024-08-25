using System.Collections;
using System.Dynamic;
using System.Reflection;

namespace DynamicDataReplica
{
    public class DynamicDataReplica : DynamicObject
    {
        private readonly object target;
        private readonly Type targetType;
        private readonly bool recursive;
        private readonly string parentPropertyPath;
        private readonly IValueModifier? modifier;

        private class CustomGetMemberBinder : GetMemberBinder
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomGetMemberBinder"/> class.
            /// </summary>
            /// <param name="name">The name of the member to get.</param>
            public CustomGetMemberBinder(string name) : base(name, false) { }

            /// <summary>
            /// Performs the binding of the dynamic get member operation.
            /// </summary>
            /// <param name="target">The target of the dynamic get member operation.</param>
            /// <param name="errorSuggestion">The error suggestion.</param>
            /// <returns>The result of the binding.</returns>
            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataReplica"/> class.
        /// </summary>
        /// <param name="target">The target object to replicate dynamically.</param>
        /// <param name="recursive">Indicates whether the replication should be recursive.</param>
        /// <param name="modifier">An optional value modifier.</param>
        public DynamicDataReplica(object target, bool recursive, IValueModifier? modifier = null)
        {
            this.target = target;
            this.targetType = target.GetType();
            this.modifier = modifier;
            this.recursive = recursive;
            this.parentPropertyPath = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataReplica"/> class with a property name prefix.
        /// </summary>
        /// <param name="target">The target object to replicate dynamically.</param>
        /// <param name="recursive">Indicates whether the replication should be recursive.</param>
        /// <param name="propertyNamePrefix">The prefix for the property name.</param>
        /// <param name="modifier">An optional value modifier.</param>
        private DynamicDataReplica(object target, bool recursive, string propertyNamePrefix, IValueModifier? modifier = null)
        {
            this.target = target;
            this.targetType = target.GetType();
            this.modifier = modifier;
            this.recursive = recursive;
            this.parentPropertyPath = propertyNamePrefix;
        }

        /// <summary>
        /// Tries to get the member with the specified name.
        /// </summary>
        /// <param name="binder">The binder that provides the name of the member to get.</param>
        /// <param name="result">When this method returns, contains the result of the get operation.</param>
        /// <returns>true if the member was found; otherwise, false.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var propName = binder.Name;
            var propInfo = targetType.GetProperty(propName);

            if (propInfo is null)
            {
                result = null;
                return false;
            }

            var propertyPath = string.IsNullOrEmpty(parentPropertyPath) ? propName : $"{parentPropertyPath}.{propName}";

            var propValue = propInfo.GetValue(target);

            result = InternalPropertyClone(propValue, propertyPath);
            return true;
        }

        /// <summary>
        /// Returns the dynamic member names.
        /// </summary>
        /// <returns>An enumerable collection of dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return GetTargetProperties().Select(p => p.Name);
        }

        /// <summary>
        /// Gets the properties of the target object.
        /// </summary>
        /// <returns>An enumerable collection of property information.</returns>
        public IEnumerable<PropertyInfo> GetTargetProperties()
        {
            return targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Tries to get the member with the specified name.
        /// </summary>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="result">When this method returns, contains the result of the get operation.</param>
        /// <returns>true if the member was found; otherwise, false.</returns>
        public bool TryGetMemberByName(string name, out object? result)
        {
            return TryGetMember(new CustomGetMemberBinder(name), out result);
        }

        /// <summary>
        /// Creates a shallow clone of the specified target object.
        /// </summary>
        /// <param name="target">The target object to clone.</param>
        /// <returns>A shallow clone of the target object.</returns>
        public static DynamicDataReplica ShallowClone(object target)
        {
            return new DynamicDataReplica(target, false);
        }

        /// <summary>
        /// Creates a shallow clone of the specified target object with a value modifier.
        /// </summary>
        /// <param name="target">The target object to clone.</param>
        /// <param name="modifier">The value modifier.</param>
        /// <returns>A shallow clone of the target object with a value modifier.</returns>
        public static DynamicDataReplica ShallowCloneWithModifier(object target, IValueModifier modifier)
        {
            return new DynamicDataReplica(target, false, modifier);
        }

        /// <summary>
        /// Creates a deep clone of the specified target object.
        /// </summary>
        /// <param name="target">The target object to clone.</param>
        /// <returns>A deep clone of the target object.</returns>
        public static DynamicDataReplica DeepClone(object target)
        {
            return new DynamicDataReplica(target, true);
        }

        /// <summary>
        /// Creates a deep clone of the specified target object with a value modifier.
        /// </summary>
        /// <param name="target">The target object to clone.</param>
        /// <param name="modifier">The value modifier.</param>
        /// <returns>A deep clone of the target object with a value modifier.</returns>
        public static DynamicDataReplica DeepCloneWithModifier(object target, IValueModifier modifier)
        {
            return new DynamicDataReplica(target, true, modifier);
        }

        /// <summary>
        /// Clones the specified property value.
        /// </summary>
        /// <param name="prop">The property value to clone.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The cloned property value.</returns>
        private object? InternalPropertyClone(object? prop, string propertyPath)
        {
            if (prop is null)
            {
                return null;
            }

            if (modifier?.TryUpdateValue(propertyPath, prop, out object? modifiedValue) ?? false)
            {
                return modifiedValue;
            }

            if (!recursive)
            {
                return prop;
            }

            var propType = prop.GetType();

            if (TypeHelper.IsAtomicType(propType))
            {
                return prop;
            }

            if (prop is DynamicDataReplica)
            {
                return prop;
            }

            if (TypeHelper.IsArrayType(propType))
            {
                return CloneArray(prop as Array, propertyPath);
            }

            if (TypeHelper.IsListType(propType))
            {
                return CloneList(prop as IList, propertyPath);
            }

            if (TypeHelper.IsDictionaryType(propType))
            {
                return CloneDictionary(prop as IDictionary, propertyPath);
            }

            if (TypeHelper.IsHashSetType(propType))
            {
                return CloneHashSet(prop as HashSet<object>, propertyPath);
            }

            if (TypeHelper.IsStructType(propType) || TypeHelper.IsClassType(propType))
            {
                return CloneClass(prop, propertyPath);
            }

            return prop;
        }

        /// <summary>
        /// Clones the specified dictionary.
        /// </summary>
        /// <param name="dict">The dictionary to clone.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The cloned dictionary.</returns>
        private Dictionary<string, object?> CloneDictionary(IDictionary dict, string propertyPath)
        {
            if (dict is null || dict.Count == 0)
            {
                return [];
            }

            var clonedDict = new Dictionary<string, object?>();
            var keys = dict.Keys;

            foreach (var key in keys)
            {
                var keyStr = key.ToString();
                var value = dict[key];
                var clonedValue = InternalPropertyClone(value, $"{propertyPath}[{keyStr}]");

                clonedDict.Add(keyStr!, clonedValue);
            }

            return clonedDict;
        }

        /// <summary>
        /// Clones the specified list.
        /// </summary>
        /// <param name="list">The list to clone.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The cloned list.</returns>
        private List<object?> CloneList(IList list, string propertyPath)
        {
            if (list is null || list.Count == 0)
            {
                return [];
            }

            var clonedList = new List<object?>();
            var count = list.Count;

            for (var i = 0; i < count; i++)
            {
                var value = list[i];
                var clonedValue = InternalPropertyClone(value, $"{propertyPath}[{i}]");

                clonedList.Add(clonedValue);
            }

            return clonedList;
        }

        /// <summary>
        /// Clones the specified array.
        /// </summary>
        /// <param name="array">The array to clone.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The cloned array.</returns>
        private Array? CloneArray(Array array, string propertyPath)
        {
            if (array is null)
            {
                return array;
            }

            Type? elementType = array.GetType().GetElementType();

            if (elementType == null)
            {
                return array;
            }

            if (array.Length == 0)
            {
                return Array.CreateInstance(elementType, 0);
            }

            var clonedArray = Array.CreateInstance(elementType, array.Length);

            for (var i = 0; i < array.Length; i++)
            {
                var value = array.GetValue(i);
                var clonedValue = InternalPropertyClone(value, $"{propertyPath}[{i}]");

                clonedArray.SetValue(clonedValue, i);
            }

            return clonedArray;
        }

        /// <summary>
        /// Clones the specified hash set.
        /// </summary>
        /// <param name="hashSet">The hash set to clone.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The cloned hash set.</returns>
        private HashSet<object?> CloneHashSet(HashSet<object> hashSet, string propertyPath)
        {
            if (hashSet is null || hashSet.Count == 0)
            {
                return [];
            }

            var clonedHashSet = new HashSet<object?>();
            var index = 0;

            foreach (var item in hashSet)
            {
                var clonedItem = InternalPropertyClone(item, $"{propertyPath}[{index++}]");

                clonedHashSet.Add(clonedItem);
            }

            return clonedHashSet;
        }

        /// <summary>
        /// Clones the specified class or struct.
        /// </summary>
        /// <param name="prop">The class or struct to clone.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>The cloned class or struct.</returns>
        private DynamicDataReplica? CloneClass(object prop, string propertyPath)
        {
            return new DynamicDataReplica(prop, recursive, propertyPath, modifier);
        }
    }
}