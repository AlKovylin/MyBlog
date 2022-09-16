namespace MiBlog.Api.Contracts.Models.Users
{
    public class UserResponse
    {
        public int id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? AboutMy { get; set; }
    }
}
