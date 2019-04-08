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
using System.IO;
using NuMo_Tabbed;

[assembly: Xamarin.Forms.Dependency(typeof(NuMo_Test.Android.sqliteservice))]
namespace NuMo_Test.Android
{

    class sqliteservice : ISQLite
    {
        public sqliteservice() { }

        //If it's the first time, make a local copy of the database and return a connection for that file path.
        //after the first time, making the copy may be skipped.
        public SQLite.SQLiteConnection GetConnection()
        {
            var sqliteFilename = "NumoData";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFilename);
            if (!File.Exists(path))
            {
                //var asset = Android.App.Application.Context.Assets.Open("numoDatabase.db"); //this line causes exception
                var asset = Application.Context.Assets.Open("numoDatabase.db");
                var dest = File.Create(path);
                asset.CopyTo(dest);
            }
            //outdated call
            //var plat = new SQLite.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var conn = new SQLite.SQLiteConnection(path);
            return conn;
        }
    }
}