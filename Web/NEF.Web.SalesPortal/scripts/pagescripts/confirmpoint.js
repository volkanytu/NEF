/// <reference path="global.js" />
/// <reference path="index.js" />


var ConfirmPointHelper = {
    "OnLoad": function () {

        ConfirmPointHelper.OnClickEvents();
        ConfirmPointHelper.OnChangeEvents();
        ConfirmPointHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");

    },
    "GetPointDetail": function (callbackFunction) {

        var jData = {};
        jData.loyaltyPointId = ConfirmPointHelper.PointId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetLoyaltyPoint",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Puan bilgileri getiriliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($(".page-content"));
            },
            success: function (data) {
                debugger;
                if (data != null) {
                    if (data.Success == true)
                        callbackFunction(data);
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Puan Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }

                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Puan Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Puan Bilgileri");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "SendToConfirm": function (callbackfunction) {

        var jData = {};
        jData.loyaltyPointId = ConfirmPointHelper.PointId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/ConfirmPointUsage",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#main"), iconOnly: false, boxed: true, message: "Puan onaylanıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#main"));
            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Puan Bilgileri");
            }
        });
    },
    "SendToRefuse": function (callbackfunction) {

        var jData = {};
        jData.loyaltyPointId = ConfirmPointHelper.PointId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/RefusePointUsage",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#main"), iconOnly: false, boxed: true, message: "Puan reddediliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#main"));
            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Puan Bilgileri");
            }
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
    "PointId": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    ConfirmPointHelper.PointId = url.param("pointid");

    $scope.IsDisabled = false;

    ConfirmPointHelper.GetPointDetail(function (e) {
        $scope.$apply(function () {
            $scope.point = e.ReturnObject;
        });

    });

    $scope.SendToConfirm = function () {
        ConfirmPointHelper.SendToConfirm(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.IsDisabled = true;
                    parent.IndexHelper.ToastrShow(true, e.Result, "Onay İşlemleri");
                    parent.window.close();
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Onay İşlemleri");
                }
            });
        });
    };

    $scope.SendToRefuse = function () {
        ConfirmPointHelper.SendToRefuse(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.IsDisabled = true;
                    parent.IndexHelper.ToastrShow(true, e.Result, "Onay İşlemleri");
                    parent.window.close();
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Onay İşlemleri");
                }
            });
        });
    };
}
