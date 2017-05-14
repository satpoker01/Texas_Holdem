﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TexasHoldem.Users;

namespace AllTests.UnitTests.Users
{
    [TestClass]
    public class TestUser
    {
        private readonly User _kobi = new User("KillingHsX1", "12345678", "pic1.jpg", "hello@gmail.com", 5000);
        private const string Notif = "10/04/2017 15:30: This is a notification.";
        private const string Notif2 = "";
        private readonly User _yossi = new User("KillingHsX", "12345678", "pic.jpg", "hello@gmail.com", 5000);

        [TestMethod]
        public void AddNotification_AddOneNotification_QueueGotBigger()
        {
            _yossi.AddNotification(Notif);
            Assert.AreEqual(1, _yossi.Notifications.Count);
            _yossi.RemoveNotification(Notif);
        }

        [TestMethod]
        public void AddNotification_AddEmptyNotification_ExceptionThrown()
        {
            try
            {
                _yossi.AddNotification(Notif2);
                Assert.Fail(); // If it gets to this line, no exception was thrown
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [TestMethod]
        public void RemoveNotification_DeleteOneNotification_QueueGotSmaller()
        {
            _kobi.AddNotification(Notif);
            _kobi.RemoveNotification(Notif);
            Assert.AreEqual(0, _yossi.Notifications.Count);
        }

        [TestMethod]
        public void RemoveNotification_DeleteEmptyNotification_ExceptionThrown()
        {
            try
            {
                _yossi.RemoveNotification(Notif2);
                Assert.Fail(); // If it gets to this line, no exception was thrown
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}