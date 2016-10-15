/// <reference path="global.js" />
/// <reference path="index.js" />


var CallCenterHelper = {
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

        $(".page-title", parent.document).html("Açık Call Center Çağrıları");
        $("#btnBreadPage", parent.document).html("Call Center Çağrıları");

        $("#liSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liHouseSearch", parent.document).removeClass("active");

        CallCenterHelper.OnClickEvents();
        CallCenterHelper.OnChangeEvents();
        CallCenterHelper.OnChangeKeyPressEvents();


        //var url = $.url(document.location);
        //if (url.param("phonenumber") != undefined) {
        //    parent.IndexHelper.PhoneNumber = url.param("phonenumber");
        //}

        //$(window).resize(function () {
        //    parent.IndexHelper.AutoResize("ifrmContent");
        //});

        //parent.IndexHelper.AutoResize("ifrmContent");


    },
    "GetCallCenterPhoneCalls": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCallCenterPhoneCalls",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
            },
            complete: function () {
               // Metronic.unblockUI($("#pcBody"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Call Center");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Call Center");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {
        //$("#txtKey").keydown(function (e) {
        //    if (e.which == 13) {
        //        $scope.makeSearch();
        //    }
        //});
    },
    "OnChangeEvents": function () {

    }
}

function mainController($scope) {
    CallCenterHelper.GetCallCenterPhoneCalls(function (e) {
        $scope.$apply(function () {
            if (e.Success) {
                $scope.phonecalls = e.ReturnObject;

                setTimeout(function () {
                    parent.IndexHelper.AutoResize("ifrmContent");
                }, 200);
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Call Center");
            }

        });
    });
}
