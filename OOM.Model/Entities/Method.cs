using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Method")]
    public partial class Method : IEntity<int>
    {
        public int Id { get; set; }

        public int ClassId { get; set; }

        [Required]
        public ElementAbstractness Abstractness { get; set; }

        [Required]
        public ElementVisibility Visibility { get; set; }

        [Required]
        public ElementScope Scope { get; set; }

        [Required]
        public ElementDefinitionType DefinitionType { get; set; }

        [Required]
        [StringLength(250)]
        public string Identifier { get; set; }

        [Required]
        public int LineCount { get; set; }

        public virtual Class Class { get; set; }
    }
}
