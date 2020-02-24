using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    public class Accounting
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public double DebtorAmount { get; set; }
        public double CreditorAmount { get; set; }
        public DateTime DateCreated
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }

        private DateTime? dateCreated = null;
    }
}
