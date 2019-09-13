using System;
using System.Data;
using System.Data.SqlClient;

namespace SmartRetail.App.DAL.UnitOfWork
{
    public sealed class Session: IDisposable
    {
        private readonly IDbConnection _connection;

        public UnitOfWork UnitOfWork { get; }

        public Session(string conn)
        {
            _connection = new SqlConnection(conn);
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
