//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OOM.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Project
    {
        public Project()
        {
            this.Nodes = new HashSet<Node>();
            this.Revisions = new HashSet<Revision>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string URI { get; set; }
    
        public virtual ICollection<Node> Nodes { get; set; }
        public virtual ICollection<Revision> Revisions { get; set; }
    }
}