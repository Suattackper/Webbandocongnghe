using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace electronics_shop.Models
{
    public class LoginViewModels
    {
        [Key]
        public int AccountCode { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "This field is required")]
        [EmailAddress(ErrorMessage = "Please enter the correct email format")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}