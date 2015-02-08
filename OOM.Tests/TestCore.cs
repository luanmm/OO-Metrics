using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOM.Core.Repositories;
using System.Collections.Generic;
using OOM.Model;
using OOM.Repositories;

namespace OOM.Tests
{
    [TestClass]
    public class TestCore
    {
        [TestMethod]
        public void TestGitRepository()
        {
            var repositoryUri = "https://luanmm@bitbucket.org/luanmm/repominer.git";
            var projectRepository = new ProjectRepository();
            var project = projectRepository.GetByUri(repositoryUri);
            if (project == null)
            {
                project = new Project
                {
                    Name = "RepoMiner",
                    URI = repositoryUri,
                    RepositoryProtocol = RepositoryProtocol.Git,
                    User = "luanmm",
                    Password = "bLitbucket?!"
                };
                projectRepository.Create(project);
            }

            var miner = new RepositoryMiner(project);
            miner.StartMining();
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
    }
}
