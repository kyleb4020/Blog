//This is the fun stuff that I want done.
function getUserTime(time) {
    var userTime = new Date(Date.parse(time))
    var hours = userTime.getHours();
    var ampm = "AM";
    if (hours > 12) {
        hours = hours - 12;
        ampm = "PM";
    }
    if (hours == 12) {
        ampm = "PM"
    }
    if (hours == 0) {
        hours = 12;
    }
    var minutes = userTime.getMinutes();
    if (minutes < 10) {
        minutes = "0" + minutes;
    }
    var seconds = userTime.getSeconds();
    if (seconds < 10) {
        seconds = "0" + seconds;
    }
    var correctTime = (userTime.getMonth() + 1) + "/" + (userTime.getDate()) + "/" + userTime.getFullYear() + " " + hours + ":" + minutes + ":" + seconds + " " + ampm
    return (correctTime)
}

function move() {
    $('.even').removeClass('col-lg-offset-1');
    $('.even').addClass('col-lg-offset-5');
    $('.odd').removeClass('col-lg-offset-5');
    $('.odd').addClass('col-lg-offset-1');
};

function move1() {
    setTimeout(function () {
        if ($('.even').hasClass("col-lg-offset-1")) {
            $('.even').removeClass('col-lg-offset-1');
            $('.even').addClass('col-lg-offset-5');
            move1();
        }
        else
            if ($('.even').hasClass("col-lg-offset-5")) {
                $('.even').removeClass('col-lg-offset-5');
                $('.even').addClass('col-lg-offset-1');
                move1();
            }
    }, 8000);
};

function move2() {
    setTimeout(function () {
        if ($('.odd').hasClass("col-lg-offset-1")) {
            $('.odd').removeClass('col-lg-offset-1');
            $('.odd').addClass('col-lg-offset-5');
            move2();
        }
        else
            if ($('.odd').hasClass("col-lg-offset-5")) {
                $('.odd').removeClass('col-lg-offset-5');
                $('.odd').addClass('col-lg-offset-1');
                move2();
            }
    }, 8000);
};

$(document).ready(function () {
    $("#Topics").multiselect();
    $("#comments-title").click(function () {
        $("#comments-list").animate({
            height: "toggle"
        });
    });
//    $("#main-carousel").flickity({
//        autoPlay: true,
//        cellAlign: 'left',
//        contain: true
    //    })
    move();
    move1();
    move2();

    $(".blog-img").removeClass("img-start");
    $(".blog-img").addClass("blog-img-base");

});