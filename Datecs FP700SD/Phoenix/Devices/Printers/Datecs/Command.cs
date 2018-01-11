namespace Phoenix.Devices.Printers.DatecsMD
{
    using System;

    public enum Command : byte
    {
        AdvancePaper = 0x2c,
      //  CancelFiscalReceipt = 0x39,
        CancelPayment = 0x34,
        ClearDisplay = 0x21,
        CloseFiscalReceipt = 0x38,
        CloseNonfiscalReceipt = 0x27,
        Cut = 0x2d,
        DayInfo = 110,
        DisplayTextLL = 0x23,
        DisplayTextUL = 0x2f,
        FiscalClosure = 0x45,
        Fiscalization = 0x48,

        GetCurrentSums = 0x43,
        GetCurrentSums700 = 0x6E,

        GetCurrentTaxes = 0x41,
        GetCurrentTaxRates = 0x61,
        GetDateTime = 0x3e,
        GetDiagnosticInfo = 90,
        GetModemDataState = 0x7a,
        GetReceiptInfo = 0x67,
        GetStatus = 0x4a,
        LabelMode = 0x7d,
        MakeReceiptCopy = 0x6d,
        OpenDrawer = 0x6a,
        OpenFiscalReceipt = 0x30,
        OpenNonfiscalReceipt = 0x26,
        OpenRefundReceipt = 0x55,
        PrintArtReport = 0x6f,
        PrintBarcode = 0x58,
        PrintFiscalText = 0x36,
        PrintNonfiscalText = 0x2a,
        PrintReportByDate = 0x4f,
        PrintReportByDateFull = 0x5e,
        PrintReportByNum = 0x5f,
        PrintReportByNumFull = 0x49,
        RegisterArt = 0x6b,
        RegisterItem = 0x31,
        RegisterItem3530T = 0x3a,
        ServiceInputOutput = 70,
        SetDateTime = 0x3d,
        SetHeaderFooter = 0x2b,
        SetINNNumber = 0x62,
        SetOperatorName = 0x66,
        SetTaxRates = 0x60,
        SetTaxRates350T = 0x53,
        SubTotal = 0x33,
        Total = 0x35,
        TotalAndClose = 0x37,
        VoidSell = 0x69,

        /////////////////////////////
        CheckCancelState = 103,
        CancelFiscalReceipt = 60

    }
}

