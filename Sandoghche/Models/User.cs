using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }
        [NotNull]
        public string FullName { get; set; }
        [NotNull]
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string PasswordHash { get; set; }
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



        //[OneToMany("RollId")]
        //public List<Roll> Rolls { get; set; }




    }
}
