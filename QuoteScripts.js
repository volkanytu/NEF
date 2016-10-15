var GlobalCurrencyName = null;
var GlobalCurrencyId = null;

function formOnLoad() {
    if (CheckPrePaymentButtonRecord() == false) {//Kapora Ödeme Kaydı Vardır.
        Xrm.Page.getControl("new_prepaymentamount").setDisabled(true);
        Xrm.Page.getControl("new_prepaymenttype").setDisabled(true);
        //Xrm.Page.getControl("new_prepaymentdate").setDisabled(true);
        Xrm.Page.getControl("new_isprepaymenttaken").setDisabled(true);
    }
    else {
        Xrm.Page.getAttribute("new_prepaymentamount").setValue(null);
        Xrm.Page.getAttribute("new_prepaymenttype").setValue(null);
        Xrm.Page.getAttribute("new_prepaymentdate").setValue(null);
        Xrm.Page.getAttribute("new_isprepaymenttaken").setValue(false);
    }



    addEventHandler();
    MakeCreateButton("new_btnvoucher", "Peşin");//Peşin
    MakeUpdateButton("new_btnvoucher", "Peşin");//Peşin
    MakeCreateButton("new_btninterval", "Ara Ödeme");//Ara Ödeme
    MakeUpdateButton("new_btninterval", "Ara Ödeme");//Ara Ödeme
    MakeCreateButton("new_btninstallment", "Düzenli Taksit");//Düzenli Taksit
    MakeUpdateButton("new_btninstallment", "Düzenli Taksit");//Düzenli Taksit
    MakeCreateButton("new_btncredit", "Kredi");//Kredi
    MakeUpdateButton("new_btncredit", "Kredi");//Kredi
    MakeCreateButton("new_btnprepayment", "Kapora");//Kapora
    OpenBankCreaditPaymenPlanForm("new_bankcreditpaymentplan", "Banka Kredi");//Banka Kredi
    ResellerChange();
    ChangePaymentPlan();
    CalculatePrePayment();
    AddEventSubGrid();
    SetSubmitAlways("tab_5");
    SetSubmitAlways("tab_6");
    Xrm.Page.getAttribute("new_vouchernumber").setValue(1);
    Xrm.Page.getAttribute("new_creditpaymentnumber").setValue(1);
    var sumInterval = Xrm.Page.getAttribute("new_suminterval").getValue();
    var instAmount = Xrm.Page.getAttribute("new_instamount").getValue();

    GlobalCurrencyName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
    GlobalCurrencyId = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;

    var status = Xrm.Page.getAttribute("statuscode").getValue();
    if (status != 100000005) {//Onaylandı değil ise
        // makeReadOnly();
    }
    var hasSecondCustomer = Xrm.Page.getAttribute("new_hassecondcustomer").getValue();
    if (hasSecondCustomer) {
        Xrm.Page.getAttribute("new_secondcontactid").setRequiredLevel("required");
    }
    else {
        Xrm.Page.getAttribute("new_secondcontactid").setRequiredLevel("none");
    }

    var ReviseButton = window.top.document.getElementById("quote|NoRelationship|Form|Mscrm.Form.quote.ReviseQuote");
    if (ReviseButton != null) {
        if (!ButtonSytemUserRoleCheck("Düzelt")) {
            ReviseButton.style.display = 'none';
        }
    }

    var projectId = Xrm.Page.getAttribute("new_projectid").getValue();
    if (projectId != null) {
        projectId = Xrm.Page.getAttribute("new_projectid").getValue()[0].id;
        projectId = projectId.replace("{", "").replace("}", "");
        if (projectId.toUpperCase() == TopkapiProjectId && Xrm.Page.getAttribute("statecode").getValue() == 0) {
            Xrm.Page.getAttribute("new_isnotarizedsales").setValue(true);
        }
    }
}

function addEventHandler() {
    // add the event handler for PreSearch Event
    Xrm.Page.getControl("new_resellerid").addPreSearch(addFilter);
}

function addFilter() {
    //find contact contains this @example.com
    var id = Xrm.Page.getAttribute("new_resellertypeid").getValue()[0].id;

    //create a filter xml
    var filter = "<filter type='and'>" +
                 "<condition attribute='new_resellertypeid' operator='eq' value='" + id + "'/>" +
                  "</filter>";

    //add filter
    Xrm.Page.getControl("new_resellerid").addCustomFilter(filter);
}


function ResellerChange() {
    if (Xrm.Page.getAttribute("new_resellertypeid").getValue() == null) {
        Xrm.Page.getAttribute("new_resellertypeid").setValue([{ id: NEFResellerType, name: "Direct", entityType: "new_resellertype" }]);
        Xrm.Page.getControl("new_resellerid").setDisabled(false);
    }
    if (Xrm.Page.getAttribute("new_resellertypeid").getValue() != null &&
		Xrm.Page.getAttribute("new_resellertypeid").getValue()[0].id.replace("{", "").replace("}", "") != NEFResellerType) {
        Xrm.Page.getControl("new_resellerid").setDisabled(false);
    }
    else {
        Xrm.Page.getControl("new_resellerid").setDisabled(true);
    }
}
function formOnSave(context) {

    ChangeDiscount(context);
    CalculatePrePayment();
}
function AddEventSubGrid() {
    if (document.getElementById("paymentgrid")) {
        var grid = document.getElementById("paymentgrid");
        // Google chrome
        if (grid.addEventListener) {
            grid.addEventListener('click', ReadSelectedSubGridRecords, false);
            // IE
            //} else if (grid.attachEvent) {
            //    grid.attachEvent('onclick', ReadSelectedSubGridRecords);
            //}

        }
    }
    else {
        setTimeout("AddEventSubGrid();", 2000);
    }
}
function ReadSelectedSubGridRecords() {
    if (document.getElementById("paymentgrid")) {
        var grid = document.getElementById("paymentgrid").control;
        if (grid.get_selectedRecords().length > 0) {
            var paymentId = grid.get_selectedRecords()[0].Id;
            paymentId = paymentId.replace("{", "").replace("}", "");
            var payment = GetPaymentById(paymentId);
            if (payment != null) {
                if (payment.new_type.Value == 1) {//Ara Ödeme
                    Xrm.Page.getAttribute("new_itype").setValue(payment.new_vtype.Value);
                    Xrm.Page.getAttribute("new_ipaymentamount").setValue(parseFloat(payment.new_paymentamount.Value));
                    Xrm.Page.getAttribute("new_ipaymentdate").setValue(new Date(parseInt(payment.new_date.substr(6))));


                    Xrm.Page.getControl("new_intervalperiod").setDisabled(true);
                    Xrm.Page.getControl("new_istartdate").setDisabled(true);
                    Xrm.Page.getControl("new_iinterestrate").setDisabled(true);
                    Xrm.Page.getControl("new_intervalnumber").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentdate").setDisabled(false);
                    Xrm.Page.getControl("new_ipaymentamount").setDisabled(false);
                    Xrm.Page.getControl("new_itype").setDisabled(false);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_type").setValue(null);
                    Xrm.Page.getAttribute("new_paydate").setValue(null);
                    Xrm.Page.getControl("new_type").setDisabled(true);
                    Xrm.Page.getControl("new_paydate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_insttype").setValue(null);
                    Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);
                    Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);
                    Xrm.Page.getAttribute("new_instnumber").setValue(null);
                    Xrm.Page.getAttribute("new_inststartdate").setValue(null);
                    Xrm.Page.getAttribute("new_instrate").setValue(null);
                    Xrm.Page.getControl("new_insttype").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_instnumber").setDisabled(true);
                    Xrm.Page.getControl("new_inststartdate").setDisabled(true);
                    Xrm.Page.getControl("new_instrate").setDisabled(true);
                    Xrm.Page.getControl("new_instamount").setDisabled(true);
                    ///////////////////////////////////////////////////////////////
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//Senet Tipi
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Ara Ödeme Adedi
                    Xrm.Page.getControl("new_creditpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_insttypecreditpayment").setDisabled(true);

                }
                else if (payment.new_type.Value == 2) {//Düzenli Taksit
                    Xrm.Page.getAttribute("new_insttype").setValue(payment.new_vtype.Value);
                    Xrm.Page.getAttribute("new_instpaymentamount").setValue(parseFloat(payment.new_paymentamount.Value));
                    Xrm.Page.getAttribute("new_instpaymentdate").setValue(new Date(parseInt(payment.new_date.substr(6))));


                    Xrm.Page.getControl("new_instamount").setDisabled(true);
                    Xrm.Page.getControl("new_instnumber").setDisabled(true);
                    Xrm.Page.getControl("new_inststartdate").setDisabled(true);
                    Xrm.Page.getControl("new_instrate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentdate").setDisabled(false);
                    Xrm.Page.getControl("new_instpaymentamount").setDisabled(false);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_itype").setValue(null);
                    Xrm.Page.getAttribute("new_ipaymentamount").setValue(null);
                    Xrm.Page.getAttribute("new_ipaymentdate").setValue(null);
                    Xrm.Page.getAttribute("new_intervalperiod").setValue(null);
                    Xrm.Page.getAttribute("new_intervalnumber").setValue(null);
                    Xrm.Page.getAttribute("new_istartdate").setValue(null);
                    Xrm.Page.getControl("new_itype").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_intervalperiod").setDisabled(true);
                    Xrm.Page.getControl("new_intervalnumber").setDisabled(true);
                    Xrm.Page.getControl("new_istartdate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_type").setValue(null);
                    Xrm.Page.getAttribute("new_paydate").setValue(null);
                    Xrm.Page.getControl("new_type").setDisabled(true);
                    Xrm.Page.getControl("new_paydate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//Senet Tipi
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Ara Ödeme Adedi
                    Xrm.Page.getControl("new_creditpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_insttypecreditpayment").setDisabled(true);
                }
                else if (payment.new_type.Value == 3) {//Peşin Ödeme
                    Xrm.Page.getAttribute("new_type").setValue(payment.new_vtype.Value);
                    Xrm.Page.getAttribute("new_totalvoucheramount").setValue(parseFloat(payment.new_paymentamount.Value));
                    Xrm.Page.getAttribute("new_paydate").setValue(new Date(parseInt(payment.new_date.substr(6))));

                    Xrm.Page.getControl("new_paydate").setDisabled(false);
                    Xrm.Page.getControl("new_type").setDisabled(false);


                    Xrm.Page.getAttribute("new_itype").setValue(null);
                    Xrm.Page.getAttribute("new_ipaymentamount").setValue(null);
                    Xrm.Page.getAttribute("new_ipaymentdate").setValue(null);
                    Xrm.Page.getAttribute("new_istartdate").setValue(null);
                    Xrm.Page.getAttribute("new_intervalnumber").setValue(null);
                    Xrm.Page.getAttribute("new_intervalperiod").setValue(null);
                    Xrm.Page.getAttribute("new_iinterestrate").setValue(null);
                    Xrm.Page.getControl("new_itype").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_istartdate").setDisabled(true);
                    Xrm.Page.getControl("new_intervalnumber").setDisabled(true);
                    Xrm.Page.getControl("new_intervalperiod").setDisabled(true);
                    Xrm.Page.getControl("new_iinterestrate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////                
                    Xrm.Page.getAttribute("new_insttype").setValue(null);//Senet Tipi
                    Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara Ödeme Adedi
                    Xrm.Page.getAttribute("new_inststartdate").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
                    Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);//Ödeme Tarihi (Düzenli Taksit)
                    Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);//Ödenecek Tutar (Düzenli Taksit)                
                    Xrm.Page.getAttribute("new_instrate").setValue(null);
                    Xrm.Page.getControl("new_insttype").setDisabled(true);
                    Xrm.Page.getControl("new_instnumber").setDisabled(true);
                    Xrm.Page.getControl("new_inststartdate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_instrate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//Senet Tipi
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Ara Ödeme Adedi
                    Xrm.Page.getControl("new_creditpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_insttypecreditpayment").setDisabled(true);

                }
                else if (payment.new_type.Value == 9) {//Banka Kredisi
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(payment.new_vtype.Value);
                    Xrm.Page.getAttribute("new_creditpaymentamount").setValue(parseFloat(payment.new_paymentamount.Value));
                    Xrm.Page.getAttribute("new_creditpaymentdate").setValue(new Date(parseInt(payment.new_date.substr(6))));

                    Xrm.Page.getControl("new_creditpaymentdate").setDisabled(false);
                    Xrm.Page.getControl("new_insttypecreditpayment").setDisabled(false);

                    Xrm.Page.getAttribute("new_itype").setValue(null);
                    Xrm.Page.getAttribute("new_ipaymentamount").setValue(null);
                    Xrm.Page.getAttribute("new_ipaymentdate").setValue(null);
                    Xrm.Page.getAttribute("new_istartdate").setValue(null);
                    Xrm.Page.getAttribute("new_intervalnumber").setValue(null);
                    Xrm.Page.getAttribute("new_intervalperiod").setValue(null);
                    Xrm.Page.getAttribute("new_iinterestrate").setValue(null);
                    Xrm.Page.getControl("new_itype").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_ipaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_istartdate").setDisabled(true);
                    Xrm.Page.getControl("new_intervalnumber").setDisabled(true);
                    Xrm.Page.getControl("new_intervalperiod").setDisabled(true);
                    Xrm.Page.getControl("new_iinterestrate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////                
                    Xrm.Page.getAttribute("new_insttype").setValue(null);//Senet Tipi
                    Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara Ödeme Adedi
                    Xrm.Page.getAttribute("new_inststartdate").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
                    Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);//Ödeme Tarihi (Düzenli Taksit)
                    Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);//Ödenecek Tutar (Düzenli Taksit)                
                    Xrm.Page.getAttribute("new_instrate").setValue(null);
                    Xrm.Page.getControl("new_insttype").setDisabled(true);
                    Xrm.Page.getControl("new_instnumber").setDisabled(true);
                    Xrm.Page.getControl("new_inststartdate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_instrate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////                
                    Xrm.Page.getAttribute("new_type").setValue(null);
                    Xrm.Page.getAttribute("new_paydate").setValue(null);
                    Xrm.Page.getControl("new_type").setDisabled(true);
                    Xrm.Page.getControl("new_paydate").setDisabled(true);

                }

            }
        }
        else {
            tabdisable("tab_6", false);
        }
    }
}

function CheckBankCreditPaymentPlan() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var result = true;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        //url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_bankcreditstatusSet?$filter=new_quoteid/Id eq guid'" + quoteId + "'&$select=new_bankcreditstatusId",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_bankcreditstatusSet?$filter=new_quoteid/Id eq guid'" + quoteId + "' and statecode/Value eq 0&$select=new_bankcreditstatusId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;
            }
            else {
                result = true;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckBankCreditPaymentPlan Error!!!");
            result = false;
        }
    });
    return result;

}

