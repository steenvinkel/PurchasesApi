using AutoFixture;
using AutoFixture.AutoMoq;
using Business.Models;
using Business.Repositories;
using Legacy.Models;
using Legacy.Repositories;
using Legacy.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Tests.Services
{
    public class LegacySumupServiceTests
    {
        private IFixture _fixture;
        private ILegacySumupRepository _sumupRepository;
        private ILegacySummaryRepository _summaryRepository;
        private ILegacyMonthlyAccountStatusRepository _monthlyAccountStatusRepository;
        private IAccountStatusRepository _accountStatusRepository;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            _sumupRepository = _fixture.Freeze<ILegacySumupRepository>();
            _summaryRepository = _fixture.Freeze<ILegacySummaryRepository>();
            _monthlyAccountStatusRepository = _fixture.Freeze<ILegacyMonthlyAccountStatusRepository>();
            _accountStatusRepository = _fixture.Freeze<IAccountStatusRepository>();
        }

        [Test]
        public void Sumup_GivenASetup_CalculateCorrectNumbers()
        {
            var userId = 1;

            var monthlyTypeSums = new List<MonthlyTypeSum>
            {
                new MonthlyTypeSum { Year = 2019, Month = 6, Type = "in", Sum = 25000 },
                new MonthlyTypeSum { Year = 2019, Month = 6, Type = "out", Sum = 20000 },
                new MonthlyTypeSum { Year = 2019, Month = 6, Type = "tax", Sum = 10000 },
                new MonthlyTypeSum { Year = 2019, Month = 6, Type = "invest", Sum = 8.33 },
                new MonthlyTypeSum { Year = 2019, Month = 7, Type = "in", Sum = 30000 },
                new MonthlyTypeSum { Year = 2019, Month = 7, Type = "out", Sum = 25000 },
                new MonthlyTypeSum { Year = 2019, Month = 7, Type = "tax", Sum = 10000 },
                new MonthlyTypeSum { Year = 2019, Month = 7, Type = "invest", Sum = 16.67 }
            };
            Mock.Get(_sumupRepository).Setup(x => x.Sumup(userId))
                .Returns(monthlyTypeSums);

            var yearMap = new Dictionary<int, Dictionary<int, double>> { {2019, new Dictionary<int, double> { { 6, 25000 } }} };
            var subMap = new Dictionary<int, Dictionary<int, Dictionary<int, double>>> { { 2, yearMap } };
            var summary = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>> { { 2, subMap } };

            Mock.Get(_summaryRepository).Setup(x => x.Summary(userId))
                .Returns((null, summary));

            var summedFortunes = new Dictionary<MonthAndYear, double> { { new MonthAndYear(2019, 6), 5000 } };
            Mock.Get(_monthlyAccountStatusRepository).Setup(x => x.CalculateSummedFortunes(userId))
                .Returns(summedFortunes);

            var accountStatuses = new List<AccountStatus> {
                CreateAccountStatus(0, 2019, 5),
                CreateAccountStatus(5000, 2019, 6),
                CreateAccountStatus(10000, 2019, 7),
            };
            Mock.Get(_accountStatusRepository).Setup(x => x.Get(userId))
                .Returns(accountStatuses);

            var sut = _fixture.Create<LegacySumupService>();

            var result = sut.Sumup(userId);

            var monthlySumup = result.SingleOrDefault(x => x.Year == 2019 && x.Month == 6);
            Assert.IsNotNull(monthlySumup);
            Assert.AreEqual(25000, monthlySumup.In);
            Assert.AreEqual(20000, monthlySumup.Out);
            Assert.AreEqual(10000, monthlySumup.PureOut);
            Assert.AreEqual(8.33, monthlySumup.Invest);
            Assert.AreEqual(33.33, monthlySumup.Savings);
            Assert.AreEqual(0, monthlySumup.SavingsLastYear);
            Assert.AreEqual(0, monthlySumup.ExpensesLastYear);
            Assert.AreEqual(17500, monthlySumup.Extra);
            Assert.AreEqual(0, monthlySumup.MonthsWithoutPay);
            Assert.AreEqual(25.58, monthlySumup.SavingsWithoutOwnContribution);

            monthlySumup = result.SingleOrDefault(x => x.Year == 2019 && x.Month == 7);
            Assert.IsNotNull(monthlySumup);
            Assert.AreEqual(30000, monthlySumup.In);
            Assert.AreEqual(25000, monthlySumup.Out);
            Assert.AreEqual(15000, monthlySumup.PureOut);
            Assert.AreEqual(16.67, monthlySumup.Invest);
            //Assert.AreEqual(33.33, monthlySumup.savings);
            //Assert.AreEqual(0, monthlySumup.savingsLastYear);
            //Assert.AreEqual(0, monthlySumup.expensesLastYear);
            //Assert.AreEqual(17500, monthlySumup.extra);
            Assert.AreEqual(0.5, monthlySumup.MonthsWithoutPay);
            //Assert.AreEqual(25.58, monthlySumup.savingsWithoutOwnContribution);
        }

        private AccountStatus CreateAccountStatus(int amount, int year, int month)
        {
            var monthAndYear = new MonthAndYear(year, month);
            return new AccountStatus(_fixture.Create<int>(), _fixture.Create<int>(), monthAndYear.LastDayOfMonth(), amount);
        }
    }
}
