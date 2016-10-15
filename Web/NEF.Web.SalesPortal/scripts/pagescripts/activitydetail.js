/// <reference path="global.js" />
/// <reference path="index.js" />


var ActivityDetailHelper = {
    "OnLoad": function () {

        ActivityDetailHelper.OnClickEvents();
        ActivityDetailHelper.OnChangeEvents();
        ActivityDetailHelper.OnChangeKeyPressEvents();


    },
    "GetActivityInfo": function (callbackFunction, activityId) {

        var jData = {};

        jData.activityId = activityId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetActivityInfo",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $(".page-content"), iconOnly: false, boxed: true, message: "Aktivite bilgileri çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($(".page-content"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);

                    if (data.Success == true)
                        callbackFunction(data);
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite");
            }
        });
    },
    "GetActivityInterestedHouse": function (callbackFunction) {
        var jData = {};
        jData.activityId = ActivityDetailHelper.ActivityId;
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
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "AppointmentId": "",
    "PhoneCallId": "",
    "ActivityId": ""
}

function mainController($scope) {

    $scope.hasProducts = false;

    var url = $.url(document.location);
    ActivityDetailHelper.ActivityId = url.data.param.query.hasOwnProperty("activityid") ? url.param("activityid") : null;

    ActivityDetailHelper.GetActivityInfo(function (e) {

        $scope.$apply(function () {

            $scope.Activity = e.ReturnObject;

            if (e.ReturnObject.ActivityType == 'Randevu') {
                $scope.IsAppointment = true;
            }

        });

    }, ActivityDetailHelper.ActivityId);

    ActivityDetailHelper.GetActivityInterestedHouse(function (e) {
        $scope.$apply(function () {
            $scope.activityProducts = e;
        });
    });

    $scope.EditHouse = function (productId) {
        //parent.IndexHelper.CloseDialog();
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };
}
