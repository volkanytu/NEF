var GlobalCurrencyName = null;
var GlobalCurrencyId = null;

function formOnLoad() {
    if (CheckPrePaymentButtonRecord() == false) {//Kapora �deme Kayd� Vard�r.
        Xrm.Page.getControl("new_prepaymentamount").setDisabled(true);
        Xrm.Page.getControl("new_prepaymenttype").setDisabled(true);
        Xrm.Page.getControl("new_prepaymentdate").setDisabled(true);
        Xrm.Page.getControl("new_isprepaymenttaken").setDisabled(true);
    }
    else {
        Xrm.Page.getAttribute("new_prepaymentamount").setValue(null);
        Xrm.Page.getAttribute("new_prepaymenttype").setValue(null);
        Xrm.Page.getAttribute("new_prepaymentdate").setValue(null);
        Xrm.Page.getAttribute("new_isprepaymenttaken").setValue(false);
    }


    MakeCreateButton("new_btnvoucher", "Pe�in");//Pe�in
    MakeUpdateButton("new_btnvoucher", "Pe�in");//Pe�in
    MakeCreateButton("new_btninterval", "Ara �deme");//Ara �deme
    MakeUpdateButton("new_btninterval", "Ara �deme");//Ara �deme
    MakeCreateButton("new_btninstallment", "D�zenli Taksit");//D�zenli Taksit
    MakeUpdateButton("new_btninstallment", "D�zenli Taksit");//D�zenli Taksit
    MakeCreateButton("new_btncredit", "Kredi");//Kredi
    MakeUpdateButton("new_btncredit", "Kredi");//Kredi
    MakeCreateButton("new_btnprepayment", "Kapora");//Kapora
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
    if (status != 100000005) {//Onayland� de�il ise
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
        if (!ButtonSytemUserRoleCheck("D�zelt")) {
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
                if (payment.new_type.Value == 1) {//Ara �deme
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
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Ara �deme Adedi
                    Xrm.Page.getControl("new_creditpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_insttypecreditpayment").setDisabled(true);

                }
                else if (payment.new_type.Value == 2) {//D�zenli Taksit
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
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Ara �deme Adedi
                    Xrm.Page.getControl("new_creditpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_insttypecreditpayment").setDisabled(true);
                }
                else if (payment.new_type.Value == 3) {//Pe�in �deme
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
                    Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara �deme Adedi
                    Xrm.Page.getAttribute("new_inststartdate").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
                    Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);//�deme Tarihi (D�zenli Taksit)
                    Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);//�denecek Tutar (D�zenli Taksit)                
                    Xrm.Page.getAttribute("new_instrate").setValue(null);
                    Xrm.Page.getControl("new_insttype").setDisabled(true);
                    Xrm.Page.getControl("new_instnumber").setDisabled(true);
                    Xrm.Page.getControl("new_inststartdate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentdate").setDisabled(true);
                    Xrm.Page.getControl("new_instpaymentamount").setDisabled(true);
                    Xrm.Page.getControl("new_instrate").setDisabled(true);
                    ///////////////////////////////////////////////////////////////
                    Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//Senet Tipi
                    Xrm.Page.getAttribute("new_insttypecreditpayment").setValue(null);//Ara �deme Adedi
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
                    Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara �deme Adedi
                    Xrm.Page.getAttribute("new_inststartdate").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
                    Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);//�deme Tarihi (D�zenli Taksit)
                    Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);//�denecek Tutar (D�zenli Taksit)                
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


function MakeCreateButton(attribute, attributeLabel) {

    if (document.getElementById(attribute) != null) {
        var sFieldID = "field" + attribute;
        var elementID = document.getElementById(attribute + "_d");
        var div = document.createElement("div");
        div.style.width = "100%";
        div.style.textAlign = "right";
        div.style.display = "inline";
        elementID.appendChild(div, elementID);
        div.innerHTML = '<button id="' + sFieldID + "Create" + '"  style="background-color:#44c767;border:none;display:inline-block;cursor:pointer;color:#ffffff;font-family:Segoe UI,Arial,Sans-Serif;font-size:12px;font-weight:bold;text-decoration:none;background-image:none;">OLU�TUR</button>';
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
        div.innerHTML = '<button id="' + sFieldID + "Update" + '"  style="background-color:#002050;border:none;margin-left:50px;display:inline-block;cursor:pointer;color:#ffffff;font-family:Segoe UI,Arial,Sans-Serif;font-size:12px;font-weight:bold;text-decoration:none;background-image:none;">G�NCELLE</button>';
        document.getElementById(attribute).style.display = "none";
        document.getElementById(sFieldID + "Update").onclick = function () { UpdateButton(attributeLabel); };
    }
}

