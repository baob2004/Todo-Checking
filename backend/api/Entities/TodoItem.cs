namespace api.Entities
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public bool IsDone { get; set; } = false;
        public DateTime? Due { get; set; }
        public string UserId { get; set; } = default!;
        public AppUser? User { get; set; }
    }
}