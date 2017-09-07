using System;

namespace SoftMarket.Devices.Printers.Datecs
{
	/// <summary>
	/// Summary description for Status.
	/// </summary>
	public class Status
	{
		public Status(byte[] statusData)
		{
			generalError          = (statusData[0] & 0x20) != 0;
			printError            = (statusData[0] & 0x10) != 0;
			invalidTime           = (statusData[0] & 0x04) != 0;
			invalidCommand        = (statusData[0] & 0x02) != 0;
			syntaxError           = (statusData[0] & 0x01) != 0;

			serviceCheckOpen      = (statusData[1] & 0x20) != 0;
			ramDestroyed          = (statusData[1] & 0x10) != 0;
			printRestart          = (statusData[1] & 0x08) != 0;
			ramCleared            = (statusData[1] & 0x04) != 0;
			commandNotAllowed     = (statusData[1] & 0x02) != 0;
			sumOverflow           = (statusData[1] & 0x01) != 0;

			nonFiscalCheckOpen    = (statusData[2] & 0x20) != 0;
			fiscalCheckOpen       = (statusData[2] & 0x08) != 0;
			paperOut              = (statusData[2] & 0x01) != 0;

			fiscalRamGeneralError = (statusData[4] & 0x20) != 0;
			fiscalRamIsFull       = (statusData[4] & 0x10) != 0;
			fiscalRamLess30       = (statusData[4] & 0x08) != 0;
			fiscalRamAbsent       = (statusData[4] & 0x04) != 0;
			fiscalRamWriteError   = (statusData[4] & 0x01) != 0;

			fiscalRamFiscalNumber = (statusData[5] & 0x20) != 0;
			fiscalRamSerialNumber = (statusData[5] & 0x10) != 0;
			fiscalRamFiscalized   = (statusData[5] & 0x08) != 0;
			fiscalRamCloseError   = (statusData[5] & 0x04) != 0;
			fiscalRamFormated     = (statusData[5] & 0x02) != 0;
			fiscalRamReadOnly     = (statusData[5] & 0x01) != 0;
		}


		bool generalError          = false;          
		bool printError            = false;              
		bool invalidTime           = false;             
		bool invalidCommand        = false;          
		bool syntaxError           = false;             
		bool serviceCheckOpen      = false;        
		bool ramDestroyed          = false;            
		bool printRestart          = false;            
		bool ramCleared            = false;              
		bool commandNotAllowed     = false;       
		bool sumOverflow           = false;             
		bool nonFiscalCheckOpen    = false;      
		bool fiscalCheckOpen       = false;         
		bool paperOut              = false;                
		bool fiscalRamGeneralError = false;   
		bool fiscalRamIsFull       = false;         
		bool fiscalRamLess30       = false;         
		bool fiscalRamAbsent       = false;         
		bool fiscalRamWriteError   = false;     
		bool fiscalRamFiscalNumber = false;   
		bool fiscalRamSerialNumber = false;   
		bool fiscalRamFiscalized   = false;     
		bool fiscalRamCloseError   = false;     
		bool fiscalRamFormated     = false;       
		bool fiscalRamReadOnly     = false;       
														
		public bool GeneralError          
		{
			get
			{
				return generalError;
			}
		}
		public bool PrintError            
		{
			get
			{
				return printError;
			}
		}
		public bool InvalidTime           
		{
			get
			{
				return invalidTime;
			}
		}
		public bool InvalidCommand        
		{
			get
			{
				return invalidCommand;
			}
		}
		public bool SyntaxError           
		{
			get
			{
				return syntaxError;
			}
		}
		public bool ServiceCheckOpen      
		{
			get
			{
				return serviceCheckOpen;
			}
		}
		public bool RamDestroyed          
		{
			get
			{
				return ramDestroyed;
			}
		}
		public bool PrintRestart          
		{
			get
			{
				return printRestart;
			}
		}
		public bool RamCleared            
		{
			get
			{
				return ramCleared;
			}
		}
		public bool CommandNotAllowed     
		{
			get
			{
				return commandNotAllowed;
			}
		}
		public bool SumOverflow           
		{
			get
			{
				return sumOverflow;
			}
		}
		public bool NonFiscalCheckOpen    
		{
			get
			{
				return nonFiscalCheckOpen;
			}
		}
		public bool FiscalCheckOpen       
		{
			get
			{
				return fiscalCheckOpen;
			}
		}
		public bool PaperOut              
		{
			get
			{
				return paperOut;
			}
		}
		public bool FiscalRamGeneralError 
		{
			get
			{
				return fiscalRamGeneralError;
			}
		}
		public bool FiscalRamIsFull       
		{
			get
			{
				return fiscalRamIsFull;
			}
		}
		public bool FiscalRamLess30       
		{
			get
			{
				return fiscalRamLess30;
			}
		}
		public bool FiscalRamAbsent       
		{
			get
			{
				return fiscalRamAbsent;
			}
		}
		public bool FiscalRamWriteError   
		{
			get
			{
				return fiscalRamWriteError;
			}
		}
		public bool FiscalRamFiscalNumber 
		{
			get
			{
				return fiscalRamFiscalNumber;
			}
		}
		public bool FiscalRamSerialNumber 
		{
			get
			{
				return fiscalRamSerialNumber;
			}
		}
		public bool FiscalRamFiscalized   
		{
			get
			{
				return fiscalRamFiscalized;
			}
		}
		public bool FiscalRamCloseError   
		{
			get
			{
				return fiscalRamCloseError;
			}
		}
		public bool FiscalRamFormated     
		{
			get
			{
				return fiscalRamFormated;
			}
		}
		public bool FiscalRamReadOnly     
		{
			get
			{
				return fiscalRamReadOnly;
			}
		}


		public bool IsOk
		{
			get
			{
				return !this.GeneralError && !this.FiscalRamGeneralError;
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder message = new System.Text.StringBuilder();

			if(PrintError           ) message.Append(DatecsStrings.GetString((int)Message.PrintError           ));
			if(InvalidCommand       ) message.Append(DatecsStrings.GetString((int)Message.InvalidCommand       ));
			if(SyntaxError          ) message.Append(DatecsStrings.GetString((int)Message.SyntaxError          ));
			if(RamDestroyed         ) message.Append(DatecsStrings.GetString((int)Message.RamDestroyed         ));
			if(PrintRestart         ) message.Append(DatecsStrings.GetString((int)Message.PrintRestart         ));
			if(RamCleared           ) message.Append(DatecsStrings.GetString((int)Message.RamCleared           ));
			if(CommandNotAllowed    ) message.Append(DatecsStrings.GetString((int)Message.CommandNotAllowed    ));
			if(PaperOut             ) message.Append(DatecsStrings.GetString((int)Message.PaperOut             ));
			if(FiscalRamIsFull      ) message.Append(DatecsStrings.GetString((int)Message.FiscalRamIsFull      ));
			if(FiscalRamWriteError  ) message.Append(DatecsStrings.GetString((int)Message.FiscalRamWriteError  ));
			if(FiscalRamCloseError  ) message.Append(DatecsStrings.GetString((int)Message.FiscalRamCloseError  ));
			if(FiscalRamReadOnly    ) message.Append(DatecsStrings.GetString((int)Message.FiscalRamReadOnly    ));

			return message.ToString ();
		}
	}
}
