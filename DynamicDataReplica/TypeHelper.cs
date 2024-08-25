namespace DynamicDataReplica
{
    internal static class TypeHelper
    {
        /// <summary>
        /// Gets the non-nullable type of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The non-nullable type.</returns>
        public static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? (Nullable.GetUnderlyingType(type))! : type;
        }

        /// <summary>
        /// Gets the generic element type of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The generic element type.</returns>
        public static Type GetGenericElementType(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// Determines whether the specified type is a simple type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is a simple type; otherwise, <c>false</c>.</returns>
        public static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal);
        }

        /// <summary>
        /// Determines whether the specified type is an atomic type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAtomicType(Type type)
        {
            type = GetNonNullableType(type);

            return IsSimpleType(type)
                || type == typeof(Guid)
                || type == typeof(Uri)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(DateTimeOffset)
                || type == typeof(Version)
                || type == typeof(Type);
        }

        /// <summary>
        /// Determines whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is a nullable type; otherwise, <c>false</c>.</returns>
        public static bool IsNullableType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Determines whether the specified type is an array type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is an array type; otherwise, <c>false</c>.</returns>
        public static bool IsArrayType(Type type)
        {
            return type.IsArray;
        }

        /// <summary>
        /// Determines whether the specified type is a list type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is a list type; otherwise, <c>false</c>.</returns>
        public static bool IsListType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        /// <summary>
        /// Determines whether the specified type is a dictionary type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is a dictionary type; otherwise, <c>false</c>.</returns>
        public static bool IsDictionaryType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        /// <summary>
        /// Determines whether the specified type is a hash set type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHashSetType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        /// <summary>
        /// Determines whether the specified type is a struct type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is a struct type; otherwise, <c>false</c>.</returns>
        public static bool IsStructType(Type type)
        {
            return type.IsValueType && !IsSimpleType(type);
        }

        /// <summary>
        /// Determines whether the specified type is a class type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is a class type; otherwise, <c>false</c>.</returns>
        public static bool IsClassType(Type type)
        {
            return type.IsClass;
        }
    }
}
