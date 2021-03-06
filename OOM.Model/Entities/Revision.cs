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
            Namespaces = new HashSet<Namespace>();
        }

        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [Required, MaxLength]
        public string RID { get; set; }

        [Required]
        public int Number { get; set; }

        [MaxLength]
        public string Message { get; set; }

        [MaxLength]
        public string Author { get; set; }

        [Display(Name = "Created at"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Namespace> Namespaces { get; set; }

        public virtual Project Project { get; set; }
    }
}
