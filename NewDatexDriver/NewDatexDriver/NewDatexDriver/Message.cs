using System;

namespace Phoenix.Devices.Printers.Datecs
{
    /// <summary>
    /// Константы сообщений.
    /// </summary>
    public enum Message
    {
        PortTypeCorrupt = 1,
        RecivedNAK = 2,
        ReciveTimeout = 3,
        RecivedBadPacket = 4,

        PrintError = 5,
        InvalidCommand = 6,
        SyntaxError = 7,
        RamDestroyed = 8,
        PrintRestart = 9,
        RamCleared = 10,
        CommandNotAllowed = 11,
        PaperOut = 12,
        FiscalRamIsFull = 13,
        FiscalRamWriteError = 14,
        FiscalRamCloseError = 15,
        FiscalRamReadOnly = 16,
        DeveceNotOpen = 17,

        ResponseF = 18,
        Response1 = 19,
        Response2 = 20,
        Response3 = 21,
        ResponseE = 22,
        ResponseD = 23,
        ResponseR = 24,
        ResponseI = 25,
        RecivedUninspected = 26,
        NotSupported = 27,
        NullReceipt = 28,
        ReceiptAlreadyOpen = 29,
        CashNum = 30,
        Check = 31,
        RefundByCheck = 32,
        CopyByCheck = 33,
        ReceiptNotOpen = 34,
        TaxCorrupt = 35,
        Cancel = 36,
        User = 37,
        Discount = 38,
        Total = 39,
        Refund = 40,
        Currency = 41,
        NDS = 42,
        Cashier = 43,
        DeviceFriendlyName = 44,
        FP3530TFriendlyName = 45,
        LicenseCorrupt = 46,
        SleepAfterPositionPrint = 47,
        ExellioFriendlyName = 48,
        AddPaymentSums = 49,
        Yes = 50,
        No = 51,
        FP3530T131FriendlyName = 52,
    }
}
