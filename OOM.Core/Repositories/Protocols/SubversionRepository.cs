﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using SharpSvn;
using System.IO;

namespace OOM.Core.Repositories.Protocols
{
    public class SubversionRepository : Repository
    {
        private SvnTarget _target;
        private SvnClient _client;

        public SubversionRepository(RepositoryConfiguration configuration)
            : base(configuration)
        {
            _target = SvnTarget.FromString(Configuration.RemotePath);

            _client = new SvnClient();
            GetRepoTreeNodes();

            //if (!_client.CheckOut(new Uri(configuration.RemotePath), LocalPath))
                //throw new Exception("An error has ocurred when trying to setup the Subversion repository.");

            //_client.
            //_client.CheckOut

            _client.Update(LocalPath);
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

        public override void Dispose()
        {
            _client.Dispose();
        }

        #region Privates

        private void GetRepoTreeNodes(string currentNodePath = null)
        {
            Collection<SvnListEventArgs> items = null;

            var revision = new SvnRevision(10);

            /*
            Collection<SvnLogEventArgs> items2 = null;
            _client.GetLog("", out items2);
            items2.FirstOrDefault().
            */

            var args = new SvnListArgs();
            args.Revision = revision;

            if (_client.GetList(_target, args, out items))
            {
                foreach (var item in items)
                {
                    if (!String.IsNullOrEmpty(item.Path))
                    {
                        //GetRepoTreeNodes(item.Path);
                    }
                }
            }
        }

        #endregion
    }
}
