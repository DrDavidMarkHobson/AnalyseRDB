using System.Collections.Generic;
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

        [Fact]
        public void WhenUpdate()
        {
            //Arrange
            var subject = Mocker.CreateInstance<UpdateRDB>();
            var testNets = AutoFixture.Create<TestNets>();
            var nets = testNets.Nets;
            var pivot = AutoFixture.Create<Point>();
            var angle = AutoFixture.Create<float>();

            var expectedPoint = new Point {X = 1, Y = 1};
            Mocker.GetMock<IMovePoint>()
                .Setup(move => move.RotateAround(It.IsAny<Point>(), pivot, angle))
                .Returns(expectedPoint);

            //Act
            var result = subject.RotateAround(nets, pivot, angle);
            var resultPoints =
                result.Nets.SelectMany(pins =>
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

            result.CentroidX.Should().Be((float)1);
            result.CentroidY.Should().Be((float)1);
        }
    }
}