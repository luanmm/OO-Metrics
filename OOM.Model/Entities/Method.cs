using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Method")]
    public partial class Method : IEntity<int>, IElement
    {
        public Method()
        {
            ReferencedFields = new HashSet<Field>();
        }

        [Key]
        public int Id { get; set; }

        public int ClassId { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; }

        [Display(Name = "Identifier"), Required, StringLength(1000)]
        public string FullyQualifiedIdentifier { get; set; }

        [Required]
        public EncapsulationTypes Encapsulation { get; set; }

        [Required]
        public QualificationTypes Qualification { get; set; }

        [Display(Name = "Line count"), Required, Range(0, Int32.MaxValue)]
        public int LineCount { get; set; }

        [Display(Name = "Exit points"), Required, Range(0, Int32.MaxValue)]
        public int ExitPoints { get; set; }

        [Required, Range(0, Int32.MaxValue)]
        public int Complexity { get; set; }

        public virtual ICollection<Field> ReferencedFields { get; set; }

        public virtual Class Class { get; set; }

        #region Not mapped properties

        [NotMapped]
        public bool IsPrivate { get { return Encapsulation.HasFlag(EncapsulationTypes.Private); } }

        [NotMapped]
        public bool IsProtected { get { return Encapsulation.HasFlag(EncapsulationTypes.Protected); } }

        [NotMapped]
        public bool IsPublic { get { return Encapsulation.HasFlag(EncapsulationTypes.Public); } }

        [NotMapped]
        public ElementType Type { get { return ElementType.Method; } }

        [NotMapped]
        public IDictionary<string, object> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, object>();

                parameters.Add("loc", LineCount);
                parameters.Add("ep", ExitPoints);
                parameters.Add("c", Complexity);

                return parameters;
            }
        }

        #endregion
    }
}
