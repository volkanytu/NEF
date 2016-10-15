/// <reference path="global.js" />
/// <reference path="index.js" />

var isValidated = false;
var accountInfo = null;

var AccountCreateHelper = {
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

        AccountCreateHelper.HandleLogin();

        AccountCreateHelper.OnClickEvents();
        AccountCreateHelper.OnChangeEvents();
        AccountCreateHelper.OnChangeKeyPressEvents();

        $("#txtMobile").mask('+90-###-#######');
    },
    "Create": function (account) {

        if (!isValidated) {
            accountInfo = account;
            return;
        }

        var jData = {};
        jData.account = account;
        jData.account.Telephone1 = $("#txtMobile").val();
        jData.account.Owner = {};
        jData.account.Owner.LogicalName = "systemuser";
        jData.account.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateOrUpdateAccount",
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
                            $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + data.CrmId).css("height", "3500px");
                        }
                        else //Eğer aktivite id dolu ise telefon görüşmesi oluşturulmadan editcontact sayfasına gönderilir
                        {
                            if (parent.IndexHelper.ActivityId != "") {
                                parent.IndexHelper.CloseDialog();
                                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + data.CrmId + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "3500px");
                            }
                            else {
                                AccountCreateHelper.CreateActivityForCallCenter(data.CrmId, account.Name, function (e) {
                                    if (e.Success) {
                                        parent.IndexHelper.CloseDialog();
                                        $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + data.CrmId + "&phonecallid=" + e.CrmId).css("height", "3500px");
                                    }
                                });
                            }

                        }
                    }
                    else {
                        parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?accountid=" + data.CrmId + "&name=" + encodeURI(account.AccountName) + "' style='width:100%;height:350px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
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
    "IsAccountExist": function (account) {

        if (!isValidated) {
            accountInfo = account;
            return;
        }

        var jData = {};
        jData.account = account;
        jData.account.Telephone1 = $("#txtMobile").val();
        jData.account.Owner = {};
        jData.account.Owner.LogicalName = "systemuser";
        jData.account.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/IsAccountExist",
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
                    AccountCreateHelper.CreateAccount(accountInfo);
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
    "CreateAccount": function (account) {


        var jData = {};
        jData.account = account;
        jData.account.Owner = {};
        jData.account.Owner.LogicalName = "systemuser";
        jData.account.Owner.Id = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateOrUpdateAccount",
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
                            $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + data.CrmId).css("height", "3500px");
                        }
                        else //Eğer aktivite id dolu ise telefon görüşmesi oluşturulmadan editcontact sayfasına gönderilir
                        {
                            if (parent.IndexHelper.ActivityId != "") {
                                parent.IndexHelper.CloseDialog();
                                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + data.CrmId + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "3500px");
                            }
                            else {
                                AccountCreateHelper.CreateActivityForCallCenter(data.CrmId, account.Name, function (e) {
                                    if (e.Success) {
                                        parent.IndexHelper.CloseDialog();
                                        $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + data.CrmId + "&phonecallid=" + e.CrmId).css("height", "3500px");
                                    }
                                });
                            }

                        }
                    }
                    else {
                        parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?accountid=" + data.CrmId + "&name=" + encodeURI(account.AccountName) + "' style='width:100%;height:350px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
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


                if (parent.IndexHelper.BusinessUnitId.toUpperCase() != RetailerBusinessUnitId.toUpperCase()) {
                    AccountCreateHelper.Create(accountInfo);
                } else {
                    AccountCreateHelper.IsAccountExist(accountInfo);
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
    "AccountId": "",
    "CreateActivityForCallCenter": function (accountId, accountName, callbackfunction) {
        var activity = {};
        activity.Direction = parent.IndexHelper.Direction == "false" ? DirectionCodes.Gelen : DirectionCodes.Giden;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "account";
        activity.ActivityParty.Id = accountId;
        activity.ActivityParty.Name = accountName;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = parent.IndexHelper.UserId;

        var jData = {};
        jData.activity = activity;
        jDate.systemUserId = parent.IndexHelper.UserId;
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
    $scope.account = {};

    $scope.ChangeCountryFormat = function () {

        if ($("#countryFormatSelection").attr('checked') === "checked") {
            $("#txtMobile").mask('+##-###-#######');
        } else {
            $("#txtMobile").mask('+90-###-#######');
        }

    }

    $scope.Save = function () {
        if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase()) {
            AccountCreateHelper.IsAccountExist($scope.account);
        }
        else {
            AccountCreateHelper.Create($scope.account);
        }
    }

    $scope.Close = function () {
        AccountCreateHelper.Close();
    }
}
