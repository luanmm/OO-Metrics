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
            Attributes = new HashSet<Attribute>();
            Methods = new HashSet<Method>();
        }

        public int Id { get; set; }

        public int NamespaceId { get; set; }

        //public int BaseClassId { get; set; }

        public ElementAbstractness Abstractness { get; set; }

        public ElementVisibility Visibility { get; set; }

        [Required]
        [StringLength(250)]
        public string Identifier { get; set; }

        public virtual ICollection<Attribute> Attributes { get; set; }

        public virtual ICollection<Method> Methods { get; set; }

        public virtual Namespace Namespace { get; set; }

        //public virtual Class BaseClass { get; set; }
    }
}
