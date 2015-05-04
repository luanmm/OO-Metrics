using OOM.Core.Analyzers;
using OOM.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OOM.Core.Repositories
{
    public static class RepositoryMiner
    {
        public static void ProcessRepository(int projectId)
        {
            Project project = null;
            string lastRevisionRID = null;
            int lastRevisionNumber = 0;

            using (var db = new OOMetricsContext())
            {
                project = db.Projects.Find(projectId);
                if (project == null)
                    return;

                var lastRevision = db.Revisions.Where(x => x.ProjectId == projectId).OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                if (lastRevision != null)
                {
                    lastRevisionNumber = lastRevision.Number;
                    lastRevisionRID = lastRevision.RID;
                }
            }

            using (var repository = RepositoryFactory.CreateRepository(project.RepositoryProtocol, new RepositoryConfiguration(project.URI, project.User, project.Password)))
            {
                var revisions = repository.ListRevisions(lastRevisionRID);
                foreach (var revision in revisions)
                {
                    var analyzedNamespaces = new List<Namespace>();
                    var nodes = new List<RepositoryNode>(repository.ListRevisionNodes(revision.RID));
                    foreach (var node in nodes)
                    {
                        var analyzer = CodeAnalyzerFactory.CreateCodeAnalyzer(node.Filename);
                        if (analyzer != null)
                        {
                            var contentStream = repository.GetNodeContent(node);
                            using (var tr = new StreamReader(contentStream, Encoding.UTF8))
                            {
                                analyzedNamespaces.AddRange(analyzer.Analyze(tr.ReadToEnd()));
                            }
                        }
                    }

                    PersistAnalyzedData(new Revision
                    {
                        Number = ++lastRevisionNumber,
                        RID = revision.RID,
                        Message = revision.Message,
                        Author = revision.Author,
                        CreatedAt = revision.CreatedAt,
                        ProjectId = project.Id
                    }, analyzedNamespaces);
                }
            }
        }

        private static void PersistAnalyzedData(Revision revision, IEnumerable<Namespace> namespaces)
        {
            using (var db = new OOMetricsContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        db.Revisions.Add(revision);
                        db.SaveChanges();

                        var unsavedClassRelations = new List<Class>();
                        foreach (var analyzedNamespace in namespaces)
                        {
                            var ns = revision.Namespaces.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedNamespace.FullyQualifiedIdentifier);
                            if (ns == null)
                            {
                                ns = db.Namespaces.Add(new Namespace
                                {
                                    Name = analyzedNamespace.Name,
                                    FullyQualifiedIdentifier = analyzedNamespace.FullyQualifiedIdentifier,
                                    RevisionId = revision.Id
                                });
                                db.SaveChanges();
                            }

                            foreach (var analyzedClass in analyzedNamespace.Classes)
                            {
                                var c = ns.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedClass.FullyQualifiedIdentifier);
                                if (c == null)
                                {
                                    if (analyzedClass.BaseClass != null)
                                        unsavedClassRelations.Add(analyzedClass);

                                    c = db.Classes.Add(new Class
                                    {
                                        Name = analyzedClass.Name,
                                        FullyQualifiedIdentifier = analyzedClass.FullyQualifiedIdentifier,
                                        Encapsulation = analyzedClass.Encapsulation,
                                        Qualification = analyzedClass.Qualification,
                                        NamespaceId = ns.Id
                                    });
                                    db.SaveChanges();
                                }

                                var savedFields = new List<Field>();
                                foreach (var analyzedField in analyzedClass.Fields)
                                {
                                    var f = c.Fields.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedField.FullyQualifiedIdentifier);
                                    if (f == null)
                                    {
                                        f = db.Fields.Add(new Field
                                        {
                                            Name = analyzedField.Name,
                                            FullyQualifiedIdentifier = analyzedField.FullyQualifiedIdentifier,
                                            Encapsulation = analyzedField.Encapsulation,
                                            Qualification = analyzedField.Qualification,
                                            ClassId = c.Id
                                        });
                                        db.SaveChanges();
                                    }
                                    savedFields.Add(f);
                                }

                                foreach (var analyzedMethod in analyzedClass.Methods)
                                {
                                    var m = c.Methods.FirstOrDefault(x => x.FullyQualifiedIdentifier == analyzedMethod.FullyQualifiedIdentifier);
                                    if (m == null)
                                    {
                                        m = db.Methods.Add(new Method
                                        {
                                            Name = analyzedMethod.Name,
                                            FullyQualifiedIdentifier = analyzedMethod.FullyQualifiedIdentifier,
                                            Encapsulation = analyzedMethod.Encapsulation,
                                            Qualification = analyzedMethod.Qualification,
                                            LineCount = analyzedMethod.LineCount,
                                            ExitPoints = analyzedMethod.ExitPoints,
                                            Complexity = analyzedMethod.Complexity,
                                            ReferencedFields = savedFields.Where(x => analyzedMethod.ReferencedFields.Any(y => y.FullyQualifiedIdentifier == x.FullyQualifiedIdentifier)).ToList(),
                                            ClassId = c.Id
                                        });
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }

                        foreach (var unsavedClassRelation in unsavedClassRelations)
                        {
                            var c = db.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == unsavedClassRelation.FullyQualifiedIdentifier);
                            if (c != null)
                            {
                                var bc = db.Classes.FirstOrDefault(x => x.FullyQualifiedIdentifier == unsavedClassRelation.BaseClass.FullyQualifiedIdentifier);
                                if (bc != null)
                                {
                                    c.BaseClassId = bc.Id;
                                    db.SaveChanges();
                                }
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (ex is DbEntityValidationException)
                        {
                            var dbEx = (DbEntityValidationException)ex;
                            Console.WriteLine(String.Format("Entity validation error: {0}", dbEx.Message));
                        }

                        dbContextTransaction.Rollback();
                    }
                }
            }
        }
    }
}
