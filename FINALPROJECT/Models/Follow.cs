namespace FINALPROJECT.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public string FollowerId { get; set; } = string.Empty;
        public User? Follower { get; set; }
        public string FollowingId { get; set; } = string.Empty;
        public User? Following { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
