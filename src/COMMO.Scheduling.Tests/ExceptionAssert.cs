using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace COMMO.Scheduling.Tests {
    /// <summary>
    /// Class that contains extensions for asserting exceptions.
    /// </summary>
    public static class ExceptionAssert
    {
        /// <summary>
        /// Checks that the function supplied throws an exception of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The expected type in the exception thrown/</typeparam>
        /// <param name="action">The function to test.</param>
        /// <param name="message">Optional. The message that the caught exception should contain.</param>
        /// <returns>The exception that was thrown.</returns>
        public static T Throws<T>(Action action, string message = null) where T : Exception
        {
            try
            {
                action();

                Assert.Fail($"Expected an exception of type {typeof(T)} be thrown, but got none.");
            }
            catch (T ex)
            {
                // check that the msg snippet is there.
                if (!string.IsNullOrWhiteSpace(message) && !ex.Message.Contains(message))
                {
                    throw new AssertFailedException($"The method expected an exception with the message '{message}'.{Environment.NewLine}Instead it cointained '{ex.Message}'.");
                }

                return ex;
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"The method threw an exception of type {ex.GetType().Name} but an exception of type {typeof(T).Name} was expected.");
            }
                
            //  The compiler doesn't know that Assert.Fail
            //  will always throw an exception
            return null;
        }
    }
}
