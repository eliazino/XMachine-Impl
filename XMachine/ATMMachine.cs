using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMachine {
    public class ATMMachine {
        public SYSTEM_STATUS status { get; private set; }
        private Account activeAccount;
        private List<Account> accounts { get; set; }
        public ATMMachine(List<Account> account) {
            this.status = SYSTEM_STATUS.IDLE;
            accounts = account;
        }
        public void Authenticate(string accountNumber) {
            if (!this.status.Equals(SYSTEM_STATUS.IDLE))
                throw new AlreadyAuthenticatedError();
            var account = accounts.Find(F=>F.accountNumber == accountNumber);
            if (account is null)
                throw new AuthenticationError("Invalid Account Details");
            this.activeAccount = account;
        }
        public void Withdraw(double amount) {
            if (!this.status.Equals(SYSTEM_STATUS.SESSION_START))
                throw new AuthenticationError("System unauthenticated");
            this.status = SYSTEM_STATUS.WITHDRAWAL;
            if (amount < 1)
                throw new InvalidAmountError();
            if (this.activeAccount.balance < amount)
                throw new InsufficientBalanceError();
            this.activeAccount.debit(amount);
        }
        public double CheckBalance() {
            if (!this.status.Equals(SYSTEM_STATUS.SESSION_START))
                throw new AuthenticationError("System unauthenticated");
            this.status = SYSTEM_STATUS.CHECK_BALANCE;
            return activeAccount.balance;
        }

        public void Deposit(double amount) {
            if (!this.status.Equals(SYSTEM_STATUS.SESSION_START))
                throw new AuthenticationError("System unauthenticated");
            this.status = SYSTEM_STATUS.DEPOSIT;
            if (amount < 1)
                throw new InvalidAmountError();
            this.activeAccount.credit(amount);
        }
        public void Cancel() {
            this.activeAccount = null;
            this.status = SYSTEM_STATUS.IDLE;
        }
        public void Accept() {
            this.status = SYSTEM_STATUS.SESSION_START;
        }
    }
    public enum SYSTEM_STATUS {
        IDLE, SESSION_START, WITHDRAWAL, CHECK_BALANCE, DEPOSIT
    }
    public class Account {
        public string accountNumber { get; private set; }
        public string PIN { get; private set; }
        public double balance { get; private set; }
        public Account(string account) {
            this.accountNumber = account;
            this.PIN = string.Empty;
            this.balance = 0;
        }
        public void debit(double amount) {
            this.balance -= amount;
        }
        public void credit(double amount) {
            this.balance += amount;
        }
    }

    public class AuthenticationError : Exception {
        public AuthenticationError(string message = "Authentication Failed") : base(message) { }
    }

    public class AlreadyAuthenticatedError : Exception {
        public AlreadyAuthenticatedError(string message = "System is already authenticated") : base(message) { }
    }

    public class InsufficientBalanceError : Exception {
        public InsufficientBalanceError(string message = "Insufficient Balance") : base(message) { }
    }

    public class InvalidAmountError : Exception {
        public InvalidAmountError(string message = "Invalid Amount") : base(message) { }
    }
}
