/// <reference path="global.js" />
/// <reference path="index.js" />


var ConfirmHelper = {
    "OnLoad": function () {

        ConfirmHelper.OnClickEvents();
        ConfirmHelper.OnChangeEvents();
        ConfirmHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");

    },
    "GetRentalDetail": function (callbackFunction) {

        var jData = {};
        jData.rentalid = ConfirmHelper.RentalId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetRentalDetail",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Satış bilgileri getiriliyor..." });
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Kiralama Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Kiralama Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });
    },
    "GetProductInfo": function (productId, callbackFunction) {

        var jData = {};
        jData.productId = productId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProductInfo",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Ürün bilgileri getiriliyor..." });
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Ürün Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Ürün Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Ürün Bilgileri");
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
        jData.userId = parent.IndexHelper.UserId;
        jData.rentalId = ConfirmHelper.RentalId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/ConfirmRental",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#main"), iconOnly: false, boxed: true, message: "Kiralama onaylanıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#main"));
            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });
    },
    "SendToRefuse": function (callbackfunction) {
        var jData = {};
        jData.userId = parent.IndexHelper.UserId;
        jData.rentalId = ConfirmHelper.RentalId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/RefuseRental",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#main"), iconOnly: false, boxed: true, message: "Satış reddediliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#main"));
            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Bilgileri");
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
    "RentalId": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    ConfirmHelper.RentalId = url.param("rentalid");

    $scope.IsDisabled = false;
    $scope.approvalTypes = GlobalHelper.ApprovalTypes;

    ConfirmHelper.GetRentalDetail(function (e) {
        $scope.$apply(function () {
            $scope.rental = e.ReturnObject;
            ConfirmHelper.GetProductInfo($scope.rental.Product.Id, function (e) {
                $scope.$apply(function () {
                    $scope.rentalProduct = e.ReturnObject;
                });
            });
        });
    });

    $scope.SendToConfirm = function () {
        ConfirmHelper.SendToConfirm(function (e) {
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
        ConfirmHelper.SendToRefuse(function (e) {
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
