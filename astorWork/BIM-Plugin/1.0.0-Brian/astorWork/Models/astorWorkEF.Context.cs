﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace astorWork.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class astorWorkEntities : DbContext
    {
        public astorWorkEntities()
            : base("name=astorWorkEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BIMSyncCurr> BIMSyncCurrs { get; set; }
        public virtual DbSet<BIMSyncHist> BIMSyncHists { get; set; }
        public virtual DbSet<BIMUserInfo> BIMUserInfoes { get; set; }
        public virtual DbSet<LocationAssociation> LocationAssociations { get; set; }
        public virtual DbSet<LocationMaster> LocationMasters { get; set; }
        public virtual DbSet<MaterialDetail> MaterialDetails { get; set; }
        public virtual DbSet<MaterialMaster> MaterialMasters { get; set; }
        public virtual DbSet<UserMaster> UserMasters { get; set; }
    }
}
