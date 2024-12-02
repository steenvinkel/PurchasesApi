﻿using System;

namespace Business.Models
{
    public class AccountStatus(int accountStatusId, int accountId, DateTime date, double amount)
    {
        public int AccountStatusId { get; private set; } = accountStatusId;
        public int AccountId { get; private set; } = accountId;
        public DateTime Date { get; private set; } = date;
        public double Amount { get; private set; } = amount;
    }
}
