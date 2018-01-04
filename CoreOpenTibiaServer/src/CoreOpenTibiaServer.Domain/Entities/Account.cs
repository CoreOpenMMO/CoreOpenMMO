using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COTS.Domain.Entities
{
    public class Account
    {
        [Key]
        public int AccountGuid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int LastDay { get; set; }
        public int PremiumDays { get; set; }
        public int AccountType { get; set; }

        [NotMapped]
        public List<string> Characters { get; set; }
    }
}
