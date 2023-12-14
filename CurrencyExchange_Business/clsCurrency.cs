using CurrencyExchange_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange_Business
{
    public class clsCurrency
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? CurrencyID { get; set; }
        public string Country { get; set; }
        public string Code { get; set; }
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }

        public clsCurrency()
        {
            this.CurrencyID = null;
            this.Country = string.Empty;
            this.Code = string.Empty;
            this.CurrencyName = string.Empty;
            this.Rate = -1M;
            Mode = enMode.AddNew;
        }

        private clsCurrency(int? CurrencyID, string Country, string Code,
            string CurrencyName, decimal Rate)
        {
            this.CurrencyID = CurrencyID;
            this.Country = Country;
            this.Code = Code;
            this.CurrencyName = CurrencyName;
            this.Rate = Rate;
            Mode = enMode.Update;
        }

        private bool _AddNewCurrency()
        {
            this.CurrencyID = clsCurrencyData.AddNewCurrency(this.Country,
                this.Code, this.CurrencyName, this.Rate);

            return (this.CurrencyID.HasValue);
        }

        private bool _UpdateCurrency()
        {
            return clsCurrencyData.UpdateCurrency(this.CurrencyID,
                this.Country, this.Code, this.CurrencyName, this.Rate);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCurrency())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateCurrency();
            }

            return false;
        }

        public static clsCurrency Find(int? CurrencyID)
        {
            string Country = string.Empty;
            string Code = string.Empty;
            string CurrencyName = string.Empty;
            decimal Rate = -1M;

            bool IsFound = clsCurrencyData.GetCurrencyInfoByID(CurrencyID,
                ref Country, ref Code, ref CurrencyName, ref Rate);

            if (IsFound)
            {
                return new clsCurrency(CurrencyID, Country, Code, CurrencyName, Rate);
            }
            else
            {
                return null;
            }
        }

        public static clsCurrency FindByCode(string Code)
        {
            int? CurrencyID = null;
            string Country = string.Empty;
            string CurrencyName = string.Empty;
            decimal Rate = -1M;

            bool IsFound = clsCurrencyData.GetCurrencyInfoByCode(Code, ref CurrencyID,
                ref Country, ref CurrencyName, ref Rate);

            if (IsFound)
            {
                return new clsCurrency(CurrencyID, Country, Code, CurrencyName, Rate);
            }
            else
            {
                return null;
            }
        }

        public static clsCurrency FindByCountry(string Country)
        {
            int? CurrencyID = null;
            string Code = string.Empty;
            string CurrencyName = string.Empty;
            decimal Rate = -1M;

            bool IsFound = clsCurrencyData.GetCurrencyInfoByCountry(Country, ref CurrencyID,
                ref Code, ref CurrencyName, ref Rate);

            if (IsFound)
            {
                return new clsCurrency(CurrencyID, Country, Code, CurrencyName, Rate);
            }
            else
            {
                return null;
            }
        }

        public static bool DeleteCurrency(int? CurrencyID)
        {
            return clsCurrencyData.DeleteCurrency(CurrencyID);
        }

        public static bool DoesCurrencyExist(int? CurrencyID)
        {
            return clsCurrencyData.DoesCurrencyExist(CurrencyID);
        }

        public static DataTable GetAllCurrencies()
        {
            return clsCurrencyData.GetAllCurrencies();
        }

        public static DataTable GetAllCurrenciesCode()
        {
            return clsCurrencyData.GetAllCurrenciesCode();
        }

        public static DataTable GetAllCountry()
        {
            return clsCurrencyData.GetAllCountry();
        }

        private decimal _ConvertToUSD(decimal Amount)
        {
            return (Amount / this.Rate);
        }

        public decimal Convert(string ConvertTo, decimal Amount)
        {
            if (Amount < 0)
                return 0m;

            decimal AmountInUSD = _ConvertToUSD(Amount);

            if (ConvertTo == "USD")
            {
                return AmountInUSD;
            }

            decimal RateTo = FindByCode(ConvertTo).Rate;        
            
            return (AmountInUSD * RateTo);
        }

    }

}
