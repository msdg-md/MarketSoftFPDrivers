//////////////////////////////////////////////////////////////////////
// Copyright (c) 2005 Tremol Ltd.
// License: Mozilla Public License 1.1
// Author: Stanimir Jordanov
// Contacts: software@tremol.bg
//////////////////////////////////////////////////////////////////////
// This is a sample program for zfplib COM library (Zeka FP)
//////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using ZFPCOMLib;

namespace zfpcs
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ZFPForm : System.Windows.Forms.Form
	{
		private ZekaFPClass zfp;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button FindCom;
		private System.Windows.Forms.ComboBox ComPorts;
		private System.Windows.Forms.Label version;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button OpenFiscalBon;
		private System.Windows.Forms.TextBox cOperator;
		private System.Windows.Forms.TextBox cPassword;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cTaxGroup;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox cPrice;
		private System.Windows.Forms.TextBox cQuantity;
		private System.Windows.Forms.TextBox cDiscount;
		private System.Windows.Forms.Button Sell;
		private System.Windows.Forms.TextBox cName;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Button CalcSum;
		private System.Windows.Forms.TextBox cSum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button Pay;
		private System.Windows.Forms.ComboBox cPayType;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.Button bonInfo;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label cPurchases;
		private System.Windows.Forms.CheckBox cNoVoid;
		private System.Windows.Forms.CheckBox cPrintVAT;
		private System.Windows.Forms.CheckBox cDetailed;
		private System.Windows.Forms.CheckBox cPayStarted;
		private System.Windows.Forms.CheckBox cPayFinished;
		private System.Windows.Forms.Label cVATa;
		private System.Windows.Forms.Label cVATc;
		private System.Windows.Forms.Label cVATb;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.Button closeFiscal;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.ComboBox cBaud;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private uint IndexToBaud(int index)
		{
			uint[] b = new uint[] { 9600, 19200, 38400, 57600, 115200 };

			if (index < 0 || index >= b.Length) return 0;

			return b[index];
		}

		private int BaudToIndex(uint baud)
		{
			switch (baud)
			{
				case 9600:
					return 0;

				case 19200:
					return 1;

				case 38400:
					return 2;

				case 57600:
					return 3;

				case 115200:
					return 4;
			}
			return 0;
		}

		public ZFPForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ComPorts.SelectedIndex = 0;
			cTaxGroup.SelectedIndex = 0;
			cPayType.SelectedIndex = 0;
			cBaud.SelectedIndex = 0;

            zfp = new ZekaFPClass();
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.ComPorts = new System.Windows.Forms.ComboBox();
			this.FindCom = new System.Windows.Forms.Button();
			this.version = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cOperator = new System.Windows.Forms.TextBox();
			this.cPassword = new System.Windows.Forms.TextBox();
			this.OpenFiscalBon = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cTaxGroup = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.cPrice = new System.Windows.Forms.TextBox();
			this.cQuantity = new System.Windows.Forms.TextBox();
			this.cDiscount = new System.Windows.Forms.TextBox();
			this.Sell = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.CalcSum = new System.Windows.Forms.Button();
			this.cSum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.cPayType = new System.Windows.Forms.ComboBox();
			this.Pay = new System.Windows.Forms.Button();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.bonInfo = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.cPurchases = new System.Windows.Forms.Label();
			this.cVATa = new System.Windows.Forms.Label();
			this.cVATc = new System.Windows.Forms.Label();
			this.cVATb = new System.Windows.Forms.Label();
			this.cNoVoid = new System.Windows.Forms.CheckBox();
			this.cPrintVAT = new System.Windows.Forms.CheckBox();
			this.cDetailed = new System.Windows.Forms.CheckBox();
			this.cPayStarted = new System.Windows.Forms.CheckBox();
			this.cPayFinished = new System.Windows.Forms.CheckBox();
			this.pictureBox4 = new System.Windows.Forms.PictureBox();
			this.closeFiscal = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.cBaud = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "COM port:";
			// 
			// ComPorts
			// 
			this.ComPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComPorts.Items.AddRange(new object[] {
														  "COM1",
														  "COM2",
														  "COM3",
														  "COM4"});
			this.ComPorts.Location = new System.Drawing.Point(68, 16);
			this.ComPorts.Name = "ComPorts";
			this.ComPorts.Size = new System.Drawing.Size(58, 21);
			this.ComPorts.TabIndex = 1;
			// 
			// FindCom
			// 
			this.FindCom.Location = new System.Drawing.Point(249, 16);
			this.FindCom.Name = "FindCom";
			this.FindCom.Size = new System.Drawing.Size(39, 23);
			this.FindCom.TabIndex = 2;
			this.FindCom.Text = "&Find";
			this.FindCom.Click += new System.EventHandler(this.FindCom_Click);
			// 
			// version
			// 
			this.version.Location = new System.Drawing.Point(295, 20);
			this.version.Name = "version";
			this.version.Size = new System.Drawing.Size(180, 16);
			this.version.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Operator:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(162, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "Password:";
			// 
			// cOperator
			// 
			this.cOperator.Location = new System.Drawing.Point(68, 52);
			this.cOperator.Name = "cOperator";
			this.cOperator.Size = new System.Drawing.Size(68, 20);
			this.cOperator.TabIndex = 6;
			this.cOperator.Text = "1";
			// 
			// cPassword
			// 
			this.cPassword.Location = new System.Drawing.Point(229, 52);
			this.cPassword.Name = "cPassword";
			this.cPassword.Size = new System.Drawing.Size(68, 20);
			this.cPassword.TabIndex = 7;
			this.cPassword.Text = "0000";
			// 
			// OpenFiscalBon
			// 
			this.OpenFiscalBon.Location = new System.Drawing.Point(309, 52);
			this.OpenFiscalBon.Name = "OpenFiscalBon";
			this.OpenFiscalBon.Size = new System.Drawing.Size(141, 23);
			this.OpenFiscalBon.TabIndex = 8;
			this.OpenFiscalBon.Text = "Open Fiscal Receipt (&1)";
			this.OpenFiscalBon.Click += new System.EventHandler(this.OpenFiscalBon_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Black;
			this.pictureBox1.Location = new System.Drawing.Point(8, 88);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(450, 1);
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(71, 14);
			this.label4.TabIndex = 10;
			this.label4.Text = "Article name:";
			// 
			// cName
			// 
			this.cName.Location = new System.Drawing.Point(77, 101);
			this.cName.MaxLength = 36;
			this.cName.Name = "cName";
			this.cName.Size = new System.Drawing.Size(247, 20);
			this.cName.TabIndex = 11;
			this.cName.Text = "Test article";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(333, 105);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 14);
			this.label5.TabIndex = 12;
			this.label5.Text = "Tax group:";
			// 
			// cTaxGroup
			// 
			this.cTaxGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cTaxGroup.Items.AddRange(new object[] {
														   "À",
														   "Á",
														   "Â"});
			this.cTaxGroup.Location = new System.Drawing.Point(398, 101);
			this.cTaxGroup.Name = "cTaxGroup";
			this.cTaxGroup.Size = new System.Drawing.Size(56, 21);
			this.cTaxGroup.TabIndex = 13;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 138);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(36, 15);
			this.label6.TabIndex = 14;
			this.label6.Text = "Price:";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(105, 138);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(52, 14);
			this.label7.TabIndex = 15;
			this.label7.Text = "Quantity:";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(242, 138);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 14);
			this.label8.TabIndex = 16;
			this.label8.Text = "Addition / discount:";
			// 
			// cPrice
			// 
			this.cPrice.Location = new System.Drawing.Point(43, 137);
			this.cPrice.MaxLength = 10;
			this.cPrice.Name = "cPrice";
			this.cPrice.Size = new System.Drawing.Size(53, 20);
			this.cPrice.TabIndex = 17;
			this.cPrice.Text = "1.23";
			this.cPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// cQuantity
			// 
			this.cQuantity.Location = new System.Drawing.Point(157, 137);
			this.cQuantity.MaxLength = 10;
			this.cQuantity.Name = "cQuantity";
			this.cQuantity.Size = new System.Drawing.Size(66, 20);
			this.cQuantity.TabIndex = 18;
			this.cQuantity.Text = "4.321";
			this.cQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// cDiscount
			// 
			this.cDiscount.Location = new System.Drawing.Point(344, 137);
			this.cDiscount.MaxLength = 10;
			this.cDiscount.Name = "cDiscount";
			this.cDiscount.Size = new System.Drawing.Size(45, 20);
			this.cDiscount.TabIndex = 19;
			this.cDiscount.Text = "0.0";
			this.cDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// Sell
			// 
			this.Sell.Location = new System.Drawing.Point(397, 134);
			this.Sell.Name = "Sell";
			this.Sell.Size = new System.Drawing.Size(57, 23);
			this.Sell.TabIndex = 20;
			this.Sell.Text = "Sell (&2)";
			this.Sell.Click += new System.EventHandler(this.Sell_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.BackColor = System.Drawing.Color.Black;
			this.pictureBox2.Location = new System.Drawing.Point(8, 178);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(450, 1);
			this.pictureBox2.TabIndex = 21;
			this.pictureBox2.TabStop = false;
			// 
			// CalcSum
			// 
			this.CalcSum.Location = new System.Drawing.Point(8, 197);
			this.CalcSum.Name = "CalcSum";
			this.CalcSum.Size = new System.Drawing.Size(150, 23);
			this.CalcSum.TabIndex = 22;
			this.CalcSum.Text = "Calc intermediate sum (&3)";
			this.CalcSum.Click += new System.EventHandler(this.CalcSum_Click);
			// 
			// cSum
			// 
			this.cSum.Location = new System.Drawing.Point(170, 197);
			this.cSum.Name = "cSum";
			this.cSum.Size = new System.Drawing.Size(92, 20);
			this.cSum.TabIndex = 23;
			this.cSum.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(277, 201);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(53, 14);
			this.label9.TabIndex = 24;
			this.label9.Text = "Pay type:";
			// 
			// cPayType
			// 
			this.cPayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cPayType.Items.AddRange(new object[] {
														  "0",
														  "1",
														  "2",
														  "3",
														  "4"});
			this.cPayType.Location = new System.Drawing.Point(331, 197);
			this.cPayType.Name = "cPayType";
			this.cPayType.Size = new System.Drawing.Size(49, 21);
			this.cPayType.TabIndex = 25;
			// 
			// Pay
			// 
			this.Pay.Location = new System.Drawing.Point(392, 197);
			this.Pay.Name = "Pay";
			this.Pay.Size = new System.Drawing.Size(62, 23);
			this.Pay.TabIndex = 26;
			this.Pay.Text = "Pay it (&4)";
			this.Pay.Click += new System.EventHandler(this.Pay_Click);
			// 
			// pictureBox3
			// 
			this.pictureBox3.BackColor = System.Drawing.Color.Black;
			this.pictureBox3.Location = new System.Drawing.Point(8, 241);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(450, 1);
			this.pictureBox3.TabIndex = 27;
			this.pictureBox3.TabStop = false;
			// 
			// bonInfo
			// 
			this.bonInfo.Location = new System.Drawing.Point(8, 258);
			this.bonInfo.Name = "bonInfo";
			this.bonInfo.Size = new System.Drawing.Size(160, 23);
			this.bonInfo.TabIndex = 28;
			this.bonInfo.Text = "Get current receipt info (&5)";
			this.bonInfo.Click += new System.EventHandler(this.bonInfo_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(230, 263);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(93, 15);
			this.label10.TabIndex = 29;
			this.label10.Text = "Purchases count:";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(8, 298);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(70, 13);
			this.label11.TabIndex = 30;
			this.label11.Text = "VAT class A:";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(290, 298);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(71, 13);
			this.label12.TabIndex = 31;
			this.label12.Text = "VAT class C:";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(147, 298);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(70, 13);
			this.label13.TabIndex = 32;
			this.label13.Text = "VAT class B:";
			// 
			// cPurchases
			// 
			this.cPurchases.Location = new System.Drawing.Point(319, 263);
			this.cPurchases.Name = "cPurchases";
			this.cPurchases.Size = new System.Drawing.Size(100, 15);
			this.cPurchases.TabIndex = 33;
			this.cPurchases.Text = "0";
			// 
			// cVATa
			// 
			this.cVATa.Location = new System.Drawing.Point(74, 298);
			this.cVATa.Name = "cVATa";
			this.cVATa.Size = new System.Drawing.Size(72, 15);
			this.cVATa.TabIndex = 34;
			this.cVATa.Text = "0";
			// 
			// cVATc
			// 
			this.cVATc.Location = new System.Drawing.Point(357, 298);
			this.cVATc.Name = "cVATc";
			this.cVATc.Size = new System.Drawing.Size(72, 15);
			this.cVATc.TabIndex = 35;
			this.cVATc.Text = "0";
			// 
			// cVATb
			// 
			this.cVATb.Location = new System.Drawing.Point(212, 298);
			this.cVATb.Name = "cVATb";
			this.cVATb.Size = new System.Drawing.Size(72, 15);
			this.cVATb.TabIndex = 36;
			this.cVATb.Text = "0";
			// 
			// cNoVoid
			// 
			this.cNoVoid.Location = new System.Drawing.Point(8, 327);
			this.cNoVoid.Name = "cNoVoid";
			this.cNoVoid.Size = new System.Drawing.Size(126, 20);
			this.cNoVoid.TabIndex = 37;
			this.cNoVoid.Text = "Void forbiden";
			// 
			// cPrintVAT
			// 
			this.cPrintVAT.Location = new System.Drawing.Point(147, 327);
			this.cPrintVAT.Name = "cPrintVAT";
			this.cPrintVAT.Size = new System.Drawing.Size(126, 20);
			this.cPrintVAT.TabIndex = 38;
			this.cPrintVAT.Text = "Print VAT in receipt";
			// 
			// cDetailed
			// 
			this.cDetailed.Location = new System.Drawing.Point(290, 327);
			this.cDetailed.Name = "cDetailed";
			this.cDetailed.Size = new System.Drawing.Size(126, 20);
			this.cDetailed.TabIndex = 39;
			this.cDetailed.Text = "Detailed receipt";
			// 
			// cPayStarted
			// 
			this.cPayStarted.Location = new System.Drawing.Point(8, 359);
			this.cPayStarted.Name = "cPayStarted";
			this.cPayStarted.Size = new System.Drawing.Size(126, 20);
			this.cPayStarted.TabIndex = 40;
			this.cPayStarted.Text = "Payment started";
			// 
			// cPayFinished
			// 
			this.cPayFinished.Location = new System.Drawing.Point(147, 359);
			this.cPayFinished.Name = "cPayFinished";
			this.cPayFinished.Size = new System.Drawing.Size(126, 20);
			this.cPayFinished.TabIndex = 41;
			this.cPayFinished.Text = "Payment finished";
			// 
			// pictureBox4
			// 
			this.pictureBox4.BackColor = System.Drawing.Color.Black;
			this.pictureBox4.Location = new System.Drawing.Point(8, 395);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new System.Drawing.Size(450, 1);
			this.pictureBox4.TabIndex = 42;
			this.pictureBox4.TabStop = false;
			// 
			// closeFiscal
			// 
			this.closeFiscal.Location = new System.Drawing.Point(316, 408);
			this.closeFiscal.Name = "closeFiscal";
			this.closeFiscal.Size = new System.Drawing.Size(138, 23);
			this.closeFiscal.TabIndex = 43;
			this.closeFiscal.Text = "Close Fiscal Receipt (&6)";
			this.closeFiscal.Click += new System.EventHandler(this.closeFiscal_Click);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(129, 20);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(35, 17);
			this.label14.TabIndex = 44;
			this.label14.Text = "Baud:";
			// 
			// cBaud
			// 
			this.cBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cBaud.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200"});
			this.cBaud.Location = new System.Drawing.Point(163, 16);
			this.cBaud.Name = "cBaud";
			this.cBaud.Size = new System.Drawing.Size(81, 21);
			this.cBaud.TabIndex = 45;
			// 
			// ZFPForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(467, 445);
			this.Controls.Add(this.cBaud);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.closeFiscal);
			this.Controls.Add(this.pictureBox4);
			this.Controls.Add(this.cPayFinished);
			this.Controls.Add(this.cPayStarted);
			this.Controls.Add(this.cDetailed);
			this.Controls.Add(this.cPrintVAT);
			this.Controls.Add(this.cNoVoid);
			this.Controls.Add(this.cVATb);
			this.Controls.Add(this.cVATc);
			this.Controls.Add(this.cVATa);
			this.Controls.Add(this.cPurchases);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.bonInfo);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.Pay);
			this.Controls.Add(this.cPayType);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.cSum);
			this.Controls.Add(this.CalcSum);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.Sell);
			this.Controls.Add(this.cDiscount);
			this.Controls.Add(this.cQuantity);
			this.Controls.Add(this.cPrice);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.cTaxGroup);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.OpenFiscalBon);
			this.Controls.Add(this.cPassword);
			this.Controls.Add(this.cOperator);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.version);
			this.Controls.Add(this.FindCom);
			this.Controls.Add(this.ComPorts);
			this.Controls.Add(this.label1);
			this.Name = "ZFPForm";
			this.Text = "Zeka Fiscal Printer - C# sample";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new ZFPForm());
		}

		private void FindCom_Click(object sender, System.EventArgs e)
		{
			uint com = zfp.FindFirstFPCOMEx();
			if (com != 0)
			{
				ComPorts.SelectedIndex = (int)(com >> 24) - 1;
				uint baud = com & 0x00FFFFFF;
				cBaud.SelectedIndex = BaudToIndex(baud);
				zfp.Setup((ushort)(com >> 24), baud, 3, 1000);
				version.Text = zfp.GetVersion();
			}
		}

		private void OpenFiscalBon_Click(object sender, System.EventArgs e)
		{
			zfp.Setup(Convert.ToUInt16(ComPorts.SelectedIndex + 1), IndexToBaud(cBaud.SelectedIndex), 3, 1000);
			byte oper = byte.Parse(cOperator.Text);
			zfp.OpenFiscalBon(oper, cPassword.Text, 0, 1);
			if (0 != zfp.errorCode) {
				string err = zfp.GetErrorString(zfp.errorCode, 0);
				MessageBox.Show(err);
			}
		}

		private void Sell_Click(object sender, System.EventArgs e)
		{
			zfp.Setup(Convert.ToUInt16(ComPorts.SelectedIndex + 1), IndexToBaud(cBaud.SelectedIndex), 3, 1000);
			zfp.SellFree(cName.Text, Convert.ToByte(cTaxGroup.SelectedIndex), 
				Single.Parse(cPrice.Text), Single.Parse(cQuantity.Text), 
				Single.Parse(cDiscount.Text));
			if (0 != zfp.errorCode) {
				string err = zfp.GetErrorString(zfp.errorCode, 0);
				MessageBox.Show(err);
			}
		}

		private void CalcSum_Click(object sender, System.EventArgs e)
		{
			zfp.Setup(Convert.ToUInt16(ComPorts.SelectedIndex + 1), IndexToBaud(cBaud.SelectedIndex), 3, 1000);
			double sum = zfp.CalcIntermediateSum(1, 1, 0, 0);
			if (0 != zfp.errorCode) {
				string err = zfp.GetErrorString(zfp.errorCode, 0);
				MessageBox.Show(err);
			}
			else {
				cSum.Text = sum.ToString();
			}
		}

		private void Pay_Click(object sender, System.EventArgs e)
		{
			zfp.Setup(Convert.ToUInt16(ComPorts.SelectedIndex + 1), IndexToBaud(cBaud.SelectedIndex), 3, 1000);
			zfp.Payment(Single.Parse(cSum.Text), Convert.ToByte(cPayType.SelectedIndex), 0);
			if (0 != zfp.errorCode) {
				string err = zfp.GetErrorString(zfp.errorCode, 0);
				MessageBox.Show(err);
			}
		}

		private void bonInfo_Click(object sender, System.EventArgs e)
		{
			zfp.Setup(Convert.ToUInt16(ComPorts.SelectedIndex + 1), IndexToBaud(cBaud.SelectedIndex), 3, 1000);
			ZFPCOMLib.GetCurrentBonInfoRes info = (ZFPCOMLib.GetCurrentBonInfoRes)zfp.GetCurrentBonInfo();
			if (0 != zfp.errorCode) {
				string err = zfp.GetErrorString(zfp.errorCode, 0);
				MessageBox.Show(err);
			}
			else {
				cPurchases.Text = info.purchases.ToString();
				cVATa.Text = info.taxgrp1.ToString();
				cVATb.Text = info.taxgrp2.ToString();
				cVATc.Text = info.taxgrp3.ToString();

				cNoVoid.Checked = info.novoid != 0 ? true : false;
				cPrintVAT.Checked = info.dds != 0 ? true : false;
				cDetailed.Checked = info.detailed != 0 ? true : false;
				cPayStarted.Checked = info.paystarted != 0 ? true : false;
				cPayFinished.Checked = info.paid != 0 ? true : false;
			}
		}

		private void closeFiscal_Click(object sender, System.EventArgs e)
		{
			zfp.Setup(Convert.ToUInt16(ComPorts.SelectedIndex + 1), IndexToBaud(cBaud.SelectedIndex), 3, 1000);
			zfp.CloseFiscalBon();
			if (0 != zfp.errorCode) {
				string err = zfp.GetErrorString(zfp.errorCode, 0);
				MessageBox.Show(err);
			}
		}
	}
}
