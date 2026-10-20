using System;

namespace Sotbifay.Builder.Editor
{
    public static class ArgumentsReaderExtensions
    {
        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.HasArgument(argumentName))
                action?.Invoke();

            return argumentsReader;
        }

        #region Bool

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<bool> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            var value = argumentsReader.GetBool(argumentName);
            action?.Invoke(value);
            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, bool defaultValue, Action<bool> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            var value = argumentsReader.GetBool(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }

        #endregion
        
        #region Byte

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<byte> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetByte(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, byte defaultValue, Action<byte> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetByte(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region SByte

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<sbyte> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetSByte(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, sbyte defaultValue, Action<sbyte> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetSByte(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region Short

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<short> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetShort(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, short defaultValue, Action<short> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetShort(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region UShort

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<ushort> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetUShort(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, ushort defaultValue, Action<ushort> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetUShort(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }

        
        #endregion

        #region Int

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<int> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetInt(argumentName, out var result)) 
                action?.Invoke(result);
            
            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, int defaultValue, Action<int> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetInt(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region UInt

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<uint> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetUInt(argumentName, out var result)) 
                action?.Invoke(result);
            
            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, uint defaultValue, Action<uint> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetUInt(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region Long

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<long> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetLong(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, long defaultValue, Action<long> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            var value = argumentsReader.GetLong(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region ULong

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<ulong> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetULong(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, ulong defaultValue, Action<ulong> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            var value = argumentsReader.GetULong(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region Float

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<float> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetFloat(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, float defaultValue, Action<float> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetFloat(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region Double

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<double> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetDouble(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, double defaultValue, Action<double> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetDouble(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region Decimal

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<decimal> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetDecimal(argumentName, out var result)) 
                action?.Invoke(result);
            
            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, decimal defaultValue, Action<decimal> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));
            
            var value = argumentsReader.GetDecimal(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region String
        
        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, Action<string> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.TryGetString(argumentName, out var result)) 
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do(this ArgumentsReader argumentsReader, string argumentName, string defaultValue, Action<string> action)
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            var value = argumentsReader.GetString(argumentName, defaultValue);
            action?.Invoke(value);
            return argumentsReader;
        }
        
        #endregion
        
        #region Enum

        public static ArgumentsReader Do<TEnum>(this ArgumentsReader argumentsReader, string argumentName, Action<TEnum> action) where TEnum : struct, Enum
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            if (argumentsReader.HasArgument(argumentName) && Enum.TryParse<TEnum>(argumentsReader.GetValue(argumentName), out var result))
                action?.Invoke(result);

            return argumentsReader;
        }

        public static ArgumentsReader Do<TEnum>(this ArgumentsReader argumentsReader, string argumentName, TEnum defaultValue, Action<TEnum> action) where TEnum : struct, Enum
        {
            if (argumentsReader is null)
                throw new ArgumentNullException(nameof(argumentsReader));

            var value = argumentsReader.HasArgument(argumentName) && Enum.TryParse<TEnum>(argumentsReader.GetValue(argumentName), out var result)
                ? result
                : defaultValue;

            action?.Invoke(value);

            return argumentsReader;
        }
        
        #endregion
    }
}