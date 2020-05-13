using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Points;
using Xunit;

namespace RDBData.Tests.Points
{
    public class UpdateRDBTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }
        public UpdateRDBTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        public class TestNets
        {
            public RdbNets Nets { get; set; }

            public IEnumerable<Point> NetsPoints =>
                Nets.Nets.SelectMany(net =>
                    net.pins).Select(pin =>
                    new Point {X = pin.x, Y = pin.y});

            public IEnumerable<Point> ExpectedPoints =>
                NetsPoints.Select(point =>
                    new Point {X = point.X + 1, Y = point.Y + 1});
        }

        private int _progress = 0;
        private bool _canComplete = false;

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progress = e.ProgressPercentage;
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _canComplete = true;
        }

        void bgw_Test(object sender, DoWorkEventArgs e)
        {
            _results = _updateRdb.RotateAround(_testNets.Nets, _pivot, _angle, _backgroundworker).Result;
        }

        private UpdateRDB _updateRdb;
        private BackgroundWorker _backgroundworker;
        private TestNets _testNets;
        private Point _pivot;
        private float _angle;
        private RdbNets _results;

        [Fact]
        public void WhenUpdate()
        {
            //Arrange
            _updateRdb = Mocker.CreateInstance<UpdateRDB>();
            _testNets = AutoFixture.Create<TestNets>();
            _pivot = AutoFixture.Create<Point>();
            _angle = AutoFixture.Create<float>();
            _backgroundworker = new BackgroundWorker();
            _results = null;
            _canComplete = false;

            var expectedPoint = new Point { X = 1, Y = 1 };
            Mocker.GetMock<IMovePoint>()
                .Setup(move => move.RotateAround(It.IsAny<Point>(), _pivot, _angle))
                .Returns(expectedPoint);

            //Act
            _backgroundworker.DoWork += new DoWorkEventHandler(bgw_Test);
            _backgroundworker.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            _backgroundworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            _backgroundworker.WorkerReportsProgress = true;
            _backgroundworker.RunWorkerAsync();

            while (_canComplete == false)
            {}

            var resultPoints =
                _results.Nets.SelectMany(pins =>
                        pins.pins.Select(pin =>
                            new Point {X = pin.x, Y = pin.y}))
                    .ToArray();

            //Assert
            var i = 0;
            while (i < resultPoints.Length)
            {
                resultPoints[i].X.Should().Be(expectedPoint.X);
                resultPoints[i].Y.Should().Be(expectedPoint.Y);
                i++;
            }

            _results.CentroidX.Should().Be((float)1);
            _results.CentroidY.Should().Be((float)1);

            _progress.Should().Be(100);
        }
    }
}