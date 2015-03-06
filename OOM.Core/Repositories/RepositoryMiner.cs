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
        private Repository _repository;
        private Project _project;

        public RepositoryMiner(Project project)
        {
            _project = _db.Projects.FirstOrDefault(x => x.URI == project.URI);
            if (_project == null)
                throw new Exception("The specified project could not be found in the database or is invalid.");

            _repository = RepositoryFactory.CreateRepository(_project.RepositoryProtocol, new RepositoryConfiguration(_project.URI, _project.User, _project.Password));
        }

        public void StartMining()
        {
            string lastRevisionRID = null;

            var lastRevision = _project.Revisions.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            if (lastRevision != null)
                lastRevisionRID = lastRevision.RID;

            var revisions = _repository.ListRevisions(lastRevisionRID);
            foreach (var revision in revisions)
            {
                var analyzedNamespaces = new List<Namespace>();
                var nodes = new List<RepositoryNode>(_repository.ListRevisionNodes(revision.RID));
                foreach (var node in nodes)
                {
                    var analyzer = CodeAnalyzerFactory.CreateCodeAnalyzer(node.Filename);
                    if (analyzer != null)
                    {
                        var contentStream = _repository.GetNodeContent(node);
                        using (var tr = new StreamReader(contentStream, Encoding.UTF8))
                        {
                            analyzedNamespaces.AddRange(analyzer.Analyze(tr.ReadToEnd()));
                        }
                    }
                }

                PersistAnalyzedData(new Revision
                {
                    RID = revision.RID,
                    Message = revision.Message,
                    Author = revision.Author,
                    CreatedAt = revision.CreatedAt,
                    ProjectId = _project.Id
                }, analyzedNamespaces);
            }
        }

        #region Privates

        private void PersistAnalyzedData(Revision revision, IEnumerable<Namespace> namespaces)
        {
            using (var dbContextTransaction = _db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    _db.Revisions.Add(revision);
                    _db.SaveChanges();

                    var unsavedClassRelations = new List<Class>();
                    foreach (var analyzedNamespace in namespaces)
                    {
                        var ns = revision.Namespaces.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedNamespace.FullyQualifiedIdentifier);
                        if (ns == null)
                        {
                            ns = _db.Namespaces.Add(new Namespace
                            {
                                Name = analyzedNamespace.Name,
                                FullyQualifiedIdentifier = analyzedNamespace.FullyQualifiedIdentifier,
                                RevisionId = revision.Id
                            });
                            _db.SaveChanges();
                        }

                        foreach (var analyzedClass in analyzedNamespace.Classes)
                        {
                            var c = ns.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedClass.FullyQualifiedIdentifier);
                            if (c == null)
                            {
                                if (analyzedClass.BaseClass != null)
                                    unsavedClassRelations.Add(analyzedClass);

                                c = _db.Classes.Add(new Class
                                {
                                    Name = analyzedClass.Name,
                                    FullyQualifiedIdentifier = analyzedClass.FullyQualifiedIdentifier,
                                    Encapsulation = analyzedClass.Encapsulation,
                                    Qualification = analyzedClass.Qualification,
                                    NamespaceId = ns.Id
                                });
                                _db.SaveChanges();
                            }

                            var savedFields = new List<Field>();
                            foreach (var analyzedField in analyzedClass.Fields)
                            {
                                var a = c.Fields.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedField.FullyQualifiedIdentifier);
                                if (a == null)
                                {
                                    a = _db.Fields.Add(new Field
                                    {
                                        Name = analyzedField.Name,
                                        FullyQualifiedIdentifier = analyzedField.FullyQualifiedIdentifier,
                                        Encapsulation = analyzedField.Encapsulation,
                                        Qualification = analyzedField.Qualification,
                                        ClassId = c.Id
                                    });
                                    _db.SaveChanges();
                                }
                                savedFields.Add(a);
                            }

                            foreach (var analyzedMethod in analyzedClass.Methods)
                            {
                                var m = c.Methods.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedMethod.FullyQualifiedIdentifier);
                                if (m == null)
                                {
                                    m = _db.Methods.Add(new Method
                                    {
                                        Name = analyzedMethod.Name,
                                        FullyQualifiedIdentifier = analyzedMethod.FullyQualifiedIdentifier,
                                        Encapsulation = analyzedMethod.Encapsulation,
                                        Qualification = analyzedMethod.Qualification,
                                        LineCount = analyzedMethod.LineCount,
                                        ExitPoints = analyzedMethod.ExitPoints,
                                        ReferencedFields = savedFields.Where(x => analyzedMethod.ReferencedFields.Any(y => y.FullyQualifiedIdentifier == x.FullyQualifiedIdentifier)).ToList(),
                                        ClassId = c.Id
                                    });
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }

                    foreach (var unsavedClassRelation in unsavedClassRelations) 
                    {
                        var c = _db.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == unsavedClassRelation.FullyQualifiedIdentifier);
                        if (c == null)
                        {
                            var bc = _db.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == unsavedClassRelation.BaseClass.FullyQualifiedIdentifier);
                            if (bc != null)
                            {
                                c.BaseClassId = bc.Id;
                                _db.SaveChanges();
                            }
                            else
                                throw new Exception("Testing it.");
                        }
                        else
                            throw new Exception("Testing it.");
                    }

                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                }
            }
        }

        #endregion

        public void Dispose()
        {
            _repository.Dispose();
            _db.Dispose();
        }
    }
}
