using System.Collections.Generic;
using SystemInterface.IO;
using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Reader;
using Xunit;

namespace RDBData.Tests.Reader
{
    public class ReadRdbTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }

        public ReadRdbTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        private IEnumerable<RdbNet> CreateTestData(int length)
        {
            var data = new List<RdbNet>();
            while (length > 0)
            {
                var current = new RdbNet
                {
                    name = AutoFixture.Create<string>(),
                    pins = AutoFixture.CreateMany<Pin>(),
                    prop = AutoFixture.Create<Prop>()
                };
                data.Add(current);
                length--;
            }

            return data;
        }

        private string[] DataToFile(IEnumerable<RdbNet> data)
        {
            var result = new List<string>();

            foreach (var rdbNet in data)
            {
                result.Add(string.Format(RdbFileLines.NetLine, rdbNet.name));
                result.Add(string.Format(RdbFileLines.PropLine, rdbNet.prop.type));
                foreach (var pin in rdbNet.pins)
                {
                    result.Add(string.Format(RdbFileLines.PinLine, pin.name, pin.x, pin.y));
                }
            }
            return result.ToArray();
        }


        [Fact]
        public void WhenFileToRDB()
        {
            //Arrange
            var dataLength = AutoFixture.Create<int>();
            var expectedData = CreateTestData(dataLength);
            var fileData = DataToFile(expectedData);
            var filePath = AutoFixture.Create<string>();

            var subject = Mocker.CreateInstance<ReadRdb>();

            Mocker.GetMock<IOpenAndReadRDBFile>()
                .Setup(reader => reader.ReadAll(filePath))
                .Returns(fileData);

            //Act
            var result = subject.Read(filePath);

            //Assert
            result.Should().BeEquivalentTo(expectedData);
        }

    }
}
