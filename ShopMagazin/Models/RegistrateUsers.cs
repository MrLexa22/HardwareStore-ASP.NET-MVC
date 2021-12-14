using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ShopMagazin.Models
{
    public class RegistrateUsers
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [Display(Name = "Email пользователя")]
        [Remote(action: "CheckEmail", controller: "Home", ErrorMessage = "Email уже используется")]
        [EmailAddress(ErrorMessage = "Здесь нужно ввести корректный Email")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Email { get; set; }
        
        
        [Display(Name = "Пароль пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Password { get; set; }

        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля должны совпадать!")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Имя пользователя")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректное имя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Ima { get; set; }


        [Display(Name = "Фамилия пользователя")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Некорректная фамилия")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректная фамилия")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Familia { get; set; }


        [Display(Name = "Роль пользователя")]
        public string Role { get; set; }
    }
}
