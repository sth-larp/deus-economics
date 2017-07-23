//Basic web resources location
//var url = "http://localhost:54294";
var url = "https://alice.digital/econ";

var cookieId = 'local_arm_id';

var ROLE = {
    NONE: { value: 0, name: "None", rusname: "Нет" },
    READ: { value: 1, name: "Read", rusname: "Просмотр" },
    WITHDRAW: { value: 2, name: "Withdraw", rusname: "Снятие" },
    ADMIN: { value: 4, name: "Admin", rusname: "Админ" },
    MASTER: { value: 8, name: "Master", rusname: "Ваш счет" }
};

var ACC_ROLE = {
    NONE: { value: 0, name: "None", rusname: "Нет" },
    PERSON: { value: 1, name: "Person", rusname: "Персона" },
    MASTER: { value: 2, name: "Master", rusname: "Мастер" },
    ADMIN: { value: 4, name: "Admin", rusname: "Админ" },
    CORP: { value: 8, name: "Corp", rusname: "Корпорация" },
    GOVT: { value: 16, name: "Govt", rusname: "Правительство" },
    COMPANY: { value: 32, name: "Company", rusname: "Компания" }
};

function get_role(role) {
    for (var i in ROLE) {
        if (ROLE[i].name == role)
            return ROLE[i];
    }
    return ROLE[0];
}

function get_acc_role(role) {
    for (var i in ACC_ROLE) {
        if (ACC_ROLE[i].name == role)
            return ACC_ROLE[i];
    }
    return ACC_ROLE[0];
}

function format(source, params) {
    $.each(params, function(i, n) {
        source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
    });
    return source;
}

function loginRedirect()
{
	document.cookie = cookieId + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
	window.location.assign("./login.html");
}


function sendAPIRequest(func_url, type, callback, data) {
    //send async API reqest, on responce proceed by callbakc function
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4) 
			callback(this.responseText, this.status);
    };
    xhttp.open(type, func_url, true);
    xhttp.setRequestHeader("Content-type", "application/json");
    xhttp.setRequestHeader("Authorization", get_auth_tok());
    xhttp.send(data);
}

function save_auth_tok(user, pass) {
    var cookieValue = 'Basic ' + btoa(user + ':' + pass);
    var myDate = new Date();
    myDate.setMonth(myDate.getMonth() + 12);
    document.cookie = cookieId + "=" + cookieValue + ";expires=" + myDate.toGMTString();
}

function get_auth_tok() {
    //get basic auth token from cookies
	var tok = document.cookie.split(';')[0];
	var tokVal = tok.substring(tok.indexOf(cookieId) + cookieId.length + 1);
    return tokVal;
}

function get_auth_tok_user() {
    //get basic auth token from cookies
	var tok = document.cookie.split(';')[0];
	var tokVal = tok.substring(tok.indexOf(cookieId) + cookieId.length + 1);
	tokVal = tokVal.split(' ')[1];
	tokVal = atob(tokVal);
	tokVal = tokVal.split(':')[0];

    //tok = document.cookie.replace(/(?:(?:^|.*;\s*)local_arm_id\s*\=\s*([^;]*).*$)|^.*$/, "$1");
    return tokVal;
}

function make_base_auth(user, password) {
    //make basic auth token from scratch
    var tok = user + ':' + password;
    var hash = btoa(tok);
    return "Basic " + hash;
}

function customDateFormat(dateStr) {
    var d = new Date(dateStr);
    var f = formatDate(d, "d MMM HH:mm");
    return f;
}

function formatDate(date, format, utc) {
    var MMMM = ["\x00", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    var MMM = ["\x01", "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек"];
    var dddd = ["\x02", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    var ddd = ["\x03", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    function ii(i, len) {
        var s = i + "";
        len = len || 2;
        while (s.length < len) s = "0" + s;
        return s;
    }

    var y = utc ? date.getUTCFullYear() : date.getFullYear();
    format = format.replace(/(^|[^\\])yyyy+/g, "$1" + y);
    format = format.replace(/(^|[^\\])yy/g, "$1" + y.toString().substr(2, 2));
    format = format.replace(/(^|[^\\])y/g, "$1" + y);

    var M = (utc ? date.getUTCMonth() : date.getMonth()) + 1;
    format = format.replace(/(^|[^\\])MMMM+/g, "$1" + MMMM[0]);
    format = format.replace(/(^|[^\\])MMM/g, "$1" + MMM[0]);
    format = format.replace(/(^|[^\\])MM/g, "$1" + ii(M));
    format = format.replace(/(^|[^\\])M/g, "$1" + M);

    var d = utc ? date.getUTCDate() : date.getDate();
    format = format.replace(/(^|[^\\])dddd+/g, "$1" + dddd[0]);
    format = format.replace(/(^|[^\\])ddd/g, "$1" + ddd[0]);
    format = format.replace(/(^|[^\\])dd/g, "$1" + ii(d));
    format = format.replace(/(^|[^\\])d/g, "$1" + d);

    var H = utc ? date.getUTCHours() : date.getHours();
    format = format.replace(/(^|[^\\])HH+/g, "$1" + ii(H));
    format = format.replace(/(^|[^\\])H/g, "$1" + H);

    var h = H > 12 ? H - 12 : H == 0 ? 12 : H;
    format = format.replace(/(^|[^\\])hh+/g, "$1" + ii(h));
    format = format.replace(/(^|[^\\])h/g, "$1" + h);

    var m = utc ? date.getUTCMinutes() : date.getMinutes();
    format = format.replace(/(^|[^\\])mm+/g, "$1" + ii(m));
    format = format.replace(/(^|[^\\])m/g, "$1" + m);

    var s = utc ? date.getUTCSeconds() : date.getSeconds();
    format = format.replace(/(^|[^\\])ss+/g, "$1" + ii(s));
    format = format.replace(/(^|[^\\])s/g, "$1" + s);

    var f = utc ? date.getUTCMilliseconds() : date.getMilliseconds();
    format = format.replace(/(^|[^\\])fff+/g, "$1" + ii(f, 3));
    f = Math.round(f / 10);
    format = format.replace(/(^|[^\\])ff/g, "$1" + ii(f));
    f = Math.round(f / 10);
    format = format.replace(/(^|[^\\])f/g, "$1" + f);

    var T = H < 12 ? "AM" : "PM";
    format = format.replace(/(^|[^\\])TT+/g, "$1" + T);
    format = format.replace(/(^|[^\\])T/g, "$1" + T.charAt(0));

    var t = T.toLowerCase();
    format = format.replace(/(^|[^\\])tt+/g, "$1" + t);
    format = format.replace(/(^|[^\\])t/g, "$1" + t.charAt(0));

    var tz = -date.getTimezoneOffset();
    var K = utc || !tz ? "Z" : tz > 0 ? "+" : "-";
    if (!utc) {
        tz = Math.abs(tz);
        var tzHrs = Math.floor(tz / 60);
        var tzMin = tz % 60;
        K += ii(tzHrs) + ":" + ii(tzMin);
    }
    format = format.replace(/(^|[^\\])K/g, "$1" + K);

    var day = (utc ? date.getUTCDay() : date.getDay()) + 1;
    format = format.replace(new RegExp(dddd[0], "g"), dddd[day]);
    format = format.replace(new RegExp(ddd[0], "g"), ddd[day]);

    format = format.replace(new RegExp(MMMM[0], "g"), MMMM[M]);
    format = format.replace(new RegExp(MMM[0], "g"), MMM[M]);

    format = format.replace(/\\(.)/g, "$1");

    return format;
};

function buildTabs(elements_list, lines_per_page, page_id_prefix, header_str) {
    var result = "<div class=\"table-nav-wrapper\">";
    var page_links = "";
    //count of pages
    for (offs = 0; offs * lines_per_page < elements_list.length; offs++) {
        result = result + "<table id = '" + page_id_prefix + "_" + offs + "' style='display: none'>" + header_str;
        var tmp_limit = lines_per_page;
        for (i = 0; i < tmp_limit; i++) if (offs * lines_per_page + i < elements_list.length) result = result + elements_list[offs * lines_per_page + i];
        result = result + "</table>";
        page_links = page_links + "<span id='" + page_id_prefix + "_page_" + offs + "'>| " + (offs + 1) + "</span>";
    }
    //change_page_buttons
    result = result + "<div class=\"table-nav\"><span id='" + page_id_prefix + "_bookmark' style='display:none'>0</span>";
    result = result + "<span id='" + page_id_prefix + "_maxpage' style='display:none'>" + offs + "</span>"
    result = result + "<span id='" + page_id_prefix + "_back'><< Назад </span>" + page_links + "<span id='" + page_id_prefix + "_fw'> | Вперед >></span></div></div>";
    return result;
}
function initTabsControl(page_id_prefix) {
    var max_page = parseInt(document.getElementById(page_id_prefix + "_maxpage").innerHTML);
    var tab_name = "";
    function createPageHandler(page_number) {
        return function () {
            //for(j = 0; j < max_page; j++) document.getElementById(page_id_prefix + "_" + j).style.display = "none";
            document.getElementById(page_id_prefix + "_" + parseInt(document.getElementById(page_id_prefix + "_bookmark").innerHTML)).style.display = "none";
            document.getElementById(page_id_prefix + "_" + page_number).style.display = "block";
            document.getElementById(page_id_prefix + "_bookmark").innerHTML = page_number;
        }
    }
    function createArrowHandler(direction) {
        return function () {
            var cur_page = parseInt(document.getElementById(page_id_prefix + "_bookmark").innerHTML);
            var new_page = cur_page + direction;
            if ((new_page >= 0 && new_page < max_page)) {
                document.getElementById(page_id_prefix + "_bookmark").innerHTML = new_page;
                document.getElementById(page_id_prefix + "_" + cur_page).style.display = "none";
                document.getElementById(page_id_prefix + "_" + new_page).style.display = "block";
            }
        }
    }
    for (var i = 0; i < max_page; i++) {
        tab_button_name = page_id_prefix + "_page_" + i;
        document.getElementById(tab_button_name).style.cursor = "pointer";
        document.getElementById(tab_button_name).onclick = createPageHandler(i);
    }
    document.getElementById(page_id_prefix + "_fw").style.cursor = "pointer";
    document.getElementById(page_id_prefix + "_back").style.cursor = "pointer";
    document.getElementById(page_id_prefix + "_fw").onclick = createArrowHandler(1);
    document.getElementById(page_id_prefix + "_back").onclick = createArrowHandler(-1);
}


function insertDocumentToWrapper(wrapper_id, document_href, rq_type) {
    //load sub-page into wrapper
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) document.getElementById(wrapper_id).innerHTML = xhttp.responseText;
    };
    xhttp.open(rq_type, document_href, true);
    xhttp.setRequestHeader("Authorization", get_auth_tok());
    xhttp.send();
}

function fillError(elem, body, status) {
    if (status == 0) body = "Сервер не отвечает";
    var txt = "Ошибка " + status + ", " + body;
    document.getElementById(elem).innerHTML = txt;
}