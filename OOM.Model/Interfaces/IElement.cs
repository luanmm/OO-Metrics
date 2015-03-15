using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OOM.Model
{
    public interface IElement
    {
        int Id { get; set; }

        string Name { get; set; }

        [Display(Name = "Identifier")]
        string FullyQualifiedIdentifier { get; set; }

        ElementType Type { get; }

        IDictionary<string, object> Parameters { get; }
    }
}
