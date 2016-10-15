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
    "GetContactQuotes": function (callbackFunction) {
        var jData = {};
        jData.contactId = ContactDetailHelper.ContactId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactQuotes",
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
    "GetContactOpportunities": function (callbackFunction) {
        var jData = {};
        jData.contactId = ContactDetailHelper.ContactId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactOpportunities",
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
    "GetContactActivities": function (callbackFunction) {
        var jData = {};
        jData.contactId = ContactDetailHelper.ContactId;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactActivities",
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
    "GetActivityStatuses": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetActivityStatuses",
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
    "UpdateContact": function (contact, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.contact = contact;
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
    "CloseRelated": function (activity, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.activity = activity;
        jData.activity.ActivityParty = {};
        jData.activity.ActivityParty.LogicalName = "contact";
        jData.activity.ActivityParty.Name = ContactDetailHelper.ContactName;
        jData.activity.ActivityParty.Id = ContactDetailHelper.ContactId;

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
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "ContactId": "",
    "ContactName": "",
    "PhoneCallId": "",
    "AppointmentId": "",
    "RefreshActivityIntrestedHouse": function () {
        alert("ADD HOUSE RELATIONS & REFRESH ACTIVITY INTRESTED HOUSES");
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
    ContactDetailHelper.ContactId = url.param("contactid");
    ContactDetailHelper.PhoneCallId = url.data.param.query.hasOwnProperty("phonecallid") ? url.param("phonecallid") : null;
    ContactDetailHelper.AppointmentId = url.data.param.query.hasOwnProperty("appointmentid") ? url.param("appointmentid") : null;

    $scope.GuidEmpty = GlobalHelper.GuidEmpty;
    $scope.activityStates = GlobalHelper.ActivityStates;
    $scope.activityStateClass = GlobalHelper.ActivityStateClass;
    $scope.selectedHouse = [];

    ContactDetailHelper.GetParticipations(function (e) {
        $scope.$apply(function () {
            $scope.participations = e;
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

        ContactDetailHelper.GetSubParticipations($scope.participation, function (e) {

            $scope.$apply(function () {

                $scope.subParticipations = e;
            });
        });
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

                        if ($scope.contact.FullName != "" && $scope.contact.FullName != null) {
                            ContactDetailHelper.ContactName = $scope.contact.FullName;
                        }

                        GetContactQuotes();
                        GetContactOpportunities();
                        GetContactActivities();
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
        $scope.activity = {}
        $scope.hasActivity = true;
        

        if (ContactDetailHelper.PhoneCallId != null) {
            $scope.activity.ActivityId = ContactDetailHelper.PhoneCallId;
            $scope.hideSunum = false;
            $scope.activity.ObjectTypeCode = 4210;

            GetActivityInterestedHouses();
        }
        else {
            $scope.activity.ActivityId = ContactDetailHelper.AppointmentId;
            $scope.hideSunum = true;
            $scope.activity.ObjectTypeCode = 4201;
            $scope.activity.PresentationType = 100000001;

            ContactDetailHelper.GetSalesOffices(function (e) {
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
        }

        ContactDetailHelper.GetActivityStatuses(function (e) {
            $scope.$apply(function () {
                $scope.activityStatuses = e;
            });
        });

        $scope.activityStatusChange = function () {
            ContactDetailHelper.GetActivityStatusDetails($scope.activity.ActivityStatus.Id, function (e) {
                $scope.$apply(function () {
                    $scope.activityStatusDetails = e;
                });
            });
        }
    }
    else {
        $scope.showActivityCreate = true;
    }

    $scope.Save = function () {
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

    $scope.ActivityUpdate = function () {
        if (!$scope.activity.hasOwnProperty("ActivityStatus")) {
            $("#divStatus").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Lütfen görüşme sonucu alanını doldurunuz!", "Aktivite Bilgileri");
        }
        else if (!$scope.activity.hasOwnProperty("ActivityStatusDetail")) {
            $("#divStatusDetail").addClass("has-error");
            parent.IndexHelper.ToastrShow(false, "Lütfen görüşme sonuç ayrıntısı alanını doldurunuz!", "Aktivite Bilgileri");
        }
        else {
            if ($scope.activityProducts != null || parent.IndexHelper.IsCallCenterSearch) {
                var activityStatusCode = $("#slctActivityStatus option:selected").attr("code");
                if (activityStatusCode == 1) {
                    if (!parent.IndexHelper.IsCallCenterSearch) //Eğer bir call center araması ise satış oluşturulamaz
                    {
                        parent.bootbox.confirm("Satış kaydı oluşturmak ister misiniz?", function (result) {
                            ContactDetailHelper.CloseRelated($scope.activity, function (e) {
                                $scope.$apply(function () {
                                    if (e.Success) {
                                        parent.IndexHelper.ToastrShow(true, e.Result, "Müşteri Bilgileri");
                                        if (result == true) {
                                            var isSaleAble = ContactDetailHelper.IsSaleable();
                                            if (isSaleAble) {
                                                if (ContactDetailHelper.PhoneCallId != null) {
                                                    parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?contactid=" + ContactDetailHelper.ContactId + "&contactname=" + ContactDetailHelper.ContactName + "&phonecallid=" + ContactDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                }
                                                else {
                                                    parent.IndexHelper.ShowDialog("<iframe src='activityintrestedhouses.html?contactid=" + ContactDetailHelper.ContactId + "&contactname=" + ContactDetailHelper.ContactName + "&appointmentid=" + ContactDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-home'></i> İlgilendiği Konutlar", "modal90");
                                                }
                                            }
                                            else {
                                                parent.IndexHelper.ToastrShow(false, "Aktivite ilgili olarak kapatılmadığı için satış kaydı oluşturulmayacak!", "Müşteri Bilgileri");
                                            }
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
                        ContactDetailHelper.CloseIrrelevant($scope.activity);
                    }
                }
                else {
                    ContactDetailHelper.CloseIrrelevant($scope.activity);
                }
            }
            else {
                parent.IndexHelper.ToastrShow(false, "İlgili konut eklemeden görüşmeyi tamamlayamazsınız!", "Görüşme Tamamlama");
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
            parent.IndexHelper.ShowDialog("<iframe src='addhouse.html?phonecallid=" + ContactDetailHelper.PhoneCallId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Konut Arama", "modal90");
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='addhouse.html?appointmentid=" + ContactDetailHelper.AppointmentId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Konut Arama", "modal90");
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

    $scope.RefreshActivityIntrestedHouse = function () {

        GetActivityInterestedHouses();
    };

    $scope.OpenQuotePage = function (quoteId) {
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.ShowDialog("<iframe src='editsale.html?quoteid=" + quoteId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-flag'></i> Satış Kaydı", "modal90");
        }
    }

    $scope.OpenActivityCreatePage = function () {
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + ContactDetailHelper.ContactId + "&name=" + encodeURI(ContactDetailHelper.ContactName) + "' style='width:100%;height:350px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
        }
    }

    $scope.OpenContactDetailPageWithActivity = function (activityId, activityType) {
        if (activityType == "Randevu") {
            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId + "&appointmentid=" + activityId).css("height", "3500px");
        }
        else {
            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ContactDetailHelper.ContactId + "&phonecallid=" + activityId).css("height", "3500px");
        }

    }

    $scope.OpenOpportunityPage = function (opportunityId) {
        if (parent.IndexHelper.UserType != UserTypes.CallCenter) //Call Center kullanıcıları sadece bu sayfayı görebilirler
        {
            parent.IndexHelper.ShowDialog("<iframe src='editopportunity.html?oppid=" + opportunityId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='fa fa-bomb'></i> Fırsat", "modal90");
        }
    }

    $scope.RefreshOpportunities = function () {
        GetContactOpportunities();
    }

    $scope.RefreshSales = function () {
        GetContactQuotes();
    }

    $scope.RefreshActivities = function () {
        GetContactActivities();
    }

    function GetActivityInterestedHouses() {
        ContactDetailHelper.GetActivityInterestedHouse(function (e) {
            $scope.$apply(function () {
                $scope.activityProducts = e;
            });
        });
    }

    function GetContactQuotes() {
        ContactDetailHelper.GetContactQuotes(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.quotes = e.ReturnObject;
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    }

    function GetContactOpportunities() {
        ContactDetailHelper.GetContactOpportunities(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.opportunities = e.ReturnObject;
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Fırsat Bilgileri");
                }
            });
        });
    }

    function GetContactActivities() {
        ContactDetailHelper.GetContactActivities(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.activities = e.ReturnObject;
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Aktivite Bilgileri");
                }
            });
        });
    }
}]);