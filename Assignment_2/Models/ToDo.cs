namespace Assignment_2.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }

        public bool IsCompleted { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ToDo @do &&
                   Id == @do.Id &&
                   Title == @do.Title &&
                   Description == @do.Description &&
                   Deadline == @do.Deadline &&
                   IsCompleted == @do.IsCompleted &&
                   CategoryId == @do.CategoryId &&
                   EqualityComparer<Category?>.Default.Equals(Category, @do.Category);
        }

        public string GetTaskState()
        {
            return IsCompleted ? "Completed" : "Not Completed";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, Deadline, IsCompleted, CategoryId, Category);
        }
    }
}
