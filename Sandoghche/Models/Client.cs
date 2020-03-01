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

        public string DateCreated
        {
            get
            {
                return this.dateCreated != null
                   ? Convert.ToDateTime(dateCreated).ToString("yyyy-MM-dd HH:mm:ss")
                   : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            set
            {
                if (this.dateCreated != null)
                    this.dateCreated = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else
                    this.dateCreated = value;
            }
        }

        private string dateCreated = null;



    }
}
