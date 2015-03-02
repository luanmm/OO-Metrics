using OOM.Core.Analyzers;
using OOM.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public class RepositoryMiner : IDisposable
    {
        private OOMetricsContext _db = new OOMetricsContext();
        private Project _project;
        private Repository _repository;

        public RepositoryMiner(Project project)
        {
            _project = _db.Projects.FirstOrDefault(x => x.URI == project.URI);
            if (_project == null)
                throw new Exception("The specified project could not be found in the database or is invalid.");

            _repository = RepositoryFactory.CreateRepository(_project.RepositoryProtocol, new RepositoryConfiguration(_project.URI, _project.User, _project.Password));
        }

        public void StartMining()
        {
            var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            return;

            string lastRevisionRID = null;

            var lastRevision = _project.Revisions.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            if (lastRevision != null)
                lastRevisionRID = lastRevision.RID;

            var revisions = _repository.ListRevisions(lastRevisionRID);
            foreach (var revision in revisions)
            {
                using (var dbContextTransaction = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var r = _db.Revisions.Add(new Revision
                        {
                            RID = revision.RID,
                            Message = revision.Message,
                            Author = revision.Author,
                            CreatedAt = revision.CreatedAt
                        });
                        _project.Revisions.Add(r);
                        _db.SaveChanges(); // TODO: Put SaveChanges after Add methods of all entities

                        var files = new List<string>(_repository.ListRevisionFiles(revision.RID));
                        foreach (var file in files)
                            SaveNodeMetrics(r, file);

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        private void SaveNodeMetrics(Revision r, string filePath)
        {
            var analyzer = CodeAnalyzerFactory.CreateCodeAnalyzer(filePath);
            if (analyzer != null)
            {
                var analyzedNamespaces = analyzer.Analyze(filePath);
                foreach (var analizedNamespace in analyzedNamespaces)
                {
                    var ns = r.Namespaces.FirstOrDefault(x => x.FullyQualifiedIdentifier == analizedNamespace.FullyQualifiedIdentifier);
                    if (ns == null)
                    {
                        ns = _db.Namespaces.Add(analizedNamespace);
                        r.Namespaces.Add(ns);
                    }

                    foreach (var analyzedClass in analizedNamespace.Classes)
                        SaveClassInformation(ns, analyzedClass);
                }
            }
        }

        private void SaveClassInformation(Namespace ns, Class analyzedClass)
        {
            var c = ns.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedClass.FullyQualifiedIdentifier);
            if (c == null)
            {
                c = _db.Classes.Add(analyzedClass);
                ns.Classes.Add(c);
            }

            foreach (var analyzedField in analyzedClass.Fields)
                SaveFieldInformation(c, analyzedField);

            foreach (var analyzedMethod in analyzedClass.Methods)
                SaveMethodInformation(c, analyzedMethod);
        }

        private void SaveFieldInformation(Class c, Field analyzedField)
        {
            var a = c.Fields.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedField.FullyQualifiedIdentifier);
            if (a == null)
                c.Fields.Add(analyzedField);
        }

        private void SaveMethodInformation(Class c, Method analyzedMethod)
        {
            var m = c.Methods.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedMethod.FullyQualifiedIdentifier);
            if (m == null)
                c.Methods.Add(analyzedMethod);
        }

        public void Dispose()
        {
            _repository.Dispose();
            _db.Dispose();
        }
    }
}
