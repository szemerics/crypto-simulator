using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Entities
{
    public class Role : AbstractEntity
    {
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
