$(document).ready(function () {
    $("#more-menu").mouseover(function () {
        if ($(".pop-more-maste-menu").css("display") == "none")
            $(".pop-more-maste-menu").show();
    });
    $("#more-menu").mouseout(function () {
        if ($(".pop-more-maste-menu").css("display") != "none")
            $(".pop-more-maste-menu").hide();
    });
});