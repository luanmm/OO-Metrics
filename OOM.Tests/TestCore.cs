using System;
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
            using (var gitRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Git, new RepositoryConfiguration("https://luanmm@bitbucket.org/luanmm/repominer.git", "luanmm", "bLitbucket?!")))
            { 
                //List<Revision> list = new List<Revision>(gitRepository.ListRevisions("41d33b9ecd1f577a1a3fd91d83c043ee66d5e972"));
            }
        }

        [TestMethod]
        public void TestMercurialRepository()
        {
            using (var mercurialRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Mercurial, new RepositoryConfiguration("https://luanmm@bitbucket.org/creaceed/mercurial-xcode-plugin")))
            {
                //Assert.IsTrue(mercurialRepository.Update());
            }
        }

        [TestMethod]
        public void TestSubversionRepository()
        {
            using (var svnRepository = RepositoryFactory.CreateRepository(ReporitoryProtocol.Subversion, new RepositoryConfiguration("http://svg-edit.googlecode.com/svn/trunk/")))
            {
                //Assert.IsTrue(svnRepository.Update());
            }
        }
    }
}
