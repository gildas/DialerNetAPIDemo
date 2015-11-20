$(function () {
    $("#loginLink").click(function () {
        $("#loading").fadeIn();
        var opts = {
            line:    12,      // The number of lines to draw
            length:   7,      // The length of each line
            width:    4,      // The line thickness
            radius:  10,      // The radius of the inner circle
            color:    '#000', // The color of the line
            speed:    1,      // Rounds per seconds
            trail:   60,      // The afterglow trail percentage
            shadow: false,   // render a shadow or not
            hwaccel: false,   // use hardware acceleration or not
        };
        var target  = document.getElementById('loading');
        var spinner = new Spinner(opts).spin(target);
    });
});