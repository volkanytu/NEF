/// <reference path="global.js" />
/// <reference path="index.js" />


var CallCenterHelper = {
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


        $(".page-title", parent.document).html("Açık Call Center Çağrıları");
        $("#btnBreadPage", parent.document).html("Call Center Çağrıları");
        
        $("#liCallCenter", parent.document).addClass("active");
        $("#liSearch", parent.document).removeClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liHouseSearch", parent.document).removeClass("active");
        $("#liAllChart", parent.document).removeClass("active");

        CallCenterHelper.OnClickEvents();
        CallCenterHelper.OnChangeEvents();
        CallCenterHelper.OnChangeKeyPressEvents();
        CallCenterHelper.ControlUserState();

        //var url = $.url(document.location);
        //if (url.param("phonenumber") != undefined) {
        //    parent.IndexHelper.PhoneNumber = url.param("phonenumber");
        //}

        //$(window).resize(function () {
        //    parent.IndexHelper.AutoResize("ifrmContent");
        //});

        //parent.IndexHelper.AutoResize("ifrmContent");


    },
    "GetCallCenterPhoneCalls": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCallCenterPhoneCalls",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
            },
            complete: function () {
                // Metronic.unblockUI($("#pcBody"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Call Center");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Call Center");
            }
        });
    },
    "GetCallCenterPhoneCallsTopkapi": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCallCenterPhoneCallsTopkapi",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
            },
            complete: function () {
                // Metronic.unblockUI($("#pcBody"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Call Center");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Call Center");
            }
        });
    },
    "ExportToExcel": function (callbackFunction) {

        var jData = [];

        //jData.phoneCallList = [];

        $('#portlet_tabAll input:checked').each(function (index) {
            var activity = {};

            activity.ActivityId = $(this).parent().parent().find("td[type='id']").html();

            activity.Contact = {};
            activity.Contact.Name = $(this).parent().parent().find("td[type='contactName']").html();

            activity.Owner = {};
            activity.Owner.Name = $(this).parent().parent().find("td[type='ownerName']").html();

            activity.PriorityString = $(this).parent().parent().find("td[type='priority']").html();
            activity.CreatedOnString = $(this).parent().parent().find("td[type='createdOn']").html();
            activity.PhoneNumber = $(this).parent().parent().find("td[type='phoneNumber']").html();
            activity.ProjectName = $(this).parent().parent().find("td[type='contactProject']").html();
            activity.ContactMessage = $(this).parent().parent().find("td[type='contactMessage']").html();

            if (index != 0)
                jData.push(activity);

        });

        if (jData.length == 0) {
            return;
        }

        var jSonData = JSON.stringify(jData);

        $.ajax({
            //url: CustomServiceUrl + "/ExportPhoneCall",
            url: "upload.ashx?operation=2",
            async: true,
            dataType: "json",
            data: { data: jSonData },
            //contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
            },
            complete: function () {
                // Metronic.unblockUI($("#pcBody"));
            },
            success: function (data) {
                if (data != null) {
                    if (data.Result != "") {
                        window.open("/attachments/data.xlsx");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Call Center");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Call Center");
            }
        });

    },
    "ExportToExcelTopkapi": function (callbackFunction) {

        var jData = [];

        //jData.phoneCallList = [];

        $('#portlet_tabTopkapi input:checked').each(function (index) {
            var activity = {};

            activity.ActivityId = $(this).parent().parent().find("td[type='id']").html();

            activity.Contact = {};
            activity.Contact.Name = $(this).parent().parent().find("td[type='contactName']").html();

            activity.Owner = {};
            activity.Owner.Name = $(this).parent().parent().find("td[type='ownerName']").html();

            activity.PriorityString = $(this).parent().parent().find("td[type='priority']").html();
            activity.CreatedOnString = $(this).parent().parent().find("td[type='createdOn']").html();
            activity.PhoneNumber = $(this).parent().parent().find("td[type='phoneNumber']").html();
            activity.ProjectName = $(this).parent().parent().find("td[type='contactProject']").html();
            activity.ContactMessage = $(this).parent().parent().find("td[type='contactMessage']").html();

            if (index != 0)
                jData.push(activity);

        });

        if (jData.length == 0) {
            CallCenterHelper.ExportToExcelTopkapiAll();
            return;
        }

        var jSonData = JSON.stringify(jData);

        $.ajax({
            //url: CustomServiceUrl + "/ExportPhoneCall",
            url: "upload.ashx?operation=2",
            async: true,
            dataType: "json",
            data: { data: jSonData },
            //contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
            },
            complete: function () {
                // Metronic.unblockUI($("#pcBody"));
            },
            success: function (data) {
                if (data != null) {
                    if (data.Result != "") {
                        window.open("/attachments/data.xlsx");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Call Center");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Call Center");
            }
        });

    },
    "ExportToExcelTopkapiAll": function (callbackFunction) {
        var jData = [];
        CallCenterHelper.GetCallCenterPhoneCallsTopkapi(function (e) {
            for (var i = 0; i < e.ReturnObject.length; i++) {
                var activity = {};

                activity.ActivityId = e.ReturnObject[i].ActivityId;
                activity.Contact = e.ReturnObject[i].Contact;
                activity.Contact.Name = e.ReturnObject[i].Contact.Name;
                activity.Owner = e.ReturnObject[i].Owner;
                activity.Owner.Name = e.ReturnObject[i].Owner.Name;
                activity.PriorityString = e.ReturnObject[i].PriorityString;
                activity.CreatedOnString = e.ReturnObject[i].CreatedOnString;
                activity.PhoneNumber = e.ReturnObject[i].PhoneNumber;
                activity.ContactMessage = e.ReturnObject[i].ContactMessage;
                activity.ProjectName = e.ReturnObject[i].ProjectName;
               
                jData.push(activity);
            }
            var jSonData = JSON.stringify(jData);
            $.ajax({
                //url: CustomServiceUrl + "/ExportPhoneCall",
                url: "upload.ashx?operation=2",
                async: true,
                dataType: "json",
                data: { data: jSonData },
                //contentType: "application/json;",
                type: "POST",
                beforeSend: function () {
                    //Metronic.blockUI({ target: $("#main"), iconOnly: true });
                },
                complete: function () {
                    //Metronic.unblockUI($("#main"));
                },
                success: function (data) {
                    if (data != null) {
                        if (data.Result != "") {
                            window.open("/attachments/data.xlsx");
                        }
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Call Center");
                    }
                },
                error: function (a, b, c) {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Call Center");
                }
            });
        });

    },
    "OnClickEvents": function () {
        $('#chcSelectAll').click(function (event) {  //on click 
            if (this.checked) { // check select status
                $("#tBodyRows input[type='checkbox']").each(function () { //loop through each checkbox
                    this.checked = true;  //select all checkboxes with class "checkbox1"               
                });
            } else {
                $("#tBodyRows input[type='checkbox']").each(function () { //loop through each checkbox
                    this.checked = false; //deselect all checkboxes with class "checkbox1"                       
                });
            }
        });

        $('#chcSelectAll2').click(function (event) {  //on click 
            if (this.checked) { // check select status
                $("#tBodyRowsTopkapi input[type='checkbox']").each(function () { //loop through each checkbox
                    this.checked = true;  //select all checkboxes with class "checkbox1"               
                });
            } else {
                $("#tBodyRowsTopkapi input[type='checkbox']").each(function () { //loop through each checkbox
                    this.checked = false; //deselect all checkboxes with class "checkbox1"                       
                });
            }
        });
    },
    "OnChangeKeyPressEvents": function () {
        //$("#txtKey").keydown(function (e) {
        //    if (e.which == 13) {
        //        $scope.makeSearch();
        //    }
        //});
    },
    "OnChangeEvents": function () {

    },
    "ControlUserState": function () {
        var callCenterUserValue = UserTypes.IsGyoCallCenter.valueOf();
        var directorUserValue = UserTypes.SatisDirektorut.valueOf();
        var loginUserType = parent.IndexHelper.UserType.valueOf();

        if (callCenterUserValue == loginUserType) {
            $("#portlet_tabTopkapi").show("slow", function () {});
            $(".topkapi").show();
            $("#portlet_tabAll").hide("slow", function () {});
            $(".all").hide();
        } else if (directorUserValue == loginUserType) {
            $("#portlet_tabTopkapi").show("slow", function () {});
            $(".topkapi").show();
            $("#portlet_tabAll").show("slow", function () {});
            $(".all").show();
        } else {
            $("#portlet_tabTopkapi").show("slow", function () {});
            $(".topkapi").hide();
            $("#portlet_tabAll").show("slow", function () {});
            $(".all").show();
        }
    }

}

function mainController($scope) {
    CallCenterHelper.GetCallCenterPhoneCalls(function (e) {
        $scope.$apply(function () {
            if (e.Success) {
                $scope.phonecalls = e.ReturnObject;
                if (e.ReturnObject.length == 0) {
                    $scope.resultNumber = ' call center çağrısı bulunmamaktadır.';
                }
                else {
                    $scope.resultNumber = ' - ' + e.ReturnObject.length + ' adet call center çağrısı bulunmaktadır.';
                }
                setTimeout(function () {
                    parent.IndexHelper.AutoResize("ifrmContent");
                }, 200);
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Call Center");
            }

        });
    });
    CallCenterHelper.GetCallCenterPhoneCallsTopkapi(function (e) {
        $scope.$apply(function () {
            if (e.Success) {
                $scope.alltopkapiphonecalls = e.ReturnObject;

                if (e.ReturnObject.length == 0) {
                    $scope.topkapiresultNumber = ' call center çağrısı bulunmamaktadır. (Topkapı)';
                }
                else {
                    $scope.topkapiresultNumber = ' - ' + e.ReturnObject.length + ' adet call center çağrısı bulunmaktadır. (Topkapı)';
                    $scope.topkapiphonecalls = $scope.alltopkapiphonecalls.slice(0, 15);
                    $('#page-selectionPhoneCalls').bootpag({
                        total: Math.ceil($scope.alltopkapiphonecalls.length / 15),
                        page: 1,
                        maxVisible: 20
                    }).on("page", function (event, num) {
                        $scope.$apply(function () {
                            $scope.topkapiphonecalls = $scope.alltopkapiphonecalls.slice((num - 1) * 15, num * 15);
                        });
                    });

                }
                setTimeout(function () {
                    parent.IndexHelper.AutoResize("ifrmContent");
                }, 200);
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Call Center");
            }

        });
    });
    $scope.ExcelExport = function () {

        CallCenterHelper.ExportToExcel(function (e) {

            var a = "";

        });

    };
    $scope.ExcelExportTopkapi = function () {

        CallCenterHelper.ExportToExcelTopkapi(function (e) {

            var a = "";

        });
    };
   



}
