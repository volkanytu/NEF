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
    "GetQuoteDetail": function (callbackFunction) {

        var jData = {};
        jData.quoteId = ConfirmHelper.QuoteId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetQuoteDetail",
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Satış Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Satış Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Bilgileri");
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
        jData.quoteId = ConfirmHelper.QuoteId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/ConfirmQuote",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#main"), iconOnly: false, boxed: true, message: "Satış onaylanıyor..." });
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
    "SendToRefuse": function (callbackfunction) {

        var jData = {};
        jData.quoteId = ConfirmHelper.QuoteId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/RefuseQuote",
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
    "GetPrePayment": function (callbackFunction) {

        var jData = {};
        jData.quoteId = ConfirmHelper.QuoteId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetQuotePrePayment",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyPrePayment"), iconOnly: false, boxed: true, message: "Kapora bilgileri alınıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyPrePayment"));
            },
            success: function (data) {

                data = JSON.parse(data);

                callbackFunction(data);

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kapora Bilgileri");
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
    "QuoteId": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    ConfirmHelper.QuoteId = url.param("quoteid");

    ConfirmHelper.GetQuoteDetail(function (e) {
        $scope.$apply(function () {
            $scope.quote = e.ReturnObject;
            $scope.quoteProduct = $scope.quote.Products[0];
            $scope.floorNumber = $scope.quoteProduct.FloorNumber != null ? $scope.quoteProduct.FloorNumber : "";
            $scope.m2Price = $scope.quote.HouseSalePrice != null && $scope.quoteProduct.Brut != null ? ($scope.quote.HouseSalePrice / $scope.quoteProduct.Brut) : "";

            $scope.quote.PaymentPlan = $scope.quote.PaymentPlan ? 1 : 0;

            ConfirmHelper.GetPrePayment(function (e) {
                $scope.$apply(function () {
                    if (e.Success == true) {
                        $scope.prePayment = e.ReturnObject[0];
                        $scope.hasPrePayment = false;

                        var dateSecond = $scope.prePayment.PaymentDate.replace("/Date(", "").replace(")/", "");
                        var date = new Date(parseInt(dateSecond));
                        $('#txtPaymentDate').data("datepicker").setDate(date);
                    }
                    else {

                        $scope.hasPrePayment = true;
                    }
                });
            });
        });

    });

    $scope.SendToConfirm = function () {
        ConfirmHelper.SendToConfirm(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    alert(e.Result);
                    parent.window.close();
                }
                else {
                    alert(e.Result);
                }
            });
        });
    };

    $scope.SendToRefuse = function () {
        ConfirmHelper.SendToRefuse(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    alert(e.Result);
                    parent.window.close();
                }
                else {
                    alert(e.Result);
                }
            });
        });
    };
}
