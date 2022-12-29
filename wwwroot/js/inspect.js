document.onkeydown = function (e) {

    var message = "Not Allowed !!";
    if (event.keyCode == 123) {
        alert(message);
        return false;
    }
    if (e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)) {
        alert(message);
        return false;
    }
    //if (e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)) {
    //    alert(message);
    //    return false;
    //}
    if (e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)) {
        alert(message);
        return false;
    }
    if (e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)) {
        alert(message);
        return false;
    } if (e.ctrlKey && e.keyCode == 'S'.charCodeAt(0)) {
        alert(message);
        return false;
    }

    //var message1 = "Function Disabled!";
    //function clickdsb() {
    //    if (event.button == 2) {
    //        alert(message1);
    //        return false;
    //    }
    //}
    //function clickbsb(e) {
    //    if (document.layers || document.getElementById && !document.all) {
    //        if (e.which == 2 || e.which == 3) {
    //            alert(message1);
    //            return false;
    //        }
    //    }
    //}
    //if (document.layers) {
    //    document.captureEvents(Event.MOUSEDOWN);
    //    document.onmousedown = clickbsb;
    //}
    //else if (document.all && !document.getElementById) {
    //    document.onmousedown = clickdsb;
    //}

    //document.oncontextmenu = new Function("alert(message1);return false")
}

// jQuery(document).ready(function ($) {

