﻿using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Data.Mapping
{
    public class ClausesTypeMap : EntityTypeConfiguration<ClauseType>
    {
        public ClausesTypeMap()
        {
            //table
            ToTable("ClausesType");

            HasKey(t => t.Id).Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //properties
            Property(t => t.ClausesName);
        }
    }
}
