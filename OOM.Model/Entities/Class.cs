using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Class")]
    public partial class Class : IEntity<int>, IElement
    {
        public Class()
        {
            Fields = new HashSet<Field>();
            Methods = new HashSet<Method>();
            ChildClasses = new HashSet<Class>();
        }

        [Key]
        public int Id { get; set; }

        public int NamespaceId { get; set; }

        public int? BaseClassId { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; }

        [Display(Name = "Identifier"), Required, StringLength(1000)]
        public string FullyQualifiedIdentifier { get; set; }

        [Required]
        public EncapsulationTypes Encapsulation { get; set; }

        [Required]
        public QualificationTypes Qualification { get; set; }

        public virtual ICollection<Field> Fields { get; set; }

        public virtual ICollection<Method> Methods { get; set; }

        public virtual ICollection<Class> ChildClasses { get; set; }

        public virtual Namespace Namespace { get; set; }

        public virtual Class BaseClass { get; set; }

        #region Not mapped properties

        [NotMapped]
        public bool IsPrivate { get { return Encapsulation.HasFlag(EncapsulationTypes.Private); } }

        [NotMapped]
        public bool IsProtected { get { return Encapsulation.HasFlag(EncapsulationTypes.Protected); } }

        [NotMapped]
        public bool IsPublic { get { return Encapsulation.HasFlag(EncapsulationTypes.Public); } }

        [NotMapped]
        public ElementType Type { get { return ElementType.Class; } }

        [NotMapped]
        public IDictionary<string, object> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, object>();

                parameters.Add("qf", Fields.Count);
                parameters.Add("qm", Methods.Count);
                parameters.Add("wmc", Methods.Sum(x => x.Complexity));
                parameters.Add("dit", GetDepthOfInheritanceTree(this));
                parameters.Add("aif", GetAttributeInheritanceFactor(this));
                parameters.Add("mif", GetMethodInheritanceFactor(this));
                parameters.Add("noc", ChildClasses.Count);
                parameters.Add("rfc", Methods.Where(x => x.IsPublic).Count());
                parameters.Add("lcom", GetLackOfCohesionOfMethods(this));
                parameters.Add("mhf", GetMethodHidingFactor(this));
                parameters.Add("pf", GetPolymorphismFactor(this));

                var fps = ElementParameter.ListParameters("f", Fields);
                foreach (var fp in fps)
                    parameters.Add(fp.Key, fp.Value);

                var mps = ElementParameter.ListParameters("m", Methods);
                foreach (var mp in mps)
                    parameters.Add(mp.Key, mp.Value);

                return parameters;
            }
        }

        private int GetDepthOfInheritanceTree(Class baseClass, int currentDepth = 0)
        {
            if (baseClass.ChildClasses.Count == 0)
                return currentDepth;

            var maxDepth = 0;
            foreach (var childClass in baseClass.ChildClasses)
                maxDepth = Math.Max(GetDepthOfInheritanceTree(baseClass, currentDepth + 1), maxDepth);

            return maxDepth;
        }

        private int GetAttributeInheritanceFactor(Class baseClass)
        {
            // TODO: Get all available fields (including base classes) and all defined fields in the class
            // MIF = 1 - (md / ma)

            return 0;
        }

        private int GetMethodInheritanceFactor(Class baseClass)
        {
            // TODO: Get all available methods (including base classes) and all defined methods in the class
            // MIF = 1 - (md / ma)

            return 0;
        }

        private int GetMethodHidingFactor(Class baseClass)
        {
            // TODO: http://www.aivosto.com/project/help/pm-oo-mood.html

            return 0;
        }

        private int GetPolymorphismFactor(Class baseClass)
        {
            // TODO: ...

            return 0;
        }

        private int GetLackOfCohesionOfMethods(Class baseClass)
        {
            // Based on: http://www.reimers.dk/jacob-reimers-blog/calculating-lack-of-cohesion-of-methods-with-roslyn

            try
            {
                var memberCount = baseClass.Methods.Count;
                var fieldUsage = baseClass.Fields.Sum(x => x.ReferencingMethods.Count);
                var fieldCount = baseClass.Fields.Count;

                return (memberCount - fieldUsage / fieldCount) / (memberCount - 1);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
    }
}
