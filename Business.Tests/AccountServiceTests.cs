using AutoFixture;
using AutoFixture.AutoMoq;
using Business.Models;
using Business.Repositories;
using Business.Services;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class AccountServiceTests
    {
        private IFixture _fixture;
        private IAccountRepository _repository;
        private AccountService _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repository = _fixture.Freeze<IAccountRepository>();
            _sut = _fixture.Create<AccountService>();
        }

        [Test]
        public void WhenDatabaseHasAccountList_ReturnTheList()
        {
            var accounts = _fixture.CreateMany<AccountDAO>().ToList();
            var userId = _fixture.Create<int>();

            Mock.Get(_repository).Setup(r => r.Get(userId)).Returns([.. accounts]);

            var result = _sut.Get(userId);

            Assert.That(result, Is.EquivalentTo(accounts));
        }
    }
}