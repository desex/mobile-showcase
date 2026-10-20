using Sotbifay.Launcher.Common.Sql;
using VContainer;
using VContainer.Unity;

namespace Sotbifay.MobileApp.Common.Sql
{
    public sealed class SqlProjectInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SqlService>()
                .As<ISqlService>();
        }
    }
}