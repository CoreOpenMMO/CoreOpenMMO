using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMMO.Domain.Entities
{
    public class Account
    {
        [Key]
        public virtual int AccountId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual int AccountType { get; set; }
        public virtual int PremiumDays { get; set; }
        public virtual int LastDay { get; set; }
        

        // Should be changed to ICollection when Characters entity be implemented
        [NotMapped]
        public virtual List<string> Characters { get; set; }
    }
}
