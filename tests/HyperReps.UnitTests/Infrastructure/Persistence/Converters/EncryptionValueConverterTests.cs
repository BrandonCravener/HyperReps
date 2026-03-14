using System;
using System.Linq.Expressions;
using HyperReps.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit;

namespace HyperReps.UnitTests.Infrastructure.Persistence.Converters
{
    public class EncryptionValueConverterTests
    {
        private const string TestKey = "TestKey1234567890123456789012345"; // 32 chars for simple valid key (UTF-8 bytes will vary but length is sufficient to test) 
        // Note: The actual key length requirement depends on AES-256 (32 bytes). 
        // "TestKey1234567890123456789012345" is 32 characters long.
        
        [Fact]
        public void Should_Encrypt_And_Decrypt_Correctly()
        {
            // Arrange
            var converter = new EncryptionValueConverter(TestKey);
            var originalText = "SecretMessage123!";

            // Act
            // EF Core ValueConverter exposes ConvertToProvider (Encrypt) and ConvertFromProvider (Decrypt) 
            // via the ConvertToProviderExpression and ConvertFromProviderExpression. 
            // However, typically we verify the functional behavior by compiling the expressions or using the functions passed to the base.
            
            // To test the logic encapsulated in the private static methods, we can use the converter's Funcs.
            var encryptFunc = converter.ConvertToProviderExpression.Compile();
            var decryptFunc = converter.ConvertFromProviderExpression.Compile();

            var encrypted = encryptFunc(originalText);
            var decrypted = decryptFunc(encrypted);

            // Assert
            Assert.NotEqual(originalText, encrypted);
            Assert.Equal(originalText, decrypted);
        }

        [Fact]
        public void Should_Handle_Null_And_Empty_Strings()
        {
            // Arrange
            var converter = new EncryptionValueConverter(TestKey);
            var encryptFunc = converter.ConvertToProviderExpression.Compile();
            var decryptFunc = converter.ConvertFromProviderExpression.Compile();

            // Act & Assert
            Assert.Null(encryptFunc(null!));
            Assert.Null(decryptFunc(null!));
            Assert.Equal(string.Empty, encryptFunc(string.Empty));
            Assert.Equal(string.Empty, decryptFunc(string.Empty));
        }

        [Fact]
        public void Should_Generate_Different_Ciphertext_For_Same_Input()
        {
            // Arrange
            var converter = new EncryptionValueConverter(TestKey);
            var encryptFunc = converter.ConvertToProviderExpression.Compile();
            var plainText = "SameMessage";

            // Act
            var cipher1 = encryptFunc(plainText);
            var cipher2 = encryptFunc(plainText);

            // Assert
            // Because a new IV is generated for each encryption, ciphertexts should be different
            Assert.NotEqual(cipher1, cipher2);
        }
    }
}
