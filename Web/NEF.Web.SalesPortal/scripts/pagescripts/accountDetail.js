/// <reference path="global.js" />
/// <reference path="../../global/plugins/bootstrap-modal/js/bootstrap-modal.js" />
jQuery.fn.ForceNumericOnly = function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.charCode || e.keyCode || 0;
            return (
                key == 8 ||
                key == 9 ||
                key == 13 ||
                key == 46 ||
                key == 110 ||
                key == 190 ||
                (key >= 35 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
        });
    });
};

var AccountDetailHelper = {
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

        $("#btnBreadPage", parent.document).html("Kişi Kartı");

        $("#liHouseSearch", parent.document).removeClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liSearch", parent.document).removeClass("active");

        AccountDetailHelper.OnClickEvents();
        AccountDetailHelper.OnChangeEvents();
        AccountDetailHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");
        //$("#txtMobilePhone").ForceNumericOnly();
        $("#txtTcNo").ForceNumericOnly();
        $("#txtMobilePhone").mask('+90-###-#######');

        $(".form_datetime").datetimepicker({
            autoclose: true,
            isRTL: Metronic.isRTL(),
            format: "dd.mm.yyyy hh:ii",
            pickerPosition: (Metronic.isRTL() ? "bottom-right" : "bottom-left")
        });

    },
    "GetDetail": function (id, callbackFunction) {

        var returnValue = null;
        var jData = {};
        jData.accountId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetAccountDetail",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyUserInfo"), iconOnly: true });
                Metronic.blockUI({ target: $("#plBodyActivities"), iconOnly: true });
                Metronic.blockUI({ target: $("#plBodyOpps"), iconOnly: true });
                Metronic.blockUI({ target: $("#plBodyIntrested"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyUserInfo"));
                Metronic.unblockUI($("#plBodyActivities"));
                Metronic.unblockUI($("#plBodyOpps"));
                Metronic.unblockUI($("#plBodyIntrested"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    returnValue = data;

                    callbackFunction(data);

                }
                else {
                    returnValue = false;
                }
            },
            error: function (a, b, c) {
                returnValue = false;
            }
        });

        return returnValue;
    },
    "GetCustomerQuotes": function (callbackFunction) {
        var jData = {};
        jData.customerId = AccountDetailHelper.AccountId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetCustomerQuotes",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodySales"), iconOnly: false, boxed: true, message: "Satışlar getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodySales"));
            },
            success: function (data) {
                data = JSON.parse(data);
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Satış Bilgileri");
            }
        });
    },
    "GetCustomerOpportunities": function (callbackFunction) {
        var jData = {};
        jData.customerId = AccountDetailHelper.AccountId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetCustomerOpportunities",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyOpps"), iconOnly: false, boxed: true, message: "Fırsatlar getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyOpps"));
            },
            success: function (data) {
                data = JSON.parse(data);
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Fırsat Bilgileri");
            }
        });
    },
    "GetCustomerActivities": function (callbackFunction) {
        var jData = {};
        jData.customerId = AccountDetailHelper.AccountId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetCustomerActivities",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyActivities"), iconOnly: false, boxed: true, message: "Aktiviteler getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyActivities"));
            },
            success: function (data) {
                data = JSON.parse(data);
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Bilgileri");
            }
        });
    },
    "GetCustomerOpportunityHouses": function (callbackFunction) {
        var jData = {};
        jData.customerId = AccountDetailHelper.AccountId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserAllIntrestedHouses",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plHouses"), iconOnly: false, boxed: true, message: "Fırsat Konutları getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plHouses"));
            },
            success: function (data) {
                data = JSON.parse(data);
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Fırsat Bilgileri");
            }
        });
    },
    "AddActivityIntrestedHouse": function (interestedHouse, callbackFunction) {
        var jData = {};
        jData.interestProduct = interestedHouse;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateActivityInterestedHouses",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plHouses"), iconOnly: false, boxed: true, message: "İlgilendiği konut olarak ekleniyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plHouses"));
            },
            success: function (data) {
                if (data != null) {
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Konut Arama");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Arama");
            }
        });
    },
    "GetTaxOffices": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetTaxOffices",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetCountries": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCountries",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetCities": function (countryId, callbackFunction) {
        var jData = {};
        jData.countryId = countryId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetCities",
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
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetTowns": function (cityId, callbackFunction) {
        var jData = {};
        jData.cityId = cityId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetTowns",
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
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetDistricts": function (townId, callbackFunction) {
        var jData = {};
        jData.townId = townId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetDistricts",
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
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetActivityStatuses": function (activtytopicId, callbackFunction) {

        var jData = {};
        jData.userTypeCode = parent.IndexHelper.UserType;
        jData.subjectId = activtytopicId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetActivityStatuses",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        //returnValue = data.ReturnObject;
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetActivityStatusDetails": function (activityStatusId, callbackFunction) {
        var jData = {};
        jData.activityStatusId = activityStatusId;
        jData.userTypeCode = parent.IndexHelper.UserType;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetActivityStatusDetails",
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
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetActivityTopics": function (callbackFunction) {

        var jData = {};
        jData.userTypeCode = parent.IndexHelper.UserType;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetActivityTopics",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Konuları");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Konuları");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Konuları");
            }
        });
    },
    "GetActivityInfo": function (activityId, callbackFunction) {

        var jData = {};

        jData.activityId = activityId;

        var jSonData = JSON.stringify(jData);

        var methodname = AccountDetailHelper.PhoneCallId != null ? "/GetPhoneCallActivityInfo" : "/GetAppointmentActivityInfo";

        $.ajax({
            url: CustomServiceUrl + methodname,
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);

                    if (data.Success == true)
                        callbackFunction(data);
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite");
            }
        });
    },
    "GetSalesOffices": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetSalesOffices",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetCallCenterAgents": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCallCenterAgents",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "UpdateAccount": function (account, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.account = account;
        jData.account.MobilePhone = $("#txtMobilePhone").val();
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateOrUpdateAccount",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyUserInfo"), iconOnly: false, boxed: true, message: "Müşteri bilgileri güncelleniyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyUserInfo"));
            },
            success: function (data) {
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });

        return returnValue;
    },
    "IsSaleable": function () {
        var activityStatusCode = $("#slctActivityStatus option:selected").attr("code");
        if (activityStatusCode == "1") {
            return true;
        }
        else {
            return false;
        }
    },
    "CloseRelated": function (activity, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.activity = activity;
        jData.activity.ActivityParty = {};
        jData.activity.ActivityParty.LogicalName = "account";
        jData.activity.ActivityParty.Name = AccountDetailHelper.AccountName;
        jData.activity.ActivityParty.Id = AccountDetailHelper.AccountId;

        jData.activity.NextCallDate = "/Date(" + $('#txtCallDate').data("datetimepicker").getDate().getTime() + ")/";
        jData.activity.NextAppointmentDate = "/Date(" + $('#txtAppointmentDate').data("datetimepicker").getDate().getTime() + ")/";
        jData.activity.NextPaymentDate = "/Date(" + $('#txtPaymentDate').data("datetimepicker").getDate().getTime() + ")/";

        jData.activity.Owner = {};
        jData.activity.Owner.LogicalName = "systemuser";
        jData.activity.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateActivity",
            async: false,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: false, boxed: true, message: "Aktivite güncelleniyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });

        return returnValue;
    },
    "CloseIrrelevant": function (activity) {
        var returnValue = null;
        var jData = {};
        jData.activity = activity;
        jData.activity.ActivityParty = {};
        jData.activity.ActivityParty.LogicalName = "account";
        jData.activity.ActivityParty.Name = AccountDetailHelper.AccountName;
        jData.activity.ActivityParty.Id = AccountDetailHelper.AccountId;

        jData.activity.NextCallDate = "/Date(" + $('#txtCallDate').data("datetimepicker").getDate().getTime() + ")/";
        jData.activity.NextAppointmentDate = "/Date(" + $('#txtAppointmentDate').data("datetimepicker").getDate().getTime() + ")/";
        jData.activity.NextPaymentDate = "/Date(" + $('#txtPaymentDate').data("datetimepicker").getDate().getTime() + ")/";

        jData.activity.Owner = {};
        jData.activity.Owner.LogicalName = "systemuser";
        jData.activity.Owner.Id = parent.IndexHelper.UserId;
        jData.activity.Direction = parent.IndexHelper.Direction == "false" ? DirectionCodes.Gelen : DirectionCodes.Giden;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateActivity",
            async: false,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: false, boxed: true, message: "Aktivite kapatılıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data.Success) {
                    $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + AccountDetailHelper.AccountId).css("height", "1300px");
                    parent.IndexHelper.ToastrShow(true, data.Result, "Aktivite Güncelleme");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Güncelleme");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Güncelleme");
            }
        });

        return returnValue;
    },
    "GetActivityInterestedHouse": function (callbackFunction) {
        var jData = {};
        jData.activityId = AccountDetailHelper.PhoneCallId != null ? AccountDetailHelper.PhoneCallId : AccountDetailHelper.AppointmentId;
        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/GetActivityInterestedHouses",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: false, boxed: true, message: "Aktivite ilgili konutları getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        callbackFunction(null);
                        //parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Bilgileri");
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
    "RemoveActivityIntrestedHouse": function (callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.interestedHouseIds = [];
        $("#tblHouses input[type='checkbox']:checked").each(function () {
            jData.interestedHouseIds.push(this.value);
        });
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/RemoveActivityInterestedHouses",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            success: function (data) {
                if (data.Success) {
                    parent.IndexHelper.ToastrShow(true, data.Result, "Müşteri Bilgileri");
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(true, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });

        return returnValue;
    },
    "RemoveActivityIntrestedProject": function (callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.interestedProjectIds = [];
        $("#tblProjects input[type='checkbox']:checked").each(function () {
            jData.interestedProjectIds.push(this.value);
        });
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/RemoveActivityInterestedProjects",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            complete: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: true });
            },
            success: function (data) {
                if (data.Success) {
                    parent.IndexHelper.ToastrShow(true, data.Result, "Müşteri Bilgileri");
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(true, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });

        return returnValue;
    },
    "GetActivityInterestedProjects": function (callbackFunction) {
        var jData = {};
        jData.activityId = AccountDetailHelper.PhoneCallId != null ? AccountDetailHelper.PhoneCallId : AccountDetailHelper.AppointmentId;
        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/GetActivityInterestedProjects",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: false, boxed: true, message: "Aktivite ilgili projeleri getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        callbackFunction(null);
                        // parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Bilgileri");
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
    "GetTeamMembers": function (callbackFunction) {
        var jData = {};
        jData.teamId = parent.IndexHelper.TeamId;
        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/GetTeamMembers",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        callbackFunction(null);
                    }
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Bilgileri");
            }
        });
    },
    "CreateActivityForCallCenter": function (customerId, callbackfunction) {
        var activity = {};
        activity.Direction = DirectionCodes.Giden;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "account";
        activity.ActivityParty.Id = customerId;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = parent.IndexHelper.UserId;

        var jData = {};
        jData.activity = activity;
        jData.systemUserId = parent.IndexHelper.UserId;
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
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Aktivite Oluşturma");
            }
        });
    },
    "CheckCustomerApartmentOwner": function (callbackFunction) {
        var jData = {};
        jData.customerId = AccountDetailHelper.AccountId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CheckCustomerApartmentOwner",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#portletActivity"), iconOnly: false, boxed: true, message: "Daire sahipliği kontrol ediliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(true, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });

        return returnValue;
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "AccountId": "",
    "AccountName": "",
    "PhoneCallId": "",
    "AppointmentId": "",
    "RefreshActivityIntrestedHouse": function () {
        alert("ADD HOUSE RELATIONS & REFRESH ACTIVITY INTRESTED HOUSES");
    },
    "CheckActivityOwnership": function (userId, activityId, callbackFunction) {

        var returnValue = false;

        var jData = {};
        jData.userId = userId;
        jData.activityId = activityId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CheckActivityOwnership",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {

            },
            complete: function (data) {

            },
            success: function (data) {

                if (data != null) {

                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Servisten data boş döndü.", "Sahiplik Kontrolü");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, c, "Sahiplik Kontrolü");
            }
        });

        return returnValue;
    }
}

var app = angular.module('testApp', ['ngRoute']).directive('onLastRepeat', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                var cbFunc = attr.onLastRepeat;

                scope.$eval(cbFunc);
                //$timeout(function () {
                //    scope.$emit('ngRepeatFinished');
                //});
            }
        }
    }
});

app.controller('ctrlTest', ['$scope', function ($scope) {


    var url = $.url(document.location);
    AccountDetailHelper.AccountId = url.param("accountid");
    AccountDetailHelper.PhoneCallId = url.data.param.query.hasOwnProperty("phonecallid") ? url.param("phonecallid") : null;
    AccountDetailHelper.AppointmentId = url.data.param.query.hasOwnProperty("appointmentid") ? url.param("appointmentid") : null;

    $scope.GuidEmpty = GlobalHelper.GuidEmpty;
    $scope.activityStates = GlobalHelper.ActivityStates;
    $scope.activityStateClass = GlobalHelper.ActivityStateClass;
    $scope.selectedHouse = [];
    $scope.isSubjectVisible = true;

    if (parent.IndexHelper.UserType == UserTypes.MusteriIliskileri) {
        $scope.showMusteriIliskileri = true;
    }


    AccountDetailHelper.GetTaxOffices(function (e) {
        $scope.$apply(function () {
            $scope.taxOffices = e;
        });
    });

    AccountDetailHelper.GetCountries(function (e) {
        $scope.$apply(function () {
            $scope.countries = e;
            $scope.overCountries = e;
            setTimeout(function () {
                $scope.$apply(function () {
                    $scope.country = $scope.countries[0].CountryId; //Default olarak Türkiye gelmesi sağlanır.
                });
            }, 500);
        });
    });

    $scope.changeCountries = function () {
        $scope.city = $scope.GuidEmpty;
        $scope.cities = null;
        $scope.town = $scope.GuidEmpty;
        $scope.towns = null;
        $scope.district = $scope.GuidEmpty;
        $scope.districts = null;
        AccountDetailHelper.GetCities($scope.country, function (e) {
            $scope.$apply(function () {
                $scope.cities = e;
            });

        });
    }

    $scope.changeCities = function () {
        $scope.town = $scope.GuidEmpty;
        $scope.towns = null;
        $scope.district = $scope.GuidEmpty;
        $scope.districts = null;
        AccountDetailHelper.GetTowns($scope.city, function (e) {
            $scope.$apply(function () {
                $scope.towns = e;
            });
        });
    }

    $scope.changeTowns = function () {
        $scope.district = $scope.GuidEmpty;
        $scope.districts = null;
        AccountDetailHelper.GetDistricts($scope.town, function (e) {
            $scope.$apply(function () {
                $scope.districts = e;
            });
        });
    }

    $scope.changeActivityTopics = function () {
        $scope.activityStatuses = null;
        $scope.activityStatusDetails = null;
        $scope.activity.ActivityStatus = null;
        $scope.activity.ActivityStatusDetail = null;


        AccountDetailHelper.GetActivityStatuses($scope.activity.ActivityTopic.Id, function (e) {
            $scope.$apply(function () {
                $scope.activityStatuses = e;
            });
        });
    }

    AccountDetailHelper.GetDetail(AccountDetailHelper.AccountId, function (e) {

        var results = e;
        if (results != null && results != false) {
            if (results.Success) {
                $scope.$apply(function () {
                    $scope.account = results.ReturnObject;
                    if ($scope.account != null) {



                        $(".page-title", parent.document).html($scope.account.AccountName);

                        if ($scope.account.TaxOffice != null) {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    $scope.taxoffice = $scope.account.TaxOffice.Id;
                                });
                            }, 500);
                        }

                        if ($scope.account.Country != null) {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    $scope.country = $scope.account.Country.Id;
                                });
                            }, 500);

                            AccountDetailHelper.GetCities($scope.account.Country.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.cities = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.city = $scope.account.City.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }
                        else //Eğer önceden set edilmemiş ise Default olarak İstanbul gelmesi sağlanır. Ardından İstanbul'a ait ilçeler doldurulur.
                        {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    AccountDetailHelper.GetCities($scope.country, function (e) {
                                        $scope.$apply(function () {
                                            $scope.cities = e;
                                            setTimeout(function () {
                                                $scope.$apply(function () {
                                                    $scope.city = $scope.cities[0].CityId;

                                                    //İstanbul'a ait ilçeler doldurulur.
                                                    AccountDetailHelper.GetTowns($scope.city, function (e) {
                                                        $scope.$apply(function () {
                                                            $scope.towns = e;
                                                        });
                                                    });

                                                });
                                            }, 500);
                                        });
                                    });
                                });
                            }, 500);
                        }

                        if ($scope.account.City != null) {
                            AccountDetailHelper.GetTowns($scope.account.City.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.towns = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.town = $scope.account.Town.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }

                        if ($scope.account.Town != null) {
                            AccountDetailHelper.GetDistricts($scope.account.Town.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.districts = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.district = $scope.account.District.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }

                        if ($scope.account.AccountName != "" && $scope.account.AccountName != null) {
                            AccountDetailHelper.AccountName = $scope.account.AccountName;
                        }

                        GetCustomerQuotes();
                        GetCustomerOpportunities();
                        GetCustomerActivities();
                        GetOpportunityHouses();

                        setInterval(function () {
                            GetCustomerQuotes();
                            GetCustomerOpportunities();
                            GetCustomerActivities();
                        }, 6000);
                    }
                });
            }
            else {
                parent.IndexHelper.ToastrShow(false, results.Result, "Müşteri Bilgileri");
            }
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
        }

    });

    if (AccountDetailHelper.PhoneCallId != null || AccountDetailHelper.AppointmentId != null) {
        AccountDetailHelper.GetActivityTopics(function (e) {
            $scope.$apply(function () {
                $scope.activityTopics = e;
            });
        });

        $scope.activity = {}
        $scope.hasActivity = true;

        if (parent.IndexHelper.IsCallCenterSearch) {
            $scope.showActivityCreate = false;
        }

        if (AccountDetailHelper.PhoneCallId != null) {
            $scope.activity.ActivityId = AccountDetailHelper.PhoneCallId;
            $scope.hideSunum = false;
            $scope.activity.ObjectTypeCode = 4210;

            GetActivityInterestedHouses();
            GetActivityInterestedProjects();

            if (parent.IndexHelper.IsCallCenterSearch) {
                $scope.isCallCenter = true
                AccountDetailHelper.GetCallCenterAgents(function (e) {
                    $scope.$apply(function () {
                        $scope.callcenteragents = e;
                    });
                });
            }


        }
        else {
            $scope.activity.ActivityId = AccountDetailHelper.AppointmentId;
            $scope.hideSunum = true;
            $scope.activity.ObjectTypeCode = 4201;
            $scope.activity.PresentationType = 100000001;

            AccountDetailHelper.GetSalesOffices(function (e) {
                $scope.$apply(function () {
                    $scope.salesOffices = e;

                    var ipStart = parent.IndexHelper.IpAddress.substring(0, 3);
                    var officeId = null;
                    for (var i = 0; i < $scope.salesOffices.length; i++) {
                        if ($scope.salesOffices[i].IpAddress.substring(0, 3) == ipStart) {
                            officeId = $scope.salesOffices[i].SalesOfficeId;
                        }
                    }

                    if (officeId != null) {
                        setTimeout(function () {
                            $scope.$apply(function () {
                                $scope.activity.SalesOffice = {};
                                $scope.activity.SalesOffice.Id = officeId;
                                $scope.IsSalefOfficeDisable = true;
                            });
                        }, 500);
                    }
                });
            });

            GetActivityInterestedHouses();
            GetActivityInterestedProjects();


        }
        AccountDetailHelper.GetTeamMembers(function (e) {
            $scope.$apply(function () {
                if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase()) {
                    $scope.teamMembers = e;
                    $scope.isRetailer = true;
                }
            });
        });
        //AccountDetailHelper.GetActivityStatuses(function (e) {
        //    $scope.$apply(function () {
        //        $scope.activityStatuses = e;
        //    });
        //});

        $scope.activityStatusChange = function () {
            var activityStatusCode = $("#slctActivityStatus option:selected").attr("code");
            activityStatusCode = parseInt(activityStatusCode);
            if (activityStatusCode == 3) {
                $scope.isSubjectVisible = false;
                $scope.activity.Subject = null;
            }
            else {
                $scope.isSubjectVisible = true;
            }

            if (activityStatusCode == 5)//Daire sahibi seçilirse gerçekliği kontrol edilir.
            {
                AccountDetailHelper.CheckCustomerApartmentOwner(function (a) {
                    $scope.$apply(function () {
                        if (!a.Success) {
                            parent.IndexHelper.ToastrShow(false, a.Result, "Müşteri Bilgileri");
                        }
                    });
                });
            }

            AccountDetailHelper.GetActivityStatusDetails($scope.activity.ActivityStatus.Id, function (e) {
                $scope.$apply(function () {
                    $scope.activityStatusDetails = e;
                });
            });
        }

        $scope.activityStatusDetailChange = function () {
            $scope.activityStatusDetailChange = function () {
                var activityStatusDetailCode = $("#slctActivityStatusDetail option:selected").attr("code");

                if (activityStatusDetailCode == GlobalHelper.ActivityStatusDetails.TekrarAranacak) {
                    $scope.IsNextCallShow = true;
                }
                else {
                    $scope.IsNextCallShow = false;
                }

                //randevu tekrar aranacak
                if (activityStatusDetailCode == GlobalHelper.ActivityStatusDetails.SatisOfisineGidecek || activityStatusDetailCode == GlobalHelper.ActivityStatusDetails.MerkezOfisineGidecek) {
                    $scope.IsNextAppointmentShow = true;
                }
                else {
                    $scope.IsNextAppointmentShow = false;
                }

                //ileri tarihte ödeyecek
                if (activityStatusDetailCode == GlobalHelper.ActivityStatusDetails.BelirliBirSureOdeyemeyecek) {
                    $scope.IsOdemeyecek = true;
                }
                else {
                    $scope.IsOdemeyecek = false;
                }
            }
        }

        AccountDetailHelper.GetActivityInfo($scope.activity.ActivityId, function (e) {
            $scope.$apply(function () {
                if (e.ReturnObject.ActivityTopic != null) {
                    $scope.activity.ActivityTopic = {};
                    $scope.activity.ActivityTopic.Id = e.ReturnObject.ActivityTopic.Id;

                    setTimeout(function () {
                        $scope.$apply(function () {
                            AccountDetailHelper.GetActivityStatuses($scope.activity.ActivityTopic.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.activityStatuses = e;

                                    if (e.ReturnObject.ActivityStatus != null) {
                                        setTimeout(function () {
                                            $scope.$apply(function () {
                                                $scope.activity.ActivityStatus = {};
                                                $scope.activity.ActivityStatus.Id = e.ReturnObject.ActivityStatus.Id;

                                                setTimeout(function () {
                                                    $scope.$apply(function () {

                                                        var activityStatusCode = $("#slctActivityStatus option:selected").attr("code");
                                                        activityStatusCode = parseInt(activityStatusCode);
                                                        if (activityStatusCode == 3) {
                                                            $scope.isSubjectVisible = false;
                                                            $scope.activity.Subject = null;
                                                        }
                                                        else {
                                                            $scope.isSubjectVisible = true;
                                                        }

                                                        AccountDetailHelper.GetActivityStatusDetails($scope.activity.ActivityStatus.Id, function (a) {
                                                            $scope.$apply(function () {
                                                                $scope.activityStatusDetails = a;
                                                                setTimeout(function () {
                                                                    $scope.$apply(function () {
                                                                        if (e.ReturnObject.ActivityStatusDetail != null) {
                                                                            $scope.activity.ActivityStatusDetail = {};
                                                                            $scope.activity.ActivityStatusDetail.Id = e.ReturnObject.ActivityStatusDetail.Id;
                                                                        }
                                                                    });
                                                                }, 500);
                                                            });
                                                        });
                                                    });
                                                }, 500);

                                            });
                                        }, 500);
                                    }
                                });
                            });
                        });
                    }, 500);
                }

                $scope.activity.Note = e.ReturnObject.Note;
            });
        });
    }
    else {
        $scope.showActivityCreate = true;
    }

    $scope.Save = function () {
        if ($scope.account.TaxOffice == null) {
            $scope.account.TaxOffice = {};
            $scope.account.TaxOffice.Id = $scope.taxoffice;
        }
        else {
            if ($scope.taxoffice == $scope.GuidEmpty) {
                $scope.account.TaxOffice = null;
            }
            else if ($scope.taxoffice != undefined) {
                $scope.account.TaxOffice.Id = $scope.taxoffice;
            }
        }

        var contactId = $("#txtContact").attr("crmid");

        if (contactId != null && contactId != undefined && contactId != "") {

            if ($scope.account.Contact == null)
                $scope.account.Contact = {};

            $scope.account.Contact.Id = $("#txtContact").attr("crmid");
            $scope.account.Contact.Name = $("#txtContact").val();
            $scope.account.Contact.LogicalName = "contact"
        }

        if ($scope.account.Country == null) {
            $scope.account.Country = {};
            $scope.account.Country.Id = $scope.country;
        }
        else {
            if ($scope.country == $scope.GuidEmpty) {
                $scope.account.Country = null;
            }
            else if ($scope.country != undefined) {
                $scope.account.Country.Id = $scope.country;
            }
        }

        if ($scope.account.City == null) {
            $scope.account.City = {};
            $scope.account.City.Id = $scope.city;
        }
        else {
            if ($scope.city == $scope.GuidEmpty) {
                $scope.account.City = null;
            }
            else if ($scope.city != undefined) {
                $scope.account.City.Id = $scope.city;
            }
        }

        if ($scope.account.Town == null) {
            $scope.account.Town = {};
            $scope.account.Town.Id = $scope.town;
        }
        else {
            if ($scope.town == $scope.GuidEmpty) {
                $scope.account.Town = null;
            }
            else if ($scope.town != undefined) {
                $scope.account.Town.Id = $scope.town;
            }
        }

        if ($scope.account.District == null) {
            $scope.account.District = {};
            $scope.account.District.Id = $scope.district;
        }
        else {
            if ($scope.district == $scope.GuidEmpty) {
                $scope.account.District = null;
            }
            else if ($scope.district != undefined) {
                $scope.account.District.Id = $scope.district;
            }
        }

        AccountDetailHelper.UpdateAccount($scope.account, function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, e.Result, "Müşteri Bilgileri");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Bilgileri");
                }
            });
        });
    }

    $scope.ActivityUpdate = function () {

        if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase() && $("#slctAlternative option:selected").text() == "") {
                $("#alternatif").addClass("has-error");
                parent.IndexHelper.ToastrShow(false, "Alternatif kişisini seçin!", "Aktivite Bilgileri");
        }else if (!$scope.activity.hasOwnProperty("ActivityStatus")) {
            $("#divStatus").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Lütfen görüşme sonucu alanını doldurunuz!", "Aktivite Bilgileri");
        }
        else if (!$scope.activity.hasOwnProperty("ActivityStatusDetail")) {
            $("#divStatusDetail").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Lütfen görüşme sonuç ayrıntısı alanını doldurunuz!", "Aktivite Bilgileri");
        }
        else {
            var activityTopicCode = $("#slctActivityTopics option:selected").attr("code");
            activityTopicCode = parseInt(activityTopicCode);

            if ($scope.activityProducts != null || parent.IndexHelper.IsCallCenterSearch || activityTopicCode == 7) {
                var activityStatusCode = $("#slctActivityStatus option:selected").attr("code");
                activityStatusCode = parseInt(activityStatusCode);

                if (activityStatusCode == 1) {
                    if (!parent.IndexHelper.IsCallCenterSearch) //Eğer bir call center araması ise satış oluşturulamaz
                    {
                        parent.bootbox.confirm("Satış kaydı oluşturmak ister misiniz?", function (result) {
                            AccountDetailHelper.CloseRelated($scope.activity, function (e) {
                                $scope.$apply(function () {
                                    if (e.Success) {
                                        parent.IndexHelper.ToastrShow(true, e.Result, "Müşteri Bilgileri");
                                        if (result == true) {
                                            var isSaleAble = AccountDetailHelper.IsSaleable();
                                            if (isSaleAble) {
                                                if (AccountDetailHelper.PhoneCallId != null) {
                                                    parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?accountid=" + AccountDetailHelper.AccountId + "&name=" + AccountDetailHelper.AccountName + "&phonecallid=" + AccountDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                }
                                                else {
                                                    parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?accountid=" + AccountDetailHelper.AccountId + "&name=" + AccountDetailHelper.AccountName + "&appointmentid=" + AccountDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                }
                                                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + AccountDetailHelper.AccountId).css("height", "1300px");
                                            }
                                            else {
                                                parent.IndexHelper.ToastrShow(false, "Aktivite ilgili olarak kapatılmadığı için satış kaydı oluşturulmayacak!", "Müşteri Bilgileri");
                                            }
                                        }
                                        else {
                                            $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + AccountDetailHelper.AccountId).css("height", "1300px");
                                        }
                                    }
                                    else {
                                        parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Bilgileri");
                                    }
                                });
                            });
                        });
                    }
                    else {
                        AccountDetailHelper.CloseIrrelevant($scope.activity);
                    }
                }
                else {
                    AccountDetailHelper.CloseIrrelevant($scope.activity);
                }
            }
            else {
                parent.IndexHelper.ToastrShow(false, "İlgili konut eklemeden görüşmeyi tamamlayamazsınız!", "Görüşme Tamamlama");
            }

        }
    }

    $scope.AddActivityIntrestedHouse = function () {
        if (AccountDetailHelper.PhoneCallId != null) {
            parent.IndexHelper.ShowDialog("<iframe src='addhouse.html?phonecallid=" + AccountDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Konut Arama", "modal90");
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='addhouse.html?appointmentid=" + AccountDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Konut Arama", "modal90");
        }
    };

    $scope.AddActivityIntrestedProject = function () {
        if (AccountDetailHelper.PhoneCallId != null) {
            parent.IndexHelper.ShowDialog("<iframe src='addinterestedprojects.html?contactid=" + AccountDetailHelper.AccountId + "&phonecallid=" + AccountDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Proje", "modal90");
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='addinterestedprojects.html?contactid=" + AccountDetailHelper.AccountId + "&appointmentid=" + AccountDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Proje", "modal90");
        }
    };

    $scope.RemoveActivityIntrestedHouse = function () {
        AccountDetailHelper.RemoveActivityIntrestedHouse(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    GetActivityInterestedHouses();
                }
            });
        });
    };

    $scope.RemoveActivityIntrestedProject = function () {
        AccountDetailHelper.RemoveActivityIntrestedProject(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    GetActivityInterestedProjects();
                }
            });
        });
    };

    $scope.RefreshActivityIntrestedHouse = function () {

        GetActivityInterestedHouses();
    };

    $scope.RefreshActivityIntrestedProjects = function () {

        GetActivityInterestedProjects();
    };

    $scope.OpenQuotePage = function (quoteId) {
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.CheckSaleCustomerIdentityAndAddress(quoteId, function (e) {

                if (e.Success == true) {
                    parent.IndexHelper.ShowDialog("<iframe src='editsale.html?quoteid=" + quoteId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-flag'></i> Satış Kaydı", "modal90");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Bilgileri");
                }
            });


        }
    }

    $scope.OpenCustomerPage = function () {
        window.open(GlobalHelper.CrmUrl + "main.aspx?etc=2&extraqs=&histKey=188304249&id=%7b" + AccountDetailHelper.ContactId + "%7d&newWindow=true&pagetype=entityrecord#179741527");
    }

    $scope.OpenActivityPage = function () {
        if (AccountDetailHelper.PhoneCallId != null) {
            window.open(GlobalHelper.CrmUrl + "main.aspx?etc=4210&extraqs=&histKey=703926701&id=%7b" + AccountDetailHelper.PhoneCallId + "%7d&newWindow=true&pagetype=entityrecord#73304780");
        }
        else {
            window.open(GlobalHelper.CrmUrl + "main.aspx?etc=4201&extraqs=&histKey=943118896&id=%7b" + AccountDetailHelper.AppointmentId + "%7d&newWindow=true&pagetype=entityrecord#632357727");
        }
    }

    $scope.OpenActivityCreatePage = function () {
        //Kemal Burak YILMAZ
        //08.04.2015
        //Erkan ÖZVAR bu kontrolün kaldırılmasını istedi.
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?accountid=" + AccountDetailHelper.AccountId + "&name=" + encodeURI(AccountDetailHelper.AccountName) + "' style='width:100%;height:450px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
        }
        else {
            AccountDetailHelper.CreateActivityForCallCenter(AccountDetailHelper.AccountId, function (createResult) {
                if (createResult.Success) {
                    $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + AccountDetailHelper.AccountId + "&phonecallid=" + createResult.CrmId).css("height", "3500px");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, createResult.Result, "Aktivite Oluşturma");
                }
            });
        }

        //parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?accountid=" + AccountDetailHelper.AccountId + "&name=" + encodeURI(AccountDetailHelper.AccountName) + "' style='width:100%;height:450px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
    }

    


    $scope.OpenAccountDetailPageWithActivity = function (activityId, activityType, stateCode) {
        if (stateCode == 0) {

            AccountDetailHelper.CheckActivityOwnership(parent.IndexHelper.UserId, activityId, function (e) {

                if (e.Success == true) {

                    if (activityType == "Randevu") {
                        $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + AccountDetailHelper.AccountId + "&appointmentid=" + activityId).css("height", "3500px");
                    }
                    else {
                        $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + AccountDetailHelper.AccountId + "&phonecallid=" + activityId).css("height", "3500px");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Aktivite sahibi olmadığınız için aktivite üzerinde işlem yapamazsınız!", "Aktivte Görüntüle");
                }
            });
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='editactivity.html?activityid=" + activityId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='fa fa-bomb'></i> Aktivite", "modal90");
        }

    }

    $scope.OpenOpportunityPage = function (opportunityId) {
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.ShowDialog("<iframe src='editopportunity.html?oppid=" + opportunityId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='fa fa-bomb'></i> Fırsat", "modal90");
        }
    }

    $scope.RefreshOpportunities = function () {
        GetCustomerOpportunities();
    }

    $scope.RefreshSales = function () {
        GetCustomerQuotes();
    }

    $scope.RefreshActivities = function () {
        GetCustomerActivities();
    }

    $scope.RefreshOpportunityHouses = function () {
        GetOpportunityHouses();
    }

    $scope.RemoveContact = function () {

        $scope.account.Contact = null;

        $("#txtContact").attr("crmid", null);
        $("#txtContact").val(null);
    }

    $scope.OpenContactSearchPage = function () {

        parent.IndexHelper.ShowDialog("<iframe src='contactsearch.html' style='width:100% !important; border:none;height:650px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Kişi Arama", "modal90");
    };

    function GetActivityInterestedHouses() {
        AccountDetailHelper.GetActivityInterestedHouse(function (e) {
            $scope.$apply(function () {
                $scope.activityProducts = e;

                $(window).resize(function () {
                    parent.IndexHelper.AutoResize("ifrmContent");
                });

                parent.IndexHelper.AutoResize("ifrmContent");
            });
        });
    }

    function GetActivityInterestedProjects() {
        AccountDetailHelper.GetActivityInterestedProjects(function (e) {
            $scope.$apply(function () {
                $scope.activityProjects = e;

                $(window).resize(function () {
                    parent.IndexHelper.AutoResize("ifrmContent");
                });

                parent.IndexHelper.AutoResize("ifrmContent");
            });
        });
    }

    function GetCustomerQuotes() {
        AccountDetailHelper.GetCustomerQuotes(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.quotes = e.ReturnObject;

                    $(window).resize(function () {
                        parent.IndexHelper.AutoResize("ifrmContent");
                    });

                    parent.IndexHelper.AutoResize("ifrmContent");
                }
                else {
                    //parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    }

    function GetCustomerOpportunities() {
        AccountDetailHelper.GetCustomerOpportunities(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.opportunities = e.ReturnObject;

                    $(window).resize(function () {
                        parent.IndexHelper.AutoResize("ifrmContent");
                    });

                    parent.IndexHelper.AutoResize("ifrmContent");
                }
                else {
                    //parent.IndexHelper.ToastrShow(false, e.Result, "Fırsat Bilgileri");
                }
            });
        });
    }

    function GetCustomerActivities() {
        AccountDetailHelper.GetCustomerActivities(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.activities = e.ReturnObject;

                    $(window).resize(function () {
                        parent.IndexHelper.AutoResize("ifrmContent");
                    });

                    parent.IndexHelper.AutoResize("ifrmContent");
                }
                else {
                    //parent.IndexHelper.ToastrShow(false, e.Result, "Aktivite Bilgileri");
                }
            });
        });
    }

    function GetOpportunityHouses() {
        AccountDetailHelper.GetCustomerOpportunityHouses(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.opportunityhouses = e.ReturnObject;

                    $(window).resize(function () {
                        parent.IndexHelper.AutoResize("ifrmContent");
                    });

                    parent.IndexHelper.AutoResize("ifrmContent");
                }
                else {
                    //parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    }

    $scope.AddActivityIntrestedHouseForOpportunity = function (productId, statusCode) {
        if (statusCode == 1) {
            var interestProduct = {};
            interestProduct.InterestedProduct = {};
            interestProduct.InterestedProduct.ProductId = productId;
            if (AccountDetailHelper.PhoneCallId != null) {
                interestProduct.PhoneCall = {};
                interestProduct.PhoneCall.Id = AccountDetailHelper.PhoneCallId;
                interestProduct.PhoneCall.LogicalName = "phonecall";
            }
            else {
                interestProduct.Appointment = {};
                interestProduct.Appointment.Id = AccountDetailHelper.AppointmentId;
                interestProduct.Appointment.LogicalName = "appointment";
            }

            AccountDetailHelper.AddActivityIntrestedHouse(interestProduct, function (e) {
                $scope.$apply(function () {
                    var result = e;
                    if (result.Success) {
                        parent.IndexHelper.ToastrShow(true, result.Result, "Konut Arama");

                        AccountDetailHelper.GetActivityInterestedHouse(function (e) {
                            $("#btnRefresh", $("#ifrmContent", parent.document)[0].contentWindow.document).click();

                            //$scope.$apply(function () {
                            // parent.$scope.activityProducts = e;
                            //$("#ifrmContent", parent.document)[0].contentWindow.$scope.activityProducts = e;
                            //});
                        });

                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, result.Result, "Konut Arama");
                    }
                });
            });
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Yalnızca durumu Boş olan konutları ekleyebilirsiniz!", "Konut Arama");
        }
    };
}]);