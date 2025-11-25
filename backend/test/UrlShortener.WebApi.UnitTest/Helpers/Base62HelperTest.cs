using UrlShortener.WebApi.Helpers;

namespace UrlShortener.WebApi.UnitTest.Helpers;

[Trait("Base62Helper", "Helpers Unit")]
public sealed class Base62HelperTest
{
    /// <summary>
    /// Deve retornar o short code esperado ao codificar o hashId fornecido.
    /// </summary>
    [Theory]
    [InlineData(0, "0")]
    [InlineData(120, "1w")]
    [InlineData(123456789, "8M0kX")]
    public void Encode_ShouldReturnExpectedShortCode(long hashId, string expectedShortCode)
    {
        // Act
        var shortCode = Base62Helper.Encode(hashId);

        // Assert
        Assert.Equal(expectedShortCode, shortCode);
    }

    /// <summary>
    /// Deve retornar o hashId esperado ao decodificar o short code fornecido.
    /// </summary>
    [Theory]
    [InlineData("0", 0)]
    [InlineData("1w", 120)]
    [InlineData("8M0kX", 123456789)]
    public void Decode_ShouldReturnExpectedHashId(string shortCode, long expectedHashId)
    {
        // Act
        var hashId = Base62Helper.Decode(shortCode);
     
        // Assert
        Assert.Equal(expectedHashId, hashId);
    }
}