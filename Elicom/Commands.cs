using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Devices.Printers.Elicom
{
    enum cmds
    {
        GetStatus = 0x20,
        ClearDisplay = 0x24,
        OpenDrawer = 0x2A,
        DisplayTextLine1 = 0x25,
        DisplayTextLine2 = 0x26,
        Feed = 0x2B,
        GetSerialNum = 0x60,
        GetPrinterTime = 0x68,
        SetPrinterTime = 0x48,
        GetCashierName = 0x6A,
        SetCashierName = 0x4A,

        PrintRecCopy = 0x3A,
        GetLastReceiptNum = 0x71,
        GetLastZRepNum = 0x73,
        DailyFiscalReport = 0x7C,
        CashIO = 0x3B,
        GetSumsByPaymetType = 0x6E,
        SetHeaderFooter = 0x49,
        CancelReceipt = 0x39,
        OpenNoFislalReceipt = 0x2E,
        CloseNoFislalReceipt = 0x2F,
        PrintTextLine = 0x37,
        OpenReceipt = 0x30,
        CloseReceipt = 0x38,
        AutomaticCloseReceipt = 0x36,
        Payment = 0x35,
        Sale = 0x31,
        Barcode = 0x51,
        ArtReport = 0x7E,
        ReadTax = 0x62,
        SetLogo = 0x4C,
        Storno = 0x3C,
        SubTotal = 0x33,
    }


    internal static class Commands
    {
        public const byte GetStatus =0x20 ;
        public const byte ClearDisplay =0x24 ;
        public const byte OpenDrawer =0x2A ;
        public const byte DisplayTextLine1 =0x25 ;
        public const byte DisplayTextLine2 =0x26 ;
        public const byte Feed =0x2B ;
        public const byte GetSerialNum =0x60 ;
        public const byte GetPrinterTime =0x68 ;
        public const byte SetPrinterTime = 0x48;
        public const byte GetCashierName = 0x6A;
        public const byte SetCashierName = 0x4A;

        public const byte PrintRecCopy = 0x3A;
        public const byte GetLastReceiptNum = 0x71;
        public const byte GetLastZRepNum = 0x73;
        public const byte DailyFiscalReport = 0x7C;
        public const byte CashIO = 0x3B;
        public const byte GetSumsByPaymetType = 0x6E;
        public const byte SetHeaderFooter = 0x49;
        public const byte CancelReceipt = 0x39;
        public const byte OpenNoFislalReceipt = 0x2E;
        public const byte CloseNoFislalReceipt = 0x2F;
        public const byte PrintTextLine = 0x37;
        public const byte OpenReceipt = 0x30;
        public const byte CloseReceipt = 0x38;
        public const byte AutomaticCloseReceipt = 0x36;
        public const byte Payment = 0x35;
        public const byte Sale = 0x31;
        public const byte Barcode = 0x51;
        public const byte ArtReport = 0x7E;
        public const byte ReadTax = 0x62;
        public const byte SetLogo = 0x4C;
        public const byte Storno = 0x3C;
        //public const byte Payment = 0x60;
        //public const byte Payment = 0x60;
    }
}
