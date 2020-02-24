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

        //[OneToMany("RollId")]
        //public List<Roll> Rolls { get; set; }




    }
}
