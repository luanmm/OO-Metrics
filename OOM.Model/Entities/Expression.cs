using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Expression")]
    public partial class Expression : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; }

        [Required, StringLength(500)]
        public string Formula { get; set; }

        [Display(Name = "Target element type")]
        public ElementType TargetType { get; set; }
    }
}
