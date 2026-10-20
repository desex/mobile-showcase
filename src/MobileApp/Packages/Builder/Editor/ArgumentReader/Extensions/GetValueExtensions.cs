using System;

namespace Sotbifay.Builder.Editor
{
    public static class GetValueExtensions
    {
        #region Bool

        public static bool TryGetBool(this ArgumentsReader argumentsReader, string argumentName, out bool result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.HasArgument(argumentName) == false)
                return result = false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            if (string.IsNullOrEmpty(argumentValue))
                return result = true;

            result = bool.TryParse(argumentValue, out result) == false || result;
            return true;
        }
        
        public static bool GetBool(this ArgumentsReader argumentsReader, string argumentName, bool defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetBool(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Byte
        
        public static bool TryGetByte(this ArgumentsReader argumentsReader, string argumentName, out byte result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return byte.TryParse(argumentValue, out result);
        }
        
        public static byte GetByte(this ArgumentsReader argumentsReader, string argumentName, byte defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetByte(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region SByte
        
        public static bool TryGetSByte(this ArgumentsReader argumentsReader, string argumentName, out sbyte result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return sbyte.TryParse(argumentValue, out result);
        }
        
        public static sbyte GetSByte(this ArgumentsReader argumentsReader, string argumentName, sbyte defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetSByte(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Short
        
        public static bool TryGetShort(this ArgumentsReader argumentsReader, string argumentName, out short result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return short.TryParse(argumentValue, out result);
        }
        
        public static short GetShort(this ArgumentsReader argumentsReader, string argumentName, short defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetShort(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region UShort
        
        public static bool TryGetUShort(this ArgumentsReader argumentsReader, string argumentName, out ushort result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return ushort.TryParse(argumentValue, out result);
        }
        
        public static ushort GetUShort(this ArgumentsReader argumentsReader, string argumentName, ushort defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetUShort(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Int
        
        public static bool TryGetInt(this ArgumentsReader argumentsReader, string argumentName, out int result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return int.TryParse(argumentValue, out result);
        }
        
        public static int GetInt(this ArgumentsReader argumentsReader, string argumentName, int defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetInt(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region UInt
        
        public static bool TryGetUInt(this ArgumentsReader argumentsReader, string argumentName, out uint result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return uint.TryParse(argumentValue, out result);
        }
        
        public static uint GetUInt(this ArgumentsReader argumentsReader, string argumentName, uint defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetUInt(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Long
        
        public static bool TryGetLong(this ArgumentsReader argumentsReader, string argumentName, out long result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return long.TryParse(argumentValue, out result);
        }
        
        public static long GetLong(this ArgumentsReader argumentsReader, string argumentName, long defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetLong(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region ULong
        
        public static bool TryGetULong(this ArgumentsReader argumentsReader, string argumentName, out ulong result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return ulong.TryParse(argumentValue, out result);
        }
        
        public static ulong GetULong(this ArgumentsReader argumentsReader, string argumentName, ulong defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetULong(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Float
        
        public static bool TryGetFloat(this ArgumentsReader argumentsReader, string argumentName, out float result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return float.TryParse(argumentValue, out result);
        }
        
        public static float GetFloat(this ArgumentsReader argumentsReader, string argumentName, float defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetFloat(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Double
        
        public static bool TryGetDouble(this ArgumentsReader argumentsReader, string argumentName, out double result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return double.TryParse(argumentValue, out result);
        }
        
        public static double GetDouble(this ArgumentsReader argumentsReader, string argumentName, double defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetDouble(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Decimal
        
        public static bool TryGetDecimal(this ArgumentsReader argumentsReader, string argumentName, out decimal result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return decimal.TryParse(argumentValue, out result);
        }
        
        public static decimal GetDecimal(this ArgumentsReader argumentsReader, string argumentName, decimal defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetDecimal(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region String
        
        public static bool TryGetString(this ArgumentsReader argumentsReader, string argumentName, out string result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            result = argumentsReader.GetValue(argumentName);
            return true;
        }
        
        public static string GetString(this ArgumentsReader argumentsReader, string argumentName, string defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            return argumentsReader.TryGetString(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
        
        #region Enum
        
        public static bool TryGetEnum(this ArgumentsReader argumentsReader, string argumentName, Type enumType, out object result)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (enumType is null)
                throw new ArgumentNullException(nameof(enumType));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return Enum.TryParse(enumType, argumentValue, out result);
        }

        public static object GetEnum(this ArgumentsReader argumentsReader, string argumentName, Type enumType, object defaultValue = default)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            if (enumType is null)
                throw new ArgumentNullException(nameof(enumType));

            return argumentsReader.TryGetEnum(argumentName, enumType, out var result)
                ? result
                : defaultValue;
        }

        public static bool TryGetEnum<TEnum>(this ArgumentsReader argumentsReader, string argumentName, out TEnum result) where TEnum : struct, Enum
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            result = default;
            if (argumentsReader.HasArgument(argumentName) == false)
                return false;

            var argumentValue = argumentsReader.GetValue(argumentName);
            return Enum.TryParse(argumentValue, out result);
        }

        public static TEnum GetEnum<TEnum>(this ArgumentsReader argumentsReader, string argumentName, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            return argumentsReader.TryGetEnum<TEnum>(argumentName, out var result)
                ? result
                : defaultValue;
        }
        
        #endregion
    }
}