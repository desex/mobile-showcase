using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace Sotbifay.Builder.Editor
{
    public sealed class ArgumentsReader
    {
        public ArgumentsReader(string parameterPrefix, string parameterSeparator, params string[] args)
        {
            Arguments = Read(parameterPrefix, parameterSeparator, args);
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Arguments { get; }

        public static ArgumentsReader Create() =>
            Create("--", "=");

        public static ArgumentsReader Create(string parameterPrefix, string parameterSeparator) =>
            Create(parameterPrefix, parameterSeparator, Environment.GetCommandLineArgs());

        public static ArgumentsReader Create(string parameterPrefix, string parameterSeparator, params string[] args) =>
            new(parameterPrefix, parameterSeparator, args);

        public bool HasArgument(string argumentName) =>
            Arguments.ContainsKey(argumentName);

        public string GetValue(string argumentName)
        {
            var values = GetValues(argumentName);
            return values.Count > 0 ? values[0] : string.Empty;
        }

        public IReadOnlyList<string> GetValues(string argumentName)
        {
            return HasArgument(argumentName)
                ? Arguments[argumentName]
                : throw new KeyNotFoundException($"Argument '{argumentName}' not found");
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<string>> Read
        (
            string parameterPrefix,
            string parameterSeparator,
            IEnumerable<string> arguments
        )
        {
            if (parameterPrefix is null)
                throw new ArgumentNullException(nameof(parameterPrefix));

            if (parameterSeparator is null)
                throw new ArgumentNullException(nameof(parameterSeparator));

            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            using (DictionaryPool<string, List<string>>.Get(out var result))
            {
                foreach (var (key, value) in ReadArguments(parameterPrefix, parameterSeparator, arguments))
                {
                    if (result.TryGetValue(key, out var values) == false)
                    {
                        values = new List<string>();
                        result.Add(key, values);
                    }

                    values.Add(value);
                }

                return result.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<string>)pair.Value);
            }
        }

        private static IEnumerable<(string Key, string Value)> ReadArguments
        (
            string parameterPrefix,
            string parameterSeparator,
            IEnumerable<string> arguments
        )
        {
            foreach (var arg in arguments)
            {
                if (arg.StartsWith(parameterPrefix, StringComparison.Ordinal) == false)
                    continue;

                var parameter = arg.Substring(parameterPrefix.Length);
                var separatorIndex = parameter.IndexOf(parameterSeparator, StringComparison.Ordinal);

                if (separatorIndex < 0)
                {
                    yield return (parameter, string.Empty);
                    continue;
                }

                var name = parameter.Substring(0, separatorIndex);
                var value = parameter.Substring(separatorIndex + parameterSeparator.Length);

                var hasDoubleQuotes = value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal);
                var hasSingleQuotes = value.StartsWith("'", StringComparison.Ordinal) && value.EndsWith("'", StringComparison.Ordinal);
                if (value.Length >= 2 && (hasDoubleQuotes || hasSingleQuotes))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                else
                {
                    value = value.Trim();
                }

                yield return (name, value);
            }
        }
    }
}