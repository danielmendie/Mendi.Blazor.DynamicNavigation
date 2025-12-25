namespace DashboardApp.Abstractions.Helpers
{
    public static class ObjectPropertyHelper
    {
        public static T UpdateProperty<T>(T target, string propertyPath, object? value)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrWhiteSpace(propertyPath))
                throw new ArgumentException("Property path cannot be null or empty.", nameof(propertyPath));

            string[] propertyParts = propertyPath.Split('.');
            object currentObject = target!;

            for (int i = 0; i < propertyParts.Length; i++)
            {
                var propertyName = propertyParts[i];
                var currentType = currentObject.GetType();
                var property = currentType.GetProperty(propertyName);

                if (property == null)
                {
                    throw new ArgumentException(
                        $"Property '{propertyName}' not found on type '{currentType.FullName}'. " +
                        $"Path: '{string.Join(".", propertyParts.Take(i + 1))}'");
                }

                if (i == propertyParts.Length - 1)
                {
                    // Last part, set the value
                    if (!property.CanWrite)
                    {
                        throw new InvalidOperationException($"Property '{propertyName}' is read-only.");
                    }

                    object? convertedValue = ConvertToPropertyType(value, property.PropertyType);
                    property.SetValue(currentObject, convertedValue);
                }
                else
                {
                    // Traverse deeper
                    var nextObject = property.GetValue(currentObject);
                    if (nextObject == null)
                    {
                        throw new NullReferenceException(
                            $"Property '{propertyName}' is null and cannot be traversed.");
                    }

                    currentObject = nextObject;
                }
            }

            return target;
        }

        private static object? ConvertToPropertyType(object? value, Type propertyType)
        {
            // Handle null assignment
            if (value == null)
            {
                // If non-nullable value type, this is invalid
                var underlying = Nullable.GetUnderlyingType(propertyType);
                if (propertyType.IsValueType && underlying == null)
                    throw new InvalidCastException(
                        $"Cannot assign null to non-nullable type '{propertyType.FullName}'.");

                return null;
            }

            // Handle nullable types
            var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            // Handle enums
            if (targetType.IsEnum)
            {
                // Support both string names and numeric values
                if (value is string s)
                {
                    return Enum.Parse(targetType, s, ignoreCase: true);
                }

                var underlyingNumeric = Convert.ChangeType(value, Enum.GetUnderlyingType(targetType));
                return Enum.ToObject(targetType, underlyingNumeric!);
            }

            // General conversion
            return Convert.ChangeType(value, targetType);
        }
    }

}
