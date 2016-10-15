/// <reference path="global.js" />
/// <reference path="index.js" />


var QuoteHelper = {
    "OnLoad": function () {

        $("#txtPrePaymentAmount").mask('000,000,000,000.00', { reverse: true });
        $("#txtDiscountPercentage").mask('000,000,000,000.00', { reverse: true });
        $('#txtDiscountPrice').mask('000,000,000,000.00', { reverse: true });
        

        QuoteHelper.OnClickEvents();
        QuoteHelper.OnChangeEvents();
        QuoteHelper.OnChangeKeyPressEvents();

        if (jQuery().datepicker) {
            $('.date-picker').datepicker({
                isRtl: false,
                orientation: "left",
                autoclose: true,
                language: "tr"
            });
        }

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");
    },
    "GetQuoteDetail": function (callbackFunction) {

        var jData = {};
        jData.quoteId = QuoteHelper.QuoteId;
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
        $("#txtDiscountPrice").change(function () {
            var price = $("#txtDiscountPrice").val();
            if (price == "" || price == "0.00") {
                $("#txtDiscountPercentage").prop("disabled", false);
            }
            else {
                $("#txtDiscountPercentage").prop("disabled", true);
            }
            
        });

        $("#txtDiscountPercentage").change(function () {
            var percentage = $("#txtDiscountPercentage").val();
            if (percentage == "" || percentage == "0.00") {
                $("#txtDiscountPrice").prop("disabled", false);
            }
            else {
                $("#txtDiscountPrice").prop("disabled", true);
            }
        });
    },
    "UpdateQuote": function (quote) {
        var discountPercentage = $("#txtDiscountPercentage").val();
        var discountPrice = $("#txtDiscountPrice").val();

        var jData = {};
        jData.quote = quote;
        jData.quote.DiscountPercentage = discountPercentage == "" || discountPercentage == "0.00" ? null : discountPercentage.replace(',', '').replace(',', '');
        jData.quote.DiscountPrice = discountPrice == "" || discountPrice == "0.00" ? null : discountPrice.replace(',', '').replace(',', '');

        jData.quote.ContratDate = "/Date(" + $('#txtContratDate').data("datepicker").getDate().getTime() + ")/";

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateQuote",
            async: true,
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
                if (data.Success) {
                    parent.IndexHelper.ToastrShow(true, data.Result, "Satış Bilgileri");
                    document.location.reload();
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Satış Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Bilgileri");
            }
        });

        return returnValue;
    },
    "SendToApproval": function (callbackfunction) {

        var jData = {};
        jData.quoteId = QuoteHelper.QuoteId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/SendQuoteToApproval",
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

        return returnValue;
    },
    "SavePrePayment": function (payment) {
        var prePaymentAmount = $("#txtPrePaymentAmount").val();
        var jData = {};
        jData.payment = payment;
        jData.payment.PaymentAmount = prePaymentAmount == "" || prePaymentAmount == "0.00" ? null : prePaymentAmount.replace(',', '').replace(',', '');
        var amountControl = jData.payment.PaymentAmount != null ? parseFloat(jData.payment.PaymentAmount) : null;

        jData.payment.PaymentType = 4;
        jData.payment.PaymentDate = "/Date(" + $('#txtPaymentDate').data("datepicker").getDate().getTime() + ")/";
        jData.payment.Name = "Kapora Ödemesi - " + jData.payment.Contact.Name;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateOrUpdatePayment",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyPrePayment"), iconOnly: false, boxed: true, message: "Kapora işlemleri yapılıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyPrePayment"));
            },
            success: function (data) {
                if (data.Success) {
                    parent.IndexHelper.ToastrShow(true, data.Result, "Satış Bilgileri");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Satış Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Bilgileri");
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
    "GetPrePayment": function (callbackFunction) {

        var jData = {};
        jData.quoteId = QuoteHelper.QuoteId;

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
    "QuoteId": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    QuoteHelper.QuoteId = url.param("quoteid");
    $scope.IsPrePaymentTaken = false;

    $scope.prePayment = {};

    QuoteHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;
        });
    });

    QuoteHelper.GetQuoteDetail(function (e) {

        $scope.$apply(function () {
            $scope.quote = e.ReturnObject;

            $scope.quote.PaymentPlan = $scope.quote.PaymentPlan ? 1 : 0;

            var dateSecond = e.ReturnObject.ContratDate.replace("/Date(", "").replace(")/", "");

            var date = new Date(parseInt(dateSecond));

            $('#txtContratDate').data("datepicker").setDate(date);

            if (e.ReturnObject.StatusCode.Value == QuoteStatuses.DevamEdiyor || e.ReturnObject.StatusCode.Value == QuoteStatuses.Reddedildi) // Taslak veya reddedildi
            {
                $scope.IsFormDisabled = false;
            }
            else {
                $scope.IsFormDisabled = true;
                $("#txtDiscountPercentage").prop("disabled", true);
                $("#txtDiscountPrice").prop("disabled", true);

                if (e.ReturnObject.StatusCode.Value == QuoteStatuses.Onaylandi) {
                    $scope.IsPrePaymentTaken = true;
                }
            }

            if ($scope.quote.DiscountPrice != null) {
                $("#txtDiscountPercentage").prop("disabled", true);
            }

            if ($scope.quote.DiscountPricePercentage != null) {
                $("#txtDiscountPrice").prop("disabled", true);
            }

            QuoteHelper.GetPrePayment(function (e) {
                $scope.$apply(function () {
                    if (e.Success == true) {
                        $scope.prePayment = e.ReturnObject[0];
                        $scope.hasPrePayment = false;

                        var dateSecond = $scope.prePayment.PaymentDate.replace("/Date(", "").replace(")/", "");
                        var date = new Date(parseInt(dateSecond));
                        $('#txtPaymentDate').data("datepicker").setDate(date);

                        setTimeout(function () {
                            $scope.$apply(function () {
                                $scope.currency = $scope.Currency.Id;
                            });
                        }, 500);
                    }
                    else {

                        $scope.hasPrePayment = true;
                    }
                });
            });
        });

    });

    $scope.Update = function () {
        QuoteHelper.UpdateQuote($scope.quote);
    }

    $scope.CreateOrUpdatePrePayment = function () {
        if ($scope.prePayment.Contact == null) {
            $scope.prePayment.Contact = $scope.quote.Contact;
        }

        if ($scope.prePayment.Quote == null) {
            $scope.prePayment.Quote = {};
            $scope.prePayment.Quote.LogicalName = "quote";
            $scope.prePayment.Quote.Name = $scope.quote.Name;
            $scope.prePayment.Quote.Id = $scope.quote.QuoteId;
        }

        if ($scope.prePayment.Currency == null) {
            $scope.prePayment.Currency = $scope.quote.Currency;
        }

        if ($scope.prePayment.Owner == null) {
            $scope.prePayment.Owner = {};
            $scope.prePayment.Owner.LogicalName = "systemuser";
            $scope.prePayment.Owner.Id = parent.parent.IndexHelper.UserId;;
        }

        QuoteHelper.SavePrePayment($scope.prePayment);
    }

    $scope.AddPrePayment = function () {
        $scope.hasPrePayment = false;
    }

    $scope.SendToApproval = function () {
        QuoteHelper.SendToApproval(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, e.Result, "Satış Bilgileri");
                    document.location.reload();
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    };

    $scope.KaporaTaken = function () {
        $scope.prePayment.PaymentStatus = 100000000; //Kapora Alındı
        $scope.CreateOrUpdatePrePayment();
    }

    $scope.DiscountPriceChange = function () {
        if ($scope.quote.DiscountPrice != null && $scope.quote.DiscountPrice != "") {
            $scope.IsDiscountPercentageDisable = true;
        }
        else {
            $scope.IsDiscountPercentageDisable = false;
        }
    };
}
