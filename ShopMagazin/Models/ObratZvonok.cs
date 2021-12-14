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
    public class ObratZvonok
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [Display(Name = "Имя")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректное имя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Ima { get; set; }

        [Display(Name = "Телефон")]
        [Remote(action: "CheckTel", controller: "Home", ErrorMessage = "Данный номер телефона уже есть в базе. Ожидайте звонка")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?[0-9]{3}-[0-9]{2}-[0-9]{2})", ErrorMessage = "Некорректный телефон")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }    
    }
}
