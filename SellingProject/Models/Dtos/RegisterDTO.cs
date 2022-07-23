using System.ComponentModel.DataAnnotations;

namespace SellingProject.Models.Dtos
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="اسم المستخدم مطلوب")]
        public string UserName { get; set; }
          [Required(ErrorMessage ="كلمة المرور مطلوبة")]
          [StringLength(8,MinimumLength = 4,ErrorMessage ="كلمة المرور يجب ان تكون اكثر من اربغة خانات ولا تقل عن ثمانية خانات ")]

        public string Password { get; set; }
    }
}