using System;
using System.IO;
using System.Linq;
using INotificator.Services.Parser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Tests.Parsers
{
    public class AvitoParserTests
    {
        [Theory]
        [InlineData("AvitoExample1")]
        public void TaskParserTest(string file)
        {
            IOptions<Options> options = Microsoft.Extensions.Options.Options.Create<Options>(new Options
            {
                Avito = new Options.AvitoConfig()
                {
                    Url = "url"
                }
            });
            var logger = new Mock<ILogger<AvitoParser>>();

            var path = Path.IsPathRooted(file)
                ? file
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), $"Files/{file}.html");

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            // Load the file
            var fileData = File.ReadAllText(path);

            var avitoParser = new AvitoParser(options, logger.Object);
            var result = avitoParser.ParseResult(fileData);
            
            Assert.False(result.HasError);
            Assert.Equal(49, result.Data.Count);
            
            //First item check
            Assert.Equal("Видеокарта nvidia geforce gtx 1050 2gb в Обнинске", result.Data.First().Name);
            Assert.Equal("6200", result.Data.First().Price);
            Assert.Equal("url", result.Data.First().Source);
            Assert.Equal("url/obninsk/tovary_dlya_kompyutera/videokarta_nvidia_geforce_gtx_1050_2gb_2141197421", result.Data.First().Url);
        }
    }
}