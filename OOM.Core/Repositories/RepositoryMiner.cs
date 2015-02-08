using OOM.Core.Analyzers;
using OOM.Model;
using OOM.Repositories;
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
        private Project _project;
        private Repository _repository;

        private ProjectRepository _projectRepository;
        private RevisionRepository _revisionRepository;
        private NodeRepository _nodeRepository;
        private NamespaceRepository _namespaceRepository;
        private ClassRepository _classRepository;
        private AttributeRepository _attributeRepository;
        private MethodRepository _methodRepository;

        public RepositoryMiner(Project project)
        {
            _projectRepository = new ProjectRepository();
            _project = _projectRepository.GetByUri(project.URI);
            if (_project == null)
                throw new Exception("The specified project could not be found in the database or is invalid.");

            _repository = RepositoryFactory.CreateRepository(_project.RepositoryProtocol, new RepositoryConfiguration(_project.URI, _project.User, _project.Password));

            _revisionRepository = new RevisionRepository();
            _nodeRepository = new NodeRepository();
            _namespaceRepository = new NamespaceRepository();
            _classRepository = new ClassRepository();
            _attributeRepository = new AttributeRepository();
            _methodRepository = new MethodRepository();
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
                var revisionId = _revisionRepository.Create(new Revision
                {
                    RID = revision.RID,
                    Message = revision.Message,
                    Author = revision.Author,
                    CreatedAt = revision.CreatedAt,
                    ProjectId = _project.Id
                });

                var nodes = _repository.ListRevisionNodes(revision.RID);
                foreach (var node in nodes)
                {
                    SaveNodeMetrics(revisionId, node);

                    _nodeRepository.Create(new Node
                    {
                        Name = node.Name,
                        NodeType = node.Type,
                        Path = node.Path,
                        RevisionId = revisionId
                    });
                }
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
                        int namespaceId;
                        var ns = _namespaceRepository.GetByIdentifier(revisionId, analizedNamespace.Identifier);
                        if (ns != null)
                            namespaceId = ns.Id;
                        else
                            namespaceId = _namespaceRepository.Create(new Namespace
                            {
                                Identifier = analizedNamespace.Identifier,
                                RevisionId = revisionId
                            });

                        foreach (var analyzedClass in analizedNamespace.Classes)
                            SaveClassInformation(namespaceId, analyzedClass);
                    }
                }
            }
        }

        private void SaveClassInformation(int namespaceId, AnalyzedClass analyzedClass)
        {
            int classId;
            var c = _classRepository.GetByIdentifier(namespaceId, analyzedClass.Identifier);
            if (c != null)
                classId = c.Id;
            else
                classId = _classRepository.Create(new Class
                {
                    Identifier = analyzedClass.Identifier,
                    Abstractness = analyzedClass.Abstractness,
                    Visibility = analyzedClass.Visibility,
                    // TODO: Base class
                    NamespaceId = namespaceId
                });

            foreach (var analyzedAttribute in analyzedClass.Attributes)
                SaveAttributeInformation(classId, analyzedAttribute);

            foreach (var analyzedMethod in analyzedClass.Methods)
                SaveMethodInformation(classId, analyzedMethod);
        }

        private void SaveAttributeInformation(int classId, AnalyzedAttribute analyzedAttribute)
        {
            int attributeId;
            var a = _attributeRepository.GetByIdentifier(classId, analyzedAttribute.Identifier);
            if (a != null)
                attributeId = a.Id;
            else
                attributeId = _attributeRepository.Create(new OOM.Model.Attribute
                {
                    Identifier = analyzedAttribute.Identifier,
                    Visibility = analyzedAttribute.Visibility,
                    Scope = analyzedAttribute.Scope,
                    ClassId = classId
                });
        }

        private void SaveMethodInformation(int classId, AnalyzedMethod analyzedMethod)
        {
            int methodId;
            var m = _methodRepository.GetByIdentifier(classId, analyzedMethod.Identifier);
            if (m != null)
                methodId = m.Id;
            else
                methodId = _methodRepository.Create(new Method
                {
                    Identifier = analyzedMethod.Identifier,
                    Abstractness = analyzedMethod.Abstractness,
                    Visibility = analyzedMethod.Visibility,
                    Scope = analyzedMethod.Scope,
                    DefinitionType = analyzedMethod.DefinitionType,
                    LineCount = analyzedMethod.LineCount,
                    ClassId = classId
                });
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
