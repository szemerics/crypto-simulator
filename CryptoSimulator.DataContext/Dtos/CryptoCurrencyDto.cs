using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Dtos
{
    public class CryptoCurrencyDto
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
