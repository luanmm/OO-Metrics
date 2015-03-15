using System;
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
                // TODO: Implement parameters for this element type
                return parameters;
            }
        }

        #endregion
    }
}
