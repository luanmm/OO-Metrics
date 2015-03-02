using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Class")]
    public partial class Class : IEntity<int>
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

        [Display(Name = "Fully qualified identifier"), Required, MaxLength(250)]
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
    }
}
