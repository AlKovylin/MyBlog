namespace MyBlog.Infrastructure.Business.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string AboutMy { get; set; }
        public string Photo { get; set; }

        public UserModel()
        {
            Photo = "https://thispersondoesnotexist.com/image";
            AboutMy = "Информация обо мне.";
        }

    }
}
