using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOM.Core.Repositories;

namespace OOM.Tests
{
    [TestClass]
    public class TestCore
    {
        [TestMethod]
        public void TestRepositoriesConnection()
        {
            var gitRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Git, new RepositoryConfiguration("https://luanmm@bitbucket.org/luanmm/repominer.git", "luanmm", "bLitbucket?!"));
            Assert.IsTrue(gitRepository.Update());

            //var mercurialRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Mercurial, "");

            //var svnRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Subversion, "");
        }
    }
}
