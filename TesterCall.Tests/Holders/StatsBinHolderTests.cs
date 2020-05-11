using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TesterCall.Holders;
using TesterCall.Models;
using TesterCall.Models.Stats;

namespace TesterCall.Tests.Holders
{
    [TestClass]
    public class StatsBinHolderTests
    {
        public class TestResponseType
        {

        }

        private ResponseContentModel _response;

        [TestInitialize]
        public void TestInitialise()
        {
            _response = new ResponseContentModel(new TimeSpan(1000),
                                                HttpStatusCode.OK,
                                                new DateTime(),
                                                new TestResponseType());
        }

        [TestMethod]
        public void CreatesBinWithNoNameAndAllowsNewAdditions()
        {
            var testStart = DateTime.Now;
            Thread.Sleep(50);

            StatsBinHolder.StartRecording(null);
            StatsBinHolder.Add(_response);

            StatsBinHolder.Recording.Should().BeTrue();
            StatsBinHolder.Bins.FirstOrDefault().Should().NotBeNull();
            StatsBinHolder.Bins.FirstOrDefault()
                                .RecordingStarted
                                .Should()
                                .BeAfter(testStart);
            StatsBinHolder.Bins.FirstOrDefault()
                                .Responses
                                .Contains(_response)
                                .Should().BeTrue();
            CleanUpHolder();
        }

        [TestMethod]
        public void CreatesBinWithNameAndPersistsAfterRecordingStops()
        {
            var testStart = DateTime.Now;
            var name = "TestName";
            Thread.Sleep(50);

            StatsBinHolder.StartRecording(name);
            StatsBinHolder.Add(_response);
            Thread.Sleep(50);
            StatsBinHolder.StopRecording();

            StatsBinHolder.Recording.Should().BeFalse();
            var bin = StatsBinHolder.Bins.FirstOrDefault(b => b.SessionName == name);
            bin.Should().NotBeNull();
            bin.RecordingStarted.Should().BeAfter(testStart);
            bin.RecordingFinished.Should().HaveValue();
            bin.RecordingFinished.Should().BeAfter(bin.RecordingStarted);
            bin.Responses.Should().Contain(_response);

            CleanUpHolder();
        }

        [TestMethod]
        public void IncrementsDuplicateNonNullSessionName()
        {
            var testStart = DateTime.Now;
            var name = "TestName";
            var expectedSecondName = "TestName_1";
            Thread.Sleep(50);

            StatsBinHolder.StartRecording(name);
            StatsBinHolder.Add(_response);
            Thread.Sleep(50);
            StatsBinHolder.StopRecording();
            Thread.Sleep(50);
            StatsBinHolder.StartRecording(name);
            StatsBinHolder.Add(_response);
            Thread.Sleep(50);
            StatsBinHolder.StopRecording();

            var firstBin = StatsBinHolder.Bins.FirstOrDefault(b => b.SessionName == name);
            var secondBin = StatsBinHolder.Bins.FirstOrDefault(b => b.SessionName == expectedSecondName);
            StatsBinHolder.Recording.Should().BeFalse();
            firstBin.Should().NotBeNull();
            firstBin.RecordingStarted.Should().BeAfter(testStart);
            firstBin.RecordingFinished.Should().HaveValue();
            firstBin.RecordingFinished.Value.Should().BeAfter(firstBin.RecordingStarted);
            firstBin.Responses.Should().Contain(_response);
            secondBin.Should().NotBeNull();
            secondBin.RecordingStarted.Should().BeAfter(firstBin.RecordingFinished.Value);
            secondBin.RecordingFinished.Should().HaveValue();
            secondBin.RecordingFinished.Should().BeAfter(secondBin.RecordingStarted);
            secondBin.Responses.Should().Contain(_response);

            CleanUpHolder();
        }

        private void CleanUpHolder()
        {
            StatsBinHolder.FlushAll();
        }
    }
}
