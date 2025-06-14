﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CryptoSimulator.DataContext.Entities
{
    public class User : AbstractEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }


        // What does the user have:
        public Wallet Wallet { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

    }
}
