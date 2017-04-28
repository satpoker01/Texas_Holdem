﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllTests.UnitTests.Loggers
{
    [TestClass]
    public class TestLogger
    {
        [TestMethod]
        public void Log_LogActionAndErrorMessages_LogsHaveChanged()
        {
            long errorLength = 0;
            long actionLength = 0;

            long errorLengthAfter = 0;
            long actionLengthAfter = 0;

            if (File.Exists(Logger.errorPath))
                errorLength = new FileInfo(Logger.errorPath).Length;
            if (File.Exists(Logger.actionPath))
                actionLength = new FileInfo(Logger.actionPath).Length;

            Logger.Log(Severity.Error, "This is an error message.");
            Logger.Log(Severity.Warning, "This is a warning message.");
            Logger.Log(Severity.Action, "This is an action message.");
            Logger.Log(Severity.Exception, "This is an exception message.");

            if (File.Exists(Logger.errorPath))
                errorLengthAfter = new FileInfo(Logger.errorPath).Length;
            if (File.Exists(Logger.actionPath))
                actionLengthAfter = new FileInfo(Logger.actionPath).Length;

            Assert.AreNotEqual(actionLength, actionLengthAfter);
            Assert.AreNotEqual(errorLength, errorLengthAfter);
        }

        [TestMethod]
        public void Log_LogEmptyErrorMessage_ExceptionThrown()
        {
            try
            {
                Logger.Log(Severity.Error, "");
                Assert.Fail(); // If it gets to this line, no exception was thrown
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void Log_LogEmptyActionMessage_ExceptionThrown()
        {
            try
            {
                Logger.Log(Severity.Action, "");
                Assert.Fail(); // If it gets to this line, no exception was thrown
            }
            catch (Exception)
            {
            }
        }
    }
}