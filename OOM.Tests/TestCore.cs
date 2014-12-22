using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOM.Core.Repositories;

namespace OOM.Tests
{
    [TestClass]
    public class TestCore
    {
        [TestMethod]
        public void TestGitRepository()
        {
            using (var gitRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Git, new RepositoryConfiguration("https://luanmm@bitbucket.org/luanmm/repominer.git", "luanmm", "bLitbucket?!")))
            { 
                Assert.IsTrue(gitRepository.Update());
            }
        }

        [TestMethod]
        public void TestMercurialRepository()
        {
            using (var mercurialRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Mercurial, new RepositoryConfiguration("https://luanmm@bitbucket.org/creaceed/mercurial-xcode-plugin")))
            {
                Assert.IsTrue(mercurialRepository.Update());
            }
        }

        [TestMethod]
        public void TestSubversionRepository()
        {
            using (var svnRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Subversion, new RepositoryConfiguration("http://svg-edit.googlecode.com/svn/trunk/")))
            {
                Assert.IsTrue(svnRepository.Update());
            }
        }
    }
}
