using OOM.Core.Analyzers;
using OOM.Model;
using System;
using System.Collections.Generic;
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
            string lastRevisionRID = null;

            var lastRevision = _project.Revisions.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            if (lastRevision != null)
                lastRevisionRID = lastRevision.RID;

            var revisions = _repository.ListRevisions(lastRevisionRID);
            foreach (var revision in revisions)
            {
                var r = _db.Revisions.Add(new Revision
                {
                    RID = revision.RID,
                    Message = revision.Message,
                    Author = revision.Author,
                    CreatedAt = revision.CreatedAt,
                    ProjectId = _project.Id
                });
                _db.SaveChanges();

                var nodes = new List<RepositoryNode>(_repository.ListRevisionTree(revision.RID));
                RecursiveMineNodes(r.Id, nodes);
            }
        }

        private void RecursiveMineNodes(int revisionId, List<RepositoryNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Type == NodeType.Directory)
                {
                    var nodeTree = new List<RepositoryNode>(_repository.ListNodeTree(node));
                    RecursiveMineNodes(revisionId, nodeTree);
                }
                else if (node.Type == NodeType.File)
                    SaveNodeMetrics(revisionId, node);
            }
        }

        private void SaveNodeMetrics(int revisionId, RepositoryNode node)
        {
            var analyzer = CodeAnalyzerFactory.CreateCodeAnalyzer(node.Name);
            if (analyzer != null)
            {
                var contentStream = _repository.GetNodeContent(node);
                using (var tr = new StreamReader(contentStream, Encoding.UTF8))
                {
                    var analyzedCode = analyzer.Analyze(tr.ReadToEnd());
                    foreach (var analizedNamespace in analyzedCode.Namespaces)
                    {
                        var ns = _db.Namespaces.FirstOrDefault(x => x.Revision.Id == revisionId && x.Identifier == analizedNamespace.Identifier);
                        if (ns == null)
                        { 
                            ns = _db.Namespaces.Add(new Namespace
                            {
                                Identifier = analizedNamespace.Identifier,
                                RevisionId = revisionId
                            });
                            _db.SaveChanges();
                        }

                        foreach (var analyzedClass in analizedNamespace.Classes)
                            SaveClassInformation(ns.Id, analyzedClass);
                    }
                }
            }
        }

        private void SaveClassInformation(int namespaceId, AnalyzedClass analyzedClass)
        {
            var c = _db.Classes.FirstOrDefault(x => x.Namespace.Id == namespaceId && x.Identifier == analyzedClass.Identifier);
            if (c == null)
            { 
                c = _db.Classes.Add(new Class
                {
                    Identifier = analyzedClass.Identifier,
                    Abstractness = analyzedClass.Abstractness,
                    Visibility = analyzedClass.Visibility,
                    // TODO: Base class
                    NamespaceId = namespaceId
                });
                _db.SaveChanges();
            }

            foreach (var analyzedAttribute in analyzedClass.Attributes)
                SaveAttributeInformation(c.Id, analyzedAttribute);

            foreach (var analyzedMethod in analyzedClass.Methods)
                SaveMethodInformation(c.Id, analyzedMethod);
        }

        private void SaveAttributeInformation(int classId, AnalyzedAttribute analyzedAttribute)
        {
            var a = _db.Attributes.FirstOrDefault(x => x.Class.Id == classId && x.Identifier == analyzedAttribute.Identifier);
            if (a == null)
            {
                _db.Attributes.Add(new OOM.Model.Attribute
                {
                    Identifier = analyzedAttribute.Identifier,
                    Visibility = analyzedAttribute.Visibility,
                    Scope = analyzedAttribute.Scope,
                    ClassId = classId
                });
                _db.SaveChanges();
            }
        }

        private void SaveMethodInformation(int classId, AnalyzedMethod analyzedMethod)
        {
            var m = _db.Methods.FirstOrDefault(x => x.Class.Id == classId && x.Identifier == analyzedMethod.Identifier);
            if (m == null)
            {
                _db.Methods.Add(new Method
                {
                    Identifier = analyzedMethod.Identifier,
                    Abstractness = analyzedMethod.Abstractness,
                    Visibility = analyzedMethod.Visibility,
                    Scope = analyzedMethod.Scope,
                    DefinitionType = analyzedMethod.DefinitionType,
                    LineCount = analyzedMethod.LineCount,
                    ClassId = classId
                });
                _db.SaveChanges();
            }
        }

        public void Dispose()
        {
            _repository.Dispose();
            _db.Dispose();
        }
    }
}
