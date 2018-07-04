// <copyright file="SchedulerTests.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.TestUtils;

    /// <summary>
    /// Tests for the <see cref="Scheduler"/> class
    /// </summary>
    [TestClass]
    public class SchedulerTests
    {
        /// <summary>
        /// Checks <see cref="Scheduler"/> initialization.
        /// </summary>
        [TestMethod]
        public void Scheduler_Initialization()
        {
            DateTime anyNonDefaultDateTime = DateTime.Now;
            DateTime defaultDateTime = default(DateTime);
            DateTime invalidDateTime = DateTime.Now - TimeSpan.FromHours(1);

            // use a default time for the reference time.
            Assert.ThrowsException<ArgumentException>(() => new Scheduler(defaultDateTime));

            // use an invalid time for the reference time.
            Assert.ThrowsException<ArgumentException>(() => new Scheduler(invalidDateTime));

            // use a non default reference time.
            Scheduler scheduler = new Scheduler(anyNonDefaultDateTime);
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.ImmediateEvent(IEvent)"/> throws when needed.
        /// </summary>
        [TestMethod]
        public void InmediateEvent_Throws_WhenBad()
        {
            DateTime anyNonDefaultDateTime = DateTime.Now;

            Scheduler scheduler = new Scheduler(anyNonDefaultDateTime);

            Assert.ThrowsException<ArgumentNullException>(() => scheduler.ImmediateEvent(null), $"Value cannot be null.{Environment.NewLine}Parameter name: eventToSchedule");

            Mock<IEvent> eventMock = new Mock<IEvent>();

            Assert.ThrowsException<ArgumentException>(() => scheduler.ImmediateEvent(eventMock.Object), $"Argument must be of type {nameof(BaseEvent)}.{Environment.NewLine}Parameter name: eventToSchedule");
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.ScheduleEvent(IEvent, DateTime)"/> throws when needed.
        /// </summary>
        [TestMethod]
        public void ScheduleEvent_Throws_WhenBad()
        {
            DateTime anyNonDefaultDateTime = DateTime.Now;
            DateTime defaultDateTime = default(DateTime);
            DateTime invalidRunAtDateTime = anyNonDefaultDateTime - TimeSpan.FromMilliseconds(1);
            DateTime validRunAtDateTime = anyNonDefaultDateTime + TimeSpan.FromMilliseconds(1);
            DateTime twoSecondsFromNowDateTime = anyNonDefaultDateTime + TimeSpan.FromSeconds(2);

            Scheduler scheduler = new Scheduler(anyNonDefaultDateTime);

            ExceptionAssert.Throws<ArgumentNullException>(() => scheduler.ScheduleEvent(null, validRunAtDateTime), $"Value cannot be null.{Environment.NewLine}Parameter name: eventToSchedule");

            Mock<IEvent> eventMock = new Mock<IEvent>();

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(eventMock.Object, validRunAtDateTime), $"Argument must be of type {nameof(BaseEvent)}.{Environment.NewLine}Parameter name: eventToSchedule");

            Mock<BaseEvent> bEventMock = new Mock<BaseEvent>();

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(bEventMock.Object, defaultDateTime), $"Parameter runAt has the default value.");

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ScheduleEvent(bEventMock.Object, invalidRunAtDateTime), $"Value cannot be earlier than the reference time of the scheduler: {anyNonDefaultDateTime}.{Environment.NewLine}Parameter name: runAt");

            // schedule twice
            scheduler.ScheduleEvent(bEventMock.Object, twoSecondsFromNowDateTime);

            ExceptionAssert.Throws<ArgumentException>(() => scheduler.ImmediateEvent(bEventMock.Object), $"The event is already scheduled.{Environment.NewLine}Parameter name: eventToSchedule");
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.CancelEvent(string)"/> does what it should.
        /// </summary>
        [TestMethod]
        public void Cancelling_SingleEvent()
        {
            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);
            DateTime anyNonDefaultDateTime = DateTime.Now;
            DateTime twoSecondsFromNowDate = anyNonDefaultDateTime + twoSecondsTimeSpan;

            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 0;

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>();

            Scheduler scheduler = new Scheduler(anyNonDefaultDateTime);

            scheduler.OnEventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                if (eventArgs.Event == bEventMockForScheduled.Object)
                {
                    scheduledEventFiredCounter++;
                }
            };

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, twoSecondsFromNowDate);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();

            // cancel this event.
            scheduler.CancelEvent(bEventMockForScheduled.Object.EventId);

            // delay for three seconds and check that the counter has NOT gone up for scheduled.
            Task.Delay(threeSecondsTimeSpan)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.CancelAllFor(uint)"/> does what it should.
        /// </summary>
        [TestMethod]
        public void Cancelling_AllEventsFor()
        {
            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan threeSecondsTimeSpan = TimeSpan.FromSeconds(3);
            DateTime anyNonDefaultDateTime = DateTime.Now;
            DateTime twoSecondsFromNowDate = anyNonDefaultDateTime + twoSecondsTimeSpan;

            const uint anyRequestorId = 100u;
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 0;

            var scheduledEventFiredCounter = 0;

            Mock<BaseEvent> bEventMockForScheduled1 = new Mock<BaseEvent>(anyRequestorId);
            Mock<BaseEvent> bEventMockForScheduled2 = new Mock<BaseEvent>(anyRequestorId);
            Mock<BaseEvent> bEventMockForScheduled3 = new Mock<BaseEvent>(anyRequestorId);

            Scheduler scheduler = new Scheduler(anyNonDefaultDateTime);

            scheduler.OnEventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                scheduledEventFiredCounter++;
            };

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled1.Object, twoSecondsFromNowDate);
            scheduler.ScheduleEvent(bEventMockForScheduled2.Object, twoSecondsFromNowDate);
            scheduler.ScheduleEvent(bEventMockForScheduled3.Object, twoSecondsFromNowDate);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();

            // cancel this event.
            scheduler.CancelAllFor(anyRequestorId);

            // delay for three seconds and check that the counter has NOT gone up for scheduled.
            Task.Delay(threeSecondsTimeSpan)
                .ContinueWith(prev =>
                {
                    Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
                })
                .Wait();
        }

        /// <summary>
        /// Checks that <see cref="Scheduler.OnEventFired"/> gets fired when an event is scheduled.
        /// </summary>
        [TestMethod]
        public void OnEventFired_IsCalled()
        {
            const int ExpectedCounterValueBeforeRun = 0;
            const int ExpectedCounterValueAfterRun = 1;

            TimeSpan twoSecondsTimeSpan = TimeSpan.FromSeconds(2);
            TimeSpan overheadDelay = TimeSpan.FromMilliseconds(100);
            DateTime anyNonDefaultDateTime = DateTime.Now;
            DateTime twoSecondsFromNowDate = anyNonDefaultDateTime + twoSecondsTimeSpan;

            Mock<BaseEvent> bEventMockForInmediate = new Mock<BaseEvent>();
            Mock<BaseEvent> bEventMockForScheduled = new Mock<BaseEvent>();

            Scheduler scheduler = new Scheduler(anyNonDefaultDateTime);

            var inmediateEventFiredCounter = 0;
            var scheduledEventFiredCounter = 0;

            scheduler.OnEventFired += (sender, eventArgs) =>
            {
                // test that sender is the same scheduler instance, while we're here.
                Assert.AreEqual(scheduler, sender);

                // check that event has a reference.
                Assert.IsNotNull(eventArgs?.Event);

                if (eventArgs.Event == bEventMockForInmediate.Object)
                {
                    inmediateEventFiredCounter++;
                }
                else if (eventArgs.Event == bEventMockForScheduled.Object)
                {
                    scheduledEventFiredCounter++;
                }
            };

            // fire a scheduled event that shall be fired only after some seconds.
            scheduler.ScheduleEvent(bEventMockForScheduled.Object, twoSecondsFromNowDate);

            // delay for 100 ms (to account for setup overhead and multi threading) and check that the counter has NOT gone up for scheduled
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                    {
                        Assert.AreEqual(ExpectedCounterValueBeforeRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueBeforeRun}, got {scheduledEventFiredCounter}.");
                    })
                .Wait();

            // fire the inmediate event, which should be run asap.
            scheduler.ImmediateEvent(bEventMockForInmediate.Object);

            // delay for 500 ms and check that the counter has gone up.
            Task.Delay(overheadDelay)
                .ContinueWith(prev =>
                    {
                        Assert.AreEqual(ExpectedCounterValueAfterRun, inmediateEventFiredCounter, $"Inmediate events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {inmediateEventFiredCounter}.");
                    })
                .Wait();

            // delay for the remaining seconds and check that the counter has gone up for scheduled.
            Task.Delay(twoSecondsTimeSpan)
                .ContinueWith(prev =>
                    {
                        Assert.AreEqual(ExpectedCounterValueAfterRun, scheduledEventFiredCounter, $"Scheduled events counter does not match: Expected {ExpectedCounterValueAfterRun}, got {scheduledEventFiredCounter}.");
                    })
                .Wait();
        }
    }
}
