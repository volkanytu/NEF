/// <reference path="global.js" />
/// <reference path="index.js" />

var isValidated = false;
var contactInfo = null;

var ContactCreateHelper = {
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

        $(".page-title", parent.document).html("Müşteri Arama");
        $("#btnBreadPage", parent.document).html("Müşteri Arama");

        //$("#txtMobile").inputmask("mask", {
        //    "mask": "(999) 999-9999"
        //});

        ContactCreateHelper.HandleLogin();

        ContactCreateHelper.OnClickEvents();
        ContactCreateHelper.OnChangeEvents();
        ContactCreateHelper.OnChangeKeyPressEvents();
        $("#txtMobile").mask('+90-###-#######');

        $("#txtTelephone").mask('+90-###-#######');
    },

    "IsContactExist": function (contact) {

        if (!isValidated) {
            contactInfo = contact;
            return;
        }

        var jData = {};
        jData.contact = contact;
        jData.contact.MobilePhone = $("#txtMobile").val();
        jData.contact.Owner = {};
        jData.contact.Owner.LogicalName = "systemuser";
        jData.contact.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/IsContactExist",
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
                if (data.Success) {
                    ContactCreateHelper.CreateContact(contactInfo);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Kayıt");
                }

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Müşteri Kayıt");
            }
        });
    },

    "Create": function (contact) {

        if (!isValidated) {
            contactInfo = contact;
            return;
        }

        var jData = {};
        jData.contact = contact;
        jData.contact.MobilePhone = $("#txtMobile").val();
        jData.contact.Owner = {};
        jData.contact.Owner.LogicalName = "systemuser";
        jData.contact.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateContact",
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
                if (data.Success) {
                    if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist) {
                        if (!parent.IndexHelper.IsCallCenterSearch) //Call center araması ise müşteri oluşturulduktan sonra bir telefon görüşmesi oluşturulur / oluşturulmaz.
                        {
                            parent.IndexHelper.CloseDialog();
                            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + data.CrmId).css("height", "3500px");
                        }
                        else //Eğer aktivite id dolu ise telefon görüşmesi oluşturulmadan editcontact sayfasına gönderilir
                        {
                            if (parent.IndexHelper.ActivityId != "") {
                                parent.IndexHelper.CloseDialog();
                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + data.CrmId + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "3500px");
                            }
                            else {
                                ContactCreateHelper.CreateActivityForCallCenter(data.CrmId, contact.FirstName + " " + contact.LastName, function (e) {
                                    if (e.Success) {
                                        parent.IndexHelper.CloseDialog();
                                        $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + data.CrmId + "&phonecallid=" + e.CrmId).css("height", "3500px");
                                    }
                                });
                            }

                        }
                    }
                    else {
                        parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + data.CrmId + "&name=" + encodeURI(contact.FirstName + " " + contact.LastName) + "' style='width:100%;height:350px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
                    }

                    //parent.IndexHelper.ToastrShow(true, data.Result, "Kişi Ekleme");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Kayıt");
                }

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Müşteri Kayıt");
            }
        });
    },

    "CreateContact": function (contact) {


        var jData = {};
        jData.contact = contact;
        jData.contact.Owner = {};
        jData.contact.Owner.LogicalName = "systemuser";
        jData.contact.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateContact",
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
                if (data.Success) {
                    if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist) {
                        if (!parent.IndexHelper.IsCallCenterSearch) //Call center araması ise müşteri oluşturulduktan sonra bir telefon görüşmesi oluşturulur / oluşturulmaz.
                        {
                            parent.IndexHelper.CloseDialog();
                            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + data.CrmId).css("height", "3500px");
                        }
                        else //Eğer aktivite id dolu ise telefon görüşmesi oluşturulmadan editcontact sayfasına gönderilir
                        {
                            if (parent.IndexHelper.ActivityId != "") {
                                parent.IndexHelper.CloseDialog();
                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + data.CrmId + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "3500px");
                            }
                            else {
                                ContactCreateHelper.CreateActivityForCallCenter(data.CrmId, contact.FirstName + " " + contact.LastName, function (e) {
                                    if (e.Success) {
                                        parent.IndexHelper.CloseDialog();
                                        $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + data.CrmId + "&phonecallid=" + e.CrmId).css("height", "3500px");
                                    }
                                });
                            }

                        }
                    }
                    else {
                        parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + data.CrmId + "&name=" + encodeURI(contact.FirstName + " " + contact.LastName) + "' style='width:100%;height:350px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
                    }

                    //parent.IndexHelper.ToastrShow(true, data.Result, "Kişi Ekleme");
                }
                else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Kayıt");
                }

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

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "HandleLogin": function () {
        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                firstname: {
                    required: true
                },
                lastname: {
                    required: true
                },
                mobilephone: {
                    required: true
                },
                emailaddress: {
                    required: true
                }
            },

            messages: {
                firstname: {
                    required: "Ad alani gerekli."
                },
                lastname: {
                    required: "Soyad alani gerekli."
                },
                mobilephone: {
                    required: "Cep tel. gerekli."
                },
                emailaddress: {
                    required: "Email gerekli."
                }
            },

            invalidHandler: function (event, validator) { //display error alert on form submit   
                $('.alert-danger', $('.login-form')).show();
            },

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element.closest('.input-icon'));
            },

            submitHandler: function (form) {

                isValidated = true;
                if (contactInfo != null) {
                    if (parent.IndexHelper.BusinessUnitId.toUpperCase() != RetailerBusinessUnitId.toUpperCase()) {
                        ContactCreateHelper.Create(contactInfo);
                    } else {
                        ContactCreateHelper.IsContactExist(contactInfo);
                    }
                }
                //alert("asdasdasd");
                //document.location.href = "index.html";

                // form.submit();
            }
        });

        $('.login-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.login-form').validate().form()) {
                    $('.login-form').submit();
                }
                return false;
            }
        });
    },
    "ContactId": "",
    "CreateActivityForCallCenter": function (contactId, contactName, callbackfunction) {
        var activity = {};
        activity.Direction = parent.IndexHelper.Direction == "false" ? DirectionCodes.Gelen : DirectionCodes.Giden;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "contact";
        activity.ActivityParty.Id = contactId;
        activity.ActivityParty.Name = contactName;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = parent.IndexHelper.UserId;

        var jData = {};
        jData.activity = activity;
        jData.systemUserId = parent.IndexHelper.UserId;;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateActivity",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Aktivite Oluşturma");
            }
        });
    },
}

