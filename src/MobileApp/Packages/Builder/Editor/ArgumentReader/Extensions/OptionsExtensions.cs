using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sotbifay.Builder.Editor
{
    public static class OptionsExtensions
    {
        public static T To<T>(this ArgumentsReader argumentsReader)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return (T)argumentsReader.To(typeof(T));
        }

        public static object To(this ArgumentsReader argumentsReader, Type optionsType)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (optionsType is null)
                throw new ArgumentNullException(nameof(optionsType));

            var optionsInstance = Activator.CreateInstance(optionsType);
            ParseFields(argumentsReader, optionsType, optionsInstance);
            ParseProperties(argumentsReader, optionsType, optionsInstance);

            return optionsInstance;
        }

        public static void To(this ArgumentsReader argumentsReader, object optionsInstance)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (optionsInstance is null)
                throw new ArgumentNullException(nameof(optionsInstance));

            var type = optionsInstance.GetType();
            ParseFields(argumentsReader, type, optionsInstance);
            ParseProperties(argumentsReader, type, optionsInstance);
        }

        private static void ParseFields(ArgumentsReader argumentsReader, Type type, object instance)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (TryGetValue(argumentsReader, fieldInfo, out var value) == false)
                    continue;

                fieldInfo.SetValue(instance, value);
            }
        }

        private static void ParseProperties(ArgumentsReader argumentsReader, Type type, object instance)
        {
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                if (TryGetValue(argumentsReader, propertyInfo, out var value) == false)
                    continue;

                propertyInfo.SetValue(instance, value);
            }
        }

        private static bool TryGetValue(ArgumentsReader argumentsReader, FieldInfo fieldInfo, out object result)
        {
            result = null;

            if (TryGetArgumentName(fieldInfo, out var argumentName) == false)
                return false;

            var value = GetValueOrNull(argumentsReader, argumentName, fieldInfo.FieldType);
            if (value is null)
                return false;

            result = value;
            return true;
        }

        private static bool TryGetValue(ArgumentsReader argumentsReader, PropertyInfo propertyInfo, out object result)
        {
            result = null;

            if (TryGetArgumentName(propertyInfo, out var argumentName) == false)
                return false;

            var value = GetValueOrNull(argumentsReader, argumentName, propertyInfo.PropertyType);
            if (value is null)
                return false;

            result = value;
            return true;
        }

        private static bool TryGetArgumentName(MemberInfo memberInfo, out string argumentName)
        {
            argumentName = string.Empty;

            var optionAttribute = memberInfo
                .GetCustomAttributes<OptionAttribute>(true)
                .FirstOrDefault();

            if (optionAttribute is null)
                return false;

            argumentName = optionAttribute.ArgumentName;
            return true;
        }

        private static object GetValueOrNull(ArgumentsReader argumentsReader, string argumentName, Type type)
        {
            if (argumentsReader.HasArgument(argumentName) == false)
                return null;

            var argumentValues = argumentsReader.GetValues(argumentName);
            if (argumentValues.Count == 0)
                return null;
            
            var enumerableElementType = type != typeof(string) 
                ? GetEnumerableElementType(type)
                : null;
            
            return enumerableElementType is not null 
                ? GetListValue(argumentValues, enumerableElementType) 
                : ParseString(argumentValues[0], type);
        }

        private static Type GetEnumerableElementType(Type type)
        {
            return type.GetInterfaces()
                .Where(static type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(static type => type.GetGenericArguments()[0])
                .FirstOrDefault();
        }

        private static object GetListValue(IReadOnlyList<string> values, Type elementType)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (System.Collections.IList)Activator.CreateInstance(listType, values.Count);

            foreach (var value in values)
            {
                var parsed = ParseString(value, elementType);
                if (parsed is null)
                    continue;

                list.Add(parsed);
            }

            return list;
        }

        private static object ParseString(string value, Type type)
        {
            if (type == typeof(bool))
                return bool.TryParse(value, out var result) ? result : null;

            if (type == typeof(byte))
                return byte.TryParse(value, out var result) ? result : null;

            if (type == typeof(sbyte))
                return sbyte.TryParse(value, out var result) ? result : null;

            if (type == typeof(short))
                return short.TryParse(value, out var result) ? result : null;

            if (type == typeof(ushort))
                return ushort.TryParse(value, out var result) ? result : null;

            if (type == typeof(int))
                return int.TryParse(value, out var result) ? result : null;

            if (type == typeof(uint))
                return uint.TryParse(value, out var result) ? result : null;

            if (type == typeof(long))
                return long.TryParse(value, out var result) ? result : null;

            if (type == typeof(ulong))
                return ulong.TryParse(value, out var result) ? result : null;

            if (type == typeof(float))
                return float.TryParse(value, out var result) ? result : null;

            if (type == typeof(double))
                return double.TryParse(value, out var result) ? result : null;

            if (type == typeof(decimal))
                return decimal.TryParse(value, out var result) ? result : null;

            if (type.IsEnum)
                return Enum.TryParse(type, value, true, out var result) ? result : null;
            
            if (type == typeof(string))
                return value;

            return null;
        }
    }
}