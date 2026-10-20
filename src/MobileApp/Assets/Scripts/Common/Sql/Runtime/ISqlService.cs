using SQLite;

namespace Sotbifay.Launcher.Common.Sql
{
    public interface ISqlService
    {
        void CreateTable<T>();
        TableQuery<T> Table<T>() where T : new();
        void Insert(object item);
        void Update(object item);
        void Delete(object item);
    }
}