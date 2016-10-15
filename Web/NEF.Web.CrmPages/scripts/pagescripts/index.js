/// <reference path="global.js" />


var IndexHelper = {
    "OnLoad": function () {

        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        QuickSidebar.init() // init quick sidebar
        Index.init();
        //Index.initDashboardDaterange();
        //Index.initJQVMAP(); // init index page's custom scripts
        //Index.initCalendar(); // init index page's custom scripts
        //Index.initCharts(); // init index page's custom scripts
        //Index.initChat();
        //Index.initMiniCharts();
        //Index.initIntro();
        //Tasks.initDashboardWidget();

        IndexHelper.UserId = $("#lblUserId").val();
        IndexHelper.UserType = $("#lblUserType").val();

        var url = $.url(document.location);
        if (url.data.param.query.hasOwnProperty("page")) {
            var page = url.param("page");
            var id = url.data.param.query.hasOwnProperty("pageid") ? url.param("pageid") : "";
            var paramName = url.data.param.query.hasOwnProperty("name") ? url.param("name") : "";

            if (id != "") {
                $("#ifrmContent").attr("src", page + ".html?"+paramName+"="+id);
            }
            else {
                $("#ifrmContent").attr("src", page + ".html");
            }
        }
        else {
            if (url.data.param.query.hasOwnProperty("direction")) {
                IndexHelper.IsCallCenterSearch = true;
                IndexHelper.Direction = url.param("direction");
                IndexHelper.SearchText = url.param("searchtext");
                IndexHelper.ActivityId = url.data.param.query.hasOwnProperty("activityid") ? url.param("activityid") : "";

                $("#ifrmContent").attr("src", "customersearch.html");
            }
            else {
                IndexHelper.Direction = "true";
                IndexHelper.ActivityId = "";
                IndexHelper.IsCallCenterSearch = false;
                $("#ifrmContent").attr("src", "main.html");
            }
        }

        $("#btnSearch").click(function () {
            $("#ifrmContent").attr("src", "customersearch.html");//.css("height", "1000px");
        });

        $("#btnHome").click(function () {
            if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist && parent.IndexHelper.UserType != UserTypes.CallCenter) //Resepsiyonistler arama sayfası dışında herhangi bir sayfaya gidemez.
            {
                $("#ifrmContent").attr("src", "main.html");//.css("height", "1500px");
            }
        });

        $("#btnHouseSearch").click(function () {
            if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist && parent.IndexHelper.UserType != UserTypes.CallCenter) //Resepsiyonistler arama sayfası dışında herhangi bir sayfaya gidemez.
            {
                $("#ifrmContent").attr("src", "housesearch.html");//.css("height", "1000px");
            }
        });

        $("#btnCallCenter").click(function () {
            if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist && parent.IndexHelper.UserType != UserTypes.CallCenter) //Resepsiyonistler arama sayfası dışında herhangi bir sayfaya gidemez.
            {
                $("#ifrmContent").attr("src", "callcenter.html");
            }
        });

        IndexHelper.GetSystemUserInfo();

        $.getJSON("http://jsonip.com?callback=?", function (data) {
            IndexHelper.IpAddress = data.ip;
        });

    },
    "ToastrShow": function (result, message, title) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "positionClass": "toast-top-right",
            "onclick": null,
            "showDuration": "1000",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }

        if (result) {
            toastr["success"](message, title);
        }
        else {
            toastr["error"](message, title);
        }

    },
    "ShowDialog": function ShowDialog(html, title, className) {
        bootbox.dialog({
            message: html,
            title: title,
            className: className
        });
    },
    "CloseDialog": function () {

        bootbox.hideAll();
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "GetSystemUserInfo": function () {
        var jData = {};

        jData.userId = IndexHelper.UserId;//  "246D808B-8774-E411-80E4-005056B0203A";

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetSystemUserInfo",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plBodySales"), iconOnly: false, boxed: true, message: "Satışlar çekiliyor..." });
            },
            complete: function () {
                //Metronic.unblockUI($("#plBodySales"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);

                    if (data.Success == true) {

                        IndexHelper.UserId = data.ReturnObject.SystemUserId;
                        $("#lblUserName").html(data.ReturnObject.FullName);

                        if (data.ReturnObject.ImageBase64 != null && data.ReturnObject.ImageBase64 != "") {
                            $("#imgProfile").attr("src", "data:image/png;base64," + data.ReturnObject.ImageBase64);
                        }
                        else {
                            $("#imgProfile").attr("src", "images/noprofileimage.png");
                        }
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Kullanıcı bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kullancı bilgileri");
            }
        });
    },
    "AutoResize": function (id) {
        try {
            var newheight;
            var newwidth;
            //debugger;
            if (document.getElementById) {
                newwidth = document.getElementById(id).contentWindow.document.body.scrollWidth;
                newheight = document.getElementById(id).contentWindow.document.getElementById("main").scrollHeight
            }

            document.getElementById(id).height = (newheight + 30) + "px";

        } catch (e) {

        }
    },
    "CreateActivityForCallCenter":function(contactId, contactName,callbackfunction)
    {
        var activity = {};
        activity.Direction = IndexHelper.Direction ? 1 : 0;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "contact";
        activity.ActivityParty.Id = contactId;
        activity.ActivityParty.Name = contactName;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = IndexHelper.UserId;

        var jData = {};
        jData.activity = activity;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateActivity",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                
            },
            complete: function () {
                
            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Aktivite Oluşturma");
            }
        });
    },
    "ContactId": "",
    "UserId": "",
    "UserType": "",
    "IpAddress": "",
    "IsCallCenterSearch": "",
    "Direction": "",
    "SearchText": "",
    "ActivityId":""
}