function CreateButton(attributeLabel) {

    switch (attributeLabel) {
        case "Pe�in": CreateVoucher();
            break;
        case "Ara �deme": CreateInterval();
            break;
        case "D�zenli Taksit": CreateInstallment();
            break;
        case "Kredi": CreateCredit();
            break;
        case "Kapora": CreatePrePaymentButton();
            break;
    }
}
function UpdateButton(attributeLabel) {
    switch (attributeLabel) {
        case "Pe�in": UpdateVoucher();
            break;
        case "Ara �deme": UpdateInterval();
            break;
        case "D�zenli Taksit": UpdateInstallment();
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
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_4").setVisible(false);
            Xrm.Page.getAttribute("new_financialoptionid").setRequiredLevel("required");
            Xrm.Page.ui.tabs.get("tab_6").sections.get("tab_6_section_4").setVisible(true);

        }
        else {//Banka Kredili �deme Plan�
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_3").setVisible(false);
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
        alert("L�tfen indirim de�erlerinden sadece birini giriniz!!!");
        Xrm.Page.getControl("discountpercentage").setFocus(true);
        context.getEventArgs().preventDefault();
        return;
    }
}
function CalculatePrePayment() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var totalVoucherAmount = GetTotalPaymentByType(quoteId, 3);//Pe�in �deme
    var sumInterval = GetTotalPaymentByType(quoteId, 1);//Ara �deme
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
function ChangeRatio()//Kredisiz �deme plan�
{
    var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
    var voucherRatio = Xrm.Page.getAttribute("new_voucherratio").getValue();//Pe�inat Taksitleri %
    var intervalRatio = Xrm.Page.getAttribute("new_intervalratio").getValue();//Ara �deme %
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
    var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();//Pe�inat Senetleri Toplam�
    var sumInterval = Xrm.Page.getAttribute("new_suminterval").getValue();//Ara �deme Toplam�
    var instAmount = Xrm.Page.getAttribute("new_instamount").getValue();//Taksitlendirilecek Tutar (D�zenli Taksit)
    var cashPaymentBankAccount = Xrm.Page.getAttribute("new_cashpaymentbankaccount").getValue();//Pe�in �deme (Banka Kredili �deme Plan�nda)
    var minCashPayment = Xrm.Page.getAttribute("new_mincashpayment").getValue();//Pe�in �deme (Banka Kredili �deme Plan�nda)
    var creditAmount = Xrm.Page.getAttribute("new_creditpaymentamount").getValue();//Kredilenecek Tutar
    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();//Kapora Tutar�
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
    else {//Banka Kredili �deme Plan�       
        if (totalVoucherAmount != 0 && totalVoucherAmount != null) {
            Xrm.Page.getAttribute("new_cashpaymentbankaccount").setValue(totalVoucherAmount);
        }
        else {
            Xrm.Page.getAttribute("new_cashpaymentbankaccount").setValue(0);
        }
        if (minCashPayment != 0 && minCashPayment != null) {
            if (totalVoucherAmount < minCashPayment) {
                alert("Girilen Pe�in �deme Tutar�, Minimum Pe�in �deme Tutar�ndan K���k Olamaz!!!");
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
    var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();//Pe�inat Senetleri Toplam�
    var type = Xrm.Page.getAttribute("new_type").getValue();//Senet Tipi
    var voucherNumber = Xrm.Page.getAttribute("new_vouchernumber").getValue();//Pe�inat Senetleri Adedi
    var payStartDate = Xrm.Page.getAttribute("new_paydate").getValue();//�deme Ba�lang�� Tarihi (Pe�inat)
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan �deme Tutar�
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen Pe�inat Senetleri Toplam�n� Giriniz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_type").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("L�tfen Pe�inat Senet Adedini Giriniz!!!");
        Xrm.Page.getControl("new_vouchernumber").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan �deme Tutar�ndan B�y�k Olamaz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("L�tfen �deme Ba�lang�� Tarihini  Giriniz!!!");
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
            alert("L�tfen �deme Tarihinini En Fazla Bug�nden 1 Ay Sonras�n� Giriniz!!!");
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
            payment.new_type = { Value: 3 };// �deme T�r� Pe�inat �demesi (Senet)
            payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
            payment.new_date = date;
            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "Pe�in �deme - " + date.format("dd.MM.yyyy");
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
    Xrm.Page.getAttribute("new_paydate").setValue(null);//�deme Ba�lang�� Tarihi (Pe�inat)
    var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
    gridControl.refresh();
    Xrm.Page.data.save();

    tabdisable("tab_6", false);
}
function CheckVoucherRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 3;//Pe�inat �demesi Senet
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
    var totalVoucherAmount = Xrm.Page.getAttribute("new_totalvoucheramount").getValue();//Pe�in �deme Toplam�
    var type = Xrm.Page.getAttribute("new_type").getValue();//Senet Tipi   
    var payStartDate = Xrm.Page.getAttribute("new_paydate").getValue();//�deme Tarihi (Pe�in)
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen Pe�in �deme Toplam�n� Giriniz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_type").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("L�tfen �deme Tarihini  Giriniz!!!");
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
            alert("L�tfen �deme Tarihinini En Fazla Bug�nden 1 Ay Sonras�n� Giriniz!!!");
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
        payment.new_type = { Value: 3 };// �deme T�r� Pe�in �deme
        payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
        payment.new_date = payStartDate;
        payment.new_paymentamount = { Value: amount };
        var now = new Date();
        payment.new_name = "Pe�in �deme - " + payStartDate.format("dd.MM.yyyy");

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
        Xrm.Page.getAttribute("new_paydate").setValue(null);//�deme  Tarihi
        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}


function CreateInterval() {
    var totalVoucherAmount = Xrm.Page.getAttribute("new_suminterval").getValue();//Ara �deme Toplam�
    var type = Xrm.Page.getAttribute("new_itype").getValue();//Senet Tipi
    var voucherNumber = Xrm.Page.getAttribute("new_intervalnumber").getValue();//Ara �deme Adedi
    var payStartDate = Xrm.Page.getAttribute("new_istartdate").getValue();//�deme Ba�lang�� Tarihi (Ara �demeler)
    var intervalPeriod = Xrm.Page.getAttribute("new_intervalperiod").getValue();//Ara �deme Periyodu
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan �deme Tutar�

    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen Ara �deme Toplam�n� Giriniz!!!");
        Xrm.Page.getControl("new_suminterval").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_itype").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("L�tfen Ara �deme Adedini Giriniz!!!");
        Xrm.Page.getControl("new_intervalnumber").setFocus(true);
        return;
    }
    if (voucherNumber > 48) {
        alert("Ara �deme Adedi 48'den Fazla Olamaz!!!");
        Xrm.Page.getControl("new_intervalnumber").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("L�tfen �deme Ba�lang�� Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_istartdate").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan �deme Tutar�ndan B�y�k Olamaz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (intervalPeriod == null) {
        alert("L�tfen Ara �deme Periyodunu Giriniz!!!");
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
        //    alert("L�tfen �deme Ba�lang�� Tarihinini Bug�n veya �leri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    CreateIntervalRecord(totalVoucherAmount, type, voucherNumber, payStartDate, intervalPeriod);
        //}
    }

}
function CreateIntervalRecord(totalVoucherAmount, type, voucherNumber, payStartDate, intervalPeriod) {
    //if (CheckIntervalRecord() == false) {
    //    alert("Ara �deme Kayd� Vard�r. \n L�tfen �lgili Kayd� G�ncelleyiniz. ");
    //}
    //else {
    // var paymentAmountBase = parseFloat(totalVoucherAmount / voucherNumber).toFixed(2);
    var paymentAmountBase = parseInt(totalVoucherAmount / voucherNumber);
    var total = 0;
    var accountList = GetCollaborateList();

    var date = payStartDate;
    for (var i = 0; i < voucherNumber; i++) {

        if (i == voucherNumber - 1) {//1000 TL 3'e b�ld���m�zde 333,33... Olarak b�lmeyi d�zeltmesi i�in
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
            payment.new_type = { Value: 1 };// �deme T�r� Ara �deme
            payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
            payment.new_date = date;

            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "Ara �deme - " + date.format("dd.MM.yyyy");
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
    Xrm.Page.getAttribute("new_intervalnumber").setValue(null);//Ara �deme Adedi
    Xrm.Page.getAttribute("new_istartdate").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
    Xrm.Page.getAttribute("new_intervalperiod").setValue(null);//Ara �deme Periyodu

    var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
    gridControl.refresh();
    Xrm.Page.data.save();
    // }
    tabdisable("tab_6", false);
}
function CheckIntervalRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 1;//Ara �deme
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
    var totalVoucherAmount = Xrm.Page.getAttribute("new_ipaymentamount").getValue();//�denecek Tutar (Ara �demeler)
    var type = Xrm.Page.getAttribute("new_itype").getValue();//Senet Tipi  
    var payStartDate = Xrm.Page.getAttribute("new_ipaymentdate").getValue();//�deme Tarihi (Ara �demeler)
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen �denecek Tutar� Giriniz!!!");
        Xrm.Page.getControl("new_ipaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_itype").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("L�tfen �deme Tarihini  Giriniz!!!");
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
        //    alert("L�tfen �deme Ba�lang�� Tarihinini Bug�n veya �leri Bir Tarih Giriniz!!!");
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
        payment.new_type = { Value: 1 };// �deme T�r� Ara �deme
        payment.new_vtype = { Value: type };//Senet Muhasebe Tipi
        payment.new_date = payStartDate;
        payment.new_paymentamount = { Value: paymentAmount };
        payment.new_isupdated = true;
        var now = new Date();
        payment.new_name = "Ara �deme - " + payStartDate.format("dd.MM.yyyy");

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
        Xrm.Page.getAttribute("new_ipaymentdate").setValue(null);//Ara �deme Adedi
        Xrm.Page.getAttribute("new_istartdate").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
        Xrm.Page.getAttribute("new_ipaymentamount").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
        Xrm.Page.getAttribute("new_intervalperiod").setValue(null);//Ara �deme Periyodu

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
    var payStartDate = Xrm.Page.getAttribute("new_inststartdate").getValue();//Taksit Ba�lang�� Tarihi
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan �deme Tutar�
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen Taksitlendirilecek Tutar� Giriniz!!!");
        Xrm.Page.getControl("new_instamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttype").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("L�tfen Taksit Adedini Giriniz!!!");
        Xrm.Page.getControl("new_instnumber").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan �deme Tutar�ndan B�y�k Olamaz!!!");
        Xrm.Page.getControl("new_totalvoucheramount").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("L�tfen Taksit Ba�lang�� Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_inststartdate").setFocus(true);
        return;
    }
    else {
        CreateInstallmentRecord(totalVoucherAmount, type, voucherNumber, payStartDate);
        //var now = new Date();
        //if (payStartDate <= now) {
        //    alert("L�tfen �deme Ba�lang�� Tarihinini Bug�nden �leri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    CreateInstallmentRecord(totalVoucherAmount, type, voucherNumber, payStartDate);
        //}
    }

}
function CreateInstallmentRecord(totalVoucherAmount, type, voucherNumber, payStartDate) {
    if (CheckInstallmentRecord() == false) {
        alert("Taksit �deme Kayd� Vard�r. \n L�tfen �lgili Kayd� G�ncelleyiniz. ");
    }
    else {

        //var paymentAmountBase = parseFloat(totalVoucherAmount / voucherNumber).toFixed(2);       
        var paymentAmountBase = parseInt(totalVoucherAmount / voucherNumber);
        var total = 0;
        var accountList = GetCollaborateList();
        var date = payStartDate;
        for (var i = 0; i < voucherNumber; i++) {
            if (i == voucherNumber - 1) {//1000 TL 3'e b�ld���m�zde 333,33... Olarak b�lmeyi d�zeltmesi i�in
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
                payment.new_type = { Value: 2 };// �deme T�r� Taksit
                payment.new_vtype = { Value: type };//Senet Muhasebe Tipi






                payment.TransactionCurrencyId = transactioncurrencyid;
                var now = new Date();
                payment.new_name = "D�zenli Taksit - " + date.format("dd.MM.yyyy");
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
        Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara �deme Adedi
        Xrm.Page.getAttribute("new_inststartdate").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}
function CheckInstallmentRecord() {
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var type = 2;//D�zenli Taksit
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
    var totalVoucherAmount = Xrm.Page.getAttribute("new_instpaymentamount").getValue();//�denecek Tutar (D�zenli Taksit)
    var type = Xrm.Page.getAttribute("new_insttype").getValue();//Senet Tipi
    var payStartDate = Xrm.Page.getAttribute("new_instpaymentdate").getValue();//�deme Tarihi (D�zenli Taksit)
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen �denecek Tutar� Giriniz!!!");
        Xrm.Page.getControl("new_instpaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttype").setFocus(true);
        return;
    }
    if (payStartDate == null) {
        alert("L�tfen �deme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_instpaymentdate").setFocus(true);
        return;
    }
    else {
        UpdateInstallmentRecord(totalVoucherAmount, type, payStartDate);
        //var now = new Date();
        //if (payStartDate <= now) {
        //    alert("L�tfen �deme Tarihini Bug�nden �leri Bir Tarih Giriniz!!!");
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

        payment.new_type = { Value: 2 };// �deme T�r� Taksit
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


        payment.new_name = "D�zenli Taksit - " + date.format("dd.MM.yyyy");

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
        Xrm.Page.getAttribute("new_instnumber").setValue(null);//Ara �deme Adedi
        Xrm.Page.getAttribute("new_inststartdate").setValue(null);//�deme Ba�lang�� Tarihi (Ara �demeler)
        Xrm.Page.getAttribute("new_instpaymentdate").setValue(null);//�deme Tarihi (D�zenli Taksit)
        Xrm.Page.getAttribute("new_instpaymentamount").setValue(null);//�denecek Tutar (D�zenli Taksit)


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
    var remainingAmount = Xrm.Page.getAttribute("new_remainingpaymentamount").getValue();//Kalan �deme Tutar�
    if (totalVoucherAmount == 0 || totalVoucherAmount == null) {
        alert("L�tfen Kredi Tutar� Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentamount").setFocus(true);
        return;
    }
    if (totalVoucherAmount > remainingAmount) {
        alert("Girilen Tutar Kalan �deme Tutar�ndan B�y�k Olamaz!!!");
        Xrm.Page.getControl("new_creditpaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttypecreditpayment").setFocus(true);
        return;
    }
    if (voucherNumber == null || voucherNumber == 0) {
        alert("L�tfen Adedini Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentnumber").setFocus(true);
        return;
    }
    if (paytDate == null) {
        alert("L�tfen �deme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentdate").setFocus(true);
        return;
    }
    else {
        CreateCreditRecord(totalVoucherAmount, type, voucherNumber, paytDate);
        //var now = new Date();
        //if (paytDate <= now) {
        //    alert("L�tfen �deme Tarihinini Bug�nden �leri Bir Tarih Giriniz!!!");
        //    return;
        //}
        //else {
        //    CreateCreditRecord(totalVoucherAmount, type, voucherNumber, paytDate);
        //}
    }
}
function CreateCreditRecord(totalVoucherAmount, type, voucherNumber, paytDate) {
    if (CheckCreditRecord() == false) {
        alert("Kredi �deme Kayd� Vard�r. \n L�tfen �lgili Kayd� G�ncelleyiniz. ");
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
        Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//�deme Tarihi
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
        alert("L�tfen Kredi Tutar� Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentamount").setFocus(true);
        return;
    }
    if (type == null) {
        alert("L�tfen Senet Tipi Giriniz!!!");
        Xrm.Page.getControl("new_insttypecreditpayment").setFocus(true);
        return;
    }
    if (paytDate == null) {
        alert("L�tfen �deme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_creditpaymentdate").setFocus(true);
        return;
    }
    else {
        UpdateCreditRecord(totalVoucherAmount, type, paytDate);
        //var now = new Date();
        //if (paytDate <= now) {
        //    alert("L�tfen �deme Tarihinini Bug�nden �leri Bir Tarih Giriniz!!!");
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

        payment.new_type = { Value: 9 };// �deme T�r� Banka Kredisi
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
        Xrm.Page.getAttribute("new_creditpaymentdate").setValue(null);//�deme Tarihi

        var gridControl = Xrm.Page.ui.controls.get("paymentgrid");
        gridControl.refresh();
        Xrm.Page.data.save();
    }
    tabdisable("tab_6", false);
}

function CreatePrePaymentButton() {

    var prePaymentAmount = Xrm.Page.getAttribute("new_prepaymentamount").getValue();//Kapora Tutar�
    var prePaymentType = Xrm.Page.getAttribute("new_prepaymenttype").getValue();//Kapora �deme T�r�   
    var prePaymentDate = Xrm.Page.getAttribute("new_prepaymentdate").getValue();//Kapora Tarihi
    if (prePaymentAmount == 0 || prePaymentAmount == null) {
        alert("L�tfen �n �deme Tutar�n� Giriniz!!!");
        Xrm.Page.getControl("new_prepaymentamount").setFocus(true);
        return;
    }
    if (prePaymentType == null) {
        alert("L�tfen �deme T�r�n� Se�iniz!!!");
        Xrm.Page.getControl("new_prepaymenttype").setFocus(true);
        return;
    }
    if (prePaymentDate == null) {
        alert("L�tfen �n �deme Tarihini  Giriniz!!!");
        Xrm.Page.getControl("new_prepaymentdate").setFocus(true);
        return;
    }
    else {
        CreatePrePaymentButtonRecord(prePaymentAmount, prePaymentType, prePaymentDate);
    }
}
function CreatePrePaymentButtonRecord(prePaymentAmount, prePaymentType, prePaymentDate) {
    if (CheckPrePaymentButtonRecord() == false) {
        alert("�n �deme Kayd� Vard�r. \n");
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
            payment.new_type = { Value: 4 };// Kapora �demesi
            payment.new_itype = { Value: prePaymentType };//�deme T�r�
            payment.new_date = prePaymentDate;
            payment.TransactionCurrencyId = transactioncurrencyid;
            var now = new Date();
            payment.new_name = "�n �deme - " + prePaymentDate.format("dd.MM.yyyy");
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

    /*Teklif �r�n� Silme*/
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
    /*Teklif �r�n� Silme*/

    Xrm.Page.getAttribute("pricelevelid").setValue([{ id: priceLevel[0].Id, name: priceLevel[0].Name, entityType: "pricelevel" }]);
    Xrm.Page.data.save();
    /*Teklif �r�n� Olu�turma*/
    var detail = {};
    var quote = {};
    var quoteId = Xrm.Page.data.entity.getId();
    quote.Id = quoteId.replace("{", "").replace("}", "");
    quote.Name = Xrm.Page.getAttribute("name").getValue();
    quote.LogicalName = "quote";
    var quantity = parseFloat("1").toFixed(2);
    detail.Quantity = quantity;
    detail.QuoteId = quote;
    detail.ProductId = product[0];//Varsay�lan �r�n
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
    /*Teklif �r�n� Olu�turma*/

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
        url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ProductSet?$filter=ProductId eq (guid'" + Id + "')&$select=new_projectid",
        data: null,
        async: false,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
            product[0] = data.d.results[0].new_projectid;

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
    if (GlobalCurrencyName == "T�rk Liras�") {
        salesRate = GetExchangeRate(prePaymentDate, CurrencyId);
        prePaymentAmount = prePaymentAmount / salesRate;
        if (salesRate == null) {
            alert("Kur olmad��� i�in �n �deme Tutar� Hesaplanamad�.");
            return;
        }

    }
    else if (CurrencyName == "T�rk Liras�" && GlobalCurrencyName != "T�rk Liras�") {
        salesRate = GetExchangeRate(prePaymentDate, GlobalCurrencyId);
        prePaymentAmount = prePaymentAmount * salesRate;
        if (salesRate == null) {
            alert("Kur olmad��� i�in �n �deme Tutar� Hesaplanamad�.");
            return;
        }
    }
    else {
        salesRate = GetExchangeRate(prePaymentDate, GlobalCurrencyId);
        var salesRate2 = GetExchangeRate(prePaymentDate, CurrencyId);
        prePaymentAmount = (prePaymentAmount * salesRate) / salesRate2;
        if (salesRate == null || salesRate2 == null) {
            alert("Kur olmad��� i�in �n �deme Tutar� Hesaplanamad�.");
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
        if (CancelReasonName == "S�zle�me Devri") {

        }
    }


}

function GetCollaborateList() {//Proje Ortaklar�

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
function ButtonVoucher()//Senet Olu�tur
{

    if (!ButtonSytemUserRoleCheck("Senet Olu�tur")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var isNotarizedSales = Xrm.Page.getAttribute("new_isnotarizedsales").getValue();//Noterli Sat��
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var transactionCurrencyName = Xrm.Page.getAttribute("transactioncurrencyid").getValue()[0].name;
    var sumTotalAmount = Xrm.Page.getAttribute("new_sumtotalpayment").getValue();
    var total = GetTotalPaymentById(quoteId);
    var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
    if (sumTotalAmount != totalAmount) {
        alert("�deme Kay�tlar� Toplam�, Toplam Sat�� Tutar�ndan Farkl�d�r!!!\n�deme Kay�tlar� Toplam�=" + sumTotalAmount + " " + transactionCurrencyName);
        return;
    }
    else if (total[1] == true) {
        alert("Mevcut Senet Kay�tlar� vard�r!!!");
        return;
    }
    else {
        var result = true;
        var paymentList = GetPaymentListById(quoteId);
        if (paymentList.length > 0) {
            for (var i = 0; i < paymentList.length; i++) {
                var payment = {};
                if (paymentList[i].new_vtype == 8 || paymentList[i].new_vtype == 9) {//�deme kay�tlar�nda Senet muhasebe tipi �ek veya Barter var demektir. yok ise Bunlar Senet De�ildir
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
        if (result == true)//KDV Ve Damga Vergisi i�in �deme Kayd� Olu�turma
        {
            var taxAmount = Xrm.Page.getAttribute("new_taxamount").getValue();//KDV Tutar�
            var taxOfStamp = Xrm.Page.getAttribute("new_taxofstamp").getValue();//Damga Vergisi Y�zdesi
            var totalAmount = Xrm.Page.getAttribute("totalamount").getValue();
            var totalOfStamp = (taxOfStamp * totalAmount) / 100;
            totalOfStamp = parseFloat(totalOfStamp).toFixed(4);
            taxAmount = parseFloat(taxAmount).toFixed(4);
            var accountList = GetCollaborateList();

            for (var i = 0; i < 2; i++) {
                //�LK BA�TA KDV DAHA SONRA DAMGA VERG�S�
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
                        var _taxAmount = parseFloat((taxAmount * accountList[j].Rate) / 100).toFixed(2);
                        payment.new_paymentamount = { Value: _taxAmount };
                        payment.new_type = { Value: 6 };// �deme T�r� KDV:6 Damga Vergisi:7
                        payment.new_vtype = { Value: 6 };//Senet Muhasebe Tipi KDV:6 Damga Vergisi:7
                        var now = new Date();


                        var grid = document.getElementById("quotedetailsGrid").control;
                        var quoteDetailId = grid.GetRecordsFromInnerGrid()[0][0];
                        quoteDetailId = quoteDetailId.replace("{", "").replace("}", "");
                        var product = GetProductFromQuoteDetail(quoteDetailId);
                        var _product = GetProductById(product[0].Id);
                        var project = GetProjectById(_product[0].Id);
                        if (project[1] != null && project[0] != null) {
                            payment.new_date = new Date(parseInt(project[1].substr(6)));
                            payment.new_name = "KDV - " + new Date(parseInt(project[1].substr(6))).format("dd.MM.yyyy");
                        }
                        else if (project[0] != null && project[1] == null) {
                            payment.new_date = new Date(parseInt(project[0].substr(6)));
                            payment.new_name = "KDV - " + new Date(parseInt(project[0].substr(6))).format("dd.MM.yyyy");
                        }
                    }
                    else {
                        if (isNotarizedSales) {
                            continue;
                        }
                        var _totalOfStamp = parseFloat((totalOfStamp * accountList[j].Rate) / 100).toFixed(2);
                        payment.new_paymentamount = { Value: _totalOfStamp };
                        payment.new_type = { Value: 7 };// �deme T�r� KDV:6 Damga Vergisi:7
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
                            if (i == 1 && j == accountList.length - 1) {//EN son olu�turalan ise
                                alert("Senetler ba�ar�yla olu�turuldu!!!");
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert("KDV ve Damga Verigisi Olu�turma Error!!!");
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
                paymentList[i].new_vtype = data.d.results[i].new_vtype.Value;////Senet muhasebe tipi �ek veya Barter ise bu Senet De�ildir
                paymentList[i].collaborateAccountName = data.d.results[i].new_collaborateaccountid.Name;//Ortakl�k Yapan Firma Ad�

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("GetPaymentListById Error!!!");
        }
    });
    return paymentList;

}

function ButtonPreSalesForm()//�n Bilgilendirme Formlar�
{
    if (!ButtonSytemUserRoleCheck("�n Bilgilendirme Formu")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/preSalesForm.aspx?Id=" + quoteId;
    window.open(Url);

}
function ButtonPrePaymentForm()//�n Sat�� Formu
{
    if (!ButtonSytemUserRoleCheck("�n Sat�� Formu")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/prePaymentForm.aspx?Id=" + quoteId;
    window.open(Url);

}
function ButtonContractCover()//S�zle�me Kapa��
{
    if (!ButtonSytemUserRoleCheck("S�zle�me Kapa��")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/contratCover.aspx?Id=" + quoteId;
    window.open(Url);

}
function ButtonContractSigned()//S�zle�me �mzala
{
    if (!ButtonSytemUserRoleCheck("S�zle�me �mzala")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var statusCode = Xrm.Page.getAttribute("statuscode").getValue();
    if (statusCode != 100000009) {//S�zle�me Haz�rland�
        alert("L�tfen �nce S�zle�meyi Haz�rlay�n�z!!!");
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
                    alert("S�zle�me Ba�ar�yla �mzaland�!!!");
                    Xrm.Page.data.refresh();
                }
                else {
                    alert("S�zle�me �mzalan�rken Hata Al�nd�!!! ");
                    Xrm.Page.data.refresh();
                }
            }
        },
        error: function (a, b, c) {
            alert("S�zle�me �mzala Error" + a);

        }
    });


}
function ButtonContractPrepared()//S�zle�me Haz�rland�
{
    if (!ButtonSytemUserRoleCheck("S�zle�me Haz�rland�")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    if (CheckVoucherSigned()) {
        if (setRecordStatus("quote", quoteId, "1", "100000009"))//Etkin- S�zle�me Haz�rland�ya �ekiliyor
        {
            alert("S�zle�me Ba�ar�yla Haz�rland�!!!");
        }

    }
    else {
        alert("Senet Olu�turulmadan S�zle�me Haz�rlanamaz!!!");
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
                result = false;//imzalanmam�� senet var demek
            }
            else {
                result = true;//T�m senetler imzalanm��
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
function ButtonCopy() {//S�zle�me Kopyala
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
                        key == "new_inststartdate" || key == "new_creditpaymentdate" ||
                        key == "new_ipaymentdate" || key == "new_instpaymentdate" ||
                        key == "new_contractnumber" || key == "new_contractnumber" || key == "new_financialaccountid" || key == "CustomerId" || key == "new_contractprocessdate"
                        || key == "new_salescanceleddate" || key == "new_salestermdate" || key == "new_effectivetermdate") {
                        return undefined;
                    }
                    else {
                        if (key == "new_contractdate" || key == "new_salesprocessdate" || key == "new_prepaymentdate") {
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
    //////////////////////// teklif �r�nlerini kopyalma
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
                        if (key == "ModifiedOn" || key == "QuoteDetailId" ||
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
            // alert("Ba�ar�l� �ekilde sat�� kopyas� olu�turuldu.");

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
        alert("Ba�ar�l� �ekilde sat�� kopyas� olu�turuldu.");
        var Url = Xrm.Page.context.getClientUrl() + "/main.aspx?etc=1084&id=%7b" + cloneQuoteId + "%7d&pagetype=entityrecord";
        window.open(Url);
    }
    else if (result == false) {
        alert("Sat�� Kopyalama hata!!!");
        return;
    }
    return result;
}
function ButtonMuhasebelestir() {// Sat��� Muhasebele�tir
    if (!ButtonSytemUserRoleCheck("Muhasebeye Aktar")) {
        alert("Yetikiniz Bulunmamaktad�r.");
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
                        alert("Sat�� Ba�ar�yla Muhasebele�tirildi!!!");
                        Xrm.Page.data.refresh();
                    }
                    else {
                        alert("Sat�� Muhasebele�tirilirken Hata Al�nd�!!! ");
                        Xrm.Page.data.refresh();
                    }
                }
            },
            error: function (a, b, c) {
                alert("Muhasebelestir Error" + a);

            }
        });

        //if (setRecordStatus("quote", quoteId, "0", "1")) {
        //    if (setRecordStatus("quote", quoteId, "1", "100000001"))//teklif Muhasebele�tir oldu
        //    {
        //        alert("Sat�� Ba�ar�yla Muhasebele�tirildi!!!");
        //    }
        //}
    }
    else {
        alert("Senetler �mzalanmadan Sat��� Muhasebele�tiremezsiniz!!!");
    }

}
function ButtonCancel() {// Sat��� �ptal Et
    if (!ButtonSytemUserRoleCheck("Sat�� �ptal")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }

    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var result = CheckSalesCancelDetail();
    if (result.length > 0) {//Etkin Sat�� �ptal Detay� Var Demek
        alert("Aktif Sat�� �ptal Detay� Vard�r.L�tfen Onu G�ncelleyiniz!!!");
        var Url = Xrm.Page.context.getClientUrl() + "/main.aspx?etc=10042&id=%7b" + result[0] + "%7d&pagetype=entityrecord";
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

        var win = window.open("/" + Xrm.Page.context.getOrgUniqueName() + "/main.aspx?etc=10042&pagetype=entityrecord&extraqs=" + encodeURIComponent(extraqs), "Sat�� �ptal Detay�", features, false);
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
function ButtonSalesForm() {//Sat�� Formlar� Senet
    if (!ButtonSytemUserRoleCheck("Senet")) {
        alert("Yetikiniz Bulunmamaktad�r.");
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
function ButtonIsGyo() {// Senetler �� GYO
    if (!ButtonSytemUserRoleCheck("Senet �� GYO")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9090/voucherisgyo.aspx?Id=" + quoteId;
    window.open(Url);
}
function ButtonVoucherSigned() {//Senetleri imzala

    if (!ButtonSytemUserRoleCheck("Senetleri �mzala")) {
        alert("Yetikiniz Bulunmamaktad�r.");
        return;
    }
    var quoteId = Xrm.Page.data.entity.getId();
    quoteId = quoteId.replace("{", "").replace("}", "");
    var Url = window.location.protocol + "//" + window.location.host + ":9091/vouchersigned.html?quoteId=" + quoteId;
    window.open(Url);
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

    if (cancelReason == "S�zle�me Devri") {
        if (ButtonCopy()) {
            CloseQuoteRequestMethod(quoteId);//Sat�� iptal
        }
    }
        //else {
        //    if (setRecordStatus("quote", quoteId, "0", "1")) {
        //        if (setRecordStatus("quote", quoteId, "1", "100000000"))//Sat�� �ptal Aktar�ld�
        //        {
        //            if (cancelReason == "Kapora �ptali") {
        //                var Url = "http://intranef.nef.com.tr/SitePages/KaporaIptal.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
        //                window.open(Url);
        //            }
        //            else if (cancelReason == "Daire De�i�ikli�i" || cancelReason == "�sim De�i�ikli�i" || cancelReason == "Hatal� Giri�" || cancelReason == "�deme De�i�ikli�i") {
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
        if (cancelReason == "Kapora �ptali" ) {
            if (projectId.toUpperCase() == TopkapiProjectId) {
                var Url = "http://intranef.nef.com.tr/SitePages/Topkap�IptalFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
                window.open(Url);
            } else {
                var Url = "http://intranef.nef.com.tr/SitePages/KaporaIptal.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
                window.open(Url);
            }
        }

        else if (cancelReason == "Daire De�i�ikli�i" || cancelReason == "�sim De�i�ikli�i" || cancelReason == "Hatal� Giri�" || cancelReason == "�deme De�i�ikli�i") {
            if (projectId.toUpperCase() == TopkapiProjectId) {
                var Url = "http://intranef.nef.com.tr/SitePages/Topkap�IptalFormu.aspx?ID=" + Xrm.Page.getAttribute("quotenumber").getValue();
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
        case "Senetleri �mzala":
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

//Sat��-efektif vade hesaplamalar�- Senet olu�tur bas�ld���nda
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
    var isNotarizedSales = Xrm.Page.getAttribute("new_isnotarizedsales").getValue();//Noterli Sat��
    var quoteId = Xrm.Page.data.entity.getId();
    var total = GetTotalPaymentById(quoteId);
    if (total[1] == true) {
        alert("Senet kay�tlar� olu�tu�u i�in Noterli sat�� g�ncellenemez!!!")
        Xrm.Page.getAttribute("new_isnotarizedsales").setValue(!isNotarizedSales);//Noterli Sat��
    }
}



