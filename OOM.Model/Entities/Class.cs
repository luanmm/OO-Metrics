using System;
using System.Linq;
using System.Collections.Generic;
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

        [Required, MaxLength(250)]
        public string Name { get; set; }

        [Display(Name = "Identifier"), Required, MaxLength(250)]
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
                parameters.Add("noc", ChildClasses.Count);
                parameters.Add("dit", GetDepthOfInheritanceTree(this));
                parameters.Add("aif", GetAttributeInheritanceFactor(this));
                parameters.Add("mif", GetMethodInheritanceFactor(this));
                parameters.Add("m.loc", Methods.Select(x => x.LineCount));
                parameters.Add("m.ep", Methods.Select(x => x.ExitPoints));
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

        #endregion
    }
}
