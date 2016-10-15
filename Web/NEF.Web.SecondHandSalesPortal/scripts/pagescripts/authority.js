
var AddAuthorityDocs = {
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
        AddAuthorityDocs.OnClickEvents();
        AddAuthorityDocs.OnChangeEvents();
        AddAuthorityDocs.OnChangeKeyPressEvents();


        if (jQuery().datepicker) {
            $('.date-picker').datepicker({
                isRtl: false,
                orientation: "left",
                autoclose: true,
                language: "tr"
            });
        }
    },
    "Create": function (doc) {
        var jData = {};
        jData.document = doc;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/CreateAuthorityDocument",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $(".page-content"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($(".page-content"));
            },
            success: function (data) {

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Müşteri Kayıt");
            }
        });
    },
    "Close": function () {
        parent.IndexHelper.CloseDialog();
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
        $("#file").change(function () {

            $("#txt_file").val(this.files[0].name);
            if ($("#txt_file").val(this.files[0].name) != null) {
                AddAuthorityDocs.AnnotationName = $("#txt_file").val(this.files[0].name);
            }
            AddAuthorityDocs.AddFile();
        });
    },
    "IsAttachmentUpload": "",
    "DocumentId": "",
    "ProductId": "",
    "DocumentId": "",
    "GetProductInfo": function (callbackFunction, productId) {

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
                Metronic.blockUI({ target: $("#plBodyUserInfo"), iconOnly: false, boxed: true, message: "Konut bilgileri çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyUserInfo"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);

                    if (data.Success == true)
                        callbackFunction(data);
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Konut Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Fırsat Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Bilgileri");
            }
        });
    },
    "AddFile": function () {
        var filepath = $("#txt_file").val();
        if (filepath != null && filepath != "") {
            var file = $("#file")[0].files[0];
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

            $.ajax({
                url: "upload.ashx?operation=9",
                async: true,
                dataType: "json",
                type: "POST",
                data: {
                    name: name,
                    size: null,
                    type: mimeType,
                    data: data,
                    documentid: AddAuthorityDocs.DocumentId
                },
                beforeSend: function () {
                    Metronic.blockUI({ target: $("#plBodyUserInfo"), iconOnly: false, boxed: true, message: "Dosya yükleniyorr..." });
                    $("#btnUpload").hide();
                },
                complete: function () {
                    Metronic.unblockUI($("#plBodyUserInfo"));
                    $("#btnUpload").show();
                },
                success: function (data) {
                    if (data != null) {
                        if (data == true) {
                            parent.IndexHelper.ToastrShow(true, "Yetki belgesi başarıyla yüklendi.", "Dosya Yükleme");
                            AddAuthorityDocs.IsAttachmentUpload = true;
                        }
                        else {
                            parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Dosya Yükleme");
                            AddAuthorityDocs.IsAttachmentUpload = false;
                        }
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Dosya Yükleme");
                        AddAuthorityDocs.IsAttachmentUpload = false;
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
    "CreateAuthorityDocument": function (document, callbackFunction) {

        var jData = {};
        jData.document = document;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/CreateAuthorityDocument",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyUserInfo"), iconOnly: false, boxed: true, message: "Kayıt oluşturuluyor...." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyUserInfo"));
            },
            success: function (data) {
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Yetki Belgesi Bilgileri");
            }
        });

        return returnValue;
    },
    "UpdateAuthorityDocument": function (document, callbackFunction) {

        var jData = {};
        jData.document = document;
        jData.document.StartDate = "/Date(" + $('#txtContractStartDate').data("datepicker").getDate().getTime() + ")/";
        jData.document.EndDate = "/Date(" + $('#txtContractEndDate').data("datepicker").getDate().getTime() + ")/";

        if (jData.document.StartDate == "/Date(NaN)/") {
            parent.IndexHelper.ToastrShow(false, "Başlangıç tarihi girilmelidir.", "Yetki Belgesi Bilgileri");
            return;
        }
        if (jData.document.EndDate == "/Date(NaN)/") {
            parent.IndexHelper.ToastrShow(false, "Bitiş tarihi girilmelidir.", "Yetki Belgesi Bilgileri");
            return;
        }
        if (jData.document.Contact == null) {
            parent.IndexHelper.ToastrShow(false, "İlgili tarihi girilmelidir.", "Yetki Belgesi Bilgileri");
            return;
        }
        if (AddAuthorityDocs.IsAttachmentUpload == false) {
            parent.IndexHelper.ToastrShow(false, "Yetki belgesi eklenmelidir.", "Yetki Belgesi Bilgileri");
            return;
        }
        jData.document.AuthorityDocumentId = AddAuthorityDocs.DocumentId


        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/CreateAuthorityDocument",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyUserInfo"), iconOnly: false, boxed: true, message: "Kayıt güncelleniyor...." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyUserInfo"));
            },
            success: function (data) {
                callbackFunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Yetki Belgesi Bilgileri");
            }
        });
    },
    "AnnotationName": ""
}

function mainController($scope) {
    var url = $.url(document.location);

    AddAuthorityDocs.ProductId = url.param("productid");
    $scope.doc = {};
    $scope.doc.Product = {};
    AddAuthorityDocs.IsAttachmentUpload = false;

    AddAuthorityDocs.GetProductInfo(function (e) {
        $scope.$apply(function () {
            $scope.doc.Product.Id = e.ReturnObject.ProductId;
            $scope.doc.Product.Name = e.ReturnObject.Name;
            $scope.doc.Product.LogicalName = "product";
            $scope.doc.Name = e.ReturnObject.Name;

            AddAuthorityDocs.CreateAuthorityDocument($scope.doc, function (e) {
                $scope.$apply(function () {
                    AddAuthorityDocs.DocumentId = e.CrmId;
                });
            });
        });
    }, AddAuthorityDocs.ProductId);

    $scope.OpenContactSearchPage = function () {
        parent.IndexHelper.ShowDialog("<iframe src='contactsearch.html' style='width:100% !important; border:none;height:650px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-calendar'></i> Kişi Arama", "modal90");
    };

    $scope.UploadToCRM = function () {
        AddAuthorityDocs.UploadImageToCrm();
    }

    $scope.CreateAuthorityDocument = function () {
        var contactId = $("#txtContact").attr("crmid");

        if (contactId != null && contactId != undefined && contactId != "") {
            if ($scope.doc.Contact == null)
                $scope.doc.Contact = {};
            $scope.doc.Contact.Id = $("#txtContact").attr("crmid");
            $scope.doc.Contact.Name = $("#txtContact").val();
            $scope.doc.Contact.LogicalName = "contact";
        }
        else {
            $scope.doc.Contact == null
        }
        AddAuthorityDocs.UpdateAuthorityDocument($scope.doc, function (e) {
            $scope.$apply(function () {
                if (e.Success) {
                    parent.IndexHelper.ToastrShow(true, "Kayıt başarı ile oluşturuldu. ", "Yetki Belgesi Bilgileri");
                    $("#ifrmContent", parent.document).attr("src", "housesearch.html")
                }
            });
        });
    }
}