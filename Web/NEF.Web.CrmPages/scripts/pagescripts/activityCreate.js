/// <reference path="global.js" />
/// <reference path="index.js" />


var ActivityCreateHelper = {
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

        $("#liSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");

        ActivityCreateHelper.OnClickEvents();
        ActivityCreateHelper.OnChangeEvents();
        ActivityCreateHelper.OnChangeKeyPressEvents();

    },
    "GetLastActivity": function (id, callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.contactId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetContactLastActivity",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {

                Metronic.blockUI({ target: $("#pBody"), iconOnly: true });
            },
            complete: function () {
                //$("#msgWarning").show();
                Metronic.unblockUI($("#pBody"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    //returnValue = data;
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Oluşturma");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Oluşturma");
            }
        });
        return returnValue;
    },
    "GetSalesConsultants": function (callbackfunction) {
        var returnValue = null;

        $.ajax({
            url: CustomServiceUrl + "/GetSalesConsultants",
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
                    callbackfunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Oluşturma");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Aktivite Oluşturma");
            }
        });

        return returnValue;
    },
    "Create": function (activity) {
        var jData = {};
        jData.activity = activity;
        var jSonData = JSON.stringify(jData);

        if (jData.activity.ObjectTypeCode != null) {
            $.ajax({
                url: CustomServiceUrl + "/CreateActivity",
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
                        //alert("Aktivite oluşturuldu.");
                        ActivityCreateHelper.Close();
                        if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist) {
                            if (activity.ObjectTypeCode == 4210) //Telefon
                            {
                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ActivityCreateHelper.ContactId + "&phonecallid=" + data.CrmId).css("height", "3500px");
                            }
                            else {
                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + ActivityCreateHelper.ContactId + "&appointmentid=" + data.CrmId).css("height", "3500px");
                            }
                        }
                        else {
                            parent.IndexHelper.ToastrShow(true, data.Result, "Aktivite Oluşturma");
                        }
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Oluşturma");
                    }

                },
                error: function (a, b, c) {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Aktivite Oluşturma");
                }
            });
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Lütfen aktivite türünü seçiniz!", "Aktivite Oluşturma");
        }
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "Close": function () {
        parent.parent.IndexHelper.CloseDialog();
    },
    "ContactId": ""
}

function mainController($scope) {
    var url = $.url(document.location);
    ActivityCreateHelper.ContactId = url.param("contactid");
    $scope.contactName = url.param("name");
    $scope.IsConsultantDisable = false;

    ActivityCreateHelper.GetLastActivity(ActivityCreateHelper.ContactId, function (e) {
        $scope.$apply(function () {

            var lastActivity = e;

            if (lastActivity != null) {
                if (lastActivity.Success) {
                    $scope.lastActivity = lastActivity.ReturnObject.Owner.Name;
                    $scope.hasActivity = true;

                    GlobalHelper.ShowTextAlert("#pBody", "prepend", "warning", "En son <strong>" + lastActivity.ReturnObject.Owner.Name + "</strong> ile görüşme yapılmıştır.", true, true, true, 0, "warning")
                }
                else {
                    $scope.lastActivity = "Müşteriye ait aktivite bulunmamaktadır.";
                    $scope.hasNoActivity = true;

                    //GlobalHelper.ShowTextAlert("#pBody", "prepend", "info", "Müşteriye ait aktivite bulunmamaktadır.", true, true, true, 0, "info")
                }
            }
        });

    });


    ActivityCreateHelper.GetSalesConsultants(function (e) {
        $scope.$apply(function () {
            if (e.Success == true) {
                $scope.consultants = e.ReturnObject;
                if (parent.IndexHelper.UserType == UserTypes.SatisDanismani) //Eğer satış danışmanı aktivite oluşturuyorsa yalnızca kendini seçebilir
                {
                    $scope.consultant = parent.IndexHelper.UserId;
                    $scope.IsConsultantDisable = true;
                }
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Aktivite Oluşturma");
            }
        });
    });

   // $scope.consultants = ActivityCreateHelper.GetSalesConsultants();

    $scope.activity = {};

    $scope.Save = function () {
        var activity = {};
        activity.Direction = parent.IndexHelper.Direction == "false" ? DirectionCodes.Gelen : DirectionCodes.Giden;
        activity.ObjectTypeCode = $scope.typeCode;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "contact";
        activity.ActivityParty.Id = ActivityCreateHelper.ContactId;
        activity.ActivityParty.Name = $scope.contactName;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = $scope.consultant;


        ActivityCreateHelper.Create(activity);
    }

    $scope.Close = function () {
        ActivityCreateHelper.Close();
    }

}