using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Reader;
using Xunit;

namespace RDBData.Tests.Reader
{
    public class GetRdbNetsTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }

        public GetRdbNetsTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        [Fact]
        public void WhenGet()
        {
            //Arrange
            var subject = Mocker.CreateInstance<GetRdbNets>();
            var nets = AutoFixture.Create<IEnumerable<RdbNet>>();
            var fileName = AutoFixture.Create<string>();
            var pins = nets.SelectMany(net => net.pins).Where(pin => pin.name != "_");
            var cenX = pins.Sum(pin => pin.x) / pins.Count();
            var cenY = pins.Sum(pin => pin.y) / pins.Count();

            Mocker.GetMock<IReadRdb>()
                .Setup(reader => reader.Read(fileName))
                .Returns(nets);

            //Act
            var result = subject.Get(fileName);

            //Assert
            result.fileName.Should().Be(fileName);
            result.Nets.Should().BeEquivalentTo(nets);
            result.CentroidX.Should().Be(cenX);
            result.CentroidY.Should().Be(cenY);
        }

        [Fact]
        public void WhenGetAndHasEmptyPin()
        {
            //Arrange
            var subject = Mocker.CreateInstance<GetRdbNets>();
            var nets = new[]
            {
                new RdbNet
                {
                    name = AutoFixture.Create<string>(),
                    pins = new []
                    {
                        new Pin
                        {
                            name = "_", 
                            x=0, 
                            y=0
                        },
                        new Pin
                        {
                            name = "1_1",
                            x=1,
                            y=1
                        },
                        new Pin
                        {
                            name = "2_2",
                            x=2,
                            y=2
                        }
                    }
                }
            };
            var fileName = AutoFixture.Create<string>();

            Mocker.GetMock<IReadRdb>()
                .Setup(reader => reader.Read(fileName))
                .Returns(nets);

            //Act
            var result = subject.Get(fileName);

            //Assert
            result.fileName.Should().Be(fileName);
            result.Nets.Should().BeEquivalentTo(nets);
            result.CentroidX.Should().Be(1.5f);
            result.CentroidY.Should().Be(1.5f);
        }
    }
}