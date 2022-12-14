using NUnit.Framework;
using System;
using XMachine;

namespace ATMMachine.Test {
    [TestFixture]
    public class ATMMachine_TEST {
        private ATMMachine testJig;
        [SetUp]
        public void Setup() {
            this.testJig = new ATMMachine(new List<Account>() { new Account("1"), new Account("6") });
        }
        [TestCase]
        public void Initial_state_must_be_IDLE() {
            Assert.That(this.testJig.status, Is.EqualTo(SYSTEM_STATUS.IDLE), "The initial State must be IDLE ");
        }

        [TestCase]
        public void TestCase_0() {
            Assert.Throws<Exception>(() => { this.testJig.Authenticate("4"); }, "Authenticate_should_throw_an_exception");

        }


        [TestCase]
        public void TestCase_1() {
            this.testJig.Authenticate("1");
            Assert.IsTrue(new List<SYSTEM_STATUS>() { SYSTEM_STATUS.SESSION_START }.Contains(this.testJig.status), " this.testJig.status Must be the Final State");
            this.testJig.Deposit(200);
            Assert.IsTrue(new List<SYSTEM_STATUS>() { SYSTEM_STATUS.DEPOSIT }.Contains(this.testJig.status), " this.testJig.status Must be the Final State");
            var result_9IO397U = this.testJig.CheckBalance();
            Assert.That(result_9IO397U, Is.EqualTo(200), "CheckBalance_should_return_200");
            Assert.IsTrue(new List<SYSTEM_STATUS>() { SYSTEM_STATUS.CHECK_BALANCE }.Contains(this.testJig.status), " this.testJig.status Must be the Final State");
            this.testJig.Withdraw(100);
            Assert.IsTrue(new List<SYSTEM_STATUS>() { SYSTEM_STATUS.WITHDRAWAL }.Contains(this.testJig.status), " this.testJig.status Must be the Final State");
            var result_50DGCMB = this.testJig.CheckBalance();
            Assert.That(result_50DGCMB, Is.EqualTo(100), "CheckBalance_should_return_100");
            Assert.IsTrue(new List<SYSTEM_STATUS>() { SYSTEM_STATUS.CHECK_BALANCE }.Contains(this.testJig.status), " this.testJig.status Must be the Final State");

        }


        [TestCase]
        public void TestCase_2() {
            this.testJig.Authenticate("1");
            Assert.IsTrue(new List<SYSTEM_STATUS>() { SYSTEM_STATUS.SESSION_START }.Contains(this.testJig.status), " this.testJig.status Must be the Final State");
            Assert.Throws<Exception>(() => { this.testJig.Withdraw(100); }, "Withdraw_should_throw_an_exception");

        }




    }
}
