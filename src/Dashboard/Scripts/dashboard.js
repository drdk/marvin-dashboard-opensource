$(document)
    .ready(function () {
        $(".showBtn")
            .click(function (event) {
                $(this).hide(0);
                $(this).next().show(400);
                return false;
            });
        $(".hideBtn")
            .click(function (event) {
                var parent = $(this).parent();
                parent.hide(400);
                parent.prev().show(400);
                return false;
            });
        window.addEventListener("hashchange", function () { scrollBy(0, -50) })
    });