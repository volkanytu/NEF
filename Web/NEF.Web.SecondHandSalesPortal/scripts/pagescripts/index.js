/// <reference path="global.js" />
/// <reference path="../purl.js" />


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

        //try {
        //    IndexHelper.Socket = io.connect("http://fenix.nef.com.tr:5555", { query: "userid=" + IndexHelper.UserId });

        //} catch (e) {
        //    IndexHelper.ToastrShow(true, e, "Node Hata");
        //    //return;
        //}

        var url = $.url(document.location);
        if (url.data.param.query.hasOwnProperty("page")) {
            var page = url.param("page");
            var id = url.data.param.query.hasOwnProperty("pageid") ? url.param("pageid") : "";
            var paramName = url.data.param.query.hasOwnProperty("name") ? url.param("name") : "";

            if (id != "") {
                $("#ifrmContent").attr("src", page + ".html?" + paramName + "=" + id);
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
                //Giden true
                //Gelen false
            }
            else {
                IndexHelper.Direction = "true";
                IndexHelper.ActivityId = "";
                IndexHelper.IsCallCenterSearch = false;
                $("#ifrmContent").attr("src", "main.html");

                if (IndexHelper.UserType == UserTypes.CallCenter) {
                    $("body").css("background-color", "white");
                    $("body").html("<center><br /> <img src='images/warning.png' height='144' /> <br /><h1>Sayfaya giriş yetkiniz yoktur.<br> BT ekibi ile irtibata geçebilirsiniz.<h1></center>");
                    return;
                }
            }
        }

        IndexHelper.OnClickEvents();
        IndexHelper.OnChangeEvents();
        IndexHelper.OnChangeKeyPressEvents();

        //IndexHelper.ListenSocketOldFeeds();
        //IndexHelper.ListenSocketFeeds();       

        IndexHelper.GetSystemUserInfo();

        IndexHelper.GetIpAddress();

        //$.getJSON("http://jsonip.com?callback=?", function (data) {
        //    IndexHelper.IpAddress = data.ip;
        //});

    },
    "ToastrShow": function (result, message, title) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "positionClass": "toast-top-right",
            "onclick": null,
            "showDuration": "1000",
            "hideDuration": "5000",
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

        $("#btnAuthorityDocs").click(function () {
            if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist && parent.IndexHelper.UserType != UserTypes.CallCenter) //Resepsiyonistler arama sayfası dışında herhangi bir sayfaya gidemez.
            {
                $("#ifrmContent").attr("src", "authoritydocsearch.html");
            }
        });

        $("#btnAllChart").click(function () {
            if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist &&
                parent.IndexHelper.UserType != UserTypes.CallCenter &&
                parent.IndexHelper.UserType != UserTypes.IsGyoCallCenter &&
                parent.IndexHelper.UserType != UserTypes.SatisDanismaniIsGyo &&
                parent.IndexHelper.UserType != UserTypes.SatisMuduruIsGyo) //Resepsiyonistler arama sayfası dışında herhangi bir sayfaya gidemez.
            {
                $("#ifrmContent").attr("src", "allchart.html");//.css("height", "1500px");
            }
        });
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
                        IndexHelper.BusinessUnitId = data.ReturnObject.BusinessUnitId;
                        IndexHelper.TeamId = data.ReturnObject.TeamId;
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
    "GetSystemUserInfoRetailer": function () {
        var jData = {};

        jData.userId = IndexHelper.UserId;//  "246D808B-8774-E411-80E4-005056B0203A";

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetSystemUserInfoRetailer",
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
                        IndexHelper.BusinessUnit = data.ReturnObject.BusinessUnit;
                        IndexHelper.Team = data.ReturnObject.Team;
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
    "CreateActivityForCallCenter": function (contactId, contactName, callbackfunction) {
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
        jData.sytemUserId = IndexHelper.UserId;;
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
    "ActivityId": "",
    "ListenSocketOldFeeds": function () {

        IndexHelper.Socket.on("oldfeeds", function (data) {

            $("#lstFeeds").html("");

            if ($("#lstFeeds li").length > 0) {

            }

            for (var i = 0; i < data.length; i++) {

                if (data[i].User != null && data[i].User.Id.toLowerCase() == IndexHelper.UserId.toLowerCase()) {

                    var html = "";

                    html += "<li>";
                    html += "	<a href='#' type='feedlink' url='" + data[i].Url + "' id='" + data[i].Id + "'>";
                    html += "		<span class='label label-sm label-icon label-danger'>";
                    html += "			<i class='fa fa-bolt'></i>";
                    html += "		</span>";
                    html += "		" + data[i].Description + " <span class='time'>";
                    html += "		</span>";
                    html += "	</a>";
                    html += "</li>";

                    $("#lstFeeds").append(html);

                    $("#lblFeedHeader").html($("#lstFeeds li").length + " bildiriminiz var");
                    $("#lblFeedCount").html($("#lstFeeds li").length).show();
                }
            }

            $("a[type='feedlink']").click(function () {

                var url = $(this).attr("url");
                var id = $(this).attr("id");
                var listItem = $(this).parent();

                if (url != null && url != "" && url != undefined && url != "undefined") {
                    $("#ifrmContent").attr("src", url);
                }

                IndexHelper.UpdateFeedAsRead(id, listItem);

            });

        });
    },
    "ListenSocketFeeds": function () {

        IndexHelper.Socket.on("message", function (data) {

            if (data.User != null && data.User.Id.toLowerCase() == IndexHelper.UserId.toLowerCase()) {

                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "positionClass": "toast-top-right",
                    "onclick": null,
                    "showDuration": "1000",
                    "hideDuration": "5000",
                    "timeOut": "5000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }

                toastr[data.FeedType.Name](data.Description, data.Name);

                var html = "";

                html += "<li>";
                html += "	<a href='#' type='feedlink' url='" + data.Url + "' id='" + data.Id + "'>";
                html += "		<span class='label label-sm label-icon label-danger'>";
                html += "			<i class='fa fa-bolt'></i>";
                html += "		</span>";
                html += "		" + data.Description + " <span class='time'>";
                html += "		</span>";
                html += "	</a>";
                html += "</li>";

                $("#lstFeeds").append(html);

                $("#lblFeedHeader").html($("#lstFeeds li").length + " bildiriminiz var");
                $("#lblFeedCount").html($("#lstFeeds li").length).show();

                //IndexHelper.ToastrShow(false, data.Description, data.Description);
            }

            $("a[type='feedlink']").click(function () {

                var url = $(this).attr("url");
                var id = $(this).attr("id");
                var listItem = $(this).parent();

                if (url != null && url != "" && url != undefined && url != "undefined") {
                    $("#ifrmContent").attr("src", url);
                }

                IndexHelper.UpdateFeedAsRead(id, listItem);

            });

        });
    },
    "Socket": {},
    "GetIpAddress": function () {
        var jData = {};

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetIpAddress",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    IndexHelper.IpAddress = data;
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "GetIpAddress");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "GetIpAddress");
            }
        });
    },
    "CheckEntityExists": function (entityId, entityName, attributeName) {

        var returnValue = false;

        var jData = {};
        jData.entityId = entityId;
        jData.entityName = entityName;
        jData.attributeName = attributeName;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CheckEntityExists",
            async: false,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {

                returnValue = data.Success;
            },
            error: function (a, b, c) {

            }
        });

        return returnValue;
    },
    "CheckSaleCustomerIdentityAndAddress": function (quoteId, callbackFunction) {

        var returnValue = false;

        var jData = {};
        jData.quoteId = quoteId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CheckSaleCustomerIdentityAndAddress",
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

                callbackFunction(data);
            },
            error: function (a, b, c) {

            }
        });

        return returnValue;
    },
    "UpdateFeedAsRead": function (feedId, listItem) {

        var jData = {};
        jData.feedId = feedId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateFeedAsRead",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {

            },
            complete: function () {

                $(listItem).remove();

                $("#lblFeedHeader").html($("#lstFeeds li").length + " bildiriminiz var");
                $("#lblFeedCount").html($("#lstFeeds li").length).show();


            },
            success: function (data) {


            },
            error: function (a, b, c) {

            }
        });
    },
    "BusinessUnitId": "",
    "TeamId": "",
}