function BankCreditPaymentPlan() {
    if (CheckBankCreditPaymentPlan() == false) {
        alert("İlgili satış için Banka Kredi Kaydı bulunmaktadır.");
    }
    else {
        var new_bankcreditstatus = {};
        var quote = {};
        var quoteId = Xrm.Page.data.entity.getId();
        quote.Id = quoteId.replace("{", "").replace("}", "");
        quote.Name = Xrm.Page.getAttribute("name").getValue();
        quote.LogicalName = "quote";


        // var bankid = {};
        // bankid.Id = Xrm.Page.getAttribute("new_bankid").getValue()[0].id;
        // bankid.Name = Xrm.Page.getAttribute("new_bankid").getValue()[0].name;
        // bankid.LogicalName = Xrm.Page.getAttribute("new_bankid").getValue()[0].entityType;

        var transactioncurrencyid = {};
        transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
        transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
        transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;

        var salesAmount = Xrm.Page.getAttribute("totalamountlessfreight").getValue();
        var appCreditAmount = Xrm.Page.getAttribute("new_creditamount").getValue();
        //new_bankcreditstatus.new_bankid = bankid;
        new_bankcreditstatus.new_quoteid = quote;
        new_bankcreditstatus.TransactionCurrencyId = transactioncurrencyid;
        new_bankcreditstatus.new_appcreditamount = { Value: parseFloat(appCreditAmount).toFixed(2) };
        new_bankcreditstatus.new_salesamount = { Value: parseFloat(salesAmount).toFixed(2) };
        new_bankcreditstatus.new_contractdate = Xrm.Page.getAttribute("new_contractdate").getValue();

        //new_contractprocessdate_c
        if (Xrm.Page.getAttribute("new_contractprocessdate").getValue() != null) {
            new_bankcreditstatus.new_cancelendingdate = addDays(Xrm.Page.getAttribute("new_contractprocessdate").getValue(), 15)
        }
        var productId = Xrm.Page.getAttribute("new_productid").getValue()[0].id;
        var productDetail = GetProductPropertiesById(productId);

        new_bankcreditstatus.new_productnumber = productDetail.new_homenumber;
        var block = {};
        block.Id = productDetail.new_blockid.Id;
        block.Name = productDetail.new_blockid.Name;
        block.LogicalName = productDetail.new_blockid.LogicalName;

        new_bankcreditstatus.new_blockid = block;
        new_bankcreditstatus.new_brutalm2 = productDetail.new_grossm2;

        new_bankcreditstatus.new_name = quote.Name;
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_bankcreditstatusSet",
            data: JSON.stringify(new_bankcreditstatus),
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("CreatePrePaymentButtonRecord Error!!!" + XMLHttpRequest.responseText);

            }
        });
        alert("Banka Kredi Kaydı başarılı bir şekilde oluşturuldu.");
    }
}

function GetBankCreditType() {
    var bankId = Xrm.Page.getAttribute("new_bankid").getValue()[0].id;
    var creditPeriod = Xrm.Page.getAttribute("new_creditperiod").getValue();
    var currentCredit = new Array();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_bankSet?$filter=new_bankId eq guid'" + bankId + "'",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                currentCredit = data.d.results[0];
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckBankCreditPaymentPlan Error!!!");

        }
    });

    switch (creditPeriod) {
        case 1:
            if (currentCredit.new_12months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_12months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 2:
            // 24 Ay
            if (currentCredit.new_24months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_24months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 3:
            //36 Ay
            if (currentCredit.new_36months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_36months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 4:
            //48 AY
            if (currentCredit.new_48months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_48months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 5:
            // 60 AY
            if (currentCredit.new_60months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_60months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 6:
            //72
            if (currentCredit.new_72months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_72months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 7:
            //84
            if (currentCredit.new_84months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_84months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 8:
            //96
            if (currentCredit.new_96months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_96months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 9:
            //108
            if (currentCredit.new_108months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_108months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
        case 10:
            //120
            if (currentCredit.new_120months != null) {
                Xrm.Page.getAttribute("new_creditrate").setSubmitMode("always");
                var newValue = parseFloat(currentCredit.new_120months);
                Xrm.Page.getAttribute("new_creditrate").setValue(newValue);
            }
            break;
    }
    Xrm.Page.data.save();
}

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

function GetProductPropertiesById(productId) {
    var product = new Array();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        //url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ProductSet?$filter=ProductId eq (guid'" + productId + "')",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ProductSet?$filter=ProductId eq (guid'" + productId + "')&$select=new_homenumber,new_blockid,new_grossm2",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                product = data.d.results[0];
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckBankCreditPaymentPlan Error!!!");

        }
    });
    return product;
}
function OpenBankCreaditPaymenPlanForm(attribute, attributeLabel) {

    if (document.getElementById(attribute) != null) {
        var sFieldID = "field" + attribute;
        var elementID = document.getElementById(attribute + "_d");
        var div = document.createElement("div");
        div.style.width = "100%";
        div.style.textAlign = "right";
        div.style.display = "inline";
        elementID.appendChild(div, elementID);
        div.innerHTML = '<button id="' + sFieldID + "Create" + '"  style="background-color:#44c767;border:none;display:inline-block;cursor:pointer;color:#ffffff;font-family:Segoe UI,Arial,Sans-Serif;font-size:12px;font-weight:bold;text-decoration:none;background-image:none;">Banka Kredi</button>';
        document.getElementById(attribute).style.display = "none";
        document.getElementById(sFieldID + "Create").onclick = function () { CreateButton(attributeLabel); };
    }
}

function MakeBankCreditButton() {

    var attribute = "new_btnbankcreditdoc";
    var attributeLabel = "Banka Kredi";

    if (document.getElementById(attribute) != null) {
        var sFieldID = "field" + attribute;
        var elementID = document.getElementById(attribute + "_d");
        var div = document.createElement("div");
        div.style.width = "100%";
        div.style.textAlign = "right";
        div.style.display = "inline";
        elementID.appendChild(div, elementID);
        div.innerHTML = '<button id="' + sFieldID + "Create" + '"  style="background-color:#44c767;border:none;display:inline-block;cursor:pointer;color:#ffffff;font-family:Segoe UI,Arial,Sans-Serif;font-size:12px;font-weight:bold;text-decoration:none;background-image:none;">OLUŞTUR</button>';
        document.getElementById(attribute).style.display = "none";
        document.getElementById(sFieldID + "Create").onclick = function () {

            if (Xrm.Page.getAttribute("new_bankid").getValue() == null) {
                alert("Lütfen Banka Giriniz!!!");
                Xrm.Page.getControl("new_bankid").setFocus(true);
                return;
            }

            var bankCreditId = Xrm.Page.data.entity.getId();
            bankCreditId = bankCreditId.replace("{", "").replace("}", "");
            var Url = window.location.protocol + "//" + window.location.host + ":9090/BankCreditPlan.aspx?Id=" + bankCreditId;
            window.open(Url);
        };
    }
}

function MakeCreateButton(attribute, attributeLabel) {

    if (document.getElementById(attribute) != null) {
        var sFieldID = "field" + attribute;
        var elementID = document.getElementById(attribute + "_d");
        var div = document.createElement("div");
        div.style.width = "100%";
        div.style.textAlign = "right";
        div.style.display = "inline";
        elementID.appendChild(div, elementID);
        div.innerHTML = '<button id="' + sFieldID + "Create" + '"  style="background-color:#44c767;border:none;display:inline-block;cursor:pointer;color:#ffffff;font-family:Segoe UI,Arial,Sans-Serif;font-size:12px;font-weight:bold;text-decoration:none;background-image:none;">OLUŞTUR</button>';
        document.getElementById(attribute).style.display = "none";
        document.getElementById(sFieldID + "Create").onclick = function () { CreateButton(attributeLabel); };
    }
}

function MakeUpdateButton(attribute, attributeLabel) {


    if (document.getElementById(attribute) != null) {
        var sFieldID = "field" + attribute;
        var elementID = document.getElementById(attribute + "_d");
        var div = document.createElement("div");
        div.style.width = "100%";
        div.style.textAlign = "right";
        div.style.display = "inline";
        elementID.appendChild(div, elementID);
        div.innerHTML = '<button id="' + sFieldID + "Update" + '"  style="background-color:#002050;border:none;margin-left:50px;display:inline-block;cursor:pointer;color:#ffffff;font-family:Segoe UI,Arial,Sans-Serif;font-size:12px;font-weight:bold;text-decoration:none;background-image:none;">GÜNCELLE</button>';
        document.getElementById(attribute).style.display = "none";
        document.getElementById(sFieldID + "Update").onclick = function () { UpdateButton(attributeLabel); };
    }
}

function CreateButton(attributeLabel) {

    switch (attributeLabel) {
        case "Peşin": CreateVoucher();
            break;
        case "Ara Ödeme": CreateInterval();
            break;
        case "Düzenli Taksit": CreateInstallment();
            break;
        case "Kredi": CreateCredit();
            break;
        case "Kapora": CreatePrePaymentButton();
            break;
        case "Banka Kredi": BankCreditPaymentPlan();
            break;
    }
}

function UpdateButton(attributeLabel) {
    switch (attributeLabel) {
        case "Peşin": UpdateVoucher();
            break;
        case "Ara Ödeme": UpdateInterval();
            break;
        case "Düzenli Taksit": UpdateInstallment();
            break;
        case "Kredi": UpdateCredit();
            break;
    }
}
function ChangePaymentPlan() {
    if (Xrm.Page.getAttribute("new_paymentplan").getValue() != null) {
        var paymentPlan = Xrm.Page.getAttribute("new_paymentplan").getValue();
        if (paymentPlan == false) {//Kredisiz
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_3").setVisible(true);
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_5").setVisible(true);
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_4").setVisible(false);
            Xrm.Page.getAttribute("new_financialoptionid").setRequiredLevel("required");
            Xrm.Page.ui.tabs.get("tab_6").sections.get("tab_6_section_4").setVisible(true);

        }
        else {//Banka Kredili Ödeme Planı
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_3").setVisible(false);
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_5").setVisible(false);
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_4").setVisible(true);
            Xrm.Page.getAttribute("new_financialoptionid").setRequiredLevel("none");
            Xrm.Page.getAttribute("new_financialoptionid").setValue(null);
            Xrm.Page.ui.tabs.get("tab_6").sections.get("tab_6_section_4").setVisible(false);



        }
    }
    CalculatePrePayment();
}
function ChangeDiscount(context) {
    var discountPercentage = Xrm.Page.getAttribute("discountpercentage").getValue();
    var discountAmount = Xrm.Page.getAttribute("discountamount").getValue();
    if (discountPercentage != null && discountAmount != null) {
        alert("Lütfen indirim değerlerinden sadece birini giriniz!!!");
        Xrm.Page.getControl("discountpercentage").setFocus(true);
        context.getEventArgs().preventDefault();
        return;
    }
}
function CalculatePrePayment() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var totalVoucherAmount = GetTotalPaymentByType(quoteId, 3);//Peşin Ödeme
    var sumInterval = GetTotalPaymentByType(quoteId, 1);//Ara Ödeme
    var instAmount = GetTotalPaymentByType(quoteId, 2);//Taksitli
    var creditPaymentAmount = GetTotalPaymentByType(quoteId, 9);//Banka Kredisi

    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();
    // var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();
    //var sumInterval = Xrm.Page.getAttribute("new_suminterval").getValue();
    //var instAmount = Xrm.Page.getAttribute("new_instamount").getValue();
    // var creditPaymentAmount = Xrm.Page.getAttribute("new_creditpaymentamount").getValue();
    var paymentPlan = Xrm.Page.getAttribute("new_paymentplan").getValue();
    var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();

    if (Xrm.Page.ui.getFormType() == 1) {
        return;
    }
    if (totalAmount == 0) {
        return;
    }
    if (totalVoucherAmount == null) {
        totalVoucherAmount = 0;
    }

    Xrm.Page.getAttribute("new_sumcash").setValue(prePaymentAmount + totalVoucherAmount);


    var paymentRatio = ((prePaymentAmount + totalVoucherAmount) / totalAmount) * 100;
    var voucherRatio = (totalVoucherAmount / totalAmount) * 100;
    var instratio = (instAmount / totalAmount) * 100;
    var intervalRatio = (sumInterval / totalAmount) * 100;
    Xrm.Page.getAttribute("new_instratio").setValue(instratio);
    Xrm.Page.getAttribute("new_intervalratio").setValue(intervalRatio);
    Xrm.Page.getAttribute("new_paymentratio").setValue(paymentRatio);
    Xrm.Page.getAttribute("new_voucherratio").setValue(voucherRatio);



    if (paymentPlan) {//Kredisiz
        Xrm.Page.getAttribute("new_sumtotalpayment").setValue(prePaymentAmount + totalVoucherAmount + sumInterval + instAmount);
        Xrm.Page.getAttribute("new_totalcashpayment").setValue(prePaymentAmount);
        Xrm.Page.getAttribute("new_remainingpaymentamount").setValue(totalAmount - (prePaymentAmount + totalVoucherAmount + sumInterval + instAmount));
    }
    else {//Banka Kredili
        Xrm.Page.getAttribute("new_sumtotalpayment").setValue(prePaymentAmount + totalVoucherAmount + sumInterval + instAmount + creditPaymentAmount);
        Xrm.Page.getAttribute("new_totalcashpayment").setValue(prePaymentAmount + totalVoucherAmount);
        Xrm.Page.getAttribute("new_cashpaymentbankaccount").setValue(totalVoucherAmount);
        Xrm.Page.getAttribute("new_creditamount").setValue(creditPaymentAmount);
        Xrm.Page.getAttribute("new_instamount").setValue(instAmount);
        Xrm.Page.getAttribute("new_intervalpaymentvsum").setValue(sumInterval);
        Xrm.Page.getAttribute("new_remainingpaymentamount").setValue(totalAmount - (prePaymentAmount + totalVoucherAmount + sumInterval + instAmount + creditPaymentAmount));


    }

    Xrm.Page.getAttribute("new_paymentratio").setSubmitMode("always");
    Xrm.Page.getAttribute("new_sumcash").setSubmitMode("always");
    Xrm.Page.getAttribute("new_totalcashpayment").setSubmitMode("always");
}
function ChangeRatio()//Kredisiz ödeme planı
{
    var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
    var voucherRatio = Xrm.Page.getAttribute("new_voucherratio").getValue();//Peşinat Taksitleri %
    var intervalRatio = Xrm.Page.getAttribute("new_intervalratio").getValue();//Ara Ödeme %
    var instraRatio = Xrm.Page.getAttribute("new_instratio").getValue();//Taksitlendirme %        
    if (voucherRatio != 0 && voucherRatio != null) {
        var total = (voucherRatio * totalAmount) / 100;
        Xrm.Page.getAttribute("new_totalvoucheramount").setValue(total);
    }
    else {
        Xrm.Page.getAttribute("new_totalvoucheramount").setValue(0);
    }
    if (intervalRatio != 0 && intervalRatio != null) {
        var total = (intervalRatio * totalAmount) / 100;
        Xrm.Page.getAttribute("new_suminterval").setValue(total);
    }
    else {
        Xrm.Page.getAttribute("new_suminterval").setValue(0);
    }
    if (instraRatio != 0 && instraRatio != null) {
        var total = (instraRatio * totalAmount) / 100;
        Xrm.Page.getAttribute("new_instamount").setValue(total);
    }
    else {
        Xrm.Page.getAttribute("new_instamount").setValue(0);
    }
}
function ChangeAmount() {
    var paymentPlan = Xrm.Page.getAttribute("new_paymentplan").getValue();
    var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
    var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();//Peşinat Senetleri Toplamı
    var sumInterval = Xrm.Page.getAttribute("new_suminterval").getValue();//Ara Ödeme Toplamı
    var instAmount = Xrm.Page.getAttribute("new_instamount").getValue();//Taksitlendirilecek Tutar (Düzenli Taksit)
    var cashPaymentBankAccount = Xrm.Page.getAttribute("new_cashpaymentbankaccount").getValue();//Peşin Ödeme (Banka Kredili Ödeme Planında)
    var minCashPayment = Xrm.Page.getAttribute("new_mincashpayment").getValue();//Peşin Ödeme (Banka Kredili Ödeme Planında)
    var creditAmount = Xrm.Page.getAttribute("new_creditpaymentamount").getValue();//Kredilenecek Tutar
    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();//Kapora Tutarı
    if (prePaymentAmount == null) {
        prePaymentAmount = 0;
    }

    if (paymentPlan == true) {//Kredisiz
        if (totalVoucherAmount != 0 && totalVoucherAmount != null) {
            var ratio = (totalVoucherAmount / totalAmount) * 100;
            Xrm.Page.getAttribute("new_voucherratio").setValue(ratio);
        }
        else {
            Xrm.Page.getAttribute("new_voucherratio").setValue(0);
        }
        if (sumInterval != 0 && sumInterval != null) {
            var ratio = (sumInterval / totalAmount) * 100;
            Xrm.Page.getAttribute("new_intervalratio").setValue(ratio);
        }
        else {
            Xrm.Page.getAttribute("new_intervalratio").setValue(0);
        }
        if (instAmount != 0 && instAmount != null) {
            var ratio = (instAmount / totalAmount) * 100;
            Xrm.Page.getAttribute("new_instratio").setValue(ratio);
        }
        else {
            Xrm.Page.getAttribute("new_instratio").setValue(0);
        }

    }
    else {//Banka Kredili Ödeme Planı       
        if (totalVoucherAmount != 0 && totalVoucherAmount != null) {
            Xrm.Page.getAttribute("new_cashpaymentbankaccount").setValue(totalVoucherAmount);
        }
        else {
            Xrm.Page.getAttribute("new_cashpaymentbankaccount").setValue(0);
        }
        if (minCashPayment != 0 && minCashPayment != null) {
            if (totalVoucherAmount < minCashPayment) {
                alert("Girilen Peşin Ödeme Tutarı, Minimum Peşin Ödeme Tutarından Küçük Olamaz!!!");
                Xrm.Page.getAttribute("new_totalvoucheramount").setValue(minCashPayment);
                Xrm.Page.getAttribute("new_totalvoucheramount").fireOnChange();
            }
        }
        if (sumInterval != 0 && sumInterval != null) {
            Xrm.Page.getAttribute("new_intervalpaymentvsum").setValue(sumInterval);
        }
        else {
            Xrm.Page.getAttribute("new_intervalpaymentvsum").setValue(0);
        }
        if (creditAmount != 0 && creditAmount != null) {
            Xrm.Page.getAttribute("new_creditamount").setValue(creditAmount);
        }
        else {
            Xrm.Page.getAttribute("new_creditamount").setValue(0);
        }
        if (instAmount != 0 && instAmount != null) {
            Xrm.Page.getAttribute("new_instamount").setValue(instAmount);
        }
        else {
            Xrm.Page.getAttribute("new_instamount").setValue(0);
        }
        Xrm.Page.getAttribute("new_totalcashpayment").setValue(Xrm.Page.getAttribute("new_cashpaymentbankaccount").getValue() + prePaymentAmount);
    }

}
function ChangeFinancialOption() {
    var financialOptionId = Xrm.Page.getAttribute("new_financialoptionid").getValue();
    if (financialOptionId != null) {
        financialOptionId = financialOptionId[0].id.replace("{", "").replace("}", "");
        var financialOption = GetFinancialOptionById(financialOptionId);
        if (financialOption != null) {
            var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
            Xrm.Page.getAttribute("new_mincashpayment").setValue(parseFloat((financialOption.new_minpaymentratio * totalAmount) / 100));
        }
        else {
            Xrm.Page.getAttribute("new_mincashpayment").setValue(null);
        }
    }
    else {
        Xrm.Page.getAttribute("new_mincashpayment").setValue(null);
    }

}

function CreateVoucher() {
    var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();//Peşinat Senetleri Toplamı
    var type = Xrm.Page.getAttribute("new_type").getValue();//Senet Tipi
    var voucherNumber = Xrm.Page.getAttribute("new_vouchernumber").getValue();//Peşinat Senetleri Adedi
    var payStartDate = Xrm.Page.getAttribute("new_paydate").getValue();//Ödeme Başlangıç Tarihi (Peşinat)
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan Ödeme Tutarı
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Peşinat Senetleri Toplamını Giriniz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_type").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("Lütfen Peşinat Senet Adedini Giriniz!!!");
        Xrm.Page.getControl("new_vouchernumber").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan Ödeme Tutarından Büyük Olamaz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("Lütfen Ödeme Başlangıç Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_paydate").setFocus(true);
        return;
    }
    else {
        var CurrentDate = new Date();
        var _date = CurrentDate.getDate();
        CurrentDate.setDate(1);
        CurrentDate.setMonth(CurrentDate.getMonth() + 1);
        CurrentDate.setDate(_date);


        if (payStartDate > CurrentDate) {
            alert("Lütfen Ödeme Tarihinini En Fazla Bugünden 1 Ay Sonrasını Giriniz!!!");
            return;
        }
        else {
            CreateVoucherRecord(totalVoucherAmount, type, voucherNumber, payStartDate);
        }
    }

}
function CreateVoucherRecord(totalVoucherAmount, type, voucherNumber, payStartDate) {

    var paymentAmountBase = parseFloat(totalVoucherAmount / voucherNumber).toFixed(2);
    var accountList = GetCollaborateList();

    var date = payStartDate;
    for (var i = 0; i < voucherNumber; i++) {
        for (var j = 0; j < accountList.length; j++) {
            var payment = {};
            var quote = {};


            var collaborateAccountId = {};
            collaborateAccountId.Id = accountList[j].Id.Id;
            collaborateAccountId.Name = accountList[j].Id.Name;
            collaborateAccountId.LogicalName = accountList[j].Id.LogicalName;
            payment.new_collaborateaccountid = collaborateAccountId;
            var paymentAmount = parseFloat((paymentAmountBase * accountList[j].Rate) / 100).toFixed(2);
            payment.new_paymentamount = { Value: paymentAmount };

            if (Xrm.Page.getAttribute("new_financialaccountid").getValue() != null) {
                var financialAccount = {};//Cari
                financialAccount.Id = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].id;
                financialAccount.Name = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].name;
                financialAccount.LogicalName = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].entityType;
                payment.new_financialaccountid = financialAccount;
            }


            var quoteId = Xrm.Page.data.entity.getId();
            quote.Id = quoteId.replace("{", "").replace("}", "");
            quote.Name = Xrm.Page.getAttribute("name").getValue();
            quote.LogicalName = "quote";
            var transactioncurrencyid = {};
            transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
            transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
            transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;
            var customerid = {};
            customerid.Id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
            customerid.Name = Xrm.Page.getAttribute("customerid").getValue()[0].name;
            customerid.LogicalName = Xrm.Page.getAttribute("customerid").getValue()[0].entityType;
            payment.new_quoteid = quote;
            payment.new_type = { Value: 3 };// Ödeme Türü Peşinat Ödemesi (Senet)
            payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
            payment.new_date = date;
            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "Peşin Ödeme - " + date.format("dd.MM.yyyy");
            if (customerid.LogicalName == "contact") {
                payment.new_contactid = customerid;
            }
            else {
                payment.new_accountid = customerid;
            }

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
                data: JSON.stringify(payment),
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    //Specifying this header ensures that the results will be returned as JSON.
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("CreateVoucherRecord Error!!!");

                }
            });
        }
    }
    Xrm.Page.getAttribute("new_type").setValue(null);//Senet Tipi       
    Xrm.Page.getAttribute("new_paydate").setValue(null);//Ödeme Başlangıç Tarihi (Peşinat)
    var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
    gridControl.refresh();
    Xrm.Page.data.save();

    tabdisable("tab_6", false);
}
function CheckVoucherRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 3;//Peşinat Ödemesi Senet
    var result = true;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_type/Value eq " + type + " and new_quoteid/Id eq guid'" + quoteId + "'&$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;
            }
            else {
                result = true;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckVoucherRecord Error!!!");
            result = false;
        }
    });
    return result;

}

