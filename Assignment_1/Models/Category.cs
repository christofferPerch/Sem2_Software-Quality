
namespace Assignment_1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public List<ToDo>? ToDos { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Category category &&
                   Id == category.Id &&
                   CategoryName == category.CategoryName &&
                   EqualityComparer<List<ToDo>?>.Default.Equals(ToDos, category.ToDos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, CategoryName, ToDos);
        }
    }
}
