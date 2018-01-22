namespace Phoenix.Devices.Printers.DatecsMD
{
    using Phoenix.Devices.Printers;
    using Phoenix.Globals.Units;
    using System;

    public class DatecsSums
    {
        public Money TaxA = new Money(0);
        public Money TaxB = new Money(0);
        public Money TaxC = new Money(0);
        public Money TaxD = new Money(0);
        public Money TaxFree = new Money(0);
        public Money TaxM = new Money(0);

        public void AddSum(TaxGrp taxGrp, Money sum)
        {
            switch (taxGrp)
            {
                case TaxGrp.TaxFree:
                    this.TaxFree += sum;
                    return;

                case TaxGrp.TaxA:
                    this.TaxA += sum;
                    return;

                case TaxGrp.TaxB:
                    this.TaxB += sum;
                    return;

                case TaxGrp.TaxC:
                    this.TaxC += sum;
                    return;

                case TaxGrp.TaxD:
                    this.TaxD += sum;
                    return;

                case TaxGrp.TaxE:
                    this.TaxM += sum;
                    return;
            }
            throw new FiscalPrinterException(DatecsStrings.GetString(0x23));
        }

        public void Clear()
        {
            this.TaxA = new Money(0);
            this.TaxB = new Money(0);
            this.TaxC = new Money(0);
            this.TaxD = new Money(0);
            this.TaxFree = new Money(0);
            this.TaxM = new Money(0);
        }

        public Money Total
        {
            get
            {
                Money money = new Money(0);
                money += this.TaxA;
                money += this.TaxB;
                money += this.TaxC;
                money += this.TaxD;
                money += this.TaxFree;
                return (money + this.TaxM);
            }
        }
    }
}

