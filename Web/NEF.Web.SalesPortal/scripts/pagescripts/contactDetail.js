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

var ContactDetailHelper = {
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

        ContactDetailHelper.OnClickEvents();
        ContactDetailHelper.OnChangeEvents();
        ContactDetailHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");
        //$("#txtMobilePhone").ForceNumericOnly();
        $("#txtTcNo").ForceNumericOnly();
        //$("#txtMobilePhone").mask('+90-###-#######');

        $(".form_datetime").datetimepicker({
            autoclose: true,
            isRTL: Metronic.isRTL(),
            format: "dd.mm.yyyy hh:ii",
            pickerPosition: (Metronic.isRTL() ? "bottom-right" : "bottom-left")
        });
        $("#txtTelephone").mask('+90-###-#######');
        $("#txtSecondyPersonTelephone").mask('+90-###-#######');
    },
    "GetDetail": function (id, callbackFunction) {

        var returnValue = null;
        var jData = {};
        jData.contactId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactDetail",
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
    "GetPointSummary": function (callbackFunction) {

        var returnValue = null;
        var jData = {};
        jData.contactId = ContactDetailHelper.ContactId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactPointSummary",
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
                debugger;
                if (data != null) {
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
        jData.customerId = ContactDetailHelper.ContactId;
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
        jData.customerId = ContactDetailHelper.ContactId;
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
        jData.customerId = ContactDetailHelper.ContactId;
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
        jData.customerId = ContactDetailHelper.ContactId;
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
    "GetParticipations": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetParticipations",
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
    "GetChannels": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetChannels",
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
    "GetNationalities": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetNationalities",
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
    "GetSubParticipations": function (id, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.participationId = id;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/GetSubParticipations",
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
                        //returnValue = data.ReturnObject;

                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Katılım Kaynağı");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Katılım Kaynağı");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });

        return returnValue;
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

        var methodname = ContactDetailHelper.PhoneCallId != null ? "/GetPhoneCallActivityInfo" : "/GetAppointmentActivityInfo";

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
    "UpdateContact": function (contact, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.contact = contact;
        jData.contact.MobilePhone = $("#txtMobilePhone").val();

        jData.contact.GrantUpdateUser = {};
        jData.contact.GrantUpdateUser.Id = parent.IndexHelper.UserId;
        jData.contact.GrantUpdateUser.LogicalName = "systemuser";

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateContact",
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
    "HideMarketingRef": function () {
        var selectedValue = $("#marketingPermission option:selected").attr("value");
        selectedValue = parseInt(selectedValue);
        if (selectedValue == 1) {
            return false;
        }
        else {
            return true;
        }
    },
    "CloseRelated": function (activity, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.activity = activity;
        jData.activity.ActivityParty = {};
        jData.activity.ActivityParty.LogicalName = "contact";
        jData.activity.ActivityParty.Name = ContactDetailHelper.ContactName;
        jData.activity.ActivityParty.Id = ContactDetailHelper.ContactId;

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
        jData.activity.ActivityParty.LogicalName = "contact";
        jData.activity.ActivityParty.Name = ContactDetailHelper.ContactName;
        jData.activity.ActivityParty.Id = ContactDetailHelper.ContactId;
     
        //var nextCallDate = $('#txtCallDate').val();

        //if (nextCallDate != null && nextCallDate != "" && nextCallDate != "undefined" && nextCallDate != undefined)
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
                    $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId).css("height", "1300px");
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
        jData.activityId = ContactDetailHelper.PhoneCallId != null ? ContactDetailHelper.PhoneCallId : ContactDetailHelper.AppointmentId;
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
        jData.activityId = ContactDetailHelper.PhoneCallId != null ? ContactDetailHelper.PhoneCallId : ContactDetailHelper.AppointmentId;
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
    "CreateActivityForCallCenter": function (customerId, callbackfunction) {
        var activity = {};
        activity.Direction = DirectionCodes.Giden;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "contact";
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
        jData.customerId = ContactDetailHelper.ContactId;
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
    "OnClickEvents": function () {
        var chk = $("#myCheckBox");
        chk.checked = true;
    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "ContactId": "",
    "ContactName": "",
    "PhoneCallId": "",
    "AppointmentId": "",
    "InvalidTcNo": false,
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
    },
    "testButton": function () {



    },
    "ChangeCountryFormat": function () {
        if ($("#countryFormatSelection option:selected").val() == "0") {
            $("#txtSecondyPersonTelephone").mask('+90-###-#######');
            $("#txtMobilePhone").mask('+90-###-#######');
            $("#txtTelephone").mask('+90-###-#######');
        } else {
            $("#txtSecondyPersonTelephone").mask('+##-###-#######');
            $("#txtMobilePhone").mask('+##-###-#######');
            $("#txtTelephone").mask('+##-###-#######');
        }
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

    $scope.contactTypeClass = GlobalHelper.ContactTypeClass;
    $scope.contactTypes = GlobalHelper.ContactTypes;


    $scope.IsHideRef = true;

    if (parent.IndexHelper.UserType == UserTypes.Resepsiyonist) {
        $scope.DisableContactInformation = true;
    }

    var url = $.url(document.location);
    ContactDetailHelper.ContactId = url.param("contactid");
    ContactDetailHelper.PhoneCallId = url.data.param.query.hasOwnProperty("phonecallid") ? url.param("phonecallid") : null;
    ContactDetailHelper.AppointmentId = url.data.param.query.hasOwnProperty("appointmentid") ? url.param("appointmentid") : null;

    if (parent.IndexHelper.UserType == UserTypes.MusteriIliskileri) {
        $scope.showMusteriIliskileri = true;
    }

    $scope.GuidEmpty = GlobalHelper.GuidEmpty;
    $scope.activityStates = GlobalHelper.ActivityStates;
    $scope.activityStateClass = GlobalHelper.ActivityStateClass;
    $scope.selectedHouse = [];
    $scope.isSubjectVisible = true;

    ContactDetailHelper.GetParticipations(function (e) {
        $scope.$apply(function () {
            $scope.participations = e;
        });
    });

    ContactDetailHelper.GetChannels(function (e) {
        $scope.$apply(function () {
            $scope.channels = e;
        });
    });

    ContactDetailHelper.GetCountries(function (e) {
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

    ContactDetailHelper.GetNationalities(function (e) {
        $scope.$apply(function () {
            $scope.nationalities = e;
            setTimeout(function () {
                $scope.$apply(function () {
                    $scope.nationality = $scope.nationalities[0].NationalityId; //Default olarak TC gelmesi sağlanır.
                });
            }, 500);
        });
    });

    $scope.changeParticipation = function () {
        //$scope.subParticipations = ContactDetailHelper.GetSubParticipations($scope.participation);

        if ($scope.participation == "e39f8ef5-18bf-e311-b9d9-00155d011c0a")
            $scope.IsHideRef = false;
        else
            $scope.IsHideRef = true;

        ContactDetailHelper.GetSubParticipations($scope.participation, function (e) {

            $scope.$apply(function () {

                $scope.subParticipations = e;
            });
        });
    }

    $scope.ChangeCountryFormat = function () {
        ContactDetailHelper.ChangeCountryFormat();
    }

    $scope.changeCountries = function () {
        $scope.city = $scope.GuidEmpty;
        $scope.cities = null;
        $scope.town = $scope.GuidEmpty;
        $scope.towns = null;
        $scope.district = $scope.GuidEmpty;
        $scope.districts = null;
        ContactDetailHelper.GetCities($scope.country, function (e) {
            $scope.$apply(function () {
                $scope.cities = e;
            });

        });
    }

    $scope.changeOverCountries = function () {
        $scope.overCities = null;
        $scope.overCity = $scope.GuidEmpty;

        ContactDetailHelper.GetCities($scope.overCountry, function (e) {
            $scope.$apply(function () {
                $scope.overCities = e;
            });

        });
    }

    $scope.changeCities = function () {
        $scope.town = $scope.GuidEmpty;
        $scope.towns = null;
        $scope.district = $scope.GuidEmpty;
        $scope.districts = null;
        ContactDetailHelper.GetTowns($scope.city, function (e) {
            $scope.$apply(function () {
                $scope.towns = e;
            });
        });
    }

    $scope.changeTowns = function () {
        $scope.district = $scope.GuidEmpty;
        $scope.districts = null;
        ContactDetailHelper.GetDistricts($scope.town, function (e) {
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


        ContactDetailHelper.GetActivityStatuses($scope.activity.ActivityTopic.Id, function (e) {
            $scope.$apply(function () {
                $scope.activityStatuses = e;
            });
        });
    }

    $scope.testButton = function () {
        ContactDetailHelper.testButton();
    }

    ContactDetailHelper.GetDetail(ContactDetailHelper.ContactId, function (e) {

        var results = e;
        if (results != null && results != false) {
            if (results.Success) {
                $scope.$apply(function () {
                    $scope.contact = results.ReturnObject;
                    if ($scope.contact != null) {

                        if ($scope.contact.ContactType == 100000001) //Gerçek Müşteri ise kişi edit edilemez.
                        {
                            $scope.isEditableContact = false;
                        }
                        else {
                            $scope.isEditableContact = true;
                        }

                        $(".page-title", parent.document).html($scope.contact.FullName);
                        if ($scope.contact.Nationality != null) {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    $scope.nationality = $scope.contact.Nationality.Id;
                                });
                            }, 500);
                        }

                        if ($scope.contact.Country != null) {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    $scope.country = $scope.contact.Country.Id;
                                });
                            }, 500);

                            ContactDetailHelper.GetCities($scope.contact.Country.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.cities = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.city = $scope.contact.City.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }
                        else //Eğer önceden set edilmemiş ise Default olarak İstanbul gelmesi sağlanır. Ardından İstanbul'a ait ilçeler doldurulur.
                        {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    ContactDetailHelper.GetCities($scope.country, function (e) {
                                        $scope.$apply(function () {
                                            $scope.cities = e;
                                            setTimeout(function () {
                                                $scope.$apply(function () {
                                                    $scope.city = $scope.cities[0].CityId;

                                                    //İstanbul'a ait ilçeler doldurulur.
                                                    ContactDetailHelper.GetTowns($scope.city, function (e) {
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

                        if ($scope.contact.City != null) {
                            ContactDetailHelper.GetTowns($scope.contact.City.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.towns = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.town = $scope.contact.Town.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }

                        if ($scope.contact.Town != null) {
                            ContactDetailHelper.GetDistricts($scope.contact.Town.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.districts = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.district = $scope.contact.District.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }

                        if ($scope.contact.OverCountry != null) {
                            setTimeout(function () {
                                $scope.$apply(function () {
                                    $scope.isYurtDisi = true;
                                    $scope.overCountry = $scope.contact.OverCountry.Id;
                                });
                            }, 500);

                            ContactDetailHelper.GetCities($scope.contact.OverCountry.Id, function (e) {
                                $scope.$apply(function () {
                                    $scope.overCities = e;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.overCity = $scope.contact.OverCity.Id;
                                        });
                                    }, 500);

                                });

                            });

                        }

                        if ($scope.contact.Participation != null) {
                            $scope.participation = $scope.contact.Participation.Id;

                            if ($scope.participation == "e39f8ef5-18bf-e311-b9d9-00155d011c0a")
                                $scope.IsHideRef = false;
                            else
                                $scope.IsHideRef = true;


                            ContactDetailHelper.GetSubParticipations($scope.participation, function (esp) {

                                $scope.$apply(function () {
                                    $scope.subParticipations = esp;
                                    setTimeout(function () {
                                        $scope.$apply(function () {
                                            $scope.subParticipation = $scope.contact.SubParticipation.Id;
                                        });
                                    }, 500);

                                });

                            });
                        }

                        if ($scope.contact.Channel != null) {
                            $scope.channel = $scope.contact.Channel.Id;
                        }

                        if ($scope.contact.FullName != "" && $scope.contact.FullName != null) {
                            ContactDetailHelper.ContactName = $scope.contact.FullName;
                        }

                        $("#chkIsVip").prop("checked", $scope.contact.IsVip);
                        $("#chkHasCreditProblem").prop("checked", $scope.contact.HasCreditProblem);
                        $("#chkHasPaymentProblem").prop("checked", $scope.contact.IsVipHasPaymentProblem);
                        $("#chkIsBlackList").prop("checked", $scope.contact.IsBlackList);

                        if ($scope.contact.MarketingGrantValue == true) {

                            $scope.divMarketingPermission = false;
                        } else {
                            $scope.divMarketingPermission = true;
                        }

                        if ($scope.contact.sendSMS === true) {
                            $("#sendSmsSelection").prop("checked", $scope.contact.sendSMS);
                        }

                        GetCustomerPointSummary();

                        GetCustomerQuotes();
                        GetCustomerOpportunities();
                        GetCustomerActivities();

                        GetOpportunityHouses();

                        setInterval(function () {
                            GetCustomerQuotes();
                            GetCustomerOpportunities();
                            GetCustomerActivities();
                            GetTeamMembers();
                        }, 60000);

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

    if (ContactDetailHelper.PhoneCallId != null || ContactDetailHelper.AppointmentId != null) {

        ContactDetailHelper.GetActivityTopics(function (e) {
            $scope.$apply(function () {
                $scope.activityTopics = e;
            });
        });

        ContactDetailHelper.GetTeamMembers(function (e) {
            $scope.$apply(function () {
                if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase()) {
                    $scope.teamMembers = e;
                    $scope.isRetailer = true;
                }
            });
        });

        if (parent.IndexHelper.IsCallCenterSearch)
            $scope.showActivityCreate = false;

        $scope.activity = {}
        $scope.hasActivity = true;


        if (ContactDetailHelper.PhoneCallId != null) {
            $scope.activity.ActivityId = ContactDetailHelper.PhoneCallId;
            $scope.hideSunum = false;



            $scope.activity.ObjectTypeCode = 4210;

            GetActivityInterestedHouses();
            GetActivityInterestedProjects();

            if (parent.IndexHelper.IsCallCenterSearch) {
                $scope.isCallCenter = true;
                ContactDetailHelper.GetCallCenterAgents(function (e) {
                    $scope.$apply(function () {
                        $scope.callcenteragents = e;
                    });
                });
            }

        }
        else {
            $scope.activity.ActivityId = ContactDetailHelper.AppointmentId;
            $scope.hideSunum = true;


            $scope.activity.ObjectTypeCode = 4201;
            $scope.activity.PresentationType = 100000001;

            ContactDetailHelper.GetSalesOffices(function (e) {
                $scope.$apply(function () {
                    $scope.salesOffices = e;

                    var ipStart = parent.IndexHelper.IpAddress.substring(0, 9);
                    var officeId = null;
                    for (var i = 0; i < $scope.salesOffices.length; i++) {
                        if ($scope.salesOffices[i].IpAddress.substring(0, 9) == ipStart) {
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



        //ContactDetailHelper.GetActivityStatuses(function (e) {
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
                ContactDetailHelper.CheckCustomerApartmentOwner(function (a) {
                    $scope.$apply(function () {
                        if (!a.Success) {
                            parent.IndexHelper.ToastrShow(false, a.Result, "Müşteri Bilgileri");
                        }
                    });
                });
            }

            ContactDetailHelper.GetActivityStatusDetails($scope.activity.ActivityStatus.Id, function (e) {
                $scope.$apply(function () {
                    $scope.activityStatusDetails = e;
                });
            });
        }

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

        ContactDetailHelper.GetActivityInfo($scope.activity.ActivityId, function (e) {
            $scope.$apply(function () {
                if (e.ReturnObject.ActivityTopic != null) {
                    $scope.activity.ActivityTopic = {};
                    $scope.activity.ActivityTopic.Id = e.ReturnObject.ActivityTopic.Id;


                    setTimeout(function () {
                        $scope.$apply(function () {
                            ContactDetailHelper.GetActivityStatuses($scope.activity.ActivityTopic.Id, function (e) {
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

                                                        ContactDetailHelper.GetActivityStatusDetails($scope.activity.ActivityStatus.Id, function (a) {
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
                if (e.ReturnObject.Retailer != null) {
                    $scope.activity.Retailer = {};
                    $scope.activity.Retailer.Id = e.ReturnObject.Retailer.Id;
                }

                $scope.activity.Note = e.ReturnObject.Note;
            });
        });
    }
    else {
        $scope.showActivityCreate = true;
    }

    $scope.Save = function () {

        if (ContactDetailHelper.InvalidTcNo == true) {
            parent.IndexHelper.ToastrShow(false, "Hatalı kimlik numarası. Kontrol edip tekrar deneyiniz.", "Müşteri Güncelle");

            return;
        }

        var contactId = $("#txtContact").attr("crmid");

        if (contactId != null && contactId != undefined && contactId != "") {

            if ($scope.contact.RefContact == null)
                $scope.contact.RefContact = {};

            $scope.contact.RefContact.Id = $("#txtContact").attr("crmid");
            $scope.contact.RefContact.Name = $("#txtContact").val();
            $scope.contact.RefContact.LogicalName = "contact";
        }

        if ($scope.contact.Participation == null) {
            $scope.contact.Participation = {};
            $scope.contact.Participation.LogicalName = 'new_sourceofparticipation';
            $scope.contact.Participation.Id = $scope.participation;
        }
        else {
            if ($scope.participation != undefined) {
                $scope.contact.Participation.Id = $scope.participation;
            }
        }

        if ($scope.contact.SubParticipation == null) {
            $scope.contact.SubParticipation = {};
            $scope.contact.SubParticipation.LogicalName = 'new_subsourceofparticipation';
            $scope.contact.SubParticipation.Id = $scope.subParticipation;;
        }
        else {
            if ($scope.subParticipation != undefined) {
                $scope.contact.SubParticipation.Id = $scope.subParticipation;
            }
        }

        if ($scope.contact.Channel == null) {
            $scope.contact.Channel = {};
            $scope.contact.Channel.LogicalName = 'new_channelofawareness';
            $scope.contact.Channel.Id = $scope.channel;
        }
        else {
            if ($scope.channel != undefined) {
                $scope.contact.Channel.Id = $scope.channel;
            }
        }

        if ($scope.contact.Nationality == null) {
            $scope.contact.Nationality = {};
            $scope.contact.Nationality.Id = $scope.nationality;
        }
        else {
            if ($scope.nationality == $scope.GuidEmpty) {
                $scope.contact.Nationality = null;
            }
            else if ($scope.nationality != undefined) {
                $scope.contact.Nationality.Id = $scope.nationality;
            }
        }

        if ($scope.contact.Country == null) {
            $scope.contact.Country = {};
            $scope.contact.Country.Id = $scope.country;
        }
        else {
            if ($scope.country == $scope.GuidEmpty) {
                $scope.contact.Country = null;
            }
            else if ($scope.country != undefined) {
                $scope.contact.Country.Id = $scope.country;
            }
        }

        if ($scope.contact.City == null) {
            $scope.contact.City = {};
            $scope.contact.City.Id = $scope.city;
        }
        else {
            if ($scope.city == $scope.GuidEmpty) {
                $scope.contact.City = null;
            }
            else if ($scope.city != undefined) {
                $scope.contact.City.Id = $scope.city;
            }
        }

        if ($scope.contact.Town == null) {
            $scope.contact.Town = {};
            $scope.contact.Town.Id = $scope.town;
        }
        else {
            if ($scope.town == $scope.GuidEmpty) {
                $scope.contact.Town = null;
            }
            else if ($scope.town != undefined) {
                $scope.contact.Town.Id = $scope.town;
            }
        }

        if ($scope.contact.District == null) {
            $scope.contact.District = {};
            $scope.contact.District.Id = $scope.district;
        }
        else {
            if ($scope.district == $scope.GuidEmpty) {
                $scope.contact.District = null;
            }
            else if ($scope.district != undefined) {
                $scope.contact.District.Id = $scope.district;
            }
        }

        if ($scope.contact.OverCountry == null) {
            $scope.contact.OverCountry = {};
            $scope.contact.OverCountry.Id = $scope.overCountry;
        }
        else {
            if ($scope.overCountry == $scope.GuidEmpty) {
                $scope.contact.OverCountry = null;
            }
            else if ($scope.overCountry != undefined) {
                $scope.contact.OverCountry.Id = $scope.overCountry;
            }
        }

        if ($scope.contact.OverCity == null) {
            $scope.contact.OverCity = {};
            $scope.contact.OverCity.Id = $scope.overCity;
        }
        else {
            if ($scope.overCity == $scope.GuidEmpty) {
                $scope.contact.OverCity = null;
            }
            else if ($scope.overCity != undefined) {
                $scope.contact.OverCity.Id = $scope.overCity;
            }
        }
        var isChoosen = false;
        var AcceptMarketing = false;
        if ($scope.contact.MarketingGrantValue == 0) {
            $scope.contact.sendSMS = false;
            $scope.contact.sendFax = false;
            $scope.contact.sendEmail = false;
            $scope.contact.sendMail = false;
            $scope.contact.contactTelephone = false;
        } else {
            AcceptMarketing = true;
            if ($scope.contact.sendSMS == "true" || $scope.contact.sendSMS == true) {
                isChoosen = true;
            }
            if ($scope.contact.sendFax == "true" || $scope.contact.sendFax == true) {
                isChoosen = true;
            }
            if ($scope.contact.sendEmail == "true" || $scope.contact.sendEmail == true) {
                isChoosen = true;
            }
            if ($scope.contact.sendMail == "true" || $scope.contact.sendMail == true) {
                isChoosen = true;
            }
            if ($scope.contact.contactTelephone == "true" || $scope.contact.contactTelephone == true) {
                isChoosen = true;
            }

        }
        if (isChoosen != true && AcceptMarketing == true) {
            parent.IndexHelper.ToastrShow(false, "İzinli pazarlama seçeneklerinden birini seçiniz.", "Müşteri Bilgileri");
        } else {
            $scope.contact.SecondryPersonName = $("#txtSecondyPersonName").val();
            $scope.contact.SecondryPersonLastName = $("#txtSecondyPersonLastName").val();
            $scope.contact.SecondryPersonPhone = $("#txtSecondyPersonTelephone").val();

            ContactDetailHelper.UpdateContact($scope.contact, function (e) {
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

    }

    $scope.ActivityUpdate = function () {

        var noteLength = $("#note").val().length;

        if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase() && $("#slctAlternative option:selected").text() == "") {
            $("#alternatif").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Alternatif kişisini seçin!", "Aktivite Bilgileri");
        } else if (!$scope.activity.hasOwnProperty("ActivityStatus")) {
            $("#divStatus").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Lütfen görüşme sonucu alanını doldurunuz!", "Aktivite Bilgileri");
        }
        else if (!$scope.activity.hasOwnProperty("ActivityStatusDetail")) {
            $("#divStatusDetail").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Lütfen görüşme sonuç ayrıntısı alanını doldurunuz!", "Aktivite Bilgileri");
        } else if (noteLength < 49) {
            parent.IndexHelper.ToastrShow(false, "Nota en az 50 karakter giriniz!", "Aktivite Bilgileri");
        }
        else {
            var activityTopicCode = $("#slctActivityTopics option:selected").attr("code");
            activityTopicCode = parseInt(activityTopicCode);
            //if ($scope.activityProducts != null || parent.IndexHelper.IsCallCenterSearch || activityTopicCode == 7) {
            var activityStatusCode = $("#slctActivityStatus option:selected").attr("code");
            var activityStatusText = $("#slctActivityStatus option:selected").text();
            var requiredForHome = ($("#slctActivityStatus option:selected").attr("requiredForHome").toLowerCase() === 'true');

            if (parent.IndexHelper.IsCallCenterSearch) {
                $scope.activity.CallCenterAgent = $scope.CallCenterAgent;
            }
            if (activityStatusCode == 1) {
                if (!parent.IndexHelper.IsCallCenterSearch || (parent.IndexHelper.UserType != UserTypes.Resepsiyonist)) //Eğer bir call center araması ise satış oluşturulamaz
                {
                    if (requiredForHome) {
                        if ($scope.activityProducts != null) {
                            parent.bootbox.confirm("Satış kaydı oluşturmak ister misiniz?", function (result) {
                                ContactDetailHelper.CloseRelated($scope.activity, function (e) {
                                    $scope.$apply(function () {
                                        if (e.Success) {
                                            parent.IndexHelper.ToastrShow(true, e.Result, "Müşteri Bilgileri");
                                            if (result == true) {
                                                var isSaleAble = ContactDetailHelper.IsSaleable();
                                                if (isSaleAble) {
                                                    if (ContactDetailHelper.PhoneCallId != null) {
                                                        parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + ContactDetailHelper.ContactName + "&phonecallid=" + ContactDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                    }
                                                    else {
                                                        parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + ContactDetailHelper.ContactName + "&appointmentid=" + ContactDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                    }
                                                    $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId).css("height", "1300px");
                                                }
                                                else {
                                                    parent.IndexHelper.ToastrShow(false, "Aktivite ilgili olarak kapatılmadığı için satış kaydı oluşturulmayacak!", "Müşteri Bilgileri");
                                                }
                                            }
                                            else {
                                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId).css("height", "1300px");
                                            }
                                        }
                                        else {
                                            parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Bilgileri");
                                        }
                                    });
                                });
                            });
                        } else {

                            parent.IndexHelper.ToastrShow(false, "İlgili konut eklemeden görüşmeyi tamamlayamazsınız!", "Görüşme Tamamlama");
                        }
                    } else {
                        parent.bootbox.confirm("Satış kaydı oluşturmak ister misiniz?", function (result) {
                            ContactDetailHelper.CloseRelated($scope.activity, function (e) {
                                $scope.$apply(function () {
                                    if (e.Success) {
                                        parent.IndexHelper.ToastrShow(true, e.Result, "Müşteri Bilgileri");
                                        if (result == true) {
                                            var isSaleAble = ContactDetailHelper.IsSaleable();
                                            if (isSaleAble) {
                                                if (ContactDetailHelper.PhoneCallId != null) {
                                                    parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + ContactDetailHelper.ContactName + "&phonecallid=" + ContactDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                }
                                                else {
                                                    parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + ContactDetailHelper.ContactName + "&appointmentid=" + ContactDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                }
                                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId).css("height", "1300px");
                                            }
                                            else {
                                                parent.IndexHelper.ToastrShow(false, "Aktivite ilgili olarak kapatılmadığı için satış kaydı oluşturulmayacak!", "Müşteri Bilgileri");
                                            }
                                        }
                                        else {
                                            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId).css("height", "1300px");
                                        }
                                    }
                                    else {
                                        parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Bilgileri");
                                    }
                                });
                            });
                        });
                    }
                }
                else {
                    ContactDetailHelper.CloseIrrelevant($scope.activity);
                }
            }
            else {
                if (requiredForHome) {
                    if ($scope.activityProducts != null) {
                        ContactDetailHelper.CloseIrrelevant($scope.activity);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, "İlgili konut eklemeden görüşmeyi tamamlayamazsınız!", "Görüşme Tamamlama");
                    }
                } else {
                    ContactDetailHelper.CloseIrrelevant($scope.activity);
                }
            }

        }
    }


    $scope.ParticipationsRepeatCompelte = function (index) {
        // alert("Test");
        //NG-REAPAT BİTTİĞİNDE TETİKLENECEK CALL BACK FUNCTION
    };

    $scope.CitiesRepeatCompelte = function (index) {
        $scope.contact.City.Id = $scope.contact.City.Id;
        //NG-REAPAT BİTTİĞİNDE TETİKLENECEK CALL BACK FUNCTION
    };

    $scope.AddActivityIntrestedHouse = function () {
        if (ContactDetailHelper.PhoneCallId != null) {
            parent.IndexHelper.ShowDialog("<iframe src='addhouse.html?phonecallid=" + ContactDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Konut Arama", "modal90");
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='addhouse.html?appointmentid=" + ContactDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Konut Arama", "modal90");
        }
    };

    $scope.AddActivityIntrestedProject = function () {
        if (ContactDetailHelper.PhoneCallId != null) {
            parent.IndexHelper.ShowDialog("<iframe src='addinterestedprojects.html?contactid=" + ContactDetailHelper.ContactId + "&phonecallid=" + ContactDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Proje", "modal90");
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='addinterestedprojects.html?contactid=" + ContactDetailHelper.ContactId + "&appointmentid=" + ContactDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:550px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Proje", "modal90");
        }
    };

    $scope.RemoveActivityIntrestedHouse = function () {
        ContactDetailHelper.RemoveActivityIntrestedHouse(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    GetActivityInterestedHouses();
                }
            });
        });
    };

    $scope.RemoveActivityIntrestedProject = function () {
        ContactDetailHelper.RemoveActivityIntrestedProject(function (e) {
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
        window.open(GlobalHelper.CrmUrl + "main.aspx?etc=2&extraqs=&histKey=188304249&id=%7b" + ContactDetailHelper.ContactId + "%7d&newWindow=true&pagetype=entityrecord#179741527");
    }

    $scope.OpenActivityPage = function () {
        if (ContactDetailHelper.PhoneCallId != null) {
            window.open(GlobalHelper.CrmUrl + "main.aspx?etc=4210&extraqs=&histKey=703926701&id=%7b" + ContactDetailHelper.PhoneCallId + "%7d&newWindow=true&pagetype=entityrecord#73304780");
        }
        else {
            window.open(GlobalHelper.CrmUrl + "main.aspx?etc=4201&extraqs=&histKey=943118896&id=%7b" + ContactDetailHelper.AppointmentId + "%7d&newWindow=true&pagetype=entityrecord#632357727");
        }
    }

    $scope.OpenActivityCreatePage = function () {
        //Kemal Burak YILMAZ
        //08.04.2015
        //Erkan ÖZVAR bu kontrolün kaldırılmasını istedi.
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + encodeURI(ContactDetailHelper.ContactName) + "' style='width:100%;height:450px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
        }
        else {
            ContactDetailHelper.CreateActivityForCallCenter(ContactDetailHelper.ContactId, function (createResult) {
                if (createResult.Success) {
                    $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactId.AccountId + "&phonecallid=" + createResult.CrmId).css("height", "3500px");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, createResult.Result, "Aktivite Oluşturma");
                }
            });
        }

        //parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + encodeURI(ContactDetailHelper.ContactName) + "' style='width:100%;height:450px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
    }

    $scope.OpenContactDetailPageWithActivity = function (activityId, activityType, stateCode) {
        if (stateCode == 0) {

            ContactDetailHelper.CheckActivityOwnership(parent.IndexHelper.UserId, activityId, function (e) {

                if (e.Success == true) {

                    if (activityType == "Randevu") {
                        $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId + "&appointmentid=" + activityId).css("height", "3500px");
                    }
                    else {
                        $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId + "&phonecallid=" + activityId).css("height", "3500px");
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

    $scope.RefreshOpportunityHouses = function () {
        GetOpportunityHouses();
    }

    $scope.RefreshActivities = function () {
        GetCustomerActivities();
    }

    $scope.HideMarketingDiv = function () {
        $scope.divMarketingPermission = ContactDetailHelper.HideMarketingRef();
    }

    $scope.checkTcNo = function ($keyEvent) {

        if ($("#nationalityDrop  option:selected").text() == "TC") {
            var tcNo = $("#txtTcNo").val();

            var result = GlobalHelper.CheckTcNumber(tcNo);

            if (result == true) {
                $("#fgTc").removeClass("has-error").addClass("has-success");
                ContactDetailHelper.InvalidTcNo = false;
            }
            else {
                $("#fgTc").removeClass("has-success").addClass("has-error");
                ContactDetailHelper.InvalidTcNo = true;
            }
        }
    };

    $scope.RemoveContact = function () {

        $scope.contact.RefContact = null;

        $("#txtContact").attr("crmid", null);
        $("#txtContact").val(null);
    }

    $scope.OpenContactSearchPage = function () {

        parent.IndexHelper.ShowDialog("<iframe src='contactsearch.html' style='width:100% !important; border:none;height:650px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Kişi Arama", "modal90");
    };

    function GetActivityInterestedHouses() {
        ContactDetailHelper.GetActivityInterestedHouse(function (e) {
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
        ContactDetailHelper.GetActivityInterestedProjects(function (e) {
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
        ContactDetailHelper.GetCustomerQuotes(function (e) {
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

    function GetCustomerPointSummary() {
        ContactDetailHelper.GetPointSummary(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.PointSummary = e.ReturnObject;
                }
                else {
                    //parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    }


    function GetCustomerOpportunities() {
        ContactDetailHelper.GetCustomerOpportunities(function (e) {
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
        ContactDetailHelper.GetCustomerActivities(function (e) {
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
        ContactDetailHelper.GetCustomerOpportunityHouses(function (e) {
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
            if (ContactDetailHelper.PhoneCallId != null) {
                interestProduct.PhoneCall = {};
                interestProduct.PhoneCall.Id = ContactDetailHelper.PhoneCallId;
                interestProduct.PhoneCall.LogicalName = "phonecall";
            }
            else {
                interestProduct.Appointment = {};
                interestProduct.Appointment.Id = ContactDetailHelper.AppointmentId;
                interestProduct.Appointment.LogicalName = "appointment";
            }

            ContactDetailHelper.AddActivityIntrestedHouse(interestProduct, function (e) {
                $scope.$apply(function () {
                    var result = e;
                    if (result.Success) {
                        parent.IndexHelper.ToastrShow(true, result.Result, "Konut Arama");

                        ContactDetailHelper.GetActivityInterestedHouse(function (e) {
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

    $scope.EditHouse = function (productId) {

        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };
}]);