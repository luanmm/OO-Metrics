using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOM.Core.Repositories;
using System.Collections.Generic;
using OOM.Model;

namespace OOM.Tests
{
    [TestClass]
    public class TestCore
    {
        [TestMethod]
        public void TestGitRepository()
        {
            using (var db = new OOMetricsContext())
            { 
                var repositoryUri = "https://luanmm@bitbucket.org/luanmm/repominer.git";
                var project = db.Projects.FirstOrDefault(x => x.URI.Equals(repositoryUri, StringComparison.InvariantCultureIgnoreCase));
                if (project == null)
                {
                    project = db.Projects.Add(new Project
                    {
                        Name = "RepoMiner",
                        URI = repositoryUri,
                        RepositoryProtocol = RepositoryProtocol.Git,
                        User = "luanmm",
                        Password = "bLitbucket?!"
                    });
                    db.SaveChanges();
                }

                var miner = new RepositoryMiner(project);
                miner.StartMining();
            }
        }

        [TestMethod]
        public void TestMercurialRepository()
        {
            using (var mercurialRepository = RepositoryFactory.CreateRepository(RepositoryProtocol.Mercurial, new RepositoryConfiguration("https://luanmm@bitbucket.org/creaceed/mercurial-xcode-plugin")))
            {
                //Assert.IsTrue(mercurialRepository.Update());
            }
        }

        [TestMethod]
        public void TestSubversionRepository()
        {
            using (var svnRepository = RepositoryFactory.CreateRepository(RepositoryProtocol.Subversion, new RepositoryConfiguration("http://svg-edit.googlecode.com/svn/trunk/")))
            {
                //Assert.IsTrue(svnRepository.Update());
            }
        }

        [TestMethod]
        public void TestDatabaseAccessTypes()
        {
            using (var db = new OOMetricsContext(OOMetricsDBAccessType.FullAccess))
            {
                Assert.AreEqual("OK", db.Database.SqlQuery<string>("SELECT 'OK'").FirstOrDefault());
            }
            using (var db = new OOMetricsContext(OOMetricsDBAccessType.ReadOnly))
            {
                Assert.AreEqual("OK", db.Database.SqlQuery<string>("SELECT 'OK'").FirstOrDefault());
            }
        }
    }
}
