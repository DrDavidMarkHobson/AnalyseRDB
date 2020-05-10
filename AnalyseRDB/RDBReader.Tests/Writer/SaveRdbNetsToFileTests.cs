using AutoFixture;
using Moq;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Writer;
using Xunit;

namespace RDBData.Tests.Writer
{
    public class SaveRdbNetsToFileTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }
        public SaveRdbNetsToFileTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        [Fact]
        public void WhenWriteRdb()
        {
            //Arrange
            var subject = Mocker.CreateInstance<SaveRdbNetsToFile>();
            var data = AutoFixture.Create<RdbNets>();
            var stringData = AutoFixture.Create<string[]>();

            Mocker.GetMock<IWriteRDBDataToFile>()
                .Setup(writer => writer.Write(stringData, data.fileName));
            Mocker.GetMock<IWriteableRdb>()
                .Setup(writer => writer.Write(data))
                .Returns(stringData);

            //Act
            subject.Write(data);

            //Assert
            Mocker.Verify<IWriteRDBDataToFile>(called => 
                called.Write(stringData, data.fileName), Times.Once());
            Mocker.Verify<IWriteableRdb>(called =>
                called.Write(data), Times.Once());
        }
    }
}