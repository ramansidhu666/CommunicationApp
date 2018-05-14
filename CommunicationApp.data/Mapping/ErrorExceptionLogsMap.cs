using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.data.Mapping
{
    class ErrorExceptionLogsMap : EntityTypeConfiguration<ErrorExceptionLogs>
    {
        public ErrorExceptionLogsMap()
        {
            ToTable("ErrorExceptionLogs");
            HasKey(c => c.EventId).Property(c => c.EventId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.LogDateTime);
            Property(c => c.Source).HasMaxLength(100);
            Property(c => c.Message).HasMaxLength(1000);
            Property(c => c.QueryString).HasMaxLength(2000);
            Property(c => c.TargetSite).HasMaxLength(300);
            Property(c => c.StackTrace).HasMaxLength(4000);
            Property(c => c.ServerName).HasMaxLength(250);
            Property(c => c.RequestURL).HasMaxLength(300);
            Property(c => c.UserAgent).HasMaxLength(300);
            Property(c => c.UserIP).HasMaxLength(300);
            Property(c => c.UserAuthentication).HasMaxLength(300);
            Property(c => c.UserName).HasMaxLength(300);

        }
    }
}
