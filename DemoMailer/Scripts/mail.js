function newMail() {
    var to = $("#to");
    var cc = $("#cc");
    var subj = $("#subject");
    var body = $("#text");

    if (to.val() || cc.val() || subj.val() || body.val())
        if (!confirm("Throw away message and start over?"))
            return;

    to.val("");
    cc.val("");
    subj.val("");
    body.val("");
}

function sendMail() {
    var to = $("#to");
    var cc = $("#cc");
    var subj = $("#subject");
    var body = $("#text");

    $.ajax({
        url: "../api/mail/send?to=" + encodeURIComponent(to.val()) + "&cc=" + encodeURIComponent(cc.val()) + "&subject=" + encodeURIComponent(subj.val()) + "&body=" + encodeURIComponent(body.val()),
        method: "post",
        success: function(data) {
            $("#resultText").text("Mail sent successfully.");
        },
        error: function (jqXHR) {
            var msg = "Unknown error.";

            if (jqXHR.status == 400) {
                msg = "The server reported that one of the e-mail addresses entered was invalid.  Please verify your addresses.";
            } else if (jqXHR.status == 500) {
                msg = "Internal server error - sending mail failed.";
            }

            $("#resultText").text(msg);
        }
    });
}