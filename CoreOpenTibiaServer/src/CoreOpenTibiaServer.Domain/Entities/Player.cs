using System.ComponentModel.DataAnnotations;

namespace COTS.Domain.Entities
{
    public class Player
    {
        [Key]
        public int PlayerGuid { get; set; }
        public int UserId { get; set; }
        public int WorldId { get; set; }
        public string Name { get; set; }
    }
}
