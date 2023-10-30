using Business.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Business.Tests.Dashboard
{
    public class MonthlyAccountCategorySumsTests
    {

        [TestCase]
        public void Constructor_NoParameters_ShouldGiveEmptyDictionary()
        {
            var sut = new MonthlyAccountCategorySums();

            var count = sut.Count;

            Assert.AreEqual(0, count);

        }

        private static readonly object[] FortuneAndInvestmentScenarios = new object[] 
        {
            new object[]{ new Dictionary<MonthAndYear, Dictionary<string, double>>(), new MonthAndYear(2019, 9), 0 },
            new object[]{ new Dictionary<MonthAndYear, Dictionary<string, double>>
                { {new MonthAndYear(2019,9), new Dictionary<string, double> { {"Fortune", 2000 } } } },
                new MonthAndYear(2019, 9), 2000
            },
            new object[]{ new Dictionary<MonthAndYear, Dictionary<string, double>>
                { {new MonthAndYear(2019,9), new Dictionary<string, double> { {"Investment", 3000 } } } },
                new MonthAndYear(2019, 9), 3000
            },
            new object[]{ new Dictionary<MonthAndYear, Dictionary<string, double>>
                { {new MonthAndYear(2019,9), new Dictionary<string, double> { { "Fortune", 2000 }, { "Investment", 3000 } } } },
                new MonthAndYear(2019, 9), 5000
            },
        };
        [TestCaseSource(nameof(FortuneAndInvestmentScenarios))]
        public void GetFortuneAndInvestmentSummed_DifferentScenarios_ShouldReturnCorrectValue(Dictionary<MonthAndYear, Dictionary<string, double>> dictionary, MonthAndYear monthAndYear, double expectedSum)
        {
            var sut = new MonthlyAccountCategorySums(dictionary);

            var fortune = sut.GetCombinedFortune(monthAndYear);

            Assert.AreEqual(expectedSum, fortune);
        }
    }
}
