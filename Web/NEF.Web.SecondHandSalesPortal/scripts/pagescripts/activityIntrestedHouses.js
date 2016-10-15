/// <reference path="global.js" />


var ActivityIntrestedHousesHelper = {
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

        ActivityIntrestedHousesHelper.OnClickEvents();
        ActivityIntrestedHousesHelper.OnChangeEvents();
        ActivityIntrestedHousesHelper.OnChangeKeyPressEvents();

    },
    "GetActivityInterestedHouse": function (callbackFunction) {
        var jData = {};
        jData.activityId = ActivityIntrestedHousesHelper.PhoneCallId != null ? ActivityIntrestedHousesHelper.PhoneCallId : ActivityIntrestedHousesHelper.AppointmentId;
        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/GetActivityInterestedHousesForSR",
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
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        callbackFunction(null);
                        parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Bilgileri");
                    }
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
    "CreateRental": function (callbackFunction) {
        var jData = {};
        jData.interestedHouses = $("#pcMain input[type='checkbox']:checked").map(function () {
            return $(this).val();
        }).toArray();


        if (ActivityIntrestedHousesHelper.ContactId != null) {
            jData.contact = {};
            jData.contact.LogicalName = "contact";
            jData.contact.Id = ActivityIntrestedHousesHelper.ContactId;
            jData.account = null;
        }
        if (ActivityIntrestedHousesHelper.AccountId != null) {
            jData.account = {};
            jData.account.LogicalName = "account";
            jData.account.Id = ActivityIntrestedHousesHelper.AccountId;
            jData.contact = null;
        }

        jData.owner = {};
        jData.owner.LogicalName = "systemuser";
        jData.owner.Id = parent.IndexHelper.UserId;

        //jData.customer = {};
        //jData.customer.LogicalName = ActivityIntrestedHousesHelper.ContactId != null ? "contact" : "account";
        //jData.customer.Name = ActivityIntrestedHousesHelper.Name;
        //jData.customer.Id = ActivityIntrestedHousesHelper.ContactId != null ? ActivityIntrestedHousesHelper.ContactId : ActivityIntrestedHousesHelper.AccountId;


        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/CreateRental",
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

                        var isOpenSalesPage = false;

                        if (jData.interestedHouses.length == 1) {
                            isOpenSalesPage = true;
                        }

                        callbackFunction(data, isOpenSalesPage);
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

    "CreateSecondHand": function (callbackFunction) {
        var jData = {};
        jData.interestedHouses = $("#pcMain input[type='checkbox']:checked").map(function () {
            return $(this).val();
        }).toArray();

        if (ActivityIntrestedHousesHelper.ContactId != null) {
            jData.contact = {};
            jData.contact.LogicalName = "contact";
            jData.contact.Id = ActivityIntrestedHousesHelper.ContactId;
            jData.account = null;
        }
        if (ActivityIntrestedHousesHelper.AccountId != null) {
            jData.account = {};
            jData.account.LogicalName = "account";
            jData.account.Id = ActivityIntrestedHousesHelper.AccountId;
            jData.contact = null;
        }

        jData.owner = {};
        jData.owner.LogicalName = "systemuser";
        jData.owner.Id = parent.IndexHelper.UserId;

        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/CreateSecondHand",
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

                        var isOpenSalesPage = false;

                        if (jData.interestedHouses.length == 1) {
                            isOpenSalesPage = true;
                        }

                        callbackFunction(data, isOpenSalesPage);
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
    ActivityIntrestedHousesHelper.ContactId = url.data.param.query.hasOwnProperty("contactid") ? url.param("contactid") : null;
    ActivityIntrestedHousesHelper.AccountId = url.data.param.query.hasOwnProperty("accountid") ? url.param("accountid") : null;
    ActivityIntrestedHousesHelper.Name = url.data.param.query.hasOwnProperty("name") ? url.param("name") : null;
    ActivityIntrestedHousesHelper.PhoneCallId = url.data.param.query.hasOwnProperty("phonecallid") ? url.param("phonecallid") : null;
    ActivityIntrestedHousesHelper.AppointmentId = url.data.param.query.hasOwnProperty("appointmentid") ? url.param("appointmentid") : null;

    ActivityIntrestedHousesHelper.GetActivityInterestedHouse(function (e) {
        $scope.$apply(function () {
            $scope.activityProducts = e;
        });
    });

    $scope.CreateRental = function () {
        ActivityIntrestedHousesHelper.CreateRental(function (e, isOpenSalesPage) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.parent.IndexHelper.ToastrShow(true, e.Result, "Kiralama Kaydı Oluşturma");

                    $("#btnRefreshSales", $("#ifrmContent", parent.document)[0].contentWindow.document).click();

                    parent.parent.IndexHelper.CloseDialog();

                    if (isOpenSalesPage == true) {
                        parent.parent.IndexHelper.ShowDialog("<iframe src='editRental.html?rentalid=" + e.CrmId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-flag'></i> Kiralama Kaydı", "modal90");
                    }
                }
            });
        });
    };

    $scope.CreateSecondHand = function () {
        ActivityIntrestedHousesHelper.CreateSecondHand(function (e, isOpenSalesPage) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.parent.IndexHelper.ToastrShow(true, e.Result, "2. El Satış Kaydı Oluşturma");

                    $("#btnRefreshSales", $("#ifrmContent", parent.document)[0].contentWindow.document).click();

                    parent.parent.IndexHelper.CloseDialog();

                    if (isOpenSalesPage == true) {
                        parent.parent.IndexHelper.ShowDialog("<iframe src='editSecondHand.html?secondhandid=" + e.CrmId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-flag'></i> 2.El Satış Kaydı", "modal90");
                    }
                }
            });
        });
    };
}
