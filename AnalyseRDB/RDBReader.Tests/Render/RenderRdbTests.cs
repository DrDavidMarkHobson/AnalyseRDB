using System.Drawing;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Render;
using Xunit;
using Point = RDB.Interface.RDBObjects.Point;

namespace RDBData.Tests.Render
{
    public class RenderRdbTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }

        public RenderRdbTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        [Fact]
        public void WhenWriteRdb()
        {
            //Arrange
            var subject = Mocker.CreateInstance<RenderRdb>();
            var data = AutoFixture.Create<RdbNets>();
            var points = data.Nets.SelectMany(net =>
                net.pins.Select(pin =>
                    new Point { X = pin.x, Y = pin.y }));

            //Act
            var result = subject.Convert(data);

            //Assert
            result.Width.Should().Be(800);
            result.Height.Should().Be(600);
            var bitmap = (Bitmap) result;

            foreach (var point in points)
            {
                if (
                    point.X >= 0 &&
                    point.Y >= 0 &&
                    point.X < bitmap.Width &&
                    point.Y < bitmap.Height)
                {
                    var pixel = bitmap.GetPixel((int) point.X, (int) point.Y);
                    pixel.R.Should().Be(255);
                    pixel.G.Should().Be(255);
                    pixel.B.Should().Be(255);
                    pixel.A.Should().Be(255);
                }
            }
        }
    }
}