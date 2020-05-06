using System;
using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Points;
using Xunit;

namespace RDBData.Tests.Points
{
    public class MovePointTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }

        public MovePointTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        [Fact]
        public void WhenRotateAround()
        {
            //Arrange
            var point = AutoFixture.Create<Point>();
            var pivot = AutoFixture.Create<Point>();
            var angle = AutoFixture.Create<float>();

            var angleInRadians = angle * (float)(Math.PI / 180);
            var cosTheta = (float)Math.Cos(angleInRadians);
            var sinTheta = (float)Math.Sin(angleInRadians);

            var subject = Mocker.CreateInstance<MovePoint>();

            //Act
            var result = subject.RotateAround(point, pivot, angle);

            //Assert
            result.X.Should().Be(
                cosTheta * (point.X - pivot.X) -
                sinTheta * (point.Y - pivot.Y) + pivot.X);
            result.Y.Should().Be(
                sinTheta * (point.X - pivot.X) +
                cosTheta * (point.Y - pivot.Y) + pivot.Y);
        }

    }
}