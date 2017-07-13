﻿//Basic web resources location
//var url = "http://localhost:54294";
var url = "http://deus2017economy.azurewebsites.net";

function sendAPIRequest(func_url, type, callback) {
    //send async API reqest, on responce proceed by callbakc function
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4) 
			callback(this.responseText, this.status);
    };
    xhttp.open(type, func_url, true);
    xhttp.setRequestHeader("Authorization", get_auth_tok());
    xhttp.send();
}

var cookieId = 'local_arm_id';

function save_auth_tok(user, pass) {
    var cookieValue = 'Basic ' + btoa(user + ':' + pass);
    var myDate = new Date();
    myDate.setMonth(myDate.getMonth() + 12);
    document.cookie = cookieId + "=" + cookieValue + ";expires=" + myDate;
}

function get_auth_tok() {
    //get basic auth token from cookies
	var tok = document.cookie.split(';')[0];
	var tokVal = tok.substring(tok.indexOf(cookieId) + cookieId.length + 1);

    //tok = document.cookie.replace(/(?:(?:^|.*;\s*)local_arm_id\s*\=\s*([^;]*).*$)|^.*$/, "$1");
    //test login, replace^ before deploy
    //tok = make_base_auth("admin", "123456"); //delete before use
    return tokVal;
}

function make_base_auth(user, password) {
    //make basic auth token from scratch
    var tok = user + ':' + password;
    var hash = btoa(tok);
    return "Basic " + hash;
}