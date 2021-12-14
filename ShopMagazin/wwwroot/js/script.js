$(function () {
    $.ajaxSetup({ cache: false });
    $(".lklk").click(function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').show();
        });
    });
    $("#btn-obrZvonok").click(function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').show();
        });
    });
    $(".AddToCart").click(function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').show();
        });
    });
});

$('.single-item').slick({
    dots: true,
    infinite: true,
    speed: 500,
    fade: true,
    cssEase: 'linear',
    nextArrow: $(".sl-nx"),
    prevArrow: $(".sl-pw"),
    autoplay: true,
    autoplaySpeed: 5000,
});
$('.single-item1').slick({
    infinite: true,
    arrows: true,
    autoplay: true,
    autoplaySpeed: 5000,
    // dots: true,
    slidesToShow: 3,
    slidesToScroll: 1,
    nextArrow: $(".sn"),
    prevArrow: $(".sp")
});

var movingBlock = $('#myDropdown').hide();
var movingBlock1 = $('#myDropdown1').hide();
var vv = $('.obratnii-zvonok').hide();
var vv1 = $('.modal-windows').hide();
var vv11 = $('.form-zvonka').hide();
var vv12 = $(".form-auth").hide();
var vv13 = $(".form-registration").hide();
var vv14 = $(".form-zabilipassword").hide();

jQuery(function ($) {
    $(document).mouseup(function (e) { // событие клика по веб-документу
        var divs1 = $(".form-auth");  // тут указываем ID элемента
        if ((!$(".fformms").is(e.target) && $(".fformms").has(e.target).length === 0)) {
            $(".modal-content").remove(); // скрываем его
        }
    });
});

$("#btn-reg-form").click(function(e) {
    e.preventDefault();
    vv12.hide();
    vv13.show();
});

$(".sbm-zabil").click(function(e) {
    e.preventDefault();
    vv12.hide();
    vv14.show();
});

$("#btn-enter-form").click(function(e) {
    e.preventDefault();
    vv13.hide();
    vv12.show();
});

$("#ret-auth").click(function(e) {
    e.preventDefault();
    vv14.hide();
    vv12.show();
});

//$('.dropbtn').click(function(e) {
//    let h = 1;
//    if (h == 0) {
//        e.preventDefault();
//        movingBlock.slideToggle();
//        movingBlock1.hide();
//    } else {
//        e.preventDefault();
//        vv12.show();
//        vv1.show();
//        vv.slideToggle();
//    }
//});

$('.btn-korzina').click(function(e) {
    let h = 1;
    e.preventDefault();
    if (h == 0) {
        $('.have-kor').hide();
        $('.pust-kor').show();
    } else {
        $('.pust-kor').hide();
        $('.have-kor').show();
    }
    movingBlock1.slideToggle();
    movingBlock.hide();
});

//const button1 = document.querySelector('.phone-open');
//const form = document.querySelector('#blablabla');

//butto1n.addEventListener('click', () => {
//    if (form.classList.contains("open") == false) {
//        form.classList.add('open');
//        button1.classList.add("rotated");
//    } else {
//        form.classList.remove('open');
//        button1.classList.remove("rotated");
//    }
//});

$('.zvonok').click(function(e) {
    e.preventDefault();
    vv11.show();
    vv1.show();
    vv.slideToggle();
});