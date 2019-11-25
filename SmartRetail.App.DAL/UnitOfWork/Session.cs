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
            _connection = new SqlConnection(@"database=d369fmctn65rq9;host=ec2-46-137-91-216.eu-west-1.compute.amazonaws.com;password=f8774cccfae05a0a4c4bd86c0d40d504db71cd50ebaa52d5ad6767709ffcd5d4;username=nfedurzdslzhpo;port=5432;pooling=True;trust server certificate=True;ssl mode=Require");
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
