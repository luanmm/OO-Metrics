using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("ExpressionResult")]
    public partial class ExpressionResult : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public int ExpressionId { get; set; }

        public int ElementId { get; set; }

        public ElementType ElementType { get; set; }

        public decimal Result { get; set; }

        public virtual Expression Expression { get; set; }
    }
}