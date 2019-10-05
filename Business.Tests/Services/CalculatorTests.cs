using AutoFixture;
using Business.Services;
using NUnit.Framework;
using System;

namespace Business.Tests.Services
{
    public class CalculatorTests
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [TestCase(10000, 40000, 4)]
        [TestCase(10000, 45000, 4.5)]
        [TestCase(5000, 100000, 20)]
        [TestCase(-10000, 100000, double.PositiveInfinity)]
        [TestCase(0, 100000, double.PositiveInfinity)]
        [TestCase(10000, -100000, 0)]
        [TestCase(5000, 0, 0)]
        public void MonthsLivableWithoutPay(double monthlyExpenses, double fortune, double expectedMonthsLivable)
        {
            var sut = _fixture.Create<Calculator>();

            var monthsLivable = sut.CalculateMonthsLivableWithoutPay(fortune, monthlyExpenses);

            Assert.AreEqual(expectedMonthsLivable, monthsLivable);
        }

        [TestCase(10000, 15000, -50)]
        [TestCase(10000, 5000, 50)]
        [TestCase(10000, 2500, 75)]
        [TestCase(10000, 9950, 0.5)]
        [TestCase(1000, 0, 100)]
        [TestCase(0, 1000, double.NegativeInfinity)]
        [TestCase(1000, -1000, 0)]
        [TestCase(-1000, 1000, 0)]
        public void SavingsRate(double income, double expenses, double expectedSavingsRate)
        {
            var sut = _fixture.Create<Calculator>();

            var savingsRate = sut.SavingsRate(income, expenses);

            Assert.AreEqual(expectedSavingsRate, savingsRate);
        }

        [TestCase(20000, 15000, 300000, 0, 20, 63, 51)]
        [TestCase(20000, 15000, 300000, 0.05, 20, 63, 39)]
        public void FireAge(double income, double expenses, double fortune, double returnRate, int currentAge, int pensionAge, double expectedFireAge)
        {
            var sut = _fixture.Create<Calculator>();

            var fireAge = sut.FireAge(income, expenses, fortune, returnRate, currentAge, pensionAge);

            Assert.AreEqual(expectedFireAge, fireAge);
        }

        [TestCase(20000, 15000, 30000, 0, 1, 0, 90000)]
        [TestCase(20000, 15000, 30000, 0, 2, 0, 150000)]
        [TestCase(20000, 15000, 30000, 0.05, 1, 0, 91500)]
        [TestCase(20000, 15000, 30000, 0.05, 2, 0, 156075)]
        [TestCase(20000, 15000, 30000, 0, 1, 42, -7470000)]
        [TestCase(20000, 15000, 30000, 0, 35, 8, 690000)]
        [TestCase(20000, 15000, 30000, 0.05, 1, 42, -23631530)]
        [TestCase(20000, 15000, 30000, 0.05, 35, 8, 6532304)]
        public void CalculateLifeScenario(double income, double expenses, double fortune, double returnRate, int yearsWorking, int yearsNotWorking, double expectedAmount)
        {
            var sut = _fixture.Create<Calculator>();

            var amount = sut.CalculateLifeScenario(income, expenses, fortune, returnRate, yearsWorking, yearsNotWorking);

            Assert.AreEqual(expectedAmount, Math.Round(amount));
        }

        [TestCase(5000, 30000, 0.05, 91500)]
        [TestCase(5000, 30000, 0, 90000)]
        [TestCase(-5000, 90000, 0, 30000)]
        [TestCase(-5000, 30000, 0.05, -28500)]
        [TestCase(5000, 30000, -0.05, 88500)]
        public void SavingsAfterOneYear(double monthlyChange, double start, double returnRate, double expectedAmount)
        {
            var sut = _fixture.Create<Calculator>();

            var amount = sut.SavingsAfterOneYear(monthlyChange, start, returnRate);

            Assert.AreEqual(expectedAmount, amount);
        }
    }
}
