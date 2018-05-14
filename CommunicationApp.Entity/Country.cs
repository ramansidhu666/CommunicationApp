using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CommunicationApp.Entity
{
    public partial class Country
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string ISOAlpha2 { get; set; }
        public string ISOAlpha3 { get; set; }
        public int? ISONumeric { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrrencySymbol { get; set; }
        public string CountryFlag { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<State> States { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
