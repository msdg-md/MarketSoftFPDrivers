using System;
using SoftMarket.Globals.Units;

namespace SoftMarket.Devices.Printers.Datecs
{
    public enum Flag : byte 
	{
		Begin     = 0x01,
		End       = 0x03,
		Separator = 0x04,
		EndData   = 0x05,

		NAK = 0x15,
		SYN = 0x16,
		
		ResponseOK = 0x50,
		ResponseF  = 0x46,
		Response1  = 0x31,
		Response2  = 0x32,
		Response3  = 0x33,
		ResponseE  = 0x45,
		ResponseD  = 0x44,
		ResponseR  = 0x52,
		ResponseI  = 0x49,
	}

	public enum Command: byte
	{
		GetStatus             = 74,
		GetDiagnosticInfo     = 90,
		OpenDrawer            = 106,
		ServiceInputOutput    = 70,
		GetCurrentTaxRates    = 97,
		AdvancePaper          = 44,
		GetCurrentSums        = 67,
		DayInfo               = 110,
		SetDateTime           = 61,
		GetDateTime           = 62,
		ClearDisplay          = 33,
		DisplayTextLL         = 35,
		DisplayTextUL         = 47,
		SetOperatorName       = 102,
		FiscalClosure         = 69, 
		PrintReportByNum      = 95,
		PrintReportByDate     = 79,
		SetHeaderFooter       = 43,
		OpenFiscalReceipt     = 48,
		PrintFiscalText       = 54,
        Total                 = 53,
        TotalAndClose         = 55,
		CloseFiscalReceipt    = 56,
		MakeReceiptCopy       = 109,
		Cut                   = 45,
		OpenNonfiscalReceipt  = 38,
		PrintNonfiscalText    = 42,
		RegisterItem          = 49,
		CloseNonfiscalReceipt = 39,
		GetReceiptInfo        = 103,
		CancelPayment         = 52,
		VoidSell              = 105,
		SetTaxRates           = 96,
        GetCurrentTaxes       = 65,
        PrintArtReport        = 111,
        RegisterArt           = 107,
        RegisterItem3530T     = 58,
        OpenRefundReceipt     = 85,
        CancelFiscalReceipt   = 57,
        SubTotal              = 51,
        SetTaxRates350T       = 83,
        SetINNNumber          = 98,
        Fiscalization         = 72,

    }

    public class DatecsPaymentTypes
	{
		public const char Cash   = 'P';
		public const char Credit = 'N';
		public const char Check  = 'C';
		public const char Card   = 'D';
	}
	public class DatecsTaxNames
	{
		public const char TaxA    = 'A';
		public const char TaxB    = 'B';
		public const char TaxC    = 'C';
		public const char TaxD    = 'D';
        public const char TaxFree = '*';
        public const char TaxFree3530T = '?';
    }							   
	public class DatecsSums
	{
		public Money TaxA    = new Money(0);
		public Money TaxB    = new Money(0);
		public Money TaxC    = new Money(0);
		public Money TaxD    = new Money(0);
		public Money TaxFree = new Money(0);
		public void Clear()
		{
			TaxA    = new Money(0);
			TaxB    = new Money(0);
			TaxC    = new Money(0);
			TaxD    = new Money(0);
			TaxFree = new Money(0);
		}
		public void AddSum(TaxGrp taxGrp, Money sum)
		{
			switch(taxGrp)
			{
				case TaxGrp.TaxA:
					TaxA += sum;
					break;
				case TaxGrp.TaxB:
					TaxB += sum;
					break;
				case TaxGrp.TaxC:
					TaxC += sum;
					break;
				case TaxGrp.TaxD:
					TaxD += sum;
					break;
				case TaxGrp.TaxFree:
					TaxFree += sum;
					break;
				default:
					throw new FiscalPrinterException(DatecsStrings.GetString((int)Message.TaxCorrupt));
			}
		}
		public Money Total
		{
			get
			{
				Money total = new Money(0);
				total += TaxA;
				total += TaxB;
				total += TaxC;
				total += TaxD;
				total += TaxFree;
				return total;
			}
		}
	}							   
	public class Const
	{
		public const int MaxCommandNumber = 0xFF - 0x20;
        public const int MaxRecItemCount  = 250;
        public const int MaxRecItemCountDatecs3530T = 500;
		public const int RepeatCount      = 2;
		public const int MaxHeaderLenght  = 40;
		public const int MaxFiscalString  = 24;
		public const int MaxNonFiscalString  = 34;
		public const int MaxPaymentString = 25;
		public const int MaxArticleString = 24;
        public const int MaxArticleString3530T = 24;
        public const int ReadTimeOut = 1000;//msc
		public const string PrinterName   = "Datecs";
		public const char DataSeparator   = ',';
		public const string DateTimeFormat = "dd-MM-yy HH:mm:ss";
		public const int DisplayLen = 20;
		public const string StdUserPassword = "0000";
		public static byte[] ToPrn = new byte[]
	{
		0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
		0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
		0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
		0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
		0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
		0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
		0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
		0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,

		0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
		0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
		0x20, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0x85, 0xA9, 0xC0, 0xAB, 0xAC, 0xAD, 0xAE, 0xC2,
		0xB0, 0xB1, 0x49, 0x69, 0xB4, 0xB5, 0xB6, 0xB7, 0xA5, 0xB9, 0xC1, 0xBB, 0xBC, 0xBD, 0xBE, 0xC3,
		0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
		0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
		0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF,
		0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF
	};

		public static byte[] ToANSI = new byte[]
	{
		0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
		0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
		0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
		0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
		0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
		0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
		0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
		0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,

		0xC0, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF,
		0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF,
		0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xC0, 0xAB, 0xAC, 0xAD, 0xAE, 0xC2,
		0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF,
		0xAA, 0xBA, 0xAF, 0xBF, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF,
		0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF,
		0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF,
		0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF
	};
	}
}
