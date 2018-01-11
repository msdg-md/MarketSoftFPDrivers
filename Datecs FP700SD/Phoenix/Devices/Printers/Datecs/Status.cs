namespace Phoenix.Devices.Printers.DatecsMD
{
    using System;
    using System.Text;

    public class Status
    {
        private bool commandNotAllowed;
        private bool fiscalCheckOpen;
        private bool fiscalRamAbsent;
        private bool fiscalRamCloseError;
        private bool fiscalRamFiscalized;
        private bool fiscalRamFiscalNumber;
        private bool fiscalRamFormated;
        private bool fiscalRamGeneralError;
        private bool fiscalRamIsFull;
        private bool fiscalRamLess30;
        private bool fiscalRamReadOnly;
        private bool fiscalRamSerialNumber;
        private bool fiscalRamWriteError;
        private bool generalError;
        private bool invalidCommand;
        private bool invalidTime;
        private bool nonFiscalCheckOpen;
        private bool paperOut;
        private bool printError;
        private bool printRestart;
        private bool ramCleared;
        private bool ramDestroyed;
        private bool serviceCheckOpen;
        private bool sumOverflow;
        private bool syntaxError;

        public Status(byte[] statusData)
        {
            this.generalError = (statusData[0] & 0x20) != 0;
            this.printError = (statusData[0] & 0x10) != 0;
            this.invalidTime = (statusData[0] & 4) != 0;
            this.invalidCommand = (statusData[0] & 2) != 0;
            this.syntaxError = (statusData[0] & 1) != 0;
            this.serviceCheckOpen = (statusData[1] & 0x20) != 0;
            this.ramDestroyed = (statusData[1] & 0x10) != 0;
            this.printRestart = (statusData[1] & 8) != 0;
            this.ramCleared = (statusData[1] & 4) != 0;
            this.commandNotAllowed = (statusData[1] & 2) != 0;
            this.sumOverflow = (statusData[1] & 1) != 0;
            this.nonFiscalCheckOpen = (statusData[2] & 0x20) != 0;
            this.fiscalCheckOpen = (statusData[2] & 8) != 0;
            this.paperOut = (statusData[2] & 1) != 0;
            this.fiscalRamGeneralError = (statusData[4] & 0x20) != 0;
            this.fiscalRamIsFull = (statusData[4] & 0x10) != 0;
            this.fiscalRamLess30 = (statusData[4] & 8) != 0;
            this.fiscalRamAbsent = (statusData[4] & 4) != 0;
            this.fiscalRamWriteError = (statusData[4] & 1) != 0;
            this.fiscalRamFiscalNumber = (statusData[5] & 0x20) != 0;
            this.fiscalRamSerialNumber = (statusData[5] & 0x10) != 0;
            this.fiscalRamFiscalized = (statusData[5] & 8) != 0;
            this.fiscalRamCloseError = (statusData[5] & 4) != 0;
            this.fiscalRamFormated = (statusData[5] & 2) != 0;
            this.fiscalRamReadOnly = (statusData[5] & 1) != 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.PrintError)
            {
                builder.Append(DatecsStrings.GetString(5));
            }
            if (this.InvalidCommand)
            {
                builder.Append(DatecsStrings.GetString(6));
            }
            if (this.SyntaxError)
            {
                builder.Append(DatecsStrings.GetString(7));
            }
            if (this.RamDestroyed)
            {
                builder.Append(DatecsStrings.GetString(8));
            }
            if (this.PrintRestart)
            {
                builder.Append(DatecsStrings.GetString(9));
            }
            if (this.RamCleared)
            {
                builder.Append(DatecsStrings.GetString(10));
            }
            if (this.CommandNotAllowed)
            {
                builder.Append(DatecsStrings.GetString(11));
            }
            if (this.PaperOut)
            {
                builder.Append(DatecsStrings.GetString(12));
            }
            if (this.FiscalRamIsFull)
            {
                builder.Append(DatecsStrings.GetString(13));
            }
            if (this.FiscalRamWriteError)
            {
                builder.Append(DatecsStrings.GetString(14));
            }
            if (this.FiscalRamCloseError)
            {
                builder.Append(DatecsStrings.GetString(15));
            }
            if (this.FiscalRamReadOnly)
            {
                builder.Append(DatecsStrings.GetString(0x10));
            }
            return builder.ToString();
        }

        public bool CommandNotAllowed =>
            this.commandNotAllowed;

        public bool FiscalCheckOpen =>
            this.fiscalCheckOpen;

        public bool FiscalRamAbsent =>
            this.fiscalRamAbsent;

        public bool FiscalRamCloseError =>
            this.fiscalRamCloseError;

        public bool FiscalRamFiscalized =>
            this.fiscalRamFiscalized;

        public bool FiscalRamFiscalNumber =>
            this.fiscalRamFiscalNumber;

        public bool FiscalRamFormated =>
            this.fiscalRamFormated;

        public bool FiscalRamGeneralError =>
            this.fiscalRamGeneralError;

        public bool FiscalRamIsFull =>
            this.fiscalRamIsFull;

        public bool FiscalRamLess30 =>
            this.fiscalRamLess30;

        public bool FiscalRamReadOnly =>
            this.fiscalRamReadOnly;

        public bool FiscalRamSerialNumber =>
            this.fiscalRamSerialNumber;

        public bool FiscalRamWriteError =>
            this.fiscalRamWriteError;

        public bool GeneralError =>
            this.generalError;

        public bool InvalidCommand =>
            this.invalidCommand;

        public bool InvalidTime =>
            this.invalidTime;

        public bool IsOk =>
            (((!this.GeneralError && !this.FiscalRamGeneralError) && !this.RamDestroyed) && !this.FiscalRamWriteError);

        public bool NonFiscalCheckOpen =>
            this.nonFiscalCheckOpen;

        public bool PaperOut =>
            this.paperOut;

        public bool PrintError =>
            this.printError;

        public bool PrintRestart =>
            this.printRestart;

        public bool RamCleared =>
            this.ramCleared;

        public bool RamDestroyed =>
            this.ramDestroyed;

        public bool ServiceCheckOpen =>
            this.serviceCheckOpen;

        public bool SumOverflow =>
            this.sumOverflow;

        public bool SyntaxError =>
            this.syntaxError;
    }
}

