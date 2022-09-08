using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Поле обязательно для заполнения")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
