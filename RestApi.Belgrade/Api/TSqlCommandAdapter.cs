﻿using Belgrade.SqlClient;
using MsSql.RestApi.DAO;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using BSC = Belgrade.SqlClient;

namespace RestApi.Belgrade.Api
{
    public class TSqlCommandAdapter : TSqlCommand
    {
        public TSqlCommandAdapter(BSC.ICommand cmd)
        {
            this.cmd = cmd;
            this.pipe = cmd;
        }

        public TSqlCommandAdapter(BSC.IQueryPipe pipe)
        {
            this.pipe = pipe;
        }

        public BSC.ICommand cmd { get; }
        public BSC.IQueryPipe pipe { get; }

        public override Task<string> GetString(string defaultOnNoResult = "")
        {
            throw new NotImplementedException(); // -> check is it null
        }

        public override TSqlCommand OnError(Action<Exception> handler)
        {
            this.pipe.OnError(handler);
            return this;
        }

        public override TSqlCommand Sql(SqlCommand cmd)
        {
            this.pipe.Sql(cmd);
            return this;
        }

        public override Task Stream(Stream output, string defaultOnNoResult)
        {
            return this.pipe.Stream(output, defaultOnNoResult);
        }

        public override Task Stream(Stream body, MsSql.RestApi.DAO.Options options)
        {
            return this.pipe.Stream(body, options: new BSC.Options() {
                Prefix = options.Prefix,
                Suffix = options.Suffix, 
                DefaultOutput = options.DefaultOutput
                // @@TODO: Encoding????
            });
        }

        public override Task Stream(StringWriter output, string defaultOnNoResult)
        {
            return this.pipe.Stream(output, defaultOnNoResult);
        }
    }
}
