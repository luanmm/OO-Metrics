using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Namespace")]
    public partial class Namespace : IEntity<int>
    {
        public Namespace()
        {
            Classes = new HashSet<Class>();
        }

        public int Id { get; set; }

        public int RevisionId { get; set; }

        [Required]
        [StringLength(250)]
        public string Identifier { get; set; }

        public virtual ICollection<Class> Classes { get; set; }

        public virtual Revision Revision { get; set; }
    }
}
