var company_details_count = 1;
var total_months = 0;

function getDateDiff(date1, date2, interval) {
    var second = 1000,
    minute = second * 60,
    hour = minute * 60,
    day = hour * 24,
    week = day * 7;
    date1 = new Date(date1);
    date2 = (date2 == 'now') ? new Date() : new Date(date2);
    var timediff = date2.getTime() - date1.getTime();
    if (isNaN(timediff)) return NaN;
    switch (interval) {
        case "years":
            return date2.getFullYear() - date1.getFullYear();
        case "months":
            return ((date2.getFullYear() * 12 + date2.getMonth()) - (date1.getFullYear() * 12 + date1.getMonth()));
        case "weeks":
            return Math.floor(timediff / week);
        case "days":
            return Math.floor(timediff / day);
        case "hours":
            return Math.floor(timediff / hour);
        case "minutes":
            return Math.floor(timediff / minute);
        case "seconds":
            return Math.floor(timediff / second);
        default:
            return undefined;
    }
}

$(function () {

    $("#JoiningDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "yy/mm/dd",
        yearRange: "-100:+10"
    });
    $("#LeavingDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "yy/mm/dd",
        yearRange: "-100:+10"
    });

    
    $("#JoiningDate").change(function () {
        if ($("#JoiningDate").val().trim() == "") {
            alert("Please enter a valid joining date.");
        }
        //alert('Joining Date:' + $("#JoiningDate").val());
        //alert('Leaving Date:' + $("#LeavingDate").val());
        if ($("#LeavingDate").val().trim() != "")
        {
            if (new Date($("#LeavingDate").val()).getTime() < new Date($("#JoiningDate").val()).getTime()) {
                alert("Date of Joining Should be greater than date of leaving");
                $("#JoiningDate").val("");
            }
            else {
                var startDateString = $("#JoiningDate").val();
                var endDateString = $("#LeavingDate").val();
                var months = getDateDiff(startDateString, endDateString, "months");
                var years = Math.floor(months / 12);
                var m = months % 12
                var complete = years + " Year(s), " + m + " Month(s)";
                $("#Expereiences").val(complete);
                $("#Expereience").val(months);

            }
        }
        
    });

    $("#LeavingDate").change(function () {
        if ($("#LeavingDate").val().trim() == "") {
            alert("Please enter a valid leaving date.");
        }
        //alert('Joining Date:' + $("#JoiningDate").val());
        //alert('Leaving Date:' + $("#LeavingDate").val());

        if ($("#JoiningDate").val().trim() != "") {
            

            if (new Date($("#LeavingDate").val()).getTime() < new Date($("#JoiningDate").val()).getTime()) {
                alert("Date of Leaving Should be greater than date of joining.");
                $("#LeavingDate").val("");
            }
            else {
                var startDateString = $("#JoiningDate").val();
                var endDateString = $("#LeavingDate").val();
                var months = getDateDiff(startDateString, endDateString, "months");
                var years = Math.floor(months / 12);
                var m = months % 12
                var complete = years + " Year(s), " + m + " Month(s)";
                $("#Expereiences").val(complete);
                $("#Expereience").val(months);

            }
        }
    });

    //$('input[name="JoiningDate"]').daterangepicker(
    //    {
    //        singleDatePicker: true,
    //        format: 'DD/MM/YYYY',
    //        startDate: '01/01/1975'
    //    },
    //    function (start, end, label) {
    //        //alert('A date range was chosen: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));
    //        JoiningDateChange();
    //    }
    //);
    //$('input[name="LeavingDate"]').daterangepicker(
    //     {
    //         singleDatePicker: true,
    //         format: 'DD/MM/YYYY',
    //         startDate: '01/01/1975'
    //     },
    //     function (start, end, label) {
    //         //alert('A date range was chosen: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));
    //         LeavingDateChange();
    //     }
    //);

    //function JoiningDateChange() {
    //    if ($("#JoiningDate").val().trim() == "") {
    //        alert("Please enter a valid joining date.");
    //    }
    //    alert('Joining Date:' + $("#JoiningDate").val());
    //    alert('Leaving Date:' + $("#LeavingDate").val());
    //    if ($("#LeavingDate").val().trim() != "") {
    //        if (new Date($("#LeavingDate").val()).getTime() < new Date($("#JoiningDate").val()).getTime()) {
    //            alert("Date of Joining Should be greater than date of leaving");
    //            $("#JoiningDate").val("");
    //        }
    //        else {
    //            var startDateString = $("#JoiningDate").val();
    //            var endDateString = $("#LeavingDate").val();
    //            var months = getDateDiff(startDateString, endDateString, "months");
    //            var years = Math.floor(months / 12);
    //            var m = months % 12
    //            var complete = "(" + years + ") Year " + "(" + m + ")";
    //            $("#Expereiences").val(complete);
    //            $("#Expereience").val(months);

    //        }
    //    }
    //}

    //function LeavingDateChange() {
    //    if ($("#LeavingDate").val().trim() == "") {
    //        alert("Please enter a valid leaving date.");
    //    }
    //    alert('Joining Date:' + $("#JoiningDate").val());
    //    alert('Leaving Date:' + $("#LeavingDate").val());

    //    if ($("#JoiningDate").val().trim() != "") {


    //        if (new Date($("#LeavingDate").val()).getTime() < new Date($("#JoiningDate").val()).getTime()) {
    //            alert("Date of Leaving Should be greater than date of joining.");
    //            $("#LeavingDate").val("");
    //        }
    //        else {
    //            var startDateString = $("#JoiningDate").val();
    //            var endDateString = $("#LeavingDate").val();
    //            var months = getDateDiff(startDateString, endDateString, "months");
    //            var years = Math.floor(months / 12);
    //            var m = months % 12
    //            var complete = "(" + years + ") Year " + "(" + m + ")";
    //            $("#Expereiences").val(complete);
    //            $("#Expereience").val(months);

    //        }
    //    }
    //}

    

});