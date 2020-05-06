using System.Collections.Generic;
using SystemInterface.IO;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using RDB.Interface.RDBObjects;
using RDBData.Reader;
using Xunit;

namespace RDBData.Tests.Reader
{
    /*

RoBAT Software Task
Sometimes a customer wants us to make a test program using data that is in an unknown, or at least undocumented, format. 
The attached file contains the netlist information for a large backplane. 
The information largely consists of pin names and X/Y coordinates, grouped into nets 
(a net is a collection of points which are all connected together). 

1. 
Open up the attached netlist file and read the positions of all points into memory into a suitable internal structure.

2. 
Create a list of component names, together with how many pins belong to each component 
(hint: J1_A1 would mean pin A1 belonging to component J1) and display it.

3. 
Calculate the centre of gravity of all the points on the board. 
Use the extents of the data for determining this.

4. 
Rotate the points, by ‘n’ degrees, around a defined point in x and y 
(these should be entered by the user).

5. 
Write the resulting points out to another file but keep the file formatting the same.

6. 
Draw the board on the screen and return the x,y position of the mouse in board coordinates.

EXTRA CREDIT will be awarded for the use of threads, progress indicators, GUI and general usability.


    */
    public class WriteRdbTests
    {
        public Fixture AutoFixture { get; set; }
        public AutoMocker Mocker { get; set; }
        public WriteRdbTests()
        {
            AutoFixture = new Fixture();
            Mocker = new AutoMocker();
        }

        [Fact]
        public void WhenWriteRdb()
        {/*
            //Arrange
            var subject = Mocker.CreateInstance<WriteRdb>();
            var fileName = AutoFixture.Create<string>();
            var contents = AutoFixture.Create<string[]>();
            

            Mocker.GetMock<IFile>()
                .Setup(writer => writer.WriteAllLines(fileName, contents));

            //Act
            var result = subject.WriteToFile(fileName, contents);

            //Assert
            Mocker.Verify<IFile>(writer => writer.WriteAllLines(fileName, contents), Times.Once);
        */
            }
    }

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
