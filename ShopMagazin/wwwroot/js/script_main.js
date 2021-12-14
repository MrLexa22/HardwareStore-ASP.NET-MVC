var movingBlock = $('#myDropdown').hide();
var movingBlock1 = $('#myDropdown1').hide();

$('.dropdowns').click(function (e) {
        e.preventDefault();
        movingBlock.slideToggle();
        movingBlock1.hide();
});