using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Method")]
    public partial class Method : IEntity<int>
    {
        public Method()
        {
            ReferencedFields = new HashSet<Field>();
            InvokedMethods = new HashSet<Method>();
        }

        [Key]
        public int Id { get; set; }

        public int ClassId { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        [Display(Name = "Fully qualified identifier"), Required, MaxLength(250)]
        public string FullyQualifiedIdentifier { get; set; }

        [Required]
        public EncapsulationTypes Encapsulation { get; set; }

        [Required]
        public QualificationTypes Qualification { get; set; }

        [Display(Name = "Line count"), Required, Range(0, Int32.MaxValue)]
        public int LineCount { get; set; }

        [Display(Name = "Exit points"), Required, Range(0, Int32.MaxValue)]
        public int ExitPoints { get; set; }

        public virtual ICollection<Field> ReferencedFields { get; set; }

        public virtual ICollection<Method> InvokedMethods { get; set; }

        public virtual Class Class { get; set; }
    }
}
