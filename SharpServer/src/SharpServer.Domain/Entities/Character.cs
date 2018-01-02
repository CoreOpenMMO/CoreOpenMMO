namespace SharpServer.Domain.Entities
{
    public class Character
    {
        public int CharacterId { get; set; }
        public int UserId { get; set; }
        public int WorldId { get; set; }
        public int Name { get; set; }
    }
}
