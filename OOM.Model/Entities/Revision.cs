using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Revision")]
    public partial class Revision : IEntity<int>
    {
        public Revision()
        {
            Nodes = new HashSet<Node>();
            Namespaces = new HashSet<Namespace>();
        }

        public int Id { get; set; }

        public int ProjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string RID { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        [StringLength(250)]
        public string Author { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Node> Nodes { get; set; }

        public virtual ICollection<Namespace> Namespaces { get; set; }

        public virtual Project Project { get; set; }
    }
}
