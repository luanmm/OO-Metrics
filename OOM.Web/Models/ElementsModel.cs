using OOM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OOM.Web.Models
{
    public class ElementDetailsModel
    {
        public Revision Revision { get; set; }
        public IElement Element { get; set; }
    }
}