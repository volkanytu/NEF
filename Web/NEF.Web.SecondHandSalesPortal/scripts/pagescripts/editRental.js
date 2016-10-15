/// <reference path="global.js" />
/// <reference path="index.js" />


var RentalHelper = {
    "OnLoad": function () {
        RentalHelper.OnClickEvents();
        RentalHelper.OnChangeEvents();
        RentalHelper.OnChangeKeyPressEvents();

        $("#CommissionAmountTxt").mask('000,000,000,000', { reverse: true, });
        $("#RentalAmountTxt").mask('000,000,000,000', { reverse: true });
        $('#DepositAmountTxt').mask('000,000,000,000', { reverse: true });


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
    "GetRentalDetail": function (callbackFunction) {

        var jData = {};
        jData.rentalid = RentalHelper.RentalId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetRentalDetail",
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

        $("#btnSelectNfz").click(function () {
            $("#nfzfile").click();
        });

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {
        $("#nfzfile").change(function () {

            $("#nfz_file").val(this.files[0].name);

            RentalHelper.AddNfzFile();
        });
    },
    "UpdateRental": function (rental) {
        var jData = {};
        jData.rental = rental;
        if ($('#txtContractStartDate').data("datepicker").getDate() == "Invalid Date") {
            jData.rental.ContractStartDate = null;
        } else {
            jData.rental.ContractStartDate = "/Date(" + $('#txtContractStartDate').data("datepicker").getDate().getTime() + ")/";
        }
        if ($('#txtContractEndDate').data("datepicker").getDate() == "Invalid Date") {
            jData.rental.ContractEndDate = null;
        } else {
            jData.rental.ContractEndDate = "/Date(" + $('#txtContractEndDate').data("datepicker").getDate().getTime() + ")/";
        }

        var newCurr = rental.Currency.Id;

        if (newCurr == RentalHelper.CurrencyId) {
            jData.rental.Currency = {};
            jData.rental.Currency.Id = "00000000-0000-0000-0000-000000000000";
        }

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateRental",
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
    "UpdateRentalGuarantor": function (rental) {
        var jData = {};
        jData.rental = rental;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateRental",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Kefil bilgileri güncelleniyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#pnlQuoteDetail"));
            },
            success: function () {
                parent.IndexHelper.ToastrShow(true, "Kefil Bilgisi alındı.", "Kiralama Bilgileri");
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
    "RentalId": "",
    "GuarantorId": "",
    "GuarantorName": "",
    "HousePrice": 0,
    "CurrencyId": "",
    "SendToApproval": function (callbackfunction) {
        var jData = {};
        jData.rentalId = RentalHelper.RentalId;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/SendRentalToApproval",
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
    "UploadNfzCrm": function (contactid) {
        try {

            var data = $("#lblNfzImage").attr('fileData');
            var mimeType = $("#lblNfzImage").attr('mimeType');
            var name = "CRM_NFZ_" + $("#lblNfzImage").attr('fileName');

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
                    contactid: contactid
                },
                beforeSend: function () {
                    Metronic.blockUI({ target: $("#pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Dosya yükleniyorr..." });
                    $("#btnNfzUpload").hide();
                },
                complete: function () {
                    Metronic.unblockUI($("#pnlQuoteDetail"));
                    $("#btnNfzUpload").show();
                },
                success: function (data) {
                    //debugger;
                    if (data != null) {
                        if (data == true) {
                            parent.IndexHelper.ToastrShow(true, "Nüfus başarıyla yüklendi.", "Dosya Yükleme");
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
    "DownloadImageToCrm": function (contactId) {
        var windowUrl = "upload.ashx?operation=2&id=" + contactId;
        window.open(windowUrl);
    },
    "GetNFZAttachment": function (contactid, callbackFunction) {
        var jData = {};
        jData.contactId = contactid;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactAttachment",
            async: true,
            data: jSonData,
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
                        if (data.ReturnObject != null) {
                            callbackFunction(data.ReturnObject);
                            $("#btnSelectNfz").hide();
                            $("#btnNfzDelete").show();
                            $("#btnNfzDownload").show();

                        } else {
                            $("#btnSelectNfz").show();
                            $("#btnNfzDelete").hide();
                            $("#btnNfzDownload").hide();
                        }

                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Nüfus Cüzdanı Belgeleri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Kiralama Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });
    },
    "DeleteAnnotionByContactId": function (contactId) {
        var jData = {};
        jData.contactId = contactId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/DeleteAnnotionByContactId",
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
                    document.location.reload();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });

        return returnValue;
    },
    "DeleteAnnotionByRentalId": function (rentalId) {
        var jData = {};
        jData.rentalId = rentalId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/DeleteAnnotionByRentalId",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pnlQuoteDetail"), iconOnly: false, boxed: true, message: "Kiralama bilgileri güncelleniyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#pnlQuoteDetail"));
            },
            success: function (data) {
                if (data.Success) {

                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kiralama Bilgileri");
            }
        });

        return returnValue;
    },
    "AddNfzFile": function () {
        var filepath = $("#nfz_file").val();
        if (filepath != null && filepath != "") {
            var file = $("#nfzfile")[0].files[0];

            var size = (file.size / 1024) / 1024;

            var reader = new FileReader();

            reader.onloadend = function () {
                var data = reader.result;
                data = data.substr(data.indexOf('base64') + 7);

                $("#btnNfzDelete").show();
                $("#btnNfzUpload").show();
                $("#lblNfzImage").attr("fileData", data).attr("mimeType", file.type).attr("fileName", file.name).html(file.name);
            }

            reader.readAsDataURL(file);
            $("#nfzfile").val(null);
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Lütfen dosya seçiniz!", "Dosya Yükleme");
        }
    },
}

