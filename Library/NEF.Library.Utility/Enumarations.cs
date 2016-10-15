using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Utility
{
    public enum CollaborateDataType
    {
        HouseData,
        OpportunityData,
        ContactData,
        AppointmentData,
        PhoneCallData,
        SalesData,
        PaymentData,
        WebFormData,
        AccountData
    }

    public enum QuoteStatus
    {
        SozlesmeHazirlandi = 100000009,
        DevamEdiyor = 1,
        Sözleşmeİmzalandı = 2,
        KaporaAlındı = 100000007,
        Kazanıldı = 4,
        Kaybedildi = 5,
        İptalEdildi = 6,
        Düzeltilmiş = 7,
        İptalAktarıldı = 100000000,
        MuhasebeyeAktarıldı = 100000001,
        TeslimEdildi = 100000002,
        BittiSatıldı = 100000003,
        OnayBekleniyor = 100000004,
        Onaylandı = 100000005,
        Reddedildi = 100000006,
        DirektorOnayiBekleniyor = 100000008
    }

    public enum RentalStatuses
    {
        Taslak = 1,
        OnOdemeAlindi = 100000000,
        Tamamlandi = 100000001,
        OnayBekleniyor = 100000002,
        Onaylandi = 100000003,
        IptalEdildi = 2
    }

    public enum SecondHandStatuses
    {
        Taslak = 1,
        OnOdemeAlindi = 100000000,
        Tamamlandi = 100000001,
        OnayBekleniyor = 100000002,
        Onaylandi = 100000003,
        Reddedildi = 100000004,
        IptalEdildi = 2
    }

    public enum GsmOperators
    {
        Turkcell,
        Avea,
        Vodafone,
    }

    public enum FallingApprovalTypes
    {
        m2 = 1,
        SariExcel = 2
    }

    public enum CustomerSalesType
    {
        personal = 1,
        Borter = 2,
        Alternative = 3,
        Corp = 4,
        ForeignSales = 5
    }

    public enum ContactTypes
    {
        Firma = 1,
        Aday = 100000000,
        Gercek = 100000001,
        Potansiyel = 100000002
    }

    public enum GenderCodes
    {
        Erkek = 1,
        Kadın
    }

    public enum FamilyStatusCodes
    {
        Bekar = 1,
        Evli,
        Bosanmis,
        Dul
    }

    public enum ActivityStateCodes
    {
        Acik = 0,
        Tamamlandi,
        IptalEdildi,
        Zamanlanmis
    }

    public enum ActivityStatusCodes
    {
        Bos = 0,
        Ilgili = 1,
        Ilgisiz
    }

    public enum Directions
    {
        Gelen = 0,
        Giden = 1
    }

    public enum ObjectTypeCodes
    {
        Telefon = 4210,
        Randevu = 4201
    }

    public enum SalesTypes
    {
        Bos = -1,
        Yeni = 100000000,
        Devir = 100000001,
        Degisim = 100000002
    }

    public enum DiscountProtocolTypes
    {
        NefAile = 100000000,
        m2indirimi = 100000001,
        SerefiyeFarkiIndirimi = 100000002,
        MemnuniyetIndirimi = 100000003,
        OzelVipIndirimi = 100000004
    }

    public enum DiscountProtocolStatus
    {
        Waiting = 100000000,
        Active = 1,
        Passive = 100000001
    }

    public enum PaymentTypes
    {
        Bos = 0,
        AraOdeme = 1,
        DuzenliTaksit = 2,
        PesinOdeme = 3,
        KaporaOdemesi = 4,
        NefAileIndirimi = 5,
        KDV = 6,
        DamgaVergisi = 7,
        HataliOdeme = 8,
        BankaOdemesi
    }

    public enum PaymentCashTypes
    {
        Havale = 100000000,
        Nakit = 1000000001,
        KrediKartı = 100000002,
        Senet = 100000003,
        Cek = 100000004,
        DamgaVergisiSenet = 100000005,
        DamgaVergisiVirman = 100000007
    }

    public enum PaymentAccountingTypes
    {
        MusteriPesinat_Nakit = 1,
        MusteriPesinat_BankaKredisi = 2,
        MusteriTaksit = 3,
        MusteriTeslim = 4,
        MusteriTaksit_TeslimSonrası = 5,
        KDV = 6,
        DamgaVergisi = 7,
        Cek = 8,
        Barter = 9
    }
    public enum PaymentStatuses
    {
        KaporaAlindi = 100000000,
        Reddedildi = 2
    }

    public enum ProductStatuses
    {
        Bos = 1,
        Satildi = 100000000,
        SozlesmeImzalandi = 100000001,
        OnSatisYapildi = 100000002,
        Opsiyonlu = 100000003,
        SozlesmeHazirlandi = 100000004,
        YoneticiOpsiyonlu = 100000007,

    }

    public enum UserTypes
    {
        SatisMuduru = 1,
        SatisDanismani = 2,
        CallCenter = 3,
        Resepsiyonist = 4,
        SatisDirektoru = 5,
        MusteriIliskileri = 6,
        IsGyoSatisMuduru = 7,
        IsGyoSatisDanismani = 8,
        IsGyoCallCenter = 9,
        IkinciElSatisYoneticisi = 10,
        IkinciElSatisDanismani = 11,
        IkinciElSatisDirektoru = 12,
        Bos = 0
    }

    public enum QueueTypes
    {
        CallCenter = 1
    }

    public enum JobTitles
    {
        Doktor = 1,
        Muhendis = 2,
        Avukat = 3,
        Mimar = 4
    }

    public enum PriorityValues
    {
        Dusuk = 0,
        Normal = 1,
        Yuksek = 2
    }

    public enum InvestmentType
    {
        Yatırım = 1,
        Oturum = 2
    }
}