function UpdateVoucher() {
    if (Xrm.Page.getAttribute("statecode").getValue() != 0) {
        return;
    }
    var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();//Peşin Ödeme Toplamı
    var type = Xrm.Page.getAttribute("new_type").getValue();//Senet Tipi   
    var payStartDate = Xrm.Page.getAttribute("new_paydate").getValue();//Ödeme Tarihi (Peşin)
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Peşin Ödeme Toplamını Giriniz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_type").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("Lütfen Ödeme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_paydate").setFocus(true);
        return;
    }
    else {
        var CurrentDate = new Date();
        var _date = CurrentDate.getDate();
        CurrentDate.setDate(1);
        CurrentDate.setMonth(CurrentDate.getMonth() + 1);
        CurrentDate.setDate(_date);


        if (payStartDate > CurrentDate) {
            alert("Lütfen Ödeme Tarihinini En Fazla Bugünden 1 Ay Sonrasını Giriniz!!!");
            return;
        }
        else {
            UpdateVoucherRecord(totalVoucherAmount, type, payStartDate);
        }
    }
}
function UpdateVoucherRecord(totalVoucherAmount, type, payStartDate) {
    var grid = document.getElementById("paymentgrid").control;
    if (grid.get_selectedRecords().length > 0) {
        var amount = parseFloat(totalVoucherAmount).toFixed(2);
        var grid = document.getElementById("paymentgrid").control;
        var paymentId = grid.get_selectedRecords()[0].Id;
        paymentId = paymentId.replace("{", "").replace("}", "");
        var payment = {};
        payment.new_type = { Value: 3 };// Ödeme Türü Peşin Ödeme
        payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
        payment.new_date = payStartDate;
        payment.new_paymentamount = { Value: amount };
        var now = new Date();
        payment.new_name = "Peşin Ödeme - " + payStartDate.format("dd.MM.yyyy");

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet(guid'" + paymentId + "')",
            data: JSON.stringify(payment),
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            success: function (data, textStatus, XmlHttpRequest) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("UpdateVoucherRecord Error!!!");

            }
        });

        Xrm.Page.getAttribute("new_type").setValue(null);//Senet Tipi        
        Xrm.Page.getAttribute("new_paydate").setValue(null);//Ödeme  Tarihi
        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}


function CreateInterval() {
    var totalVoucherAmount = Xrm.Page.getAttribute("new_suminterval").getValue();//Ara Ödeme Toplamı
    var type = Xrm.Page.getAttribute("new_itype").getValue();//Senet Tipi
    var voucherNumber = Xrm.Page.getAttribute("new_intervalnumber").getValue();//Ara Ödeme Adedi
    var payStartDate = Xrm.Page.getAttribute("new_istartdate").getValue();//Ödeme Başlangıç Tarihi (Ara Ödemeler)
    var intervalPeriod = Xrm.Page.getAttribute("new_intervalperiod").getValue();//Ara Ödeme Periyodu
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan Ödeme Tutarı

    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Ara Ödeme Toplamını Giriniz!!!");
        Xrm.Page.getControl("new_suminterval").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_itype").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("Lütfen Ara Ödeme Adedini Giriniz!!!");
        Xrm.Page.getControl("new_intervalnumber").setFocus(true);
        return;
    }
    if (voucherNumber > 48) {
        alert("Ara Ödeme Adedi 48'den Fazla Olamaz!!!");
        Xrm.Page.getControl("new_intervalnumber").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("Lütfen Ödeme Başlangıç Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_istartdate").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan Ödeme Tutarından Büyük Olamaz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (intervalPeriod == null) {
        alert("Lütfen Ara Ödeme Periyodunu Giriniz!!!");
        Xrm.Page.getControl("new_intervalperiod").setFocus(true);
        return;
    }
    else {
        CreateIntervalRecord(totalVoucherAmount, type, voucherNumber, payStartDate, intervalPeriod);
        //var now = new Date();
        //var _date = now.getDate();
        //var _month = now.getMonth();
        //var _year = now.getFullYear();
        //now = new Date(_year, _month, _date);
        //if (payStartDate < now) {
        //    alert("Lütfen Ödeme Başlangıç Tarihinini Bugün veya İleri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    CreateIntervalRecord(totalVoucherAmount, type, voucherNumber, payStartDate, intervalPeriod);
        //}
    }

}
function CreateIntervalRecord(totalVoucherAmount, type, voucherNumber, payStartDate, intervalPeriod) {
    //if (CheckIntervalRecord() == false) {
    //    alert("Ara Ödeme Kaydı Vardır. \n Lütfen İlgili Kaydı Güncelleyiniz. ");
    //}
    //else {
    // var paymentAmountBase = parseFloat(totalVoucherAmount / voucherNumber).toFixed(2);
    var paymentAmountBase = parseInt(totalVoucherAmount / voucherNumber);
    var total = 0;
    var accountList = GetCollaborateList();

    var date = payStartDate;
    for (var i = 0; i < voucherNumber; i++) {

        if (i == voucherNumber - 1) {//1000 TL 3'e böldüğümüzde 333,33... Olarak bölmeyi düzeltmesi için
            paymentAmountBase = totalVoucherAmount - total;
        }
        else {
            total += paymentAmountBase;
        }
        for (var j = 0; j < accountList.length; j++) {

            var payment = {};
            var quote = {};
            var collaborateAccountId = {};
            collaborateAccountId.Id = accountList[j].Id.Id;
            collaborateAccountId.Name = accountList[j].Id.Name;
            collaborateAccountId.LogicalName = accountList[j].Id.LogicalName;
            payment.new_collaborateaccountid = collaborateAccountId;
            var paymentAmount = parseFloat((paymentAmountBase * accountList[j].Rate) / 100).toFixed(2);
            payment.new_paymentamount = { Value: paymentAmount };

            if (Xrm.Page.getAttribute("new_financialaccountid").getValue() != null) {
                var financialAccount = {};//Cari
                financialAccount.Id = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].id;
                financialAccount.Name = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].name;
                financialAccount.LogicalName = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].entityType;
                payment.new_financialaccountid = financialAccount;
            }
            var quoteId = Xrm.Page.data.entity.getId();
            quote.Id = quoteId.replace("{", "").replace("}", "");
            quote.Name = Xrm.Page.getAttribute("name").getValue();
            quote.LogicalName = "quote";
            var transactioncurrencyid = {};
            transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
            transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
            transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;
            var customerid = {};
            customerid.Id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
            customerid.Name = Xrm.Page.getAttribute("customerid").getValue()[0].name;
            customerid.LogicalName = Xrm.Page.getAttribute("customerid").getValue()[0].entityType;
            payment.new_quoteid = quote;
            payment.new_type = { Value: 1 };// Ödeme Türü Ara Ödeme
            payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
            payment.new_date = date;

            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "Ara Ödeme - " + date.format("dd.MM.yyyy");
            if (customerid.LogicalName == "contact") {
                payment.new_contactid = customerid;
            }
            else {
                payment.new_accountid = customerid;
            }

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
                data: JSON.stringify(payment),
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    //Specifying this header ensures that the results will be returned as JSON.
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("CreateVoucherRecord Error!!!");

                }
            });
        }
        var CurrentDate = new Date(date);
        var _date = CurrentDate.getDate();
        CurrentDate.setDate(1);
        CurrentDate.setMonth(CurrentDate.getMonth() + intervalPeriod);
        CurrentDate.setDate(_date);
        date = CurrentDate;
    }
    Xrm.Page.getAttribute("new_itype").setValue(null);//Senet Tipi
    Xrm.Page.getAttribute("new_intervalnumber").setValue(null);//Ara Ödeme Adedi
    Xrm.Page.getAttribute("new_istartdate").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
    Xrm.Page.getAttribute("new_intervalperiod").setValue(null);//Ara Ödeme Periyodu

    var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
    gridControl.refresh();
    Xrm.Page.data.save();
    // }
    tabdisable("tab_6", false);
}
function CheckIntervalRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 1;//Ara Ödeme
    var result = true;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_type/Value eq " + type + " and new_quoteid/Id eq guid'" + quoteId + "'&$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;
            }
            else {
                result = true;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckIntervalRecord Error!!!");
            result = false;
        }
    });
    return result;

}

function UpdateInterval() {
    if (Xrm.Page.getAttribute("statecode").getValue() != 0) {
        return;
    }
    var totalVoucherAmount = Xrm.Page.getAttribute("new_ipaymentamount").getValue();//Ödenecek Tutar (Ara Ödemeler)
    var type = Xrm.Page.getAttribute("new_itype").getValue();//Senet Tipi  
    var payStartDate = Xrm.Page.getAttribute("new_ipaymentdate").getValue();//Ödeme Tarihi (Ara Ödemeler)
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Ödenecek Tutarı Giriniz!!!");
        Xrm.Page.getControl("new_ipaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_itype").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("Lütfen Ödeme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_istartdate").setFocus(true);
        return;
    }
    else {
        UpdateIntervalRecord(totalVoucherAmount, type, payStartDate);
        //var now = new Date();
        //var _date = now.getDate();
        //var _month = now.getMonth();
        //var _year = now.getFullYear();
        //now = new Date(_year, _month, _date);
        //if (payStartDate < now) {
        //    alert("Lütfen Ödeme Başlangıç Tarihinini Bugün veya İleri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    UpdateIntervalRecord(totalVoucherAmount, type, payStartDate);
        //}
    }

}
function UpdateIntervalRecord(totalVoucherAmount, type, payStartDate) {
    var grid = document.getElementById("paymentgrid").control;
    if (grid.get_selectedRecords().length > 0) {
        var paymentAmount = parseFloat(totalVoucherAmount).toFixed(2);
        var grid = document.getElementById("paymentgrid").control;
        var paymentId = grid.get_selectedRecords()[0].Id;
        paymentId = paymentId.replace("{", "").replace("}", "");
        var payment = {};
        payment.new_type = { Value: 1 };// Ödeme Türü Ara Ödeme
        payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
        payment.new_date = payStartDate;
        payment.new_paymentamount = { Value: paymentAmount };
        payment.new_isupdated = true;
        var now = new Date();
        payment.new_name = "Ara Ödeme - " + payStartDate.format("dd.MM.yyyy");

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet(guid'" + paymentId + "')",
            data: JSON.stringify(payment),
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            success: function (data, textStatus, XmlHttpRequest) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("UpdateIntervalRecord Error!!!");

            }
        });

        Xrm.Page.getAttribute("new_itype").setValue(null);//Senet Tipi
        Xrm.Page.getAttribute("new_ipaymentdate").setValue(null);//Ara Ödeme Adedi
        Xrm.Page.getAttribute("new_istartdate").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
        Xrm.Page.getAttribute("new_ipaymentamount").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
        Xrm.Page.getAttribute("new_intervalperiod").setValue(null);//Ara Ödeme Periyodu

        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}


function CreateInstallment() {
    var totalVoucherAmount = Xrm.Page.getAttribute("new_instamount").getValue();//Taksitlendirilecek Tutar
    var type = Xrm.Page.getAttribute("new_insttype").getValue();//Senet Tipi
    var voucherNumber = Xrm.Page.getAttribute("new_instnumber").getValue();//Taksit Adedi
    var payStartDate = Xrm.Page.getAttribute("new_inststartdate").getValue();//Taksit Başlangıç Tarihi
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan Ödeme Tutarı
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Taksitlendirilecek Tutarı Giriniz!!!");
        Xrm.Page.getControl("new_instamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttype").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("Lütfen Taksit Adedini Giriniz!!!");
        Xrm.Page.getControl("new_instnumber").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan Ödeme Tutarından Büyük Olamaz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("Lütfen Taksit Başlangıç Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_inststartdate").setFocus(true);
        return;
    }
    else {
        CreateInstallmentRecord(totalVoucherAmount, type, voucherNumber, payStartDate);
        //var now = new Date();
        //if (payStartDate <= now) {
        //    alert("Lütfen Ödeme Başlangıç Tarihinini Bugünden İleri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    CreateInstallmentRecord(totalVoucherAmount, type, voucherNumber, payStartDate);
        //}
    }

}
function CreateInstallmentRecord(totalVoucherAmount, type, voucherNumber, payStartDate) {
    if (CheckInstallmentRecord() == false) {
        alert("Taksit Ödeme Kaydı Vardır. \n Lütfen İlgili Kaydı Güncelleyiniz. ");
    }
    else {

        //var paymentAmountBase = parseFloat(totalVoucherAmount / voucherNumber).toFixed(2);       
        var paymentAmountBase = parseInt(totalVoucherAmount / voucherNumber);
        var total = 0;
        var accountList = GetCollaborateList();
        var date = payStartDate;
        for (var i = 0; i < voucherNumber; i++) {
            if (i == voucherNumber - 1) {//1000 TL 3'e böldüğümüzde 333,33... Olarak bölmeyi düzeltmesi için
                paymentAmountBase = totalVoucherAmount - total;
            }
            else {
                total += paymentAmountBase;
            }

            var paymentList = GetIntervalListByDate();
            if (paymentList != null && paymentList.length > 0) {
                for (var k = 0; k < paymentList.length; k++) {
                    var paymentDate = new Date(parseInt(paymentList[k].substr(6)));
                    var _month = paymentDate.getMonth();
                    var _year = paymentDate.getFullYear();
                    if (date.getMonth() == _month && date.getFullYear() == _year) {
                        var CurrentDate = new Date(date);
                        var _date = CurrentDate.getDate();
                        CurrentDate.setDate(1);
                        CurrentDate.setMonth(CurrentDate.getMonth() + 1);
                        CurrentDate.setDate(_date);
                        date = CurrentDate;
                        k--;
                    }
                }

            }
            else {
                //payment.new_date = date;
                //var CurrentDate = new Date(date);
                //var _date = CurrentDate.getDate();
                //CurrentDate.setDate(1);
                //CurrentDate.setMonth(CurrentDate.getMonth() + 1);
                //CurrentDate.setDate(_date);
                //date = CurrentDate;
            }



            for (var j = 0; j < accountList.length; j++) {
                var payment = {};
                var quote = {};
                payment.new_date = date;
                var collaborateAccountId = {};
                collaborateAccountId.Id = accountList[j].Id.Id;
                collaborateAccountId.Name = accountList[j].Id.Name;
                collaborateAccountId.LogicalName = accountList[j].Id.LogicalName;
                payment.new_collaborateaccountid = collaborateAccountId;
                var paymentAmount = parseFloat((paymentAmountBase * accountList[j].Rate) / 100).toFixed(2);
                payment.new_paymentamount = { Value: paymentAmount };
                if (Xrm.Page.getAttribute("new_financialaccountid").getValue() != null) {
                    var financialAccount = {};//Cari
                    financialAccount.Id = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].id;
                    financialAccount.Name = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].name;
                    financialAccount.LogicalName = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].entityType;
                    payment.new_financialaccountid = financialAccount;
                }
                var quoteId = Xrm.Page.data.entity.getId();
                quote.Id = quoteId.replace("{", "").replace("}", "");
                quote.Name = Xrm.Page.getAttribute("name").getValue();
                quote.LogicalName = "quote";
                var transactioncurrencyid = {};
                transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
                transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
                transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;
                var customerid = {};
                customerid.Id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
                customerid.Name = Xrm.Page.getAttribute("customerid").getValue()[0].name;
                customerid.LogicalName = Xrm.Page.getAttribute("customerid").getValue()[0].entityType;
                payment.new_quoteid = quote;
                payment.new_type = { Value: 2 };// Ödeme Türü Taksit
                payment.new_vtype = { Value: type };//Senet Muhasebe Tipi






                payment.TransactionCurrencyId = transactioncurrencyid;
                var now = new Date();
                payment.new_name = "Düzenli Taksit - " + date.format("dd.MM.yyyy");
                if (customerid.LogicalName == "contact") {
                    payment.new_contactid = customerid;
                }
                else {
                    payment.new_accountid = customerid;
                }

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    datatype: "json",
                    url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
                    data: JSON.stringify(payment),
                    async: false,
                    beforeSend: function (XMLHttpRequest) {
                        //Specifying this header ensures that the results will be returned as JSON.
                        XMLHttpRequest.setRequestHeader("Accept", "application/json");
                    },
                    success: function (data, textStatus, XmlHttpRequest) {

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("CreateVoucherRecord Error!!!");

                    }
                });
                payment.new_date = date;
            }
            var CurrentDate = new Date(date);
            var _date = CurrentDate.getDate();
            CurrentDate.setDate(1);
            CurrentDate.setMonth(CurrentDate.getMonth() + 1);
            CurrentDate.setDate(_date);
            date = CurrentDate;

        }
        Xrm.Page.getAttribute("new_insttype").setValue(null);//Senet Tipi
        Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara Ödeme Adedi
        Xrm.Page.getAttribute("new_inststartdate").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}
function CheckInstallmentRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 2;//Düzenli Taksit
    var result = true;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_type/Value eq " + type + " and new_quoteid/Id eq guid'" + quoteId + "'&$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;
            }
            else {
                result = true;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckInstallmentRecord Error!!!");
            result = false;
        }
    });
    return result;

}

function UpdateInstallment() {
    if (Xrm.Page.getAttribute("statecode").getValue() != 0) {
        return;
    }
    var totalVoucherAmount = Xrm.Page.getAttribute("new_instpaymentamount").getValue();//Ödenecek Tutar (Düzenli Taksit)
    var type = Xrm.Page.getAttribute("new_insttype").getValue();//Senet Tipi
    var payStartDate = Xrm.Page.getAttribute("new_instpaymentdate").getValue();//Ödeme Tarihi (Düzenli Taksit)
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Ödenecek Tutarı Giriniz!!!");
        Xrm.Page.getControl("new_instpaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttype").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("Lütfen Ödeme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_instpaymentdate").setFocus(true);
        return;
    }
    else {
        UpdateInstallmentRecord(totalVoucherAmount, type, payStartDate);
        //var now = new Date();
        //if (payStartDate <= now) {
        //    alert("Lütfen Ödeme Tarihini Bugünden İleri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    UpdateInstallmentRecord(totalVoucherAmount, type, payStartDate);
        //}
    }
}
function UpdateInstallmentRecord(totalVoucherAmount, type, payStartDate) {
    var grid = document.getElementById("paymentgrid").control;
    if (grid.get_selectedRecords().length > 0) {
        var date = payStartDate;
        var paymentAmount = parseFloat(totalVoucherAmount).toFixed(2);
        var grid = document.getElementById("paymentgrid").control;
        var paymentId = grid.get_selectedRecords()[0].Id;
        paymentId = paymentId.replace("{", "").replace("}", "");
        var payment = {};

        payment.new_type = { Value: 2 };// Ödeme Türü Taksit
        payment.new_vtype = { Value: type };//Senet Muhasebe Tipi        
        payment.new_paymentamount = { Value: paymentAmount };
        payment.new_isupdated = true;
        var now = new Date();
        var paymentList = GetIntervalListByDate();
        if (paymentList != null && paymentList.length > 0) {
            for (var j = 0; j < paymentList.length; j++) {
                var paymentDate = new Date(parseInt(paymentList[j].substr(6)));
                var _month = paymentDate.getMonth();
                var _year = paymentDate.getFullYear();
                if (date.getMonth() == _month && date.getFullYear() == _year) {
                    var CurrentDate = new Date(date);
                    var _date = CurrentDate.getDate();
                    CurrentDate.setDate(1);
                    CurrentDate.setMonth(CurrentDate.getMonth() + 1);
                    CurrentDate.setDate(_date);
                    date = CurrentDate;
                    j--;
                }
            }
            payment.new_date = date;

        }
        else {
            payment.new_date = date;
            //var CurrentDate = new Date(date);
            //var _date = CurrentDate.getDate();
            //CurrentDate.setDate(1);
            //CurrentDate.setMonth(CurrentDate.getMonth() + 1);
            //CurrentDate.setDate(_date);
            //date = CurrentDate;
        }


        payment.new_name = "Düzenli Taksit - " + date.format("dd.MM.yyyy");

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet(guid'" + paymentId + "')",
            data: JSON.stringify(payment),
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            success: function (data, textStatus, XmlHttpRequest) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("CreateVoucherRecord Error!!!");

            }
        });

        Xrm.Page.getAttribute("new_insttype").setValue(null);//Senet Tipi
        Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara Ödeme Adedi
        Xrm.Page.getAttribute("new_inststartdate").setValue(null);//Ödeme Başlangıç Tarihi (Ara Ödemeler)
        Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);//Ödeme Tarihi (Düzenli Taksit)
        Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);//Ödenecek Tutar (Düzenli Taksit)


        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}

function CreateCredit() {
    var totalVoucherAmount = Xrm.Page.getAttribute("new_creditpaymentamount").getValue();
    var type = Xrm.Page.getAttribute("new_insttypecreditpayment").getValue();//Senet Tipi
    var voucherNumber = Xrm.Page.getAttribute("new_creditpaymentnumber").getValue();
    var paytDate = Xrm.Page.getAttribute("new_creditpaymentdate").getValue();
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan Ödeme Tutarı
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Kredi Tutarı Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentamount").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan Ödeme Tutarından Büyük Olamaz!!!");
        Xrm.Page.getControl("new_creditpaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttypecreditpayment").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("Lütfen Adedini Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentnumber").setFocus(true);
        return;
    }
    if (paytDate == null) {
        alert("Lütfen Ödeme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentdate").setFocus(true);
        return;
    }
    else {
        CreateCreditRecord(totalVoucherAmount, type, voucherNumber, paytDate);
        //var now = new Date();
        //if (paytDate <= now) {
        //    alert("Lütfen Ödeme Tarihinini Bugünden İleri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    CreateCreditRecord(totalVoucherAmount, type, voucherNumber, paytDate);
        //}
    }
}
function CreateCreditRecord(totalVoucherAmount, type, voucherNumber, paytDate) {
    if (CheckCreditRecord() == false) {
        alert("Kredi Ödeme Kaydı Vardır. \n Lütfen İlgili Kaydı Güncelleyiniz. ");
    }
    else {
        var paymentAmountBase = parseFloat(totalVoucherAmount / voucherNumber).toFixed(2);
        var accountList = GetCollaborateList();
        for (var j = 0; j < accountList.length; j++) {
            var payment = {};
            var quote = {};
            var collaborateAccountId = {};
            collaborateAccountId.Id = accountList[j].Id.Id;
            collaborateAccountId.Name = accountList[j].Id.Name;
            collaborateAccountId.LogicalName = accountList[j].Id.LogicalName;
            payment.new_collaborateaccountid = collaborateAccountId;
            var paymentAmount = parseFloat((paymentAmountBase * accountList[j].Rate) / 100).toFixed(2);
            payment.new_paymentamount = { Value: paymentAmount };
            if (Xrm.Page.getAttribute("new_financialaccountid").getValue() != null) {
                var financialAccount = {};//Cari
                financialAccount.Id = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].id;
                financialAccount.Name = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].name;
                financialAccount.LogicalName = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].entityType;
                payment.new_financialaccountid = financialAccount;
            }
            var quoteId = Xrm.Page.data.entity.getId();
            quote.Id = quoteId.replace("{", "").replace("}", "");
            quote.Name = Xrm.Page.getAttribute("name").getValue();
            quote.LogicalName = "quote";
            var transactioncurrencyid = {};
            transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
            transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
            transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;
            var customerid = {};
            customerid.Id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
            customerid.Name = Xrm.Page.getAttribute("customerid").getValue()[0].name;
            customerid.LogicalName = Xrm.Page.getAttribute("customerid").getValue()[0].entityType;
            payment.new_quoteid = quote;
            payment.new_type = { Value: 9 };// Banka Kredisi
            payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
            payment.new_date = paytDate;
            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "Banka Kredisi - " + paytDate.format("dd.MM.yyyy");
            if (customerid.LogicalName == "contact") {
                payment.new_contactid = customerid;
            }
            else {
                payment.new_accountid = customerid;
            }

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
                data: JSON.stringify(payment),
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    //Specifying this header ensures that the results will be returned as JSON.
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("CreateCreditRecord Error!!!");

                }
            });

        }

        Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Senet Tipi
        Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//Ödeme Tarihi
        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}
function CheckCreditRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 9;//Banka Kredisi
    var result = true;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_type/Value eq " + type + " and new_quoteid/Id eq guid'" + quoteId + "'&$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;
            }
            else {
                result = true;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckCreditRecord Error!!!");
            result = false;
        }
    });
    return result;

}

function UpdateCredit() {
    if (Xrm.Page.getAttribute("statecode").getValue() != 0) {
        return;
    }
    var totalVoucherAmount = Xrm.Page.getAttribute("new_creditpaymentamount").getValue();
    var type = Xrm.Page.getAttribute("new_insttypecreditpayment").getValue();//Senet Tipi
    var paytDate = Xrm.Page.getAttribute("new_creditpaymentdate").getValue();
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("Lütfen Kredi Tutarı Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("Lütfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttypecreditpayment").setFocus(true);
        return;
    }
    if (paytDate == null) {
        alert("Lütfen Ödeme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentdate").setFocus(true);
        return;
    }
    else {
        UpdateCreditRecord(totalVoucherAmount, type, paytDate);
        //var now = new Date();
        //if (paytDate <= now) {
        //    alert("Lütfen Ödeme Tarihinini Bugünden İleri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    UpdateCreditRecord(totalVoucherAmount, type, paytDate);
        //}
    }
}
function UpdateCreditRecord(totalVoucherAmount, type, payStartDate) {
    var grid = document.getElementById("paymentgrid").control;
    if (grid.get_selectedRecords().length > 0) {
        var paymentAmount = parseFloat(totalVoucherAmount).toFixed(2);
        var grid = document.getElementById("paymentgrid").control;
        var paymentId = grid.get_selectedRecords()[0].Id;
        paymentId = paymentId.replace("{", "").replace("}", "");
        var payment = {};

        payment.new_type = { Value: 9 };// Ödeme Türü Banka Kredisi
        payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
        payment.new_date = payStartDate;
        payment.new_paymentamount = { Value: paymentAmount };
        var now = new Date();
        payment.new_name = "Banka Kredisi - " + payStartDate.format("dd.MM.yyyy");

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet(guid'" + paymentId + "')",
            data: JSON.stringify(payment),
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            success: function (data, textStatus, XmlHttpRequest) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("UpdateCreditRecord Error!!!");

            }
        });

        Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Senet Tipi
        Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//Ödeme Tarihi

        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}

function CreatePrePaymentButton() {

    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();//Kapora Tutarı
    var prePaymentType = Xrm.Page.getAttribute("new_prepaymenttype").getValue();//Kapora Ödeme Türü   
    var prePaymentDate = Xrm.Page.getAttribute("new_prepaymentdate").getValue();//Kapora Tarihi
    if (prePaymentAmount == 0 || prePaymentAmount == null) {
        alert("Lütfen Ön Ödeme Tutarını Giriniz!!!");
        Xrm.Page.getControl("new_prepaymentamount").setFocus(true);
        return;
    }
    if (prePaymentType == null) {
        alert("Lütfen Ödeme Türünü Seçiniz!!!");
        Xrm.Page.getControl("new_prepaymenttype").setFocus(true);
        return;
    }
    if (prePaymentDate == null) {
        alert("Lütfen Ön Ödeme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_prepaymentdate").setFocus(true);
        return;
    }
    else {
        CreatePrePaymentButtonRecord(prePaymentAmount, prePaymentType, prePaymentDate);
    }
}
function CreatePrePaymentButtonRecord(prePaymentAmount, prePaymentType, prePaymentDate) {
    if (CheckPrePaymentButtonRecord() == false) {
        alert("Ön Ödeme Kaydı Vardır. \n");
    }
    else {
        var paymentAmountBase = parseFloat(prePaymentAmount).toFixed(2);
        var projectId = Xrm.Page.getAttribute("new_projectid").getValue();
        if (projectId != null) {
            projectId = Xrm.Page.getAttribute("new_projectid").getValue()[0].id;
            projectId = projectId.replace("{", "").replace("}", "");

        }


        var accountList = GetCollaborateList();
        for (var j = 0; j < accountList.length; j++) {
            var paymentAmount = null;
            var payment = {};
            var quote = {};
            var collaborateAccountId = {};
            collaborateAccountId.Id = accountList[j].Id.Id;
            collaborateAccountId.Name = accountList[j].Id.Name;
            collaborateAccountId.LogicalName = accountList[j].Id.LogicalName;
            payment.new_collaborateaccountid = collaborateAccountId;
            ///
            //var accountId = accountList[j].Id.Id;
            //accountId = accountId.replace("{", "").replace("}", "");
            //if (projectId.toUpperCase() == TopkapiProjectId && accountId.toUpperCase() == NEFSalesCollaboraterateId) {
            //    continue;
            //}
            //else if (projectId.toUpperCase() == TopkapiProjectId && accountId.toUpperCase() != NEFSalesCollaboraterateId) {
            //    paymentAmount = parseFloat(paymentAmountBase).toFixed(2);
            //}
            //else {
            //    paymentAmount = parseFloat((paymentAmountBase * accountList[j].Rate) / 100).toFixed(2);
            //}



            ///
            paymentAmount = parseFloat((paymentAmountBase * accountList[j].Rate) / 100).toFixed(2);
            payment.new_paymentamount = { Value: paymentAmount };
            if (Xrm.Page.getAttribute("new_financialaccountid").getValue() != null) {
                var financialAccount = {};//Cari
                financialAccount.Id = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].id;
                financialAccount.Name = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].name;
                financialAccount.LogicalName = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].entityType;
                payment.new_financialaccountid = financialAccount;
            }
            var quoteId = Xrm.Page.data.entity.getId();
            quote.Id = quoteId.replace("{", "").replace("}", "");
            quote.Name = Xrm.Page.getAttribute("name").getValue();
            quote.LogicalName = "quote";
            var transactioncurrencyid = {};
            transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
            transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
            transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;
            var customerid = {};
            customerid.Id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
            customerid.Name = Xrm.Page.getAttribute("customerid").getValue()[0].name;
            customerid.LogicalName = Xrm.Page.getAttribute("customerid").getValue()[0].entityType;
            payment.new_quoteid = quote;
            payment.new_type = { Value: 4 };// Kapora Ödemesi
            payment.new_itype = { Value: prePaymentType };//Ödeme Türü
            payment.new_date = prePaymentDate;
            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "Ön Ödeme - " + prePaymentDate.format("dd.MM.yyyy");
            if (customerid.LogicalName == "contact") {
                payment.new_contactid = customerid;
            }
            else {
                payment.new_accountid = customerid;
            }

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
                data: JSON.stringify(payment),
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    //Specifying this header ensures that the results will be returned as JSON.
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    Xrm.Page.getAttribute("new_isprepaymenttaken").setValue(true);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("CreatePrePaymentButtonRecord Error!!!" + XMLHttpRequest.responseText);

                }
            });
        }
        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
        Xrm.Page.data.refresh();
    }
}
function CheckPrePaymentButtonRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 4;//Kapora
    var result = true;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_type/Value eq " + type + " and new_quoteid/Id eq guid'" + quoteId + "'&$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;
            }
            else {
                result = true;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckPrePaymentButtonRecord Error!!!");
            result = false;
        }
    });
    return result;
}



function GetPaymentById(paymentId) {
    var payment = {};

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_paymentId eq (guid'" + paymentId + "')&$select=new_type,new_vtype,new_paymentamount,new_date",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            payment.new_type = data.d.results[0].new_type;
            payment.new_vtype = data.d.results[0].new_vtype;
            payment.new_paymentamount = data.d.results[0].new_paymentamount;
            payment.new_date = data.d.results[0].new_date;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetPaymentById Error!!!");

        }
    });
    return payment;

}

function GetFinancialOptionById(financialOptionId) {
    var financialOption = {};

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_financialoptionSet?$filter=new_financialoptionId eq (guid'" + financialOptionId + "')&$select=new_minpaymentratio",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            financialOption.new_minpaymentratio = data.d.results[0].new_minpaymentratio;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetFinancialOptionById Error!!!");

        }
    });
    return financialOption;

}

function SetSubmitAlways(TabName) {

    var tabs = Xrm.Page.ui.tabs.get();
    for (var i = 0; i < tabs.length; i++) {
        var _tabName = tabs[i].getName();
        if (TabName == _tabName) {
            var sections = tabs[i].sections.get();
            for (var j = 0; j < sections.length; j++) {
                var controls = sections[j].controls.get();
                for (var k = 0; k < controls.length; k++) {
                    var attribute = controls[k].getAttribute();
                    attribute.setSubmitMode("always");

                }
            }
        }
    }
}

function tabdisable(tabname, disablestatus) {

    var tabs = Xrm.Page.ui.tabs.get();
    for (var i = 0; i < tabs.length; i++) {
        var _tabName = tabs[i].getName();
        if (tabname == _tabName) {
            var sections = tabs[i].sections.get();
            for (var j = 0; j < sections.length; j++) {
                var controls = sections[j].controls.get();
                for (var k = 0; k < controls.length; k++) {
                    controls[k].setDisabled(disablestatus);
                }
            }
        }
    }
    Xrm.Page.getControl("new_creditpaymentnumber").setDisabled(true);
    Xrm.Page.getControl("new_vouchernumber").setDisabled(true);



}

function ChangeTransactionCurrency() {
    var grid = document.getElementById("quotedetailsGrid").control;
    var quoteDetailId = grid.GetRecordsFromInnerGrid()[0][0];
    quoteDetailId = quoteDetailId.replace("{", "").replace("}", "");
    var transactionCurrencyId = Xrm.Page.getAttribute("transactioncurrencyid").getValue();
    if (transactionCurrencyId != null) {
        transactionCurrencyId = transactionCurrencyId[0].id.replace("{", "").replace("}", "");
    }
    else {
        return;
    }
    var priceLevel = GetPriceLevel(transactionCurrencyId);
    var product = GetProductFromQuoteDetail(quoteDetailId);

    /*Teklif Ürünü Silme*/
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteDetailSet(guid'" + quoteDetailId + "')",
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("X-HTTP-Method", "DELETE");
        },
        success: function (data, textStatus, XmlHttpRequest) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("QuoteDetail Delete Error!!!");

        }
    });
    /*Teklif Ürünü Silme*/

    Xrm.Page.getAttribute("pricelevelid").setValue([{ id: priceLevel[0].Id, name: priceLevel[0].Name, entityType: "pricelevel" }]);
    Xrm.Page.data.save();
    /*Teklif Ürünü Oluşturma*/
    var detail = {};
    var quote = {};
    var quoteId = Xrm.Page.data.entity.getId();
    quote.Id = quoteId.replace("{", "").replace("}", "");
    quote.Name = Xrm.Page.getAttribute("name").getValue();
    quote.LogicalName = "quote";
    var quantity = parseFloat("1").toFixed(2);
    detail.Quantity = quantity;
    detail.QuoteId = quote;
    detail.ProductId = product[0];//Varsayılan Ürün
    detail.UoMId = product[1];//UOM
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteDetailSet",
        data: JSON.stringify(detail),
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("ChangeTransactionCurrency Create Function Error!!!");
            return;

        }
    });
    /*Teklif Ürünü Oluşturma*/

    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();
    if (prePaymentAmount != null && prePaymentAmount != 0) {
        CalculatePrePaymentForChangeCurrency();
    }

    location.reload();



}

function GetPriceLevel(Id) {
    var priceLevel = new Array();
    priceLevel[0] = new Object();

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/PriceLevelSet?$filter=TransactionCurrencyId/Id eq guid'" + Id + "'&$select=PriceLevelId,Name",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            priceLevel[0].Id = data.d.results[0].PriceLevelId;
            priceLevel[0].Name = data.d.results[0].Name;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetPriceLevel Error!!!");

        }
    });
    return priceLevel;
}

function GetProductFromQuoteDetail(Id) {
    var product = new Array();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteDetailSet?$filter=QuoteDetailId eq (guid'" + Id + "')&$select=ProductId,UoMId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            product[0] = data.d.results[0].ProductId;
            product[1] = data.d.results[0].UoMId;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetProduct Error!!!");

        }
    });
    return product;
}

function GetProductById(Id) {
    var product = new Array();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ProductSet?$filter=ProductId eq (guid'" + Id + "')&$select=new_projectid,new_deliverydate",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            product[0] = data.d.results[0].new_projectid;
            product[1] = data.d.results[0].new_deliverydate;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetProductById Error!!!");

        }
    });
    return product;
}

function GetProjectById(Id) {
    var project = new Array();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_projectSet?$filter=new_projectId eq (guid'" + Id + "')&$select=new_deliverydate,new_revisiondeliverydate,new_projectId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            project[0] = data.d.results[0].new_deliverydate;
            project[1] = data.d.results[0].new_revisiondeliverydate;
            project[2] = data.d.results[0].new_projectId;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetProjectById Error!!!");

        }
    });
    return project;
}

function CalculatePrePaymentForChangeCurrency() {

    var CurrencyName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
    var CurrencyId = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
    CurrencyId = CurrencyId.replace("{", "").replace("}", "");
    GlobalCurrencyId = GlobalCurrencyId.replace("{", "").replace("}", "");
    var prePaymentDate = Xrm.Page.getAttribute("new_prepaymentdate").getValue();
    prePaymentDate = (new Date(prePaymentDate)).toJSON();
    var salesRate = null;
    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();
    if (GlobalCurrencyName == "Türk Lirası") {
        salesRate = GetExchangeRate(prePaymentDate, CurrencyId);
        prePaymentAmount = prePaymentAmount / salesRate;
        if (salesRate == null) {
            alert("Kur olmadığı için Ön Ödeme Tutarı Hesaplanamadı.");
            return;
        }

    }
    else if (CurrencyName == "Türk Lirası" && GlobalCurrencyName != "Türk Lirası") {
        salesRate = GetExchangeRate(prePaymentDate, GlobalCurrencyId);
        prePaymentAmount = prePaymentAmount * salesRate;
        if (salesRate == null) {
            alert("Kur olmadığı için Ön Ödeme Tutarı Hesaplanamadı.");
            return;
        }
    }
    else {
        salesRate = GetExchangeRate(prePaymentDate, GlobalCurrencyId);
        var salesRate2 = GetExchangeRate(prePaymentDate, CurrencyId);
        prePaymentAmount = (prePaymentAmount * salesRate) / salesRate2;
        if (salesRate == null || salesRate2 == null) {
            alert("Kur olmadığı için Ön Ödeme Tutarı Hesaplanamadı.");
            return;
        }

    }

    var prePaymentAmount = parseFloat(prePaymentAmount).toFixed(2);
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var quote = {};
    quote.new_prepaymentamount = { Value: prePaymentAmount };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteSet(guid'" + quoteId + "')",
        data: JSON.stringify(quote),
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
        },
        success: function (data, textStatus, XmlHttpRequest) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Error!!!");

        }
    });






}

function GetExchangeRate(exchangeDate, CurrencyId) {
    var salesRate = null;
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_exchangerateSet?$filter=new_currencydate eq datetime'" + exchangeDate + "' and new_currencyid/Id eq guid'" + CurrencyId + "'&$select=new_salesrate",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                salesRate = data.d.results[0].new_salesrate;
            }


        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetExchangeRate Error!!!");

        }
    });
    return salesRate;
}

function GetTotalPaymentByType(Id, Type) {
    var total = 0;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "' and (new_type/Value eq " + Type + ")&$select=new_paymentamount",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            for (var i = 0; i < data.d.results.length; i++) {
                total = parseFloat(total) + parseFloat(data.d.results[i].new_paymentamount.Value);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetTotalPaymentByType Error!!!");
        }
    });
    return total;
}


function GetIntervalListByDate() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var paymentList = new Array();

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + quoteId + "' and (new_type/Value eq 1)&$select=new_date",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            for (var i = 0; i < data.d.results.length; i++) {
                paymentList[i] = data.d.results[i].new_date;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetIntervalListByDate Error!!!");
        }
    });
    return paymentList;
}

function ChangeCancelReason() {

    var CancelReason = Xrm.Page.getAttribute("new_canceledreasonid").getValue();
    if (CancelReason != null) {
        var CancelReasonName = Xrm.Page.getAttribute("new_canceledreasonid").getValue()[0].name;
        if (CancelReasonName == "Sözleşme Devri") {

        }
    }


}

function GetCollaborateList() {//Proje Ortakları

    var grid = document.getElementById("quotedetailsGrid").control;
    var quoteDetailId = grid.GetRecordsFromInnerGrid()[0][0];
    quoteDetailId = quoteDetailId.replace("{", "").replace("}", "");
    var product = GetProductFromQuoteDetail(quoteDetailId);
    var _product = GetProductById(product[0].Id);
    var project = GetProjectById(_product[0].Id);
    var accountList = new Array();
    if (project[2] != null) {

        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_projectsalescollaborateSet?$filter=statecode/Value eq 0 and new_projectid/Id eq guid'" + project[2] + "'&$select=new_accountid,new_salescollaboraterate",
            data: null,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                for (var i = 0; i < data.d.results.length; i++) {
                    accountList[i] = new Object();
                    accountList[i].Id = data.d.results[i].new_accountid;
                    accountList[i].Rate = data.d.results[i].new_salescollaboraterate;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("GetCollaborateList Error!!!");
            }
        });



    }
    return accountList;
}


/*
 Ribbon Buttons Actions
 */
function ButtonVoucher()//Senet Oluştur
{

    if (!ButtonSytemUserRoleCheck("Senet Oluştur")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var isNotarizedSales = Xrm.Page.getAttribute("new_isnotarizedsales").getValue();//Noterli Satış
    var isContainsTax = Xrm.Page.getAttribute("new_containstax").getValue();//KDV li Satış
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var transactionCurrencyName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
    var sumTotalAmount = Xrm.Page.getAttribute("new_sumtotalpayment").getValue();
    var total = GetTotalPaymentById(quoteId);
    var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();

    if (sumTotalAmount != totalAmount) {
        alert("Ödeme Kayıtları Toplamı, Toplam Satış Tutarından Farklıdır!!!\nÖdeme Kayıtları Toplamı=" + sumTotalAmount + " " + transactionCurrencyName);
        return;
    }
    else if (total[1] == true) {
        alert("Mevcut Senet Kayıtları vardır!!!");
        return;
    }
    else {
        var result = true;
        var paymentList = GetPaymentListById(quoteId);
        if (paymentList.length > 0) {
            for (var i = 0; i < paymentList.length; i++) {
                var payment = {};
                if (paymentList[i].new_vtype == 8 || paymentList[i].new_vtype == 9) {//Ödeme kayıtlarında Senet muhasebe tipi Çek veya Barter var demektir. yok ise Bunlar Senet Değildir
                    payment.new_isvoucher = false;
                }
                else {
                    payment.new_isvoucher = true;
                }

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    datatype: "json",
                    url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet(guid'" + paymentList[i].new_paymentId + "')",
                    data: JSON.stringify(payment),
                    async: false,
                    beforeSend: function (XMLHttpRequest) {
                        //Specifying this header ensures that the results will be returned as JSON.
                        XMLHttpRequest.setRequestHeader("Accept", "application/json");
                        XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
                    },
                    success: function (data, textStatus, XmlHttpRequest) {

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Update PaymentList Error!!!");
                        result = false;

                    }
                });
            }
        }
        if (result == true)//KDV Ve Damga Vergisi için Ödeme Kaydı Oluşturma
        {
            var taxAmount = Xrm.Page.getAttribute("new_taxamount").getValue();//KDV Tutarı
            var taxOfStamp = Xrm.Page.getAttribute("new_taxofstamp").getValue();//Damga Vergisi Yüzdesi
            var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
            var totalOfStamp = (taxOfStamp * totalAmount) / 100;
            totalOfStamp = parseFloat(totalOfStamp).toFixed(4);
            taxAmount = parseFloat(taxAmount).toFixed(4);
            var accountList = GetCollaborateList();

            for (var i = 0; i < 2; i++) {
                //İLK BAŞTA KDV DAHA SONRA DAMGA VERGİSİ
                for (var j = 0; j < accountList.length; j++) {
                    var payment = {};
                    var quote = {};
                    var collaborateAccountId = {};
                    collaborateAccountId.Id = accountList[j].Id.Id;
                    collaborateAccountId.Name = accountList[j].Id.Name;
                    collaborateAccountId.LogicalName = accountList[j].Id.LogicalName;
                    payment.new_collaborateaccountid = collaborateAccountId;
                    payment.new_isvoucher = true;//Senet mi?

                    if (Xrm.Page.getAttribute("new_financialaccountid").getValue() != null) {
                        var financialAccount = {};//Cari
                        financialAccount.Id = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].id;
                        financialAccount.Name = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].name;
                        financialAccount.LogicalName = Xrm.Page.getAttribute("new_financialaccountid").getValue()[0].entityType;
                        payment.new_financialaccountid = financialAccount;
                    }
                    var quoteId = Xrm.Page.data.entity.getId();
                    quote.Id = quoteId.replace("{", "").replace("}", "");
                    quote.Name = Xrm.Page.getAttribute("name").getValue();
                    quote.LogicalName = "quote";
                    var transactioncurrencyid = {};
                    transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
                    transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
                    transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;
                    var customerid = {};
                    customerid.Id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
                    customerid.Name = Xrm.Page.getAttribute("customerid").getValue()[0].name;
                    customerid.LogicalName = Xrm.Page.getAttribute("customerid").getValue()[0].entityType;
                    payment.new_quoteid = quote;
                    if (i == 0) {
                        if (isContainsTax) {
                            continue;
                        }
                        var _taxAmount = parseFloat((taxAmount * accountList[j].Rate) / 100).toFixed(2);
                        payment.new_paymentamount = { Value: _taxAmount };
                        payment.new_type = { Value: 6 };// Ödeme Türü KDV:6 Damga Vergisi:7
                        payment.new_vtype = { Value: 6 };//Senet Muhasebe Tipi KDV:6 Damga Vergisi:7
                        var now = new Date();


                        var grid = document.getElementById("quotedetailsGrid").control;
                        var quoteDetailId = grid.GetRecordsFromInnerGrid()[0][0];
                        quoteDetailId = quoteDetailId.replace("{", "").replace("}", "");
                        var product = GetProductFromQuoteDetail(quoteDetailId);
                        var _product = GetProductById(product[0].Id);
                        if (_product[1] != null) {
                            payment.new_date = new Date(parseInt(_product[1].substr(6)));
                            payment.new_name = "KDV - " + new Date(parseInt(_product[1].substr(6))).format("dd.MM.yyyy");
                        }



                        //var project = GetProjectById(_product[0].Id);
                        //if (project[1] != null && project[0] != null) {
                        //    payment.new_date = new Date(parseInt(project[1].substr(6)));
                        //    payment.new_name = "KDV - " + new Date(parseInt(project[1].substr(6))).format("dd.MM.yyyy");
                        //}
                        //else if (project[0] != null && project[1] == null) {
                        //    payment.new_date = new Date(parseInt(project[0].substr(6)));
                        //    payment.new_name = "KDV - " + new Date(parseInt(project[0].substr(6))).format("dd.MM.yyyy");
                        //}
                    }
                    else {
                        if (isNotarizedSales) {
                            continue;
                        }

                        var _totalOfStamp = parseFloat((totalOfStamp * accountList[j].Rate) / 100).toFixed(2);
                        payment.new_paymentamount = { Value: _totalOfStamp };
                        payment.new_type = { Value: 7 };// Ödeme Türü KDV:6 Damga Vergisi:7
                        payment.new_vtype = { Value: 7 };//Senet Muhasebe Tipi KDV:6 Damga Vergisi:7
                        var now = new Date();
                        var date = Xrm.Page.getAttribute("new_contractdate").getValue();
                        var CurrentDate = new Date(date);
                        var _date = CurrentDate.getDate();
                        CurrentDate.setDate(1);
                        CurrentDate.setMonth(CurrentDate.getMonth() + 2);
                        CurrentDate.setDate(_date);
                        date = CurrentDate;
                        payment.new_date = date;
                        payment.new_name = "Damga Vergisi- " + date.format("dd.MM.yyyy");

                    }

                    payment.TransactionCurrencyId = transactioncurrencyid;

                    if (customerid.LogicalName == "contact") {
                        payment.new_contactid = customerid;
                    }
                    else {
                        payment.new_accountid = customerid;
                    }


                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        datatype: "json",
                        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
                        data: JSON.stringify(payment),
                        async: false,
                        beforeSend: function (XMLHttpRequest) {
                            //Specifying this header ensures that the results will be returned as JSON.
                            XMLHttpRequest.setRequestHeader("Accept", "application/json");
                        },
                        success: function (data, textStatus, XmlHttpRequest) {
                            if (i == 1 && j == accountList.length - 1) {//EN son oluşturalan ise
                                alert("Senetler başarıyla oluşturuldu!!!");
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert("KDV ve Damga Verigisi Oluşturma Error!!!");
                            return;

                        }
                    });

                }
            }
        }
    }
    CalculateTerms();
}

function GetTotalPaymentById(Id) {
    var total = new Array();
    total[0] = 0;
    total[1] = false;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "' and (new_type/Value eq 1 or new_type/Value eq 2 or new_type/Value eq 3 or new_type/Value eq 4 or new_type/Value eq 9)&$select=new_paymentamount,new_isvoucher",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            for (var i = 0; i < data.d.results.length; i++) {
                total[0] = parseFloat(total[0]) + parseFloat(data.d.results[i].new_paymentamount.Value);
                if (data.d.results[i].new_isvoucher == true) {
                    total[1] = true;
                }
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetTotalPaymentById Error!!!");

        }
    });
    return total;

}

function GetPaymentListById(Id) {
    var paymentList = new Array();

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "' and (new_type/Value eq 1 or new_type/Value eq 2 or new_type/Value eq 9)&$select=new_paymentId,new_vtype,new_collaborateaccountid",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            for (var i = 0; i < data.d.results.length; i++) {
                paymentList[i] = new Object();
                paymentList[i].new_paymentId = data.d.results[i].new_paymentId;
                paymentList[i].new_vtype = data.d.results[i].new_vtype.Value;////Senet muhasebe tipi Çek veya Barter ise bu Senet Değildir
                paymentList[i].collaborateAccountName = data.d.results[i].new_collaborateaccountid.Name;//Ortaklık Yapan Firma Adı

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetPaymentListById Error!!!");
        }
    });
    return paymentList;

}

function ButtonPreSalesForm()//Ön Bilgilendirme Formları
{
    if (!ButtonSytemUserRoleCheck("Ön Bilgilendirme Formu")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/preSalesForm.aspx?Id=" + quoteId;
    window.open(Url);

}
function ButtonPrePaymentForm()//Ön Satış Formu
{
    if (!ButtonSytemUserRoleCheck("Ön Satış Formu")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/prePaymentForm.aspx?Id=" + quoteId;
    window.open(Url);

}
function ButtonContractCover()//Sözleşme Kapağı
{
    if (!ButtonSytemUserRoleCheck("Sözleşme Kapağı")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/contratCover.aspx?Id=" + quoteId;
    window.open(Url);

}

function ButtonContractCoverEng()//Sözleşme Kapağı
{
    if (!ButtonSytemUserRoleCheck("Sözleşme Kapağı")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/contratCoverEng.aspx?Id=" + quoteId;
    window.open(Url);
}

function ButtonContractSigned()//Sözleşme İmzala
{
    if (!ButtonSytemUserRoleCheck("Sözleşme İmzala")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var statusCode = Xrm.Page.getAttribute("statuscode").getValue();
    if (statusCode != 100000009) {//Sözleşme Hazırlandı
        alert("Lütfen Önce Sözleşmeyi Hazırlayınız!!!");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var jData = {};
    jData.quoteId = quoteId;
    var jSonData = JSON.stringify(jData);
    var baseURL = location.protocol + "//" + location.host;
    $.ajax({
        url: baseURL + ":9091/CommonServices/Common.svc/ContractSigned",
        async: false,
        dataType: "json",
        data: jSonData,
        contentType: "application/json; charset=utf-8",
        type: "POST",
        success: function (data) {
            if (data != null) {
                data = JSON.parse(data);
                if (data) {
                    alert("Sözleşme Başarıyla İmzalandı!!!");
                    Xrm.Page.data.refresh();
                }
                else {
                    alert("Sözleşme İmzalanırken Hata Alındı!!! ");
                    Xrm.Page.data.refresh();
                }
            }
        },
        error: function (a, b, c) {
            alert("Sözleşme İmzala Error" + a);

        }
    });


}
function ButtonContractPrepared()//Sözleşme Hazırlandı
{
    if (!ButtonSytemUserRoleCheck("Sözleşme Hazırlandı")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    if (CheckVoucherSigned()) {
        if (setRecordStatus("quote", quoteId, "1", "100000009"))//Etkin- Sözleşme Hazırlandıya çekiliyor
        {
            alert("Sözleşme Başarıyla Hazırlandı!!!");
        }

    }
    else {
        alert("Senet Oluşturulmadan Sözleşme Hazırlanamaz!!!");
    }
}

function CheckVoucherSigned() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var result = false;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + quoteId + "' and new_isvoucher eq true&$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = true;
            }
            else {
                result = false;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckVoucherSigned Error!!!");
        }
    });
    return result;

}
function UpdatePaymentRecord(quoteId) {
    var result = false;
    var paymentList = GetPaymentListAllById(quoteId);
    if (paymentList.length > 0) {
        for (var i = 0; i < paymentList.length; i++) {
            var payment = {};
            payment.new_sign = true;
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet(guid'" + paymentList[i] + "')",
                data: JSON.stringify(payment),
                async: false,
                beforeSend: function (XMLHttpRequest) {
                    //Specifying this header ensures that the results will be returned as JSON.
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                    XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    result = true;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("UpdatePaymentRecordError!!!");
                    result = false;

                }
            });
        }
    }
    return result;

}
function CheckVoucherSignedForMuhasebe() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var result = false;

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + quoteId + "' and (new_type/Value eq 1 or new_type/Value eq 2 or new_type/Value eq 9 or new_type/Value eq 6 or new_type/Value eq 7) and (new_vtype/Value ne 8 and new_vtype/Value ne 9 )and new_sign eq false and new_isvoucher eq true &$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result = false;//imzalanmamış senet var demek
            }
            else {
                result = true;//Tüm senetler imzalanmış
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckVoucherSigned Error!!!");
        }
    });
    return result;

}
function GetPaymentListAllById(Id) {
    var paymentList = new Array();

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "' &$select=new_paymentId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            for (var i = 0; i < data.d.results.length; i++) {
                paymentList[i] = data.d.results[i].new_paymentId;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetPaymentListById Error!!!");
        }
    });
    return paymentList;

}
function setRecordStatus(entitySchemaName, recordGuid, stateCode, statusCode) {
    var result = false;
    // create the request
    var request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
    request += "<s:Body>";
    request += "<Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">";
    request += "<request i:type=\"b:SetStateRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\">";
    request += "<a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">";
    request += "<a:KeyValuePairOfstringanyType>";
    request += "<c:key>EntityMoniker</c:key>";
    request += "<c:value i:type=\"a:EntityReference\">";
    request += "<a:Id>" + recordGuid + "</a:Id>";
    request += "<a:LogicalName>" + entitySchemaName + "</a:LogicalName>";
    request += "<a:Name i:nil=\"true\" />";
    request += "</c:value>";
    request += "</a:KeyValuePairOfstringanyType>";
    request += "<a:KeyValuePairOfstringanyType>";
    request += "<c:key>State</c:key>";
    request += "<c:value i:type=\"a:OptionSetValue\">";
    request += "<a:Value>" + stateCode + "</a:Value>";
    request += "</c:value>";
    request += "</a:KeyValuePairOfstringanyType>";
    request += "<a:KeyValuePairOfstringanyType>";
    request += "<c:key>Status</c:key>";
    request += "<c:value i:type=\"a:OptionSetValue\">";
    request += "<a:Value>" + statusCode + "</a:Value>";
    request += "</c:value>";
    request += "</a:KeyValuePairOfstringanyType>";
    request += "</a:Parameters>";
    request += "<a:RequestId i:nil=\"true\" />";
    request += "<a:RequestName>SetState</a:RequestName>";
    request += "</request>";
    request += "</Execute>";
    request += "</s:Body>";
    request += "</s:Envelope>";
    //send set state request  
    $.ajax({
        type: "POST",
        contentType: "text/xml; charset=utf-8",
        datatype: "xml",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/Organization.svc/web",
        data: request,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("Accept", "application/xml, text/xml, */*");
            XMLHttpRequest.setRequestHeader("SOAPAction",
            "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            result = true;
            Xrm.Page.data.refresh();

        },

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("setRecordStatus Function Error!!!" + errorThrown);
            result = false;
        }
    });
    return result;
}
function CloseQuoteRequestMethod(recordGuid) {

    var result = false;
    var jData = {};
    jData.quoteId = recordGuid;
    var jSonData = JSON.stringify(jData);
    var baseURL = location.protocol + "//" + location.host;
    $.ajax({
        url: baseURL + ":9091/CommonServices/Common.svc/CloseQuoteRequest",
        async: false,
        dataType: "json",
        data: jSonData,
        contentType: "application/json;",
        type: "POST",
        beforeSend: function () {

        },
        complete: function () {

        },
        success: function (data) {
            if (data != null) {
                data = JSON.parse(data);
                result = data;
            }
            else {
                result = false;
            }
        },
        error: function (a, b, c) {
            alert("CloseQuoteRequest Error");
            result = false;
        }
    });

    return result;
}
function ButtonCopy() {//Sözleşme Kopyala
    var result = true;
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var cloneData = {};

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteSet?$filter=QuoteId eq (guid'" + quoteId + "')",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data && data.d && data.d.results) {

                cloneData = data.d.results[0];

                replacer = function (key, value) {
                    if (key == "ModifiedOn" || key == "QuoteId" ||
                        key == "CreatedOn" || key == "StateCode" ||
                        key == "StatusCode" || key == "QuoteNumber" ||
                        key == "new_paydate" || key == "new_istartdate" ||
                        key == "new_inststartdate" || key == "new_creditpaymentdate" || key == "RequestDeliveryBy" ||
                        key == "new_ipaymentdate" || key == "new_instpaymentdate" ||
                        key == "new_contractnumber" || key == "new_contractnumber" || key == "new_financialaccountid" || key == "CustomerId" || key == "new_contractprocessdate"
                        || key == "new_salescanceleddate" || key == "new_salestermdate" || key == "new_effectivetermdate") {
                        return undefined;
                    }
                    else {
                        if (key == "new_contractdate" || key == "new_salesprocessdate" || key == "new_prepaymentdate" || key == "ExpiresOn") {
                            if (value != null) {
                                return new Date(parseInt(value.substr(6))).toJSON();
                            }
                        }
                        return value;
                    };
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetQuoteData Error!!!");
            result = false;
            return result;

        }
    });
    var cloneQuoteId = null;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteSet",
        data: JSON.stringify(cloneData, replacer),
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data && data.d) {
                cloneQuoteId = data.d.QuoteId;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CreateCloneQuoteData Error!!!");
            result = false;
            return result;
        }
    });
    //////////////////////// teklif ürünlerini kopyalma
    var cloneDetailData = {};
    var cloneDetailQuoteData = {};
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteDetailSet?$filter=QuoteId/Id eq (guid'" + quoteId + "')",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data && data.d && data.d.results) {
                for (var i = 0; i < data.d.results.length; i++) {
                    cloneDetailData = data.d.results[i];
                    cloneDetailQuoteData = data.d.results[i].QuoteId;
                    replacer = function (key, value) {
                        if (key == "ModifiedOn" || key == "QuoteDetailId" || key == "RequestDeliveryBy" ||
                            key == "CreatedOn" || key == "StateCode" ||
                            key == "StatusCode" || key == "QuoteNumber") {
                            return undefined;
                        }
                        else if (key == "QuoteId") {
                            cloneDetailQuoteData.Id = cloneQuoteId;
                            return cloneDetailQuoteData;
                        }
                        else return value;
                    }
                    CreateDetail(cloneDetailData, replacer, cloneQuoteId, result);
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetQuoteDetailData Error!!!");
            result = false;
            return result;
        }
    });
    //////////////////////// Kapora kopyalma
    var clonePrePaymentData = {};
    var clonePrePaymentQuoteData = {};
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$filter=new_quoteid/Id eq guid'" + quoteId + "' and (new_type/Value ne 6 and new_type/Value ne 7 and new_vtype/Value ne 6 and new_vtype/Value ne 7 )",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data && data.d && data.d.results) {
                for (var i = 0; i < data.d.results.length; i++) {
                    clonePrePaymentData = data.d.results[i];
                    clonePrePaymentQuoteData = data.d.results[i].new_quoteid;
                    replacer = function (key, value) {
                        if (key == "ModifiedOn" || key == "new_paymentId" ||
                            key == "CreatedOn" || key == "StateCode" || key == "statecode" ||
                            key == "statuscode" || key == "StatusCode" || key == "QuoteNumber" || key == "new_sign" || key == "new_isvoucher" || key == "new_vnumber") {
                            return undefined;
                        }
                        else if (key == "new_quoteid") {
                            clonePrePaymentQuoteData.Id = cloneQuoteId;
                            return clonePrePaymentQuoteData;
                        }
                        else if (key == "new_date") {
                            return new Date(parseInt(value.substr(6))).toJSON();

                        }
                        else return value;
                    }
                    result = CreatePrePayment(clonePrePaymentData, replacer, cloneQuoteId, i, data.d.results.length, result);
                    if (result == false)
                    { return; }
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetQuotePaymentData Error!!!");
            result = false;
            return result;
        }
    });
    return result;
}
function CreateDetail(cloneDetailData, replacer, cloneQuoteId, result) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteDetailSet",
        data: JSON.stringify(cloneDetailData, replacer),
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            // alert("Başarılı şekilde satış kopyası oluşturuldu.");

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CreateCloneQuoteDetailData Error!!!");
            result = false;
            return result;
        }
    });
}
function CreatePrePayment(clonePrePaymentData, replacer, cloneQuoteId, i, length, result) {
    result = false;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet",
        data: JSON.stringify(clonePrePaymentData, replacer),
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            result = true;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CreatePrePayment Error!!!");
            result = false;
        }
    });
    if (result == true && i == length - 1) {
        alert("Başarılı şekilde satış kopyası oluşturuldu.");
        var Url = Xrm.Page.context.getClientUrl() + "/main.aspx?etc=1084&id=%7b" + cloneQuoteId + "%7d&pagetype=entityrecord";
        window.open(Url);
    }
    else if (result == false) {
        alert("Satış Kopyalama hata!!!");
        return;
    }
    return result;
}
function ButtonMuhasebelestir() {// Satışı Muhasebeleştir
    if (!ButtonSytemUserRoleCheck("Muhasebeye Aktar")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }


    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    if (CheckVoucherSignedForMuhasebe()) {

        var jData = {};
        jData.quoteId = quoteId;
        var jSonData = JSON.stringify(jData);
        var baseURL = location.protocol + "//" + location.host;
        $.ajax({
            url: baseURL + ":9091/CommonServices/Common.svc/Muhasebelestir",
            async: false,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data) {
                        alert("Satış Başarıyla Muhasebeleştirildi!!!");
                        Xrm.Page.data.refresh();
                    }
                    else {
                        alert("Satış Muhasebeleştirilirken Hata Alındı!!! ");
                        Xrm.Page.data.refresh();
                    }
                }
            },
            error: function (a, b, c) {
                alert("Muhasebelestir Error" + a);

            }
        });

        //if (setRecordStatus("quote", quoteId, "0", "1")) {
        //    if (setRecordStatus("quote", quoteId, "1", "100000001"))//teklif Muhasebeleştir oldu
        //    {
        //        alert("Satış Başarıyla Muhasebeleştirildi!!!");
        //    }
        //}
    }
    else {
        alert("Senetler İmzalanmadan Satışı Muhasebeleştiremezsiniz!!!");
    }

}
function ButtonCancel() {// Satışı İptal Et
    if (!ButtonSytemUserRoleCheck("Satış İptal")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }

    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var result = CheckSalesCancelDetail();
    if (result.length > 0) {//Etkin Satış İptal Detayı Var Demek
        alert("Aktif Satış İptal Detayı Vardır.Lütfen Onu Güncelleyiniz!!!");
        var Url = Xrm.Page.context.getClientUrl() + "/main.aspx?etc=10044&id=%7b" + result[0] + "%7d&pagetype=entityrecord";
        window.open(Url);
        //etc = 10059 NEF ORG
        //etc = 10042 pilot ORG
        //etc = 10044 CANLI

    }
    else {
        var name = Xrm.Page.getAttribute("name").getValue();
        var extraqs = "&new_quoteid=" + quoteId;
        extraqs += "&new_quoteidname=" + name;
        var features = "location=no,menubar=no,status=no,toolbar=no";

        var win = window.open("/" + Xrm.Page.context.getOrgUniqueName() + "/main.aspx?etc=10044&pagetype=entityrecord&extraqs=" + encodeURIComponent(extraqs), "Satış İptal Detayı", features, false);
        //etc = 10059 NEF ORG
        //etc = 10042 pilot ORG
        //etc = 10044 CANLI
    }





}
function CheckSalesCancelDetail() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var result = new Array();

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_salescanceldetailSet?$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + quoteId + "'&$select=new_salescanceldetailId",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                result[0] = data.d.results[0].new_salescanceldetailId;
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CheckSalesCancelDetail Error!!!");
        }
    });
    return result;
}
function ButtonSalesForm() {//Satış Formları Senet
    if (!ButtonSytemUserRoleCheck("Senet")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/voucher.aspx?Id=" + quoteId;
    window.open(Url);

    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/voucherisgyo.aspx?Id=" + quoteId;
    window.open(Url);
}
function ButtonIsGyo() {// Senetler İŞ GYO
    if (!ButtonSytemUserRoleCheck("Senet İş GYO")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/voucherisgyo.aspx?Id=" + quoteId;
    window.open(Url);
}
function ButtonVoucherSigned() {//Senetleri imzala

    if (!ButtonSytemUserRoleCheck("Senetleri İmzala")) {
        alert("Yetikiniz Bulunmamaktadır.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9091/vouchersigned.html?quoteId=" + quoteId;
    window.open(Url);
}

function CreateDiscountProtocol() {
    var new_discountprotocol = {};
    var quote = {};
    var quoteId = Xrm.Page.data.entity.getId();
    quote.Id = quoteId.replace("{", "").replace("}", "");
    quote.Name = Xrm.Page.getAttribute("name").getValue();
    quote.LogicalName = "quote";


    var transactioncurrencyid = {};
    transactioncurrencyid.Id = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].id;
    transactioncurrencyid.Name = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
    transactioncurrencyid.LogicalName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].entityType;

    var salesAmount = Xrm.Page.getAttribute("totalamountlessfreight").getValue();
    var appCreditAmount = Xrm.Page.getAttribute("new_creditamount").getValue();
    //new_bankcreditstatus.new_bankid = bankid;
    new_discountprotocol.new_sales = quote;
    new_discountprotocol.TransactionCurrencyId = transactioncurrencyid;
    new_discountprotocol.new_name = quote.Name;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_discountprotocolsSet",
        data: JSON.stringify(new_discountprotocol),
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            alert("İndirim Protokol Kaydı başarılı bir şekilde oluşturuldu.");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CreateDiscountProtocol Post opretion error!!!" + XMLHttpRequest.responseText);

        }
    });
}

