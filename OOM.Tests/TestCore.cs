using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOM.Core.Repositories;
using System.Collections.Generic;
using OOM.Model;
using OOM.Core.Math;

namespace OOM.Tests
{
    [TestClass]
    public class TestCore
    {
        [TestMethod]
        public void TestMathParser()
        {
            // Max
            var p1 = new Dictionary<string, object>();
            p1.Add("a", new decimal[] { 1M, 2M, 3M });
            Assert.AreEqual(3M, ExpressionEvaluator.Instance.Evaluate("max(a)", p1));

            var p2 = new Dictionary<string, object>();
            p2.Add("a", new int[] { 1, 2, 3 });
            Assert.AreEqual(3M, ExpressionEvaluator.Instance.Evaluate("max(a)", p2));

            Assert.AreEqual(3M, ExpressionEvaluator.Instance.Evaluate("max(1, 2, 3)"));

            // Min
            var p4 = new Dictionary<string, object>();
            p4.Add("a", new decimal[] { 1M, 2M, 3M });
            Assert.AreEqual(1M, ExpressionEvaluator.Instance.Evaluate("min(a)", p4));

            var p5 = new Dictionary<string, object>();
            p5.Add("a", new int[] { 1, 2, 3 });
            Assert.AreEqual(1M, ExpressionEvaluator.Instance.Evaluate("min(a)", p5));

            Assert.AreEqual(1M, ExpressionEvaluator.Instance.Evaluate("min(1, 2, 3)"));

            // Sum
            var p7 = new Dictionary<string, object>();
            p7.Add("a", new decimal[] { 1M, 2M, 3M });
            Assert.AreEqual(6M, ExpressionEvaluator.Instance.Evaluate("sum(a)", p7));

            var p8 = new Dictionary<string, object>();
            p8.Add("a", new int[] { 1, 2, 3 });
            Assert.AreEqual(6M, ExpressionEvaluator.Instance.Evaluate("sum(a)", p8));

            Assert.AreEqual(6M, ExpressionEvaluator.Instance.Evaluate("sum(1, 2, 3)"));

            // Avg
            var p10 = new Dictionary<string, object>();
            p10.Add("a", new decimal[] { 1M, 2M, 3M });
            Assert.AreEqual(2M, ExpressionEvaluator.Instance.Evaluate("avg(a)", p10));

            var p11 = new Dictionary<string, object>();
            p11.Add("a", new int[] { 1, 2, 3 });
            Assert.AreEqual(2M, ExpressionEvaluator.Instance.Evaluate("avg(a)", p11));

            Assert.AreEqual(2M, ExpressionEvaluator.Instance.Evaluate("avg(1, 2, 3)"));
        }

        [TestMethod]
        public void TestRealGitRepository()
        {
            using (var db = new OOMetricsContext())
            {
                //var repositoryUri = "https://luanmm@bitbucket.org/luanmm/oo-metrics.git";
                var repositoryUri = "https://luanmm@bitbucket.org/idealizers/raf.git";
                var project = db.Projects.FirstOrDefault(x => x.URI.Equals(repositoryUri, StringComparison.InvariantCultureIgnoreCase));
                if (project == null)
                {
                    project = db.Projects.Add(new Project
                    {
                        Name = "RaF", // OO-Metrics
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

        /*
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
        }*/
    }
}
