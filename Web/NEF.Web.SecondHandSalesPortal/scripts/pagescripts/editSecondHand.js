/// <reference path="global.js" />
/// <reference path="index.js" />


var SecondHandHelper = {
    "OnLoad": function () {
        SecondHandHelper.OnClickEvents();
        SecondHandHelper.OnChangeEvents();
        SecondHandHelper.OnChangeKeyPressEvents();

        $("#txtPrePaymentAmount").mask('000,000,000,000', { reverse: true, });
        $("#SecondHandAmountTxt").mask('000,000,000,000', { reverse: true });

        if (jQuery().datepicker) {
            $('.date-picker').datepicker({
                isRtl: false,
                orientation: "left",
                autoclose: true,
                language: "tr"
            });
        }

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ShowContent");
        });

        parent.IndexHelper.AutoResize("ShowContent");


    },
    "GetSecondHandDetail": function (callbackFunction) {

        var jData = {};
        jData.secondhandid = SecondHandHelper.SecondHandId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetSecondHandDetail",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Kiralama bilgileri getiriliyor..." });
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
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Satış Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "UpdateSecondHand": function (secondhand) {
        var jData = {};
        jData.secondhand = secondhand;
        //jData.secondhand.ContractStartDate = "/Date(" + $('#txtContractStartDate').data("datepicker").getDate().getTime() + ")/";
        //jData.secondhand.ContractEndDate = "/Date(" + $('#txtContractEndDate').data("datepicker").getDate().getTime() + ")/";
        var newCurr = secondhand.Currency.Id;

        if (newCurr == SecondHandHelper.CurrencyId) {
            jData.secondhand.Currency = {};
            jData.secondhand.Currency.Id = "00000000-0000-0000-0000-000000000000";
        }

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateSecondHand",
            async: false,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Satış bilgileri güncelleniyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#pnlQuoteDetail"));
            },
            success: function (data) {
                if (data != null) {

                    if (data.Success) {
                        parent.IndexHelper.ToastrShow(true, "Satış bilgileri başarıyla güncellendi.", "Kiralama Bilgileri");

                    } else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Kiralama Bilgileri");
                    }
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });
    },
    "GetCurrencies": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCurrencies",
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Kapora Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Kapora Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kapora Bilgileri");
            }
        });
    },
    "SecondHandId": "",
    "HousePrice": 0,
    "CurrencyId": "",
    "SendToApproval": function (callbackfunction) {
        var jData = {};
        jData.secondHandId = SecondHandHelper.SecondHandId;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/SendSecondHandToApproval",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Satış onaya gönderiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#pnlQuoteDetail"));
            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Bilgileri");
            }
        });
    },
    "KaporaTaken": function () {
        var jData = {};
        jData.userId = parent.IndexHelper.UserId;
        jData.secondHandId = SecondHandHelper.SecondHandId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/ConfirmSecondHandKaporaTaken",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#main"), iconOnly: false, boxed: true, message: "Ön ödeme alınıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#main"));
            },
            success: function (data) {
                parent.IndexHelper.ToastrShow(true, "İşlem tamamlandı", "2.El Satış Bilgileri");
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "2.El Satış Bilgileri");
            }
        });
    }
}

function mainController($scope) {

    var url = $.url(document.location);
    SecondHandHelper.SecondHandId = url.param("secondhandid");
    $scope.IsHideRef = false;
    $scope.IsPrePaymentTaken = true;

    SecondHandHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;
        });
    });

    SecondHandHelper.GetSecondHandDetail(function (e) {
        $scope.$apply(function () {
            $scope.secondhand = e.ReturnObject;
            if ($scope.secondhand.StatusCode != null) {

                if ($scope.secondhand.StatusCode.Value == SecondHandStatuses.IptalEdildi) {
                    $scope.IsFormDisabled = true;
                } else if ($scope.secondhand.StatusCode.Value == SecondHandStatuses.OnayBekleniyor) {
                    $scope.IsFormDisabled = true;
                } else if ($scope.secondhand.StatusCode.Value == SecondHandStatuses.Onaylandi) {
                    $scope.IsFormDisabled = true;
                    $scope.CheckPaymentStatus = true;
                } else if ($scope.secondhand.StatusCode.Value == SecondHandStatuses.OnOdemeAlindi) {
                    $scope.IsFormDisabled = true;
                    $scope.IsPrePaymentTaken = false;
                    $scope.CheckPaymentStatus = true;
                }
                else {
                    $scope.IsFormDisabled = false;
                }
            }

            if ($scope.secondhand.Currency != null) {
                setTimeout(function () {
                    $scope.$apply(function () {
                        $scope.currency = $scope.secondhand.Currency.Id;
                        SecondHandHelper.CurrencyId = $scope.secondhand.Currency.Id;
                    });
                }, 500);
            }
        });
    });

    $scope.Update = function () {

        if ($scope.secondhand.Currency == null) {
            $scope.secondhand.Currency = {};
            $scope.secondhand.Currency.Id = $scope.currency;
        }
        SecondHandHelper.UpdateSecondHand($scope.secondhand);
    }

    $scope.KaporaTaken = function () {
        SecondHandHelper.KaporaTaken();
    }

    $scope.SendToApproval = function () {
        if ($scope.secondhand.Currency == null) {
            $scope.secondhand.Currency = {};
            $scope.secondhand.Currency.Id = $scope.currency;
        }
        else {
            if ($scope.currency == $scope.GuidEmpty) {
                $scope.secondhand.Currency = null;
            }
            else if ($scope.currency != undefined) {
                $scope.secondhand.Currency.Id = $scope.currency;
            }
        }
        SecondHandHelper.SendToApproval(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, e.Result, "2. El Satış Bilgileri");
                    document.location.reload();
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "2. El Satış Bilgileri");
                }
            });
        });
    }

    $scope.EditHouse = function (productId) {
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");
    }
}
