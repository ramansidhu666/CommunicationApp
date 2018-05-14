using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace CommunicationApp.Core
{
    public class BaseEntity<T>
    {
        public T ID { get; set; }
    }
}
