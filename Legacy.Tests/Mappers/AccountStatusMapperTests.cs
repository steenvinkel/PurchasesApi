using AutoFixture;
using Business.Models;
using Legacy.Mappers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Tests.Mappers
{
    public class AccountStatusMapperTests
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void TestSomething()
        {
            var accountStatuses = _fixture.CreateMany<AccountStatus>().ToList();

            var sut = new AccountStatusMapper();

            var result = sut.Map(accountStatuses);
        }
    }
}
