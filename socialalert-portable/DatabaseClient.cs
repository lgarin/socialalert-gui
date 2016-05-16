using Bravson.Socialalert.Portable.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public interface IDatabaseFactory
    {
        SQLiteConnection CreateConnection(string databaseName);
        SQLiteAsyncConnection CreateAsyncConnection(string databaseName);
    }

    public class DatabaseClient : IDisposable
    {
        private readonly SQLiteConnection connection;

        public DatabaseClient(string databaseName)
        {
            connection = DependencyService.Get<IDatabaseFactory>().CreateConnection(databaseName);
            connection.CreateTable<PendingUpload>();
        }
        
        public Task<List<PendingUpload>> FetchAllPendingUploads()
        {
            return Task.Run(() => connection.Table<PendingUpload>().ToList());
        }

        public Task UpsertPendingUpload(PendingUpload entry)
        {
            if (entry.Id.HasValue)
            {
                return Task.Run(() => connection.Update(entry));
            }
            else
            {
                return Task.Run(() => connection.Insert(entry));
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
