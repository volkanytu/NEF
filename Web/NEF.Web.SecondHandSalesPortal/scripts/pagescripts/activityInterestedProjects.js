/// <reference path="global.js" />


var ActivityIntrestedProjectHelper = {
    "OnLoad": function () {
        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        QuickSidebar.init() // init quick sidebar
        Index.init();
        Index.initDashboardDaterange();
        Index.initJQVMAP(); // init index page's custom scripts
        Index.initCalendar(); // init index page's custom scripts
        Index.initCharts(); // init index page's custom scripts
        Index.initChat();
        Index.initMiniCharts();
        //Index.initIntro();
        Tasks.initDashboardWidget();

        $(".page-title", parent.document).html("Konut Arama");
        $("#btnBreadPage", parent.document).html("Konut Arama");

        $("#liHouseSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liSearch", parent.document).removeClass("active");

        ActivityIntrestedProjectHelper.OnClickEvents();
        ActivityIntrestedProjectHelper.OnChangeEvents();
        ActivityIntrestedProjectHelper.OnChangeKeyPressEvents();

    },
    "GetProjectsForActivity": function (callbackFunction) {
        var jData = {};
        jData.phonecallId = ActivityIntrestedProjectHelper.PhoneCallId != null ? ActivityIntrestedProjectHelper.PhoneCallId : GuidEmpty;
        jData.appointmentId = ActivityIntrestedProjectHelper.AppointmentId != null ? ActivityIntrestedProjectHelper.AppointmentId : GuidEmpty;
        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/GetProjectsForActivity",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pcMain"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#pcMain"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);

                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Bilgileri");
            }
        });
    },
    "CreateInterestedProjects": function (callbackFunction) {
        var jData = {};
        jData.interestedProjects = $("#pcMain input[type='checkbox']:checked").map(function () {
            return $(this).val();
        }).toArray();

        jData.phonecallId = ActivityIntrestedProjectHelper.PhoneCallId != null ? ActivityIntrestedProjectHelper.PhoneCallId : GuidEmpty;
        jData.appointmentId = ActivityIntrestedProjectHelper.AppointmentId != null ? ActivityIntrestedProjectHelper.AppointmentId : GuidEmpty;

        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/CreateInterestedProject",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pcMain"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#pcMain"));
            },
            success: function (data) {
                if (data != null) {
                    if (data.Success) {
                        callbackFunction(data);
                    }
                    else {
                        parent.parent.IndexHelper.ToastrShow(false, data.Result, "Satış Oluşturma");
                    }
                }
                else {
                    parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Satış Oluşturma");
                }
            },
            error: function (a, b, c) {
                parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Oluşturma");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {
    },
    "PhoneCallId": "",
    "AppointmentId": "",
    "ContactId": "",
    "AccountId": "",
    "Name": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    ActivityIntrestedProjectHelper.ContactId = url.data.param.query.hasOwnProperty("contactid") ? url.param("contactid") : null;
    ActivityIntrestedProjectHelper.AccountId = url.data.param.query.hasOwnProperty("accountid") ? url.param("accountid") : null;
    ActivityIntrestedProjectHelper.Name = url.data.param.query.hasOwnProperty("name") ? url.param("name") : null;
    ActivityIntrestedProjectHelper.PhoneCallId = url.data.param.query.hasOwnProperty("phonecallid") ? url.param("phonecallid") : null;
    ActivityIntrestedProjectHelper.AppointmentId = url.data.param.query.hasOwnProperty("appointmentid") ? url.param("appointmentid") : null;

    ActivityIntrestedProjectHelper.GetProjectsForActivity(function (e) {
        $scope.$apply(function () {
            if (e.Success == true) {
                $scope.projects = e.ReturnObject;
                // $scope.search.Project.Id = e.ReturnObject[0].ProjectId;
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Konut Arama");
            }
        });
    });

    $scope.CreateInterestedProjects = function () {

        ActivityIntrestedProjectHelper.CreateInterestedProjects(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $("#btnRefreshProjects", $("#ifrmContent", parent.document)[0].contentWindow.document).click();

                    parent.parent.IndexHelper.CloseDialog();
                }
            });
        });
    };
}
