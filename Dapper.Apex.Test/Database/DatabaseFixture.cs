using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace Dapper.Apex.Test.Database
{
    public class DatabaseFixture: IDisposable
    {
        IDbConnection sqlServerConnection = DbConnectionGenerator.CreateSqlServerConnection(false);
        IDbConnection mysqlConnection = DbConnectionGenerator.CreateMySqlConnection(false);

        public DatabaseFixture()
        {
            try
            {
                sqlServerConnection.Open();
                mysqlConnection.Open();

                var sqlServerScript = GetInitScript("SqlServer");
                var mysqlScript = GetInitScript("MySql");

                sqlServerConnection.Execute("USE master; ALTER DATABASE DapperTest SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE IF EXISTS DapperTest; CREATE DATABASE DapperTest;");
                mysqlConnection.Execute("DROP DATABASE IF EXISTS DapperTest; CREATE DATABASE DapperTest;");

                var sqlServerTask = sqlServerConnection.ExecuteAsync(sqlServerScript);
                var mysqlTask = mysqlConnection.ExecuteAsync(mysqlScript);

                Task.WaitAll(sqlServerTask, mysqlTask);

                var dbInitDataScript = GetInitScript("Data");
                
                sqlServerTask = sqlServerConnection.ExecuteAsync(dbInitDataScript);
                mysqlTask = mysqlConnection.ExecuteAsync(dbInitDataScript);

                Task.WaitAll(sqlServerTask, mysqlTask);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Environment.Exit(-1);
            }
            finally
            {
                sqlServerConnection.Close();
                mysqlConnection.Close();
            }
        }

        public string GetInitScript(string dbtype)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Dapper.Apex.Test.Database.DbInit{dbtype}.sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("!!!!!! Disposing !!!!!!!");
            sqlServerConnection.Dispose();
            mysqlConnection.Dispose();
        }
    }
}
