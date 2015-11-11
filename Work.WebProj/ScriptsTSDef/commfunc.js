function uniqid() {
    var newDate = new Date();
    return newDate.getTime();
}
function obj_prop_list(obj) {
    for (var prop in obj) {
        if (obj.hasOwnProperty(prop)) {
            console.log(prop + " :" + obj[prop]);
        }
    }
}
function isValidJSONDate(value, userFormat) {
    var userFormat = userFormat || 'yyyy-mm-dd';
    var delimiter = /[^mdy]/.exec(userFormat)[0];
    var theFormat = userFormat.split(delimiter);
    var splitDatePart = value.split('T');
    if (splitDatePart.length == 1)
        return false;
    var theDate = splitDatePart[0].split(delimiter);
    var isDate = function (date, format) {
        var m, d, y;
        for (var i = 0, len = format.length; i < len; i++) {
            if (/m/.test(format[i]))
                m = date[i];
            if (/d/.test(format[i]))
                d = date[i];
            if (/y/.test(format[i]))
                y = date[i];
        }
        ;
        return (m > 0 && m < 13 && y && y.length === 4 && d > 0 && d <= (new Date(y, m, 0)).getDate());
    };
    return isDate(theDate, theFormat);
}
function obj_prop_date(obj) {
    for (var prop in obj) {
        if (obj.hasOwnProperty(prop)) {
            var getUTCStr = obj[prop];
            if (typeof getUTCStr == 'string') {
                var isValid = isValidJSONDate(getUTCStr, null);
                if (isValid) {
                    obj[prop] = new Date(getUTCStr);
                }
            }
        }
    }
    return obj;
}
function stand_date(getDateStr) {
    var d = new Date(getDateStr);
    var r = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate;
    return r;
}
function now_date_str() {
    var d = new Date();
    var r = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate;
    return r;
}
function month_first_day() {
    var d = new Date();
    var r = new Date(d.getFullYear(), d.getMonth(), 1);
    return r;
}
function month_last_day() {
    var d = new Date();
    var r = new Date(d.getFullYear(), d.getMonth() + 1, 0);
    return r;
}
function tim() {
    var d = new Date();
    return d.toUTCString() + '.' + d.getMilliseconds().toString();
}
function pad(str, len, pad, dir) {
    var padlen;
    if (typeof (len) == "undefined") {
        var len = 0;
    }
    if (typeof (pad) == "undefined") {
        var pad = ' ';
    }
    if (typeof (dir) == "undefined") {
        var dir = 3;
    }
    if (len + 1 >= str.length) {
        switch (dir) {
            case 1:
                str = Array(len + 1 - str.length).join(pad) + str;
                break;
            case 2:
                str = str + Array(len + 1 - str.length).join(pad);
                break;
            case 3:
                var right = Math.ceil((padlen = len - str.length) / 2);
                var left = padlen - right;
                str = Array(left + 1).join(pad) + str + Array(right + 1).join(pad);
                break;
        }
    }
    return str;
}
function showAjaxError(data) {
    alert('Ajax error,check console info!');
    console.log(data);
}
function jqGet(url, data) {
    return $.ajax({
        type: "GET",
        url: url,
        data: data,
        dataType: 'json'
    });
}
;
function jqPost(url, data) {
    return $.ajax({
        type: "POST",
        url: url,
        data: data,
        dataType: 'json'
    });
}
