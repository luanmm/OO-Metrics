using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace OOM.Model
{
    [Table("Namespace")]
    public partial class Namespace : IEntity<int>, IElement
    {
        public Namespace()
        {
            Classes = new HashSet<Class>();
        }

        [Key]
        public int Id { get; set; }

        public int RevisionId { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; }

        [Display(Name = "Identifier"), Required, StringLength(1000)]
        public string FullyQualifiedIdentifier { get; set; }

        public virtual ICollection<Class> Classes { get; set; }

        public virtual Revision Revision { get; set; }

        #region Not mapped properties

        [NotMapped]
        public ElementType Type { get { return ElementType.Namespace; } }

        [NotMapped]
        public IDictionary<string, object> Parameters
        {
            get
            {
                var parameters = new Dictionary<string, object>();

                parameters.Add("qc", Classes.Count);

                var cps = ElementParameter.ListParameters("c", Classes);
                foreach (var cp in cps)
                    parameters.Add(cp.Key, cp.Value);

                return parameters;
            }
        }

        #endregion
    }
}
