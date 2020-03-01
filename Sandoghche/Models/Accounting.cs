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
