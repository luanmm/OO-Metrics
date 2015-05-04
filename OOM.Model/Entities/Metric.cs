using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Metric")]
    public partial class Metric : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; }

        [Required, StringLength(500)]
        public string Expression { get; set; }

        [Display(Name = "Target element type")]
        public ElementType TargetType { get; set; }
    }
}