/*
 Ribbon Buttons Actions
 */

var intervalId;
function makeReadOnly() {

    try {

        var subgridsLoaded = false;

        Xrm.Page.ui.controls.get().forEach(function (control, index) {

            if (control.setDisabled && Xrm.Page.ui.getFormType() != 3) {

                control.setDisabled(true);

            }

            else {

                removeButtonsFromSubGrid(control);

                subgridsLoaded = true;

            }

        });

        if ($("div[id$='_crmGridTD']").length > 0 && !subgridsLoaded) {

            intervalId = setInterval(function () {

                var subgridsArr = Xrm.Page.getControl(function (control, index) {

                    return control.getControlType() == 'subgrid';

                });

                subgridsArr.forEach(function (control, index) {

                    removeButtonsFromSubGrid(control);

                });

            }, 500);

        }

    }

    catch (e) {

        alert("makeReadOnly() Error: " + e.message);

    }

}
function removeButtonsFromSubGrid(subgridControl) {

    if (intervalId) {

        $('#' + subgridControl.getName() + '_addImageButton').css('display', 'none');

        $('#' + subgridControl.getName() + '_openAssociatedGridViewImageButton').css('display', 'none');

        clearInterval(intervalId);

    }

}

function CreateReason(cancelReason) {
    var quoteId = Xrm.Page.data.entity.getId();
    var projectId = Xrm.Page.getAttribute("new_projectid").getValue();
    quoteId = quoteId.replace("{", "").replace("}", "");

    if (cancelReason == "Sözleşme Devri") {
        if (ButtonCopy()) {
            CloseQuoteRequestMethod(quoteId);//Satış iptal
        }
    }
        //else {
        //    if (setRecordStatus("quote", quoteId, "0", "1")) {
        //        if (setRecordStatus("quote", quoteId, "1", "100000000"))//Satış İptal Aktarıldı
        //        {
        //            if (cancelReason == "Kapora İptali") {
        //                var Url = "http://intranef.nef.com.tr/SitePages/KaporaIptal.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
        //                window.open(Url);
        //            }
        //            else if (cancelReason == "Daire Değişikliği" || cancelReason == "İsim Değişikliği" || cancelReason == "Hatalı Giriş" || cancelReason == "Ödeme Değişikliği") {
        //                var Url = "http://intranef.nef.com.tr/SitePages/DegisiklikTalepFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
        //                window.open(Url);
        //            }
        //            else {
        //                var Url = "http://intranef.nef.com.tr/SitePages/SatisIptalFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
        //                window.open(Url);
        //            }
        //        }
        //    }
        //}
    else {
        if (projectId != null) {
            projectId = Xrm.Page.getAttribute("new_projectid").getValue()[0].id;
            projectId = projectId.replace("{", "").replace("}", "");
        }
        if (cancelReason == "Kapora İptali") {
            if (projectId.toUpperCase() == TopkapiProjectId) {
                var Url = "http://intranef.nef.com.tr/SitePages/TopkapıIptalFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
                window.open(Url);
            } else {
                var Url = "http://intranef.nef.com.tr/SitePages/KaporaIptal.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
                window.open(Url);
            }
        }
        else if (cancelReason == "Daire Değişikliği" || cancelReason == "İsim Değişikliği" || cancelReason == "Hatalı Giriş" || cancelReason == "Ödeme Değişikliği") {
            if (projectId.toUpperCase() == TopkapiProjectId) {
                var Url = "http://intranef.nef.com.tr/SitePages/TopkapıIptalFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
                window.open(Url);
            } else {
                var Url = "http://intranef.nef.com.tr/SitePages/DegisiklikTalepFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
                window.open(Url);
            }
        }
        else {
            var Url = "http://intranef.nef.com.tr/SitePages/SatisIptalFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
            window.open(Url);
        }
    }
}



function ButtonSytemUserRoleCheck(functionName) {
    var result = false;
    switch (functionName) {
        case "Senetleri İmzala":
        case "Muhasebeye Aktar":
            var userRoles = Xrm.Page.context.getUserRoles();
            for (var i = 0; i < userRoles.length; i++) {
                if (userRoles[i].toUpperCase() == MFTRoleId || userRoles[i].toUpperCase() == SystemAdminRoleId) {
                    result = true;
                }
            }
            break;
        default:
            var userRoles = Xrm.Page.context.getUserRoles();
            for (var i = 0; i < userRoles.length; i++) {
                if (userRoles[i].toUpperCase() == NEFSatisDestekRoleId || userRoles[i].toUpperCase() == NEFSatisDanismaniRoleId || userRoles[i].toUpperCase() == NEFIsGYOSatisDanismaniRoleId ||
                    userRoles[i].toUpperCase() == NEFIsGYOSatisDestekRoleId || userRoles[i].toUpperCase() == NEFSatisDirektoruRoleId || userRoles[i].toUpperCase() == NEFSatisMuduruRoleId || userRoles[i].toUpperCase() == SystemAdminRoleId) {
                    result = true;
                }
            }
            break;

    }
    return result;

}

//Satış-efektif vade hesaplamaları- Senet oluştur basıldığında
function CalculateTerms() {
    var Id = Xrm.Page.data.entity.getId();
    Id = Id.replace("{", "").replace("}", "");
    var firstPaymentDate;
    var lastPaymentDate;
    var salesTermDay;
    var effectiveTermDay;
    var salesTermDate;
    var total = 0;
    var contractDate = Xrm.Page.getAttribute("new_contractdate").getValue();
    var amountWithTax = Xrm.Page.getAttribute("new_amountwithtax").getValue();

    var one_day = 1000 * 60 * 60 * 24;
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$orderby=new_date asc&$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "'and new_isvoucher eq true and new_type/Value ne 7&$select=new_date,new_paymentamount",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                firstPaymentDate = parseInt(data.d.results[0].new_date.substr(6));
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CalculateTerms Error!!!");
        }
    });

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$orderby=new_date desc&$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "'and new_isvoucher eq true and new_type/Value ne 7&$select=new_date,new_paymentamount",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (data.d.results.length > 0) {
                lastPaymentDate = parseInt(data.d.results[0].new_date.substr(6));
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CalculateTerms Error!!!");
        }
    });
    var difference_ms = lastPaymentDate - firstPaymentDate;
    Xrm.Page.getAttribute("new_paymentterm").setSubmitMode("always");
    Xrm.Page.getAttribute("new_paymentterm").setValue(Math.round(difference_ms / one_day));
    ///////////////////////////////////////////////////////////////////////////////////////
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_paymentSet?$orderby=new_date desc&$filter=statecode/Value eq 0 and new_quoteid/Id eq guid'" + Id + "'and new_type/Value ne 7&$select=new_date,new_paymentamount",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            for (var i = 0; i < data.d.results.length; i++) {
                if (i == 0) {
                    salesTermDay = ((parseInt(data.d.results[0].new_date.substr(6)) - contractDate.getTime()) / one_day) + 1;
                    salesTermDate = new Date(parseInt(data.d.results[0].new_date.substr(6)));
                }
                total += (((parseInt(data.d.results[i].new_date.substr(6)) - contractDate.getTime()) / one_day) + 1) * parseFloat(data.d.results[i].new_paymentamount.Value);

            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("CalculateTerms Error!!!");
        }
    });

    effectiveTermDay = total / amountWithTax;
    Xrm.Page.getAttribute("new_effectivetermday").setSubmitMode("always");
    Xrm.Page.getAttribute("new_effectivetermday").setValue(Math.round(effectiveTermDay).toString());
    Xrm.Page.getAttribute("new_effectivetermdate").setSubmitMode("always");
    Xrm.Page.getAttribute("new_effectivetermdate").setValue(new Date(contractDate.getTime() + (one_day * (Math.round(effectiveTermDay) - 1))));

    Xrm.Page.getAttribute("new_salestermday").setSubmitMode("always");
    Xrm.Page.getAttribute("new_salestermday").setValue(Math.round(salesTermDay).toString());
    Xrm.Page.getAttribute("new_salestermdate").setSubmitMode("always");
    Xrm.Page.getAttribute("new_salestermdate").setValue(salesTermDate);
    Xrm.Page.data.save();
}

function ChangeDiscountAmount() {
    var discountAmount = Xrm.Page.getAttribute("new_discountamount").getValue();
    var totalLineAmount = Xrm.Page.getAttribute("totallineitemamount").getValue();
    if (totalLineAmount != null) {
        if (discountAmount != null) {
            Xrm.Page.getAttribute("discountpercentage").setValue(discountAmount * 100 / totalLineAmount);
        }
        else {
            Xrm.Page.getAttribute("discountpercentage").setValue(0);
        }
    }
}
function ChangeDiscountPercentage() {
    var discountPercentage = Xrm.Page.getAttribute("discountpercentage").getValue();
    var totalLineAmount = Xrm.Page.getAttribute("totallineitemamount").getValue();
    if (totalLineAmount != null) {
        if (discountPercentage != null) {
            Xrm.Page.getAttribute("new_discountamount").setValue((discountPercentage * totalLineAmount / 100));
        }
        else {
            Xrm.Page.getAttribute("new_discountamount").setValue(0);
        }
    }
}

function ChangeIsNotarizedSales() {
    var isNotarizedSales = Xrm.Page.getAttribute("new_isnotarizedsales").getValue();//Noterli Satış
    var quoteId = Xrm.Page.data.entity.getId();
    var total = GetTotalPaymentById(quoteId);
    if (total[1] == true) {
        alert("Senet kayıtları oluştuğu için Noterli satış güncellenemez!!!")
        Xrm.Page.getAttribute("new_isnotarizedsales").setValue(!isNotarizedSales);//Noterli Satış
    }
}