function mainController($scope) {

    $scope.contact = {};

    $scope.ChangeCountryFormat = function () {

        if ($("#countryFormatSelection").attr('checked') === "checked") {
            $("#txtMobile").mask('+##-###-#######');
        } else {
            $("#txtMobile").mask('+90-###-#######');
        }

    }

    $scope.Save = function () {


        var isChoosen = false;

        if ($scope.contact.MarketingGrantValue == 1) {
            if ($scope.contact.sendSMS === true || $scope.contact.sendSMS === "true") {
                isChoosen = true
            }
            if ($scope.contact.sendFax === true || $scope.contact.sendFax === "true") {
                isChoosen = true
            }
            if ($scope.contact.sendEmail === true || $scope.contact.sendEmail === "true") {
                isChoosen = true
            }
            if ($scope.contact.sendMail === true || $scope.contact.sendMail === "true") {
                isChoosen = true
            }
            if ($scope.contact.contactTelephone === true || $scope.contact.contactTelephone === "true") {
                isChoosen = true
            }
            if (isChoosen == false) {
                parent.IndexHelper.ToastrShow(false, "İzinli pazarlama seçeneklerinden birini seçiniz.", "Müşteri Bilgileri");
            }
            else {

                $scope.contact.GrantUpdateUser = {};
                $scope.contact.GrantUpdateUser.Id = parent.IndexHelper.UserId;
                $scope.contact.GrantUpdateUser.LogicalName = "systemuser";

                if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase()) {
                    ContactCreateHelper.IsContactExist($scope.contact);
                }
                else {
                    ContactCreateHelper.Create($scope.contact);
                }
            }
        } else {
            if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase()) {
                ContactCreateHelper.IsContactExist($scope.contact);
            }
            else {
                ContactCreateHelper.Create($scope.contact);
            }
        }
    }

    $scope.Close = function () {
        ContactCreateHelper.Close();
    }

    $scope.HideMarketingDiv = function () {

        if ($scope.contact.MarketingGrantValue == true) {

            $scope.contact.sendSMS = true;
            $scope.contact.sendFax = true;
            $scope.contact.sendEmail = true;
            $scope.contact.sendMail = true;
            $scope.contact.contactTelephone = true;
        }
        else {

            $scope.contact.sendSMS = false;
            $scope.contact.sendFax = false;
            $scope.contact.sendEmail = false;
            $scope.contact.sendMail = false;
            $scope.contact.contactTelephone = false;
        }
    }
}