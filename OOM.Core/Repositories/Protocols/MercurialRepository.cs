using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace OOM.Core.Repositories.Protocols
{
    public class MercurialRepository : Repository
    {
        private RepositoryConfiguration _repositoryConfig;
        private string _clientPath;

        public MercurialRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            var architecture = "x86";
            if (Environment.Is64BitOperatingSystem)
                architecture = "x64";

            _clientPath = new Uri(String.Format("{0}\\App_Data\\Binaries\\Mercurial\\{1}\\hg.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), architecture)).LocalPath;
            if (!File.Exists(_clientPath))
                throw new DirectoryNotFoundException("The Mercurial client wasn't found.");

            _repositoryConfig = configuration;
            InitializeRepository();
        }

        public override IEnumerable<RepositoryRevision> ListRevisions(string fromRevision = null)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RepositoryNode> ListRevisionNodes(string revision)
        {
            throw new NotImplementedException();
        }

        public override Stream GetNodeContent(RepositoryNode node)
        {
            throw new NotImplementedException();
        }

        #region Privates

        private void InitializeRepository()
        {
            if (!Directory.Exists(String.Format("{0}\\.hg", LocalPath)))
            {
                base.EmptyRepository();
                if (!HgClone())
                    throw new DirectoryNotFoundException("Clone command wasn't sucessful.");
            }
        }

        private bool HgClone()
        {
            var result = ExecuteCmd(String.Format("clone --noupdate {0} {1}", _repositoryConfig.RemotePath, LocalPath));
            return result.ExitCode == 0;
        }

        private ExecutionResult ExecuteCmd(string command)
        {
            var startInfo = new ProcessStartInfo("cmd", String.Format("/c {0} {1}", _clientPath, command));
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            startInfo.EnvironmentVariables.Add("LANG", "en_US.UTF-8");
            startInfo.WorkingDirectory = LocalPath;

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            return new ExecutionResult {
                Output = process.StandardOutput.ReadToEnd(),
                ExitCode = process.ExitCode
            };
        }

        class ExecutionResult
        {
            public string Output { get; set; }
            public int ExitCode { get; set; }
        }

        #endregion
    }
}
