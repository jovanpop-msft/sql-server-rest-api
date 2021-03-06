﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using MsSql.RestApi.DAO;

namespace RestApi.Dapper.Api
{
    public class TSqlCommandAdapter : TSqlCommand
    {
        public TSqlCommandAdapter(SqlConnection connection)
        {
            this.connection = connection;
        }

        public TSqlCommandAdapter(string connection)
        {
            this.connection = new SqlConnection(connection);
        }

        public SqlConnection connection { get; }
        public string SqlText { get; private set; }

        public DynamicParameters parameters;

        public override Task<string> GetString(string defaultOnNoResult = "")
        {
            return this.connection.ExecuteScalarAsync<string>(this.SqlText, param: this.parameters);
        }

        public override TSqlCommand OnError(Action<Exception> handler)
        {
            return this;
            //throw new NotImplementedException("Built-in error handling is not implemented in Dapper RestApi");
        }

        static readonly Dictionary<SqlDbType, DbType> typeMap = new Dictionary<SqlDbType, DbType>()
        {
            { SqlDbType.VarChar, DbType.String },
            { SqlDbType.NVarChar, DbType.String },
            { SqlDbType.Text, DbType.String },
            { SqlDbType.NText, DbType.String },
            { SqlDbType.Char, DbType.String },
            { SqlDbType.NChar, DbType.String },
            { SqlDbType.TinyInt, DbType.Byte },
            { SqlDbType.SmallInt, DbType.Int16 },
            { SqlDbType.Int, DbType.Int32 },
            { SqlDbType.BigInt, DbType.Int64 },
            { SqlDbType.Image, DbType.Binary },
            { SqlDbType.Bit, DbType.Boolean },
            { SqlDbType.Binary, DbType.Binary },
            { SqlDbType.VarBinary, DbType.Binary },
            { SqlDbType.SmallMoney, DbType.Currency },
            { SqlDbType.Money, DbType.Currency },
            { SqlDbType.Decimal, DbType.Decimal },
            { SqlDbType.Real, DbType.Single },
            { SqlDbType.Float, DbType.Double },
            { SqlDbType.SmallDateTime, DbType.DateTime },
            { SqlDbType.Time, DbType.Time },
            { SqlDbType.DateTime, DbType.DateTime },
            { SqlDbType.DateTime2, DbType.DateTime2 },
            { SqlDbType.Date, DbType.Date },
            { SqlDbType.DateTimeOffset, DbType.DateTimeOffset },
            { SqlDbType.Timestamp, DbType.Binary },
            { SqlDbType.UniqueIdentifier, DbType.Guid }
        };

        public override TSqlCommand Sql(SqlCommand cmd)
        {
            this.SqlText = cmd.CommandText;

            this.parameters = new DynamicParameters();
            for (int i = 0; i < cmd.Parameters.Count; i++) {
                var p = cmd.Parameters[i];
                parameters.Add(p.ParameterName, p.Value, dbType: typeMap[p.SqlDbType], size: p.Size, direction: p.Direction);
            }

            return this;
        }

        public override Task Stream(Stream output, string defaultOnNoResult)
        {
            return this.connection.QueryAsyncInto(output, this.SqlText, param: this.parameters, defaultOutput: defaultOnNoResult);
        }

        public override Task Stream(Stream body, Options options)
        {
            var dso = new DapperStreamOptions() { 
                Prefix = options.Prefix,
                DefaultOutput = options.DefaultOutput,
                Suffix = options.Suffix
            };
            return this.connection.QueryAsyncInto(body, this.SqlText, dso, param: this.parameters);
        }

        public override Task Stream(StringWriter output, string defaultOnNoResult)
        {
            throw new NotImplementedException();
        }
    }
}
