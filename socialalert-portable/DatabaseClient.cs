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
            return ExecuteCommand((c) => c.Table<PendingUpload>().ToList());
        }

        public Task<int> UpsertPendingUpload(PendingUpload entry)
        {
            if (entry.Id.HasValue)
            {
                return ExecuteCommand((c) => c.Update(entry));
            }
            else
            {
                return ExecuteCommand((c) => c.Insert(entry));
            }
        }

        public Task<int> DeletePendingUpload(PendingUpload upload)
        {
            return ExecuteCommand((c) => c.Delete(upload));
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        private Task<TResult> ExecuteCommand<TResult>(Func<SQLiteConnection, TResult> function)
        {
            return Task.Run(() => { lock (connection) return function(connection); });
        }

        internal Task<PendingUpload> FindPendingUpload(int uploadId)
        {
            return ExecuteCommand((c) => c.Find<PendingUpload>(uploadId));
        }
    }
}
