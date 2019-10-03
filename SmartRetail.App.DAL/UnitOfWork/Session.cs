using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SmartRetail.App.DAL.UnitOfWork
{
    public sealed class Session: IDisposable
    {
        private readonly IDbConnection _connection;

        public UnitOfWork UnitOfWork { get; }

        public Session()
        {
            _connection = new SqlConnection(@"Data Source=SQL6007.site4now.net;Initial Catalog=DB_A4E50E_smartretail;User Id=DB_A4E50E_smartretail_admin;Password=1234QWer;");
            _connection.Open();
            UnitOfWork = new UnitOfWork(_connection);
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
            _connection.Dispose();
        }
    }
}
