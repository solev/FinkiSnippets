using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.ViewModels
{
    public class RegisterViewModel
    {

        public string ID { get; set; }

        [Required]
        [Display(Name="Корисничко име")]
        public string Username { get; set; }

        [Required]
        [Display(Name="Име")]
        public string Ime { get; set; }

        [Required]
        [Display(Name = "Презиме")]
        public string Prezime { get; set; }

        [Display(Name = "Email Адреса")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Лозинка")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторете Лозинка")]
        [Compare("Password")]        
        public string ConfirmPassword { get; set; }
    }
}