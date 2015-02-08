using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Node")]
    public partial class Node : IEntity<int>
    {
        public Node()
        {
            Classes = new HashSet<Class>();
        }

        public int Id { get; set; }

        public int RevisionId { get; set; }

        public NodeType NodeType { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Path { get; set; }

        public virtual ICollection<Class> Classes { get; set; }

        public virtual Revision Revision { get; set; }
    }
}
