using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ShopMagazin.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [Display(Name = "Email пользователя")]
        [Remote(action: "CheckEmail", controller: "Home", ErrorMessage = "Email уже используется")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Email { get; set; }
        
        
        [Display(Name = "Пароль пользователя")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Длина пароля минимум 8 символов")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Password { get; set; }
        
        
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
        [Remote(action: "CheckRole", controller: "Home", ErrorMessage = "Выберите роль пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Role { get; set; }

        public bool ConfirmedEmail { get; set; }

        public string Token { get; set; }
    }
}
