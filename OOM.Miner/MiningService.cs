using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Miner
{
    partial class MiningService : ServiceBase
    {
        private readonly BackgroundJobServer _server;

        public MiningService()
        {
            InitializeComponent();

            var storage = new SqlServerStorage("HangfireContext", new SqlServerStorageOptions { PrepareSchemaIfNecessary = true });
            var options = new BackgroundJobServerOptions();

            _server = new BackgroundJobServer(options, storage);
        }

        protected override void OnStart(string[] args)
        {
            _server.Start();
        }

        protected override void OnStop()
        {
            _server.Dispose();
        }
    }
}
