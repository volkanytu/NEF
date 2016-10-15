// JavaScript source code
//MÜÞTERÝ GERÝ BÝLDÝRÝM
function Form_onload() {
    Xrm.Page.getAttribute("new_acikkalmasuresi").setSubmitMode("always");
    Xrm.Page.getAttribute("new_mailgonderimi").setSubmitMode("always");
    var stateCode = Xrm.Page.getAttribute("statecode").getValue();
    if (stateCode != null) {
        if (stateCode != 1) {
            FormElemanlariKontrol();
            acikKalmaSuresiHesapla();
        }
    }
    if (Xrm.Page.ui.getFormType() == 1) {
        var pickListField = Xrm.Page.getControl("statuscode");
        pickListField.removeOption(100000000);//Çözümlendi
    }
}
function Form_onsave(context) {
    var customerSatisfactionCode = Xrm.Page.getAttribute("customersatisfactioncode").getValue();
   
    if (Xrm.Page.ui.getFormType() == 1) {
        Xrm.Page.getAttribute("new_mailgonderimi").setValue(true);
    }

    if (Xrm.Page.getAttribute('statuscode').getValue() == 100000000) {
        if (Xrm.Page.ui.getFormType() != 2) {
            alert('Ýþleme devam etmeden önce lütfen kayýt ediniz');
            context.getEventArgs().preventDefault();
            return false;
        }
        if (customerSatisfactionCode == null) {
            alert('Ýþlemi Kapatmadan Önce Müþteri Memnuniyet Seviyenizi Seçiniz');
            context.getEventArgs().preventDefault();
            return false;
        }
        
    }
}
function new_geribildirimtarihi_onchange() {

    acikKalmaSuresiHesapla();
}
function followupby_onchange() {

    acikKalmaSuresiHesapla();
}

function acikKalmaSuresiHesapla() {

    var to = Xrm.Page.getAttribute("followupby").getValue();
    var from = Xrm.Page.getAttribute("new_geribldirimtarihi").getValue();
    var Today = new Date();

    if (to != null && from != null && to < from) {
        alert('Ýzleme sonu tarihi, geri bildirim tarihinden önce olamaz.');
        Xrm.Page.getAttribute("followupby").setValue(null);
    }

    if (from != null && to != null) {
        var gun = (to - from) / (24 * 60 * 60 * 1000);
        if (gun > 0)
            Xrm.Page.getAttribute("new_acikkalmasuresi").setValue(gun);
        else
            Xrm.Page.getAttribute("new_acikkalmasuresi").setValue(null);
    }

    else {

        if (from != null && Today != null) {
            var gun = (Today - from) / (24 * 60 * 60 * 1000);
            if (gun > 0)
                Xrm.Page.getAttribute("new_acikkalmasuresi").setValue(gun);
            else
                Xrm.Page.getAttribute("new_acikkalmasuresi").setValue(null);
        }

        else {
            Xrm.Page.getAttribute("new_acikkalmasuresi").setValue(null);
        }
    }
}

function ServisTalepCancel() {
    Xrm.Page.getAttribute("statecode").setSubmitMode("always");
    Xrm.Page.getAttribute("new_cozumayrintisi").setSubmitMode("always");
    Xrm.Page.getControl("new_cozumayrintisi").setVisible(true);
    Xrm.Page.getAttribute("customersatisfactioncode").setRequiredLevel("required");
    Xrm.Page.getAttribute("new_sorumlukisi").setRequiredLevel("required");
    Xrm.Page.getAttribute("followupby").setRequiredLevel("required");


    if (Xrm.Page.getAttribute("customersatisfactioncode").getValue() == null) {
        alert("Lütfen müþteri geri bildirimini çözümlemeden önce memnuniyet alanýný doldurunuz.");
        Xrm.Page.getControl("customersatisfactioncode").setFocus(true);
        return false;
    }
    if (Xrm.Page.getAttribute("new_sorumlukisi").getValue() == null) {
        alert("Lütfen müþteri geri bildirimini çözümlemeden önce sorumlu kiþi alanýný doldurunuz.");
        Xrm.Page.getControl("new_sorumlukisi").setFocus(true);
        return false;
    }
    if (Xrm.Page.getAttribute("followupby").getValue() == null) {
        alert("Lütfen müþteri geri bildirimini çözümlemeden önce izleme sonu alanýný doldurunuz.");
        Xrm.Page.getControl("followupby").setFocus(true);
        return false;
    }
    if (Xrm.Page.getAttribute("customersatisfactioncode").getValue() != null && Xrm.Page.getAttribute("followupby").getValue() != null && Xrm.Page.getAttribute("new_sorumlukisi").getValue() != null) {
        resolve();
    }
    //Mscrm.CommandBarActions.resolve
    ///_static/_common/scripts/CommandBarActions.js
}

function FormElemanlariKontrol() {
    Xrm.Page.getControl("new_sikayetnedeni").setVisible(false);
    var stateCode = Xrm.Page.getAttribute("statecode").getValue();

    var gBildirimTuru = Xrm.Page.getAttribute("new_geribildirimturu").getValue();

    if (gBildirimTuru != null && gBildirimTuru == 1) {
        Xrm.Page.getControl("new_sikayetnedeni").setVisible(true);
        Xrm.Page.getAttribute("new_sikayetnedeni").setRequiredLevel("required");
    }

    else {
        Xrm.Page.getControl("new_sikayetnedeni").setVisible(false);
        Xrm.Page.getAttribute("new_sikayetnedeni").setRequiredLevel("none");
    }

    if (stateCode == 1) {
        Xrm.Page.getControl("new_cozumayrintisi").setVisible(true);
    }
    else {
        Xrm.Page.getControl("new_cozumayrintisi").setVisible(false);
    }
}

function IlkHarfiBuyut() {

    var incelemedetayi = Xrm.Page.getAttribute("new_incelemedetayi").getValue();
    var cozumayrintisi = Xrm.Page.getAttribute("new_cozumayrintisi").getValue();
    var title = Xrm.Page.getAttribute("title").getValue();
    var description = Xrm.Page.getAttribute("description").getValue();
    if (title != null) {
        title = capitaliseFirstLetter(title);
        Xrm.Page.getAttribute("title").setValue(title);
    }

    if (description != null) {
        description = capitaliseFirstLetter(description);
        Xrm.Page.getAttribute("description").setValue(description);
    }

    if (incelemedetayi != null) {
        incelemedetayi = capitaliseFirstLetter(incelemedetayi);
        Xrm.Page.getAttribute("new_incelemedetayi").setValue(new_incelemedetayi);
    }

    if (cozumayrintisi != null) {       
        cozumayrintisi = capitaliseFirstLetter(cozumayrintisi);
        Xrm.Page.getAttribute("new_cozumayrintisi").setValue(cozumayrintisi);
    }
}

function capitaliseFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}
function queryString(val) {
    var q = unescape(location.search.substr(1)).split('&');
    for (var i = 0; i < q.length; i++) {
        var t = q[i].split('=');
        if (t[0].toLowerCase() == val.toLowerCase()) return t[1];
    }
    return '';
}