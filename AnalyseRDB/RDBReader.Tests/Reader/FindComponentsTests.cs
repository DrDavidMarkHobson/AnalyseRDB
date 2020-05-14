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
    public class FindComponentsTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }

        public FindComponentsTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        private IEnumerable<Pin> CreateTestDataPins(string pinName, int pins)
        {
            var pinList = new List<Pin>();
            for (int pin = 1; pin < pins; pin++)
            {
                var newPin = new Pin
                {
                    name = pinName + "_" + AutoFixture.Create<string>(),
                    x = AutoFixture.Create<float>(),
                    y = AutoFixture.Create<float>()
                };
                pinList.Add(newPin);
            }

            return pinList;
        }

        private IEnumerable<RdbComponent> CreateSimpleTestData(int length)
        {
            var data = new List<RdbComponent>();
            while (length > 0)
            {
                var componentName = AutoFixture.Create<string>();

                var current = new RdbComponent
                {
                    name = componentName,
                    pins = CreateTestDataPins(componentName,3)
                };
                data.Add(current);
                length--;
            }

            return data;
        }

        [Fact]
        public void WhenFindAComponent()
        {
            //Arrange
            var subject = Mocker.CreateInstance<FindComponents>();
            var expectedComponents = CreateSimpleTestData(AutoFixture.Create<int>());
            var nets = expectedComponents.Select(component =>
            {
                return new RdbNet
                {
                    components = null,
                    name = AutoFixture.Create<string>(),
                    pins = component.pins,
                    prop = AutoFixture.Create<Prop>(),
                };
            });

            //Act
            var result = subject.Find(nets);

            //Assert
            result.Should().BeEquivalentTo(expectedComponents);
        }
    }
}