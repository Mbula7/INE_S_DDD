//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace S_DDD.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_user()
        {
            this.tb_utilidade = new HashSet<tb_utilidade>();
        }
    
        public int id_user { get; set; }
        public string username { get; set; }
        public string password_user { get; set; }
        public string categoria { get; set; }
        public Nullable<int> estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_utilidade> tb_utilidade { get; set; }
    }
}
