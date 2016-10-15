var CustomServiceUrl = "http://localhost:49444/SalesPortal.svc";
//var CustomServiceUrl = "/WebServices/NEF.WebServices.SalesPortal/SalesPortal.svc";

var GuidEmpty = '00000000-0000-0000-0000-000000000000';

var RetailerBusinessUnitId = 'CF301385-E04B-E511-80F6-005056A60603';


var ContactTypes = {
    "Aday": 100000000,
    "Gercek": 100000001,
    "Potansiyel": 100000002,
    "": 0
};

var UserTypes = {
    "SatisMuduru": 1,
    "SatisDanismani": 2,
    "CallCenter": 3,
    "Resepsiyonist": 4,
    "SatisDirektorut": 5,
    "MusteriIliskileri": 6,
    "SatisMuduruIsGyo": 7,
    "SatisDanismaniIsGyo": 8,
    "IsGyoCallCenter": 9
};

var DirectionCodes = {
    "Gelen": 0,
    "Giden": 1
}

var EntityTypes = {
    "Firma": 1,
    "Kisi": 2
}

var QuoteStatuses =
{
    "DevamEdiyor": 1,
    "SözleşmeImzalandi": 2,
    "KaporaAlindi": 3,
    "Kazanildi": 4,
    "Kaybedildi": 5,
    "İptalEdildi": 6,
    "Düzeltilmis": 7,
    "IptalAktarildi": 100000000,
    "MuhasebeyeAktarildi": 100000001,
    "TeslimEdildi": 100000002,
    "BittiSatildi": 100000003,
    "OnayBekleniyor": 100000004,
    "Onaylandi": 100000005,
    "Reddedildi": 100000006
}

var GlobalHelper = {
    "CrmUrl": "http://fenixcrm.nef.com.tr/FENiX/",
    "CrmDocumentMergeUrl": "http://fenixcrm.nef.com.tr:9090/",
    "ContactTypes": {
        100000000: "Aday",
        100000001: "Gerçek",
        100000002: "Potansiyel",
        0: "Aday",
        1: "Firma",
        "Aday": 100000000,
        "Gercek": 100000001,
        "Potansiyel": 100000002,
        "Firma": 1
    },
    "ContactTypeClass": {
        100000000: "warning",
        100000001: "success",
        100000002: "danger",
        0: "warning",
        1: "info"
    },
    "ActivityStates": {
        0: "Açık",
        1: "Tamamlandı",
        2: "İptal Edildi",
        3: "Zamanlanmış"
    },
    "ActivityStateClass": {
        0: "warning",
        1: "success",
        2: "danger",
        3: "info"
    },
    "ApprovalTypes": {
        1: "Birim m2",
        2: "Sarı excel indirim oranı"
    },
    "GuidEmpty": "00000000-0000-0000-0000-000000000000",
    "ShowTextAlert": function (containerElement, appendORprepend, alertType, message, isClosable, isResetPrevious, isFocus, closeSecond, icon) {
        //AlerType:success, danger, warning, info
        //CloseSecond 0 ise NeverClose
        Metronic.alert({
            container: containerElement,
            place: appendORprepend,
            type: alertType,
            message: message,
            close: isClosable,
            reset: isResetPrevious,
            focus: isFocus,
            closeInSeconds: closeSecond,
            icon: icon
        });

    },
    "QuoteStatusClasses": {
        100000000: "bg-red-pink",
        100000001: "bg-blue-steel",
        100000002: "bg-green-jungle",
        100000003: "bg-yellow-gold",
        100000004: "bg-yellow",
        100000005: "bg-green",
        100000006: "bg-red",
        100000007: "bg-red-intense",
        100000008: "bg-yellow",
        1: "bg-red-pink",
        2: "bg-blue-steel",
        4: "bg-green-jungle",
        5: "bg-yellow-gold",
        6: "bg-yellow",
        7: "bg-green"

    },
    "ActivityStatusDetails": {
        "TekrarAranacak": "1",
        "SatisOfisineGidecek": "2",
        "MerkezOfisineGidecek": "3",
        "BelirliBirSureOdeyemeyecek": "7"
    },
    "CheckTcNumber": function (tcNo) {

        var returnValue = "";

        if (!tcNo.match(/^[0-9]{11}$/)) {
            returnValue = "Girmiş olduğunuz TC Numarası yanlıştır. Lütfen Geçerli bir TC Numrası girin!";

            return returnValue;
        }

        pr1 = parseInt(tcNo.substr(0, 1));
        pr2 = parseInt(tcNo.substr(1, 1));
        pr3 = parseInt(tcNo.substr(2, 1));
        pr4 = parseInt(tcNo.substr(3, 1));
        pr5 = parseInt(tcNo.substr(4, 1));
        pr6 = parseInt(tcNo.substr(5, 1));
        pr7 = parseInt(tcNo.substr(6, 1));
        pr8 = parseInt(tcNo.substr(7, 1));
        pr9 = parseInt(tcNo.substr(8, 1));
        pr10 = parseInt(tcNo.substr(9, 1));
        pr11 = parseInt(tcNo.substr(10, 1));

        if ((pr1 + pr3 + pr5 + pr7 + pr9 + pr2 + pr4 + pr6 + pr8 + pr10) % 10 != pr11) {
            returnValue = "Girmiş olduğunuz TC Numarası yanlıştır. Lütfen Geçerli bir TC Numrası girin";
            return returnValue;
        }

        if (((pr1 + pr3 + pr5 + pr7 + pr9) * 7 + (pr2 + pr4 + pr6 + pr8) * 9) % 10 != pr10) {
            returnValue = "Girmiş olduğunuz TC Numarası yanlıştır. Lütfen Geçerli bir TC Numrası girin";
            return returnValue;
        }

        if (((pr1 + pr3 + pr5 + pr7 + pr9) * 8) % 10 != pr11) {
            returnValue = "Girmiş olduğunuz TC Numarası yanlıştır. Lütfen Geçerli bir TC Numrası girin";
            return returnValue;
        }

        return true;
    }

}