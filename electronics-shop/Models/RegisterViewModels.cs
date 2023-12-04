using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace electronics_shop.Models
{
    public class RegisterViewModels
    {
        [Key]
        public int AccountCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(20)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]))(?=.*[#$^+=!*()@%&]).{8,}$",
                           ErrorMessage = "At least 8 characters: include 1 lowercase letter, 1 uppercase letter, 1 number and 1 special character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter the correct email format")]
        [Remote("EmailExists", "Account", ErrorMessage = "Email had already in use")]
        public string Email { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [StringLength(10)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^(0)([0-9]{9})$",
                            ErrorMessage = "Phone number must start with 0 and have 10 digits")]
        [Remote("PhoneNumberExists", "Account", ErrorMessage = "Phone number had already in use")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string FullName { get; set; }
    }
}