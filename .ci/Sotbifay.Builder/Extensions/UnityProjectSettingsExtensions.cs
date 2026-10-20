using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.Tools.Unity;

namespace Extensions;

public static class UnityProjectSettingsExtensions
{
    private const string Prefix = "--";
    private const string Separator = "=";
    
    public static T SetCustomArguments<T>(this T toolSettings, params ValueTuple<string, object>[] arguments) where T : UnityProjectOptions
    {
        return toolSettings.SetCustomArguments(arguments.AsEnumerable());
    }
    
    public static T SetCustomArguments<T>(this T toolSettings, IEnumerable<ValueTuple<string, object>> arguments) where T : UnityProjectOptions
    {
        return toolSettings.SetCustomArguments(arguments.Select(tuple => new KeyValuePair<string, object>(tuple.Item1, tuple.Item2)));
    }
    
    public static T SetCustomArguments<T>(this T toolSettings, params Tuple<string, object>[] arguments) where T : UnityProjectOptions
    {
        return toolSettings.SetCustomArguments(arguments.AsEnumerable());
    }
    
    public static T SetCustomArguments<T>(this T toolSettings, IEnumerable<Tuple<string, object>> arguments) where T : UnityProjectOptions
    {
        return toolSettings.SetCustomArguments(arguments.Select(tuple => new KeyValuePair<string, object>(tuple.Item1, tuple.Item2)));
    }
    
    public static T SetCustomArguments<T>(this T toolSettings, params KeyValuePair<string, object>[] arguments) where T : UnityProjectOptions
    {
        return toolSettings.SetCustomArguments(arguments.AsEnumerable());
    }
    
    public static T SetCustomArguments<T>(this T toolSettings, IEnumerable<KeyValuePair<string, object>> arguments) where T : UnityProjectOptions
    {
        return toolSettings.SetCustomArguments(arguments.Select(pair => $"{Prefix}{pair.Key}{Separator}\"{pair.Value}\""));
    }
}