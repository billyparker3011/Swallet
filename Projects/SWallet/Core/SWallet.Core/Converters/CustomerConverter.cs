using HnMicro.Core.Helpers;
using SWallet.Core.Enums;
using SWallet.Core.Models;
using SWallet.Core.Models.Customers;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class CustomerConverter
    {
        public static CustomerModel ToCustomerModel(this Customer customer)
        {
            return new CustomerModel
            {
                CustomerId = customer.CustomerId,
                AgentId = customer.AgentId,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                IsAffiliate = customer.IsAffiliate,
                MasterId = customer.MasterId,
                Phone = customer.Phone,
                RoleId = customer.RoleId,
                State = customer.State.ToEnum<CustomerState>(),
                SupermasterId = customer.SupermasterId,
                Telegram = customer.Telegram,
                Username = customer.Username,
                UsernameUpper = customer.UsernameUpper,
                DepositAllowed = customer.DepositAllowed,
                DiscountAllowed = customer.DiscountAllowed,
                Lock = customer.Lock,
                WithdrawAllowed = customer.WithdrawAllowed
            };
        }

        public static MyBalanceCustomerModel ToMyCustomerBalanceModel(this BalanceCustomer balanceCustomer, Setting setting)
        {
            return new MyBalanceCustomerModel
            {
                Balance = balanceCustomer.Balance,
                CurrencySymbol = setting.CurrencySymbol
            };
        }

        public static MyCustomerProfileModel ToMyCustomerProfileModel(this Customer customer)
        {
            return new MyCustomerProfileModel
            {
                CustomerId = customer.CustomerId,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                State = customer.State.ToEnum<CustomerState>(),
                Telegram = customer.Telegram,
                Username = customer.Username,
                UsernameUpper = customer.UsernameUpper,
                DepositAllowed = customer.DepositAllowed,
                DiscountAllowed = customer.DiscountAllowed,
                Lock = customer.Lock,
                WithdrawAllowed = customer.WithdrawAllowed
            };
        }

        public static CustomerSessionModel ToCustomerSessionModel(this CustomerSession session)
        {
            return new CustomerSessionModel
            {
                CustomerId = session.CustomerId,
                Hash = session.Hash,
                IpAddress = session.IpAddress,
                LatestDoingTime = session.LatestDoingTime,
                Platform = session.Platform,
                State = session.State.ToEnum<SessionState>(),
                UserAgent = session.UserAgent,
            };
        }

        public static CustomerBankAccountModel ToCustomerBankAccountModel(this CustomerBankAccount customerBankAccount)
        {
            return new CustomerBankAccountModel
            {
                Id = customerBankAccount.Id,
                BankId = customerBankAccount.BankId,
                BankIcon = customerBankAccount.Bank.Icon,
                BankName = customerBankAccount.Bank.Name,
                CardHolder = customerBankAccount.CardHolder,
                NumberAccount = customerBankAccount.NumberAccount
            };
        }
    }
}