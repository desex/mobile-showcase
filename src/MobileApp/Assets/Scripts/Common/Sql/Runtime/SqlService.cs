using System;
using System.IO;
using SQLite;

namespace Sotbifay.Launcher.Common.Sql
{
    public sealed class SqlService : IDisposable, ISqlService
    {
        private const string DatabaseFileName = "database.db";
        private static readonly FileInfo DatabaseFile = new(Path.Combine(UnityEngine.Application.persistentDataPath, DatabaseFileName));

        private readonly SQLiteConnection _databaseConnection = new(DatabaseFile.FullName);
        private readonly object _databaseLock = new();

        public void CreateTable<T>()
        {
            lock (_databaseLock) 
                _databaseConnection.CreateTable<T>();
        }

        public TableQuery<T> Table<T>() where T : new()
        {
            lock (_databaseLock)
            {
                return _databaseConnection.Table<T>();
            }
        }

        public void Insert(object item)
        {
            if(item is null)
                throw new ArgumentNullException(nameof(item));
            
            lock (_databaseLock)
            {
                _databaseConnection.Insert(item);
            }
        }

        public void Update(object item)
        {
            if(item is null)
                throw new ArgumentNullException(nameof(item));
            
            lock (_databaseLock)
            {
                _databaseConnection.Update(item);
            }
        }

        public void Delete(object item)
        {
            if(item is null)
                throw new ArgumentNullException(nameof(item));
            
            lock (_databaseLock)
            {
                _databaseConnection.Delete(item);
            }
        }

        public void Dispose()
        {
            _databaseConnection.Dispose();
        }
    }
}