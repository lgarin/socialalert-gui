using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SQLitePCL;
using Bravson.Socialalert.Portable;
using SQLite;
using System.IO;

[assembly: Dependency(typeof(Bravson.Socialalert.Android.DatabaseFactory))]
namespace Bravson.Socialalert.Android
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly string basePath;

        public DatabaseFactory()
        {
            basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
        }

        public SQLiteAsyncConnection CreateAsyncConnection(string databaseName)
        {
            return new SQLiteAsyncConnection(Path.Combine(basePath, databaseName));
        }

        public SQLiteConnection CreateConnection(string databaseName)
        {
            return new SQLite.SQLiteConnection(Path.Combine(basePath, databaseName));
        }
    }
}