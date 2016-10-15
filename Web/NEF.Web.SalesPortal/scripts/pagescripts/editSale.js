/// <reference path="global.js" />
/// <reference path="index.js" />


var QuoteHelper = {
    "OnLoad": function () {

        $("#txtPrePaymentAmount").mask('000,000,000,000', { reverse: true, });
        $("#txtDiscountPercentage").mask('000,000,000,000.0', { reverse: true });
        $('#txtDiscountPrice').mask('000,000,000,000', { reverse: true });


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

        var date = new Date();
        $('#txtPaymentDate').data("datepicker").setDate(date);
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
        $("#btnSelectPhoto").click(function () {
            $("#file").click();
        });

        $("#btnDelete").click(function () {
            $("#lblImage").attr("fileData", null).attr("mimeType", null).attr("fileName", null).html("");
            $(this).hide();
            $("#btnUpload").hide();
        });
    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {
        $("#txtDiscountPrice").change(function () {
            var price = $("#txtDiscountPrice").val();

            var percentage = (parseFloat(price.replace(",", "")) / QuoteHelper.HousePrice) * 100;

            $("#txtDiscountPercentage").attr("percentage", percentage)
            $("#txtDiscountPercentage").val(percentage.toFixed(2));
            $("#txtDiscountPercentage").mask('000,000,000,000.0', { reverse: true });

            //if (price == "" || price == "0.00") {
            //    $("#txtDiscountPercentage").prop("disabled", false);
            //}
            //else {
            //    $("#txtDiscountPercentage").prop("disabled", true);
            //}

        });

        $("#txtDiscountPercentage").change(function () {
            var percentage = $("#txtDiscountPercentage").val();

            var discount = QuoteHelper.HousePrice * (percentage / 100);

            $("#txtDiscountPercentage").attr("percentage", percentage)
            $("#txtDiscountPrice").val(discount.toFixed(2));
            $('#txtDiscountPrice').mask('000,000,000,000', { reverse: true });


            //if (percentage == "" || percentage == "0.00") {
            //    $("#txtDiscountPrice").prop("disabled", false);
            //}
            //else {

            //    var discount = QuoteHelper.HousePrice * (percentage / 100);
            //    $("#txtPercentagePrice").val(discount);
            //    $('#txtPercentagePrice').mask('000,000,000,000', { reverse: true });

            //    $("#txtDiscountPrice").prop("disabled", true);
            //}
        });

        $("#file").change(function () {

            $("#txt_file").val(this.files[0].name);

            QuoteHelper.AddFile();
        });
    },
    "UpdateQuote": function (quote, callbackFunction) {
        //var discountPercentage = $("#txtDiscountPercentage").val();
        var discountPercentage = $("#txtDiscountPercentage").attr("percentage");
        var discountPrice = $("#txtDiscountPrice").val();

        var jData = {};
        jData.quote = quote;
        if ((discountPercentage == "" || discountPercentage == "0.00") && (discountPrice == "" || discountPrice == "0.00")) {
            jData.quote.DiscountPercentage = 0;
            jData.quote.DiscountPrice = null;
        }
        else {
            jData.quote.DiscountPercentage = discountPercentage == "" || discountPercentage == "0.00" ? null : parseFloat(discountPercentage.replace(',', '').replace(',', ''));
            jData.quote.DiscountPrice = null;// discountPrice == "" || discountPrice == "0.00" ? null : discountPrice.replace(',', '').replace(',', '');
        }

        jData.quote.ContratDate = "/Date(" + $('#txtContratDate').data("datepicker").getDate().getTime() + ")/";

        var newCurr = quote.Currency.Id;

        if (newCurr == QuoteHelper.CurrencyId) {
            jData.quote.Currency = {};
            jData.quote.Currency.Id = "00000000-0000-0000-0000-000000000000";
        }

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
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Bilgileri");
            }
        });

        return returnValue;
    },
    "SendToApproval": function (callbackfunction) {

        var checkHasAnnotation = parent.IndexHelper.CheckEntityExists(QuoteHelper.QuoteId, "Annotation", "ObjectId");

        if (checkHasAnnotation == false) {
            parent.IndexHelper.ToastrShow(false, "Sarı Excel upload edilmeden satışı onaya göndermezsiniz. Lütfen tekrar deneyiniz.", "Onaya Gönder");
            return;
        }

        var jData = {};
        jData.quoteId = QuoteHelper.QuoteId;

        parent.bootbox.prompt("Eklemek istediğiniz not varsa belirtiniz.", function (result) {
            if (result === null) {
                jData.comment = "";
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
            } else {
                jData.comment = result;

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

            }
        });



        //return returnValue;
    },
    "SavePrePayment": function (payment, callbackFunction) {
        var prePaymentAmount = $("#txtPrePaymentAmount").val();
        var name = payment.Contact != null ? payment.Contact.Name : payment.Account.Name;
        var jData = {};
        jData.payment = payment;
        jData.payment.PaymentAmount = prePaymentAmount == "" || prePaymentAmount == "0.00" ? null : prePaymentAmount.replace(',', '').replace(',', '');
        var amountControl = jData.payment.PaymentAmount != null ? parseFloat(jData.payment.PaymentAmount) : null;

        jData.payment.PaymentType = 4;
        jData.payment.PaymentDate = "/Date(" + $('#txtPaymentDate').data("datepicker").getDate().getTime() + ")/";
        jData.payment.Name = "Kapora Ödemesi - " + name;
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
                callbackFunction(data);
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
    "AddFile": function () {
        var filepath = $("#txt_file").val();
        if (filepath != null && filepath != "") {
            var file = $("#file")[0].files[0];

            var file = $("#file")[0].files[0];

            var isExcel = file.name.indexOf("xlsx");

            if (isExcel == -1) {
                parent.IndexHelper.ToastrShow(false, "Lütfen XLSX uzantılı bir dosya seçiniz!", "Dosya Format Hatası");
                return;
            }
            var size = (file.size / 1024) / 1024;

            var reader = new FileReader();

            reader.onloadend = function () {
                var data = reader.result;
                data = data.substr(data.indexOf('base64') + 7);

                $("#btnDelete").show();
                $("#btnUpload").show();
                $("#lblImage").attr("fileData", data).attr("mimeType", file.type).attr("fileName", file.name).html(file.name);
            }

            reader.readAsDataURL(file);

            $("#file").val(null);
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Lütfen dosya seçiniz!", "Dosya Yükleme");
        }
    },
    "UploadImageToCrm": function () {
        try {

            var data = $("#lblImage").attr('fileData');
            var mimeType = $("#lblImage").attr('mimeType');
            var name = $("#lblImage").attr('fileName');

            //debugger;
            $.ajax({
                url: "upload.ashx?operation=1",
                async: true,
                dataType: "json",
                //contentType: "application/json;",
                type: "POST",
                data: {
                    name: name,
                    size: null,
                    type: mimeType,
                    data: data,
                    quoteid: QuoteHelper.QuoteId
                },
                beforeSend: function () {
                    Metronic.blockUI({ target: $("#pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Sarı Excel yükleniyorr..." });
                    $("#btnUpload").hide();
                },
                complete: function () {
                    Metronic.unblockUI($("#pnlQuoteDetail"));
                    $("#btnUpload").show();
                },
                success: function (data) {
                    //debugger;
                    if (data != null) {
                        if (data == true) {
                            parent.IndexHelper.ToastrShow(true, "Sarı Excel başarıyla yüklendi.", "Dosya Yükleme");

                            document.location.reload();
                        }
                        else {
                            parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Dosya Yükleme");
                        }
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Dosya Yükleme");
                    }
                },
                error: function (a, b, c) {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Dosya Yükleme");
                    return false;
                }
            });

        } catch (e) {

        }
    },
    "QuoteId": "",
    "HousePrice": 0,
    "CurrencyId": ""
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

            var percentage = $scope.quote.DiscountPercentage;
            $("#txtDiscountPercentage").attr("percentage", percentage);

            var discount = $scope.quote.HouseListPrice * (percentage / 100);

            if (percentage != null) {
                $scope.quote.DiscountPercentage = percentage.toFixed(2);
                $scope.quote.DiscountPrice = discount.toFixed(2);
            }


            $('#txtDiscountPrice').mask('000,000,000,000', { reverse: true });


            $scope.quote.PaymentPlan = $scope.quote.PaymentPlan ? 1 : 0;
            QuoteHelper.HousePrice = $scope.quote.HouseListPrice;

            var dateSecond = e.ReturnObject.ContratDate.replace("/Date(", "").replace(")/", "");

            var date = new Date(parseInt(dateSecond));

            $('#txtContratDate').data("datepicker").setDate(date);

            if ($scope.quote.Currency != null) {
                setTimeout(function () {
                    $scope.$apply(function () {
                        $scope.currency = $scope.quote.Currency.Id;
                        QuoteHelper.CurrencyId = $scope.quote.Currency.Id;
                    });
                }, 500);
            }

            if (e.ReturnObject.StatusCode.Value == QuoteStatuses.DevamEdiyor || e.ReturnObject.StatusCode.Value == QuoteStatuses.Reddedildi) // Taslak veya reddedildi
            {
                $scope.IsFormDisabled = false;
            }
            else {
                //$scope.IsFormDisabled = true;
                $("#txtDiscountPercentage").prop("disabled", true);
                $("#txtDiscountPrice").prop("disabled", true);

                if (e.ReturnObject.StatusCode.Value == QuoteStatuses.Onaylandi) {
                    $scope.IsPrePaymentTaken = true;
                }
            }

            //if ($scope.quote.DiscountPrice != null) {
            //    $("#txtDiscountPercentage").prop("disabled", true);
            //}

            //if ($scope.quote.DiscountPricePercentage != null) {
            //    $("#txtDiscountPrice").prop("disabled", true);
            //}

            QuoteHelper.GetPrePayment(function (e) {
                $scope.$apply(function () {
                    if (e.Success == true) {

                        $scope.prePaymentList = e.ReturnObject;

                        var total = 0;

                        for (var i = 0; i < e.ReturnObject.length; i++) {
                            total += e.ReturnObject[i].PaymentAmount;
                        }

                        $scope.prePayment = e.ReturnObject[0];
                        $scope.prePayment.PaymentAmount = total;// e.ReturnObject[0];

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
        if ($scope.quote.Currency == null) {
            $scope.quote.Currency = {};
            $scope.quote.Currency.Id = $scope.currency;
        }
        else {
            if ($scope.currency == $scope.GuidEmpty) {
                $scope.quote.Currency = null;
            }
            else if ($scope.currency != undefined) {
                $scope.quote.Currency.Id = $scope.currency;
            }
        }

        QuoteHelper.UpdateQuote($scope.quote, function (e) {
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
    }

    $scope.CreateOrUpdatePrePayment = function (prePayment, isShowMessage) {

        if (prePayment == null || prePayment == undefined) {
            prePayment = $scope.prePayment;
            isShowMessage = true;
        }

        if (prePayment.Contact == null) {
            if ($scope.quote.Contact.LogicalName == "contact") {
                prePayment.Contact = $scope.quote.Contact;
            }
        }

        if (prePayment.Account == null) {
            if ($scope.quote.Contact.LogicalName == "account") {
                prePayment.Account = $scope.quote.Contact;
            }
        }

        if (prePayment.Quote == null) {
            prePayment.Quote = {};
            prePayment.Quote.LogicalName = "quote";
            prePayment.Quote.Name = $scope.quote.Name;
            prePayment.Quote.Id = $scope.quote.QuoteId;
        }

        if (prePayment.Currency == null) {
            prePayment.Currency = $scope.quote.Currency;
        }

        if (prePayment.Owner == null) {
            prePayment.Owner = {};
            prePayment.Owner.LogicalName = "systemuser";
            prePayment.Owner.Id = parent.parent.IndexHelper.UserId;;
        }

        QuoteHelper.SavePrePayment(prePayment, function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    $scope.IsPrePaymentClick = true;

                    if (isShowMessage == true)
                        parent.IndexHelper.ToastrShow(true, e.Result, "Satış Bilgileri");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    }

    $scope.AddPrePayment = function () {
        $scope.hasPrePayment = false;
    }

    $scope.UploadCrm = function () {
        QuoteHelper.UploadImageToCrm();
    }

    $scope.SendToApproval = function () {
        if ($scope.quote.Currency == null) {
            $scope.quote.Currency = {};
            $scope.quote.Currency.Id = $scope.currency;
        }
        else {
            if ($scope.currency == $scope.GuidEmpty) {
                $scope.quote.Currency = null;
            }
            else if ($scope.currency != undefined) {
                $scope.quote.Currency.Id = $scope.currency;
            }
        }

        QuoteHelper.UpdateQuote($scope.quote, function (e) {
            $scope.$apply(function () {
                if (e.Success) {
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
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Satış Bilgileri");
                }
            });
        });
    };

    $scope.KaporaTaken = function () {

        var kaporaList = $scope.prePaymentList;

        for (var i = 0; i < kaporaList.length; i++) {

            kaporaList[i].PaymentStatus = 100000000; //Kapora Alındı

            if (i == (kaporaList.length - 1))
                $scope.CreateOrUpdatePrePayment(kaporaList[i], true);
            else
                $scope.CreateOrUpdatePrePayment(kaporaList[i], false);
        }

        window.open(GlobalHelper.CrmDocumentMergeUrl + "prePaymentForm.aspx?Id=" + QuoteHelper.QuoteId);
    }

    $scope.DiscountPriceChange = function () {
        if ($scope.quote.DiscountPrice != null && $scope.quote.DiscountPrice != "") {
            $scope.IsDiscountPercentageDisable = true;

            //var houseList = $scope.quote.HouseListPrice;
            //var discountPrice = $("#txtDiscountPrice").val();
            //var discountPrice = discountPrice == "" || discountPrice == "0.00" ? null : discountPrice.replace(',', '').replace(',', '');
            //if (discountPrice != null) {
            //    var percentage = (discountPrice * 100) / houseList;
            //    $("#txtDiscountPercentage").val(percentage);
            //    $("#txtDiscountPercentage").mask('000,000,000,000', { reverse: true });
            //}
        }
        else {
            $scope.IsDiscountPercentageDisable = false;
        }
    };

    $scope.EditHouse = function (productId) {
        //parent.IndexHelper.CloseDialog();
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };
}