function mainController($scope) {

    var url = $.url(document.location);
    RentalHelper.RentalId = url.param("rentalid");
    $scope.IsHideRef = false;

    RentalHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;
        });
    });

    RentalHelper.GetRentalDetail(function (e) {
        $scope.$apply(function () {
            $scope.rental = e.ReturnObject;

            //Tamamlandıysa kontroller disable olmalı
            if ($scope.rental.StatusCode != null) {

                if ($scope.rental.StatusCode.Value == RentalStatuses.Tamamlandi) {
                    $scope.IsFormDisabled = true;
                } else if ($scope.rental.StatusCode.Value == RentalStatuses.OnayBekleniyor) {
                    $scope.IsFormDisabled = true;
                }
                else {
                    $scope.IsFormDisabled = false;
                }
            }

            RentalHelper.GetNFZAttachment($scope.rental.Contact.Id, function (e) {
                $scope.$apply(function () {
                    $scope.GetNFZAttachment = e;
                });
            });
            if (e.ReturnObject.ContractStartDate != undefined) {
                var startDate = e.ReturnObject.ContractStartDate.replace("/Date(", "").replace(")/", "");
                var startDateStr = new Date(parseInt(startDate));
                $('#txtContractStartDate').data("datepicker").setDate(startDateStr);
            }

            if (e.ReturnObject.ContractEndDate != undefined) {
                var endDate = e.ReturnObject.ContractEndDate.replace("/Date(", "").replace(")/", "");
                var endDateStr = new Date(parseInt(endDate));
                $('#txtContractEndDate').data("datepicker").setDate(endDateStr);
            }

            if ($scope.rental.Currency != null) {
                setTimeout(function () {
                    $scope.$apply(function () {
                        $scope.currency = $scope.rental.Currency.Id;
                        RentalHelper.CurrencyId = $scope.rental.Currency.Id;
                    });
                }, 500);
            }
        });
    });

    $scope.Update = function () {

        RentalHelper.UpdateRental($scope.rental);

        if ($scope.rental.Currency == null) {
            $scope.rental.Currency = {};
            $scope.rental.Currency.Id = $scope.currency;
        }
        if ($scope.GetNFZAttachment == null) {
            parent.IndexHelper.ToastrShow(false, "Kefil'e ait Nüfus Cüzdanı olmadan kiralama yapılamaz.", "Kiralama Bilgileri");
        }
    }

    $scope.UploadCrm = function () {
        RentalHelper.UpdateRentalGuarantor($scope.rental, function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, "Kefil Bilgisi alındı.", "Kiralama Bilgileri");
                    $scope.IsHideRef = false;
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Kefil Bilgisi alınamadı.", "Kiralama Bilgileri");
                    $scope.IsHideRef = true;
                }
            });
        });

        RentalHelper.UploadDocToCrm(RentalHelper.RentalId);
    }

    $scope.UploadNfzCrm = function () {
        //Verileri kaybetmemek için crm'e önceden bir formu kaydet
        RentalHelper.UpdateRentalGuarantor($scope.rental, function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, "Kefil Bilgisi alındı.", "Kiralama Bilgileri");
                    $scope.IsHideRef = false;
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Kefil Bilgisi alınamadı.", "Kiralama Bilgileri");
                    $scope.IsHideRef = true;
                }
            });
        });

        RentalHelper.UploadNfzCrm($scope.rental.Contact.Id);
    }

    $scope.SendToApproval = function () {
        if ($scope.rental.Currency == null) {
            $scope.rental.Currency = {};
            $scope.rental.Currency.Id = $scope.currency;
        }
        else {
            if ($scope.currency == $scope.GuidEmpty) {
                $scope.rental.Currency = null;
            }
            else if ($scope.currency != undefined) {
                $scope.rental.Currency.Id = $scope.currency;
            }
        }

        if ($scope.rental.DepositAmount == null || $scope.rental.DepositAmount == "") {
            parent.IndexHelper.ToastrShow(false, "Depozito boş olamaz.", "Kiralama Bilgileri");
            return;
        }

        if ($scope.rental.CommissionAmount == null || $scope.rental.CommissionAmount == "") {
            parent.IndexHelper.ToastrShow(false, "Komisyon boş olamaz.", "Kiralama Bilgileri");
            return;
        }

        if ($scope.rental.RentalAmount == null || $scope.rental.RentalAmount == "") {
            parent.IndexHelper.ToastrShow(false, "Kiralama Fiyatı boş olamaz.", "Kiralama Bilgileri");
            return;
        }

        RentalHelper.SendToApproval(function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, e.Result, "Kiralama Bilgileri");
                    document.location.reload();
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Kiralama Bilgileri");
                }
            });
        });
    }

    $scope.EditHouse = function (productId) {
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");
    }

    $scope.DownloadCrm = function () {
        RentalHelper.DownloadImageToCrm($scope.rental.Quantor.Id);
    }

    $scope.DownloadFileCrm = function () {
        RentalHelper.DownloadTTCrm(RentalHelper.RentalId);
    }

    $scope.DeleteNFZdoc = function (contactId) {
        RentalHelper.DeleteAnnotionByContactId(contactId);
    }

    $scope.DeleteTTdoc = function () {
        RentalHelper.DeleteAnnotionByRentalId(RentalHelper.RentalId);
    }

}
