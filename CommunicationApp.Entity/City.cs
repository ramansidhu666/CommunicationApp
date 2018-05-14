using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CommunicationApp.Entity
{
    public partial class City
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateID { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual State States { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
