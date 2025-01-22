using Business.Services;
using NUnit.Framework;
using System;

namespace Business.Tests.Services
{
    public class CalculatorTests
    {
        [TestCase(10000, 40000, 4)]
        [TestCase(10000, 45000, 4.5)]
        [TestCase(5000, 100000, 20)]
        [TestCase(-10000, 100000, null)]
        [TestCase(0, 100000, null)]
        [TestCase(10000, -100000, 0)]
        [TestCase(5000, 0, 0)]
        public void MonthsLivableWithoutPay(decimal monthlyExpenses, decimal fortune, decimal? expectedMonthsLivable)
        {
            var monthsLivable = Calculator.CalculateMonthsLivableWithoutPay(fortune, monthlyExpenses);

            Assert.That(monthsLivable, Is.EqualTo(expectedMonthsLivable));
        }

        [TestCase(10000, 15000, -50)]
        [TestCase(10000, 5000, 50)]
        [TestCase(10000, 2500, 75)]
        [TestCase(10000, 9950, 0.5)]
        [TestCase(1000, 0, 100)]
        [TestCase(0, 1000, null)]
        [TestCase(1000, -1000, 0)]
        [TestCase(-1000, 1000, 0)]
        public void SavingsRate(decimal income, decimal expenses, decimal? expectedSavingsRate)
        {
            var savingsRate = Calculator.SavingsRate(income, expenses);

            Assert.That(savingsRate, Is.EqualTo(expectedSavingsRate));
        }

        [TestCase(20000, 15000, 300000, 0, 20, 63, 51)]
        [TestCase(20000, 15000, 300000, 0.05, 20, 63, 39)]
        public void FireAge(decimal income, decimal expenses, decimal fortune, decimal returnRate, int currentAge, int pensionAge, decimal expectedFireAge)
        {
            var fireAge = Calculator.FireAge(income, expenses, fortune, returnRate, currentAge, pensionAge);

            Assert.That(fireAge, Is.EqualTo(expectedFireAge));
        }

        [TestCase(20000, 15000, 30000, 0, 1, 0, 90000)]
        [TestCase(20000, 15000, 30000, 0, 2, 0, 150000)]
        [TestCase(20000, 15000, 30000, 0.05, 1, 0, 91500)]
        [TestCase(20000, 15000, 30000, 0.05, 2, 0, 156075)]
        [TestCase(20000, 15000, 30000, 0, 1, 42, -7470000)]
        [TestCase(20000, 15000, 30000, 0, 35, 8, 690000)]
        [TestCase(20000, 15000, 30000, 0.05, 1, 42, -23631530)]
        [TestCase(20000, 15000, 30000, 0.05, 35, 8, 6532304)]
        public void CalculateLifeScenario(decimal income, decimal expenses, decimal fortune, decimal returnRate, int yearsWorking, int yearsNotWorking, decimal expectedAmount)
        {
            var amount = Calculator.CalculateLifeScenario(income, expenses, fortune, returnRate, yearsWorking, yearsNotWorking);

            Assert.That(Math.Round(amount), Is.EqualTo(expectedAmount));
        }

        [TestCase(5000, 30000, 0.05, 91500)]
        [TestCase(5000, 30000, 0, 90000)]
        [TestCase(-5000, 90000, 0, 30000)]
        [TestCase(-5000, 30000, 0.05, -28500)]
        [TestCase(5000, 30000, -0.05, 88500)]
        public void SavingsAfterOneYear(decimal monthlyChange, decimal start, decimal returnRate, decimal expectedAmount)
        {
            var amount = Calculator.SavingsAfterOneYear(monthlyChange, start, returnRate);

            Assert.That(amount, Is.EqualTo(expectedAmount));
        }
    }
}
