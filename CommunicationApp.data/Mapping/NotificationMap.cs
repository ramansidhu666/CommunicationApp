using CommunicationApp.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationApp.Data.Mapping
{
    class NotificationMap : EntityTypeConfiguration<Notification>
    {
      public NotificationMap()
        {
            //table
            ToTable("Notification");

            HasKey(t => t.NotificationId).Property(c => c.NotificationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            //properties
            Property(t => t.RequestMessage);
            Property(t => t.NotificationSendTo);
            Property(t => t.NotificationSendBy);
            Property(t => t.IsRead);
            Property(t => t.Flag);

        }

    }
}
