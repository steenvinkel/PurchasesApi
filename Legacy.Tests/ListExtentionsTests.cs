using AutoFixture;
using Legacy.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Tests
{
    public class ListExtentionsTests
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void WhenListIsEmpty_DoNotRemove()
        {
            var list = new List<MonthlyTypeSum>();

            list.RemoveFirstMonthInFirstYear();

            CollectionAssert.IsEmpty(list);
        }

        [Test]
        public void AMonthlyTypeSumList_ShouldHaveTheFirstMonthInTheFirstYearRemoved_()
        {
            var list = _fixture.CreateMany<MonthlyTypeSum>().ToList();
            list[0].Year = 2019;
            list[0].Month = 9;
            list[1].Year = 2014;
            list[1].Month = 8;
            list[2].Year = 2014;
            list[2].Month = 9;

            list.RemoveFirstMonthInFirstYear();

            Assert.AreEqual(2, list.Count());
            Assert.IsNotNull(list.FirstOrDefault(x => x.Year == 2019 && x.Month == 9));
            Assert.IsNotNull(list.FirstOrDefault(x => x.Year == 2014 && x.Month == 9));
            Assert.IsNull(list.FirstOrDefault(x => x.Year == 2014 && x.Month == 8));
        }
    }
}
