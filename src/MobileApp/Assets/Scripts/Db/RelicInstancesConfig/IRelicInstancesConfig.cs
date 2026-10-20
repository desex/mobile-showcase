using System.Collections.Generic;

namespace Sotbify.MobileApp.Db.RelicInstancesConfig
{
    public interface IRelicInstancesConfig
    {
        IReadOnlyDictionary<string, RelicInstanceInfo> Instances { get; }
    }
}