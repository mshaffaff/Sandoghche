using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Clients")]
    public class Client
    {
        [PrimaryKey, AutoIncrement]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }

        public string Email { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

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
