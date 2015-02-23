using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Attribute")]
    public partial class Attribute : IEntity<int>
    {
        public Attribute()
        {
            ReferencingMethods = new HashSet<Method>();
        }

        public int Id { get; set; }

        public int ClassId { get; set; }

        public ElementVisibility Visibility { get; set; }

        public ElementScope Scope { get; set; }

        [Required]
        [StringLength(250)]
        public string Identifier { get; set; }

        public virtual ICollection<Method> ReferencingMethods { get; set; }

        public virtual Class Class { get; set; }
    }
}
