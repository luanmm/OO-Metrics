using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Project")]
    public partial class Project : IEntity<int>
    {
        public Project()
        {
            Revisions = new HashSet<Revision>();
        }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        [Display(Name = "Repository protocol")]
        public RepositoryProtocol RepositoryProtocol { get; set; }

        [Required, MaxLength(500)]
        public string URI { get; set; }

        [MaxLength(250)]
        public string User { get; set; }

        [MaxLength(250)]
        public string Password { get; set; }

        public virtual ICollection<Revision> Revisions { get; set; }
    }
}
