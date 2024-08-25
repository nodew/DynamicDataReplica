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
        private readonly IValueModifier? modifer;

        private class CustomGetMemberBinder : GetMemberBinder
        {
            public CustomGetMemberBinder(string name) : base(name, false) { }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }


        public DynamicDataReplica(object target, bool recursive, IValueModifier? modifier = null)
        {
            this.target = target;
            this.targetType = target.GetType();
            this.modifer = modifier;
            this.recursive = recursive;
            this.parentPropertyPath = string.Empty;
        }

        private DynamicDataReplica(object target, bool recursive, string propertyNamePrefix, IValueModifier? modifier = null)
        {
            this.target = target;
            this.targetType = target.GetType();
            this.modifer = modifier;
            this.recursive = recursive;
            this.parentPropertyPath = propertyNamePrefix;
        }

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

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return GetTargetProperties().Select(p => p.Name);
        }

        public IEnumerable<PropertyInfo> GetTargetProperties()
        {
            return targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public bool TryGetMemberByName(string name, out object? result)
        {
            return TryGetMember(new CustomGetMemberBinder(name), out result);
        }

        public static DynamicDataReplica ShadowClone(object target)
        {
            return new DynamicDataReplica(target, false);
        }

        public static DynamicDataReplica ShadowCloneWithModifier(object target, IValueModifier modifier)
        {
            return new DynamicDataReplica(target, false, modifier);
        }

        public static DynamicDataReplica DeepClone(object target)
        {
            return new DynamicDataReplica(target, true);
        }

        public static DynamicDataReplica DeepCloneWithModifier(object target, IValueModifier modifier)
        {
            return new DynamicDataReplica(target, true, modifier);
        }

        private object? InternalPropertyClone(object? prop, string propertyPath)
        {
            if (prop is null)
            {
                return null;
            }

            if (modifer?.TryUpdateValue(propertyPath, prop, out object? modifiedValue) ?? false)
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

        private Dictionary<string, object?> CloneDictionary(IDictionary dict, string propertyPath)
        {
            if (dict is null || dict.Count == 0)
            {
                return new Dictionary<string, object?>();
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

        private List<object?> CloneList(IList list, string propertyPath)
        {
            if (list is null || list.Count == 0)
            {
                return new List<object?>();
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

        private DynamicDataReplica? CloneClass(object prop, string propertyPath)
        {
            return new DynamicDataReplica(prop, recursive, propertyPath, modifer);
        }
    }
}
