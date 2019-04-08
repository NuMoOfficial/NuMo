using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuMo_Tabbed.iOS;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(NuMo_Tabbed.iOS.sqliteservice))]
namespace NuMo_Tabbed.iOS
{

    class sqliteservice : ISQLite
    {
        public sqliteservice() { }

        //If it's the first time, make a local copy of the database and return a connection for that file path.
        //after the first time, making the copy may be skipped.
        public SQLite.SQLiteConnection GetConnection()
        {
            var sqliteFilename = "numoDatabase.db";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, sqliteFilename);

            try
            {
                //---copy only if file does not exist---
                if (!File.Exists(path))
                {
                    File.Copy(sqliteFilename, path);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            //var plat = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var conn = new SQLite.SQLiteConnection(path);

            return conn;
        }
    }
}