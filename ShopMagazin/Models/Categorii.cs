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
using PagedList;
using PagedList.Mvc;

namespace ShopMagazin.Models
{
    public class Categorii
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [Display(Name = "Категория товаров")]
        [Remote(action: "CheckCategoris", controller: "Tovari", ErrorMessage = "Категория уже существует")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NameCategori { get; set; }
        
        
        [Display(Name = "Изображение категории")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ImageId { get; set; }

        public bool HasImage()
        {
            return !String.IsNullOrWhiteSpace(ImageId);
        }
    }

    public class PodCategorii
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Категория товаров")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NameCategori { get; set; }

        [Display(Name = "Подкатегория товаров")]
        [Remote(action: "CheckPodCategoris", controller: "Tovari", ErrorMessage = "Категория уже существует")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NamePodCategori { get; set; }


        [Display(Name = "Изображение категории")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ImageId { get; set; }

        public bool HasImage()
        {
            return !String.IsNullOrWhiteSpace(ImageId);
        }
    }

    public class PodCatG
    {
        public Categorii categMain { get; set; }
        public IEnumerable<Categorii> PodU { get; set; }

        public IEnumerable<PodCategorii> PodCategsList { get; set; }
        public PodCategorii podcategs { get; set; }

        public Tovar tovar { get; set; }
        public IEnumerable<Tovar> TovariList { get; set; }
        public string KolTovarov { get; set; }

        public string ZhachPoiska { get; set; }
        public string ZnachPoiskaCateg { get; set; }
        public string ZnachPoiskaPodCateg { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }

    public class ZakazUser
    {
        public IEnumerable<Zakaz> zakazes { get; set; }
        public Zakaz zakaz { get; set; }
    }

    public class PoiskTovarov
    {
        public IEnumerable<Tovar> TovariResult { get; set; }
        public string search_string { get; set; }
    }

    public class Zakaz
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Id_User { get; set; }

        public string status { get; set; }

        [Display(Name = "Улица")]
        [RegularExpression(@"^[А-Яа-яЁё\,. ]+$", ErrorMessage = "Некорректный адрес")]
        [StringLength(70, MinimumLength = 10, ErrorMessage = "Адрес должен состоять как мининимум из 10 символов")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Address_Ulica { get; set; }

        [Display(Name = "Дом")]
        [RegularExpression(@"^[0-9/-]+$", ErrorMessage = "Некорректный дом")]
        [StringLength(4, MinimumLength = 1, ErrorMessage = "Дом не должен содержать так много цифр")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Address_Dom { get; set; }

        [Display(Name = "Корпус")]
        [RegularExpression(@"^[0-9А-Яа-яЕё]+$", ErrorMessage = "Некорректный Корпус")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Корпус не должен содержать так много цифр")]
        public string Address_Korpus { get; set; }


        [Display(Name = "Квартира")]
        [RegularExpression(@"^[0-9А-Яа-яЕё]+$", ErrorMessage = "Некорректная квартира")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "Квартира не должен содержать так много цифр")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Address_Kvartira { get; set; }

        [Display(Name = "Подъезд")]
        [RegularExpression(@"^[0-9А-Яа-яЕё]+$", ErrorMessage = "Некорректный подъезд")]
        [StringLength(4, MinimumLength = 1, ErrorMessage = "Подъезд не должен содержать так много цифр")]
        public string Address_Podezd { get; set; }

        [Display(Name = "Этаж")]
        [RegularExpression(@"^[0-9-]+$", ErrorMessage = "Некорректный этаж")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Этаж не должен содержать так много цифр")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Address_Etazh { get; set; }

        [Display(Name = "Домофон")]
        [RegularExpression(@"^[0-9А-Яа-яЕёA-Za-z]+$", ErrorMessage = "Некорректный домофон")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Домофон не должен содержать так много цифр")]
        public string Address_Domofon { get; set; }

        [Display(Name = "Имя")]
        [RegularExpression(@"^[А-Яа-яЕё., ]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Имя не должно содержать так много символов")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Contact_Name { get; set; }

        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Здесь нужно ввести корректный Email")]
        [StringLength(60, MinimumLength = 1, ErrorMessage = "Email не должно содержать так много символов")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Contact_Email { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?[0-9]{3}-[0-9]{2}-[0-9]{2})", ErrorMessage = "Некорректный телефон")]
        public string Contact_Telefon { get; set; }

        public string StoimostZakakaAll { get; set; }
        public string StoimostZakakaTovari { get; set; }
        public string CenaDostavki { get; set; }

        public List<TovarsInZakaz> tovars { get; set; }
    }

    public class TovarsInZakaz
    {
        public string Id_tovara { get; set; }
        public string NameTovar { get; set; }
        public string ModelTovar { get; set; }
        public string Cena { get; set; }
        public int kolvo_tovara { get; set; }
    }

    public class TovariInCart
    {
        public IEnumerable<TovarForCart> Korzina { get; set; }
        public IEnumerable<Tovar> Tovar { get; set; }
        public string summazakaza { get; set; }

        public Zakaz zakaz { get; set; }
    }

    public class Korzina
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string id_user { get; set; }
        public string id_tovara { get; set; }
        public int kolvo_tovara { get; set; }
    }

    public class TovarForCart
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int kolvo_tovara { get; set; }

        [Display(Name = "Артикул")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Артикул должен состоять из 7 цифр")]
        [Remote(action: "CheckArticul", controller: "ManageTovari", ErrorMessage = "Артикул с таким номером уже существует")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string articul { get; set; }

        [Display(Name = "Наименование товара")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NameTovar { get; set; }

        [Display(Name = "Модель")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ModelTovar { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Cena { get; set; }

        [Display(Name = "Страна-производитель")]
        [StringLength(20, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string StranaProizvoditel { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NameCategori { get; set; }

        [Display(Name = "Подкатегория")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NamePodCategori { get; set; }

        [Display(Name = "Изображение товара")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ImageId { get; set; }

        public bool HasTovarInCart { get; set; }

        public bool HasImage()
        {
            return !String.IsNullOrWhiteSpace(ImageId);
        }
    }

    public class Tovar
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Артикул")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Артикул должен состоять из 7 цифр")]
        [Remote(action: "CheckArticul", controller: "ManageTovari", ErrorMessage = "Артикул с таким номером уже существует")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string articul { get; set; }

        [Display(Name = "Наименование товара")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NameTovar { get; set; }

        [Display(Name = "Модель")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ModelTovar { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Cena { get; set; }

        [Display(Name = "Страна-производитель")]
        [StringLength(20, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string StranaProizvoditel { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NameCategori { get; set; }

        [Display(Name = "Подкатегория")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NamePodCategori { get; set; }

        [Display(Name = "Изображение товара")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ImageId { get; set; }

        public bool HasTovarInCart { get; set; }

        public bool HasImage()
        {
            return !String.IsNullOrWhiteSpace(ImageId);
        }

    }

    public class TovariManage
    {
        public PodCatG PodCatG { get; set; }
        public Tovar tovar { get; set; }
        public IEnumerable<Tovar> TovariList { get; set; }
    }

    public class PageViewModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
