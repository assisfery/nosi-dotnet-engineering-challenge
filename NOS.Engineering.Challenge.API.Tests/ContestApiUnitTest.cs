using NOS.Engineering.Challenge.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using FluentAssert;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.Models;
using Microsoft.AspNetCore.Mvc;

namespace NOS.Engineering.Challenge.API.Tests
{
    public class ContestApiUnitTest
    {
        Mock<IContentsManager> _mockContentManager;
        Mock<ILogger<ContentController>> _mockLogger;
        Mock<IMemoryCache> _mockCache;
        ContentController _contentController;

        public ContestApiUnitTest()
        {
            _mockContentManager = new Mock<IContentsManager>();
            _mockLogger = new Mock<ILogger<ContentController>>();
            _mockCache = new Mock<IMemoryCache>();

            _contentController = new ContentController(_mockContentManager.Object, _mockLogger.Object, _mockCache.Object);
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

            String x = "";
            Object obj = null;

            _mockContentManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);
            //_mockCache.Setup(c => c.Set("GetManyContents", new Content(), new MemoryCacheEntryOptions())).Verifiable();

            // Act
            var result = await _contentController.GetManyContents(true);

            // Assert
            result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async void GetManyContents_NotFound()
        {
            // Arrange
            var contents = new List<Content>{};

            String x = "";
            Object obj = null;

            _mockContentManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);
            //_mockCache.Setup(c => c.Set("GetManyContents", new Content(), new MemoryCacheEntryOptions())).Verifiable();

            // Act
            var result = await _contentController.GetManyContents(true);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
