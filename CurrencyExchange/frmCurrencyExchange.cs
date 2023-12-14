using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CurrencyExchange_Business;
using Guna.UI2.WinForms;

namespace CurrencyExchange
{
    public partial class frmCurrencyExchange : Form
    {
        private string _CurrencyCodeFrom;
        private string _CurrencyCodeTo;

        #region Declare Event
        public class CurrencyCodeSelectedEventArgs : EventArgs
        {
            public string CurrencyCodeFrom;
            public string CurrencyCodeTo;

            public CurrencyCodeSelectedEventArgs(string currencyCodeFrom, string currencyCodeTo)
            {
                this.CurrencyCodeFrom = currencyCodeFrom;
                this.CurrencyCodeTo = currencyCodeTo;
            }
        }

        public event EventHandler<CurrencyCodeSelectedEventArgs> OnCurrencyCodeSelected;

        public void RaiseOnCurrencyCodeSelected(string currencyCodeFrom, string currencyCodeTo)
        {
            RaiseOnCurrencyCodeSelected(new CurrencyCodeSelectedEventArgs(currencyCodeFrom, currencyCodeTo));
        }

        protected virtual void RaiseOnCurrencyCodeSelected(CurrencyCodeSelectedEventArgs e)
        {
            OnCurrencyCodeSelected?.Invoke(this, e);
        }
        #endregion

        public frmCurrencyExchange()
        {
            InitializeComponent();
        }

        private void _FillComboBoxWithData()
        {
            DataTable dtCurrencies = clsCurrency.GetAllCurrencies();
            StringBuilder currency = new StringBuilder();

            foreach (DataRow drCurrency in dtCurrencies.Rows)
            {
                currency = currency.Append(drCurrency[1].ToString() + " - " + drCurrency[3].ToString() + " (" + drCurrency[2].ToString() + ")");

                cbConvertFrom.Items.Add(currency.ToString());
                cbConvertTo.Items.Add(currency.ToString());

                currency.Clear();
            }

        }

        private void _HandleEnablingBtnConvert()
        {
            if (string.IsNullOrWhiteSpace(cbConvertFrom.Text) ||
                string.IsNullOrWhiteSpace(cbConvertTo.Text))
            {
                return;
            }

            if (cbConvertFrom.SelectedIndex >= 0 &&
                cbConvertTo.SelectedIndex >= 0 &&
                !string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                string CurrencyCodeFrom = cbConvertFrom.Text.Substring(cbConvertFrom.Text.Length - 4, 3);
                string CurrencyCodeTo = cbConvertTo.Text.Substring(cbConvertTo.Text.Length - 4, 3);

                RaiseOnCurrencyCodeSelected(CurrencyCodeFrom, CurrencyCodeTo);
            }
            else
            {
                btnConvert.Enabled = false;
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            char inputChar = e.KeyChar;

            // Allow digits, the decimal point, and the backspace.
            bool isDigit = Char.IsDigit(inputChar);
            bool isDecimalPoint = (inputChar == '.');
            bool isBackspace = (inputChar == '\b');

            // If the input character is not a digit, decimal point, or backspace, suppress it.
            if (!isDigit && !isDecimalPoint && !isBackspace)
            {
                e.Handled = true;
            }

            // Make sure there is only one decimal point in the input.
            if (isDecimalPoint && ((sender as Guna2TextBox).Text.Contains(".") || (sender as Guna2TextBox).Text.Length == 0))
            {
                e.Handled = true;
            }
        }

        private void frmCurrencyExchange_Load(object sender, EventArgs e)
        {
            _FillComboBoxWithData();

            OnCurrencyCodeSelected += EnableBtnConvert_OnCurrencyCodeSelected;
        }

        private void EnableBtnConvert_OnCurrencyCodeSelected(object sender, CurrencyCodeSelectedEventArgs e)
        {
            btnConvert.Enabled = true;

            _CurrencyCodeFrom = e.CurrencyCodeFrom;
            _CurrencyCodeTo = e.CurrencyCodeTo;
        }

        private void cbConvertFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            _HandleEnablingBtnConvert();
        }

        private void cbConvertTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            _HandleEnablingBtnConvert();
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            _HandleEnablingBtnConvert();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbConvertFrom.Text) ||
                string.IsNullOrWhiteSpace(cbConvertTo.Text) ||
                string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                return;
            }

            clsCurrency CurrencyFrom = clsCurrency.FindByCode(_CurrencyCodeFrom);

            if (CurrencyFrom == null)
            {
                return;
            }

            decimal ConvertToAmount = CurrencyFrom.Convert(_CurrencyCodeTo, decimal.Parse(txtAmount.Text.Trim()));


            lblResult.Text = "$" + decimal.Parse(txtAmount.Text.Trim()).ToString("N") + " (" + _CurrencyCodeFrom + ")" + "  =  ";

            lblResult.Text += "$" + ConvertToAmount.ToString("N") + " (" + _CurrencyCodeTo + ")";

            panelResult.Visible = true;
        }

        private void btnOpposite_Click(object sender, EventArgs e)
        {
            string Temp = cbConvertFrom.Text;

            cbConvertFrom.Text = cbConvertTo.Text;
            cbConvertTo.Text = Temp;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtAmount.Clear();
            cbConvertFrom.Text = "";
            cbConvertTo.Text = "";

            panelResult.Visible = false;
        }
    }
}
