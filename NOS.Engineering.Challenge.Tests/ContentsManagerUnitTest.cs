using FluentAssert;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Tests
{
    public class ContentsManagerUnitTest
    {

        Mock<IDatabase<Content?, ContentDto>> databaseMock;
        ContentsManager contentsManager;

        public ContentsManagerUnitTest()
        {
            databaseMock = new Mock<IDatabase<Content?, ContentDto>>();
            contentsManager = new ContentsManager(databaseMock.Object);
        }

        [Fact]
        public async void GetManyContents_HappyPath()
        {
            // Arrange
            var contents = new List<Content>
            {
                new Content()
                {
                    Title = "Test 1",
                },
                new Content()
                {
                    Title = "Test 2"
                }
            };

            databaseMock.Setup(m => m.ReadAll()).ReturnsAsync(contents);

            // Act
            var result = await contentsManager.GetManyContents();

            // Assert
            result.ShouldBeEqualTo(contents);
        }
    }
}
