using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using GGolbik.SecurityTools.Credentials;

namespace GGolbik.SecurityToolsTest;


public class KeyCredentialsTest
{

    public static IEnumerable<object?[]> CredentialsData = new List<object?[]>
    {
        new object?[] { new KeyCredentialsDescription(), new AnonymousCredentials(), null },
        new object?[] { new KeyCredentialsDescription(), new UsernamePasswordCredentials("admin"), null },
        new object?[] { new KeyCredentialsDescription(), new TokenCredentials("{}", TokenCredentials.TokenTypeJwt, "https://"), null },
        new object?[] { new KeyCredentialsDescription(), new CertificateCredentials(X509Certificate2.CreateFromPem(TestData.TestCodePem)), null },
        new object?[] { new KeyCredentialsDescription(), new AnonymousCredentials(), "1234" },
        new object?[] { new KeyCredentialsDescription(), new UsernamePasswordCredentials("admin"), "1234" },
        new object?[] { new KeyCredentialsDescription(), new TokenCredentials("{}", TokenCredentials.TokenTypeJwt, "https://"), "1234" },
        new object?[] { new KeyCredentialsDescription(), new CertificateCredentials(X509Certificate2.CreateFromPem(TestData.TestCodePem)), "1234" }
    };

    [Theory]
    [MemberData(nameof(CredentialsData))]
    public void Test_Json_With_Value(KeyCredentialsDescription description, KeyCredentialsValue credentialsValue, string? password)
    {
        KeyCredentials credentials = new(description, credentialsValue, password == null ? null : Encoding.UTF8.GetBytes(password));
        var json = JsonSerializer.Serialize(credentials);
        var jsonCredentials = JsonSerializer.Deserialize<KeyCredentials>(json);
        jsonCredentials.Should().NotBeNull();
        jsonCredentials?.Description.Label.Should().Be(description.Label);
        jsonCredentials?.Description.Details.Should().Be(description.Details);
        jsonCredentials?.Value.Should().BeEquivalentTo(credentials.Value);
        var value = jsonCredentials?.ToKeyCredentialsValue(password == null ? null : Encoding.UTF8.GetBytes(password));
        value?.GetType().Should().Be(credentialsValue.GetType());
        if (value is UsernamePasswordCredentials user && credentialsValue is UsernamePasswordCredentials expectedUser)
        {
            user.Username.Should().Be(expectedUser.Username);
            user.Password.Should().BeEquivalentTo(expectedUser.Password);
        }
        if (value is TokenCredentials token && credentialsValue is TokenCredentials expectedToken)
        {
            token.Token.Should().Be(expectedToken.Token);
            token.TokenType.Should().Be(expectedToken.TokenType);
            token.IssuerEndpointUrl.Should().Be(expectedToken.IssuerEndpointUrl);
        }
        if (value is CertificateCredentials certificate && credentialsValue is CertificateCredentials expectedCertificate)
        {
            certificate.Certificate?.Should().BeEquivalentTo(expectedCertificate.Certificate);
        }
    }

    [Theory]
    [MemberData(nameof(CredentialsData))]
    public void Test_Json_Without_value(KeyCredentialsDescription description, KeyCredentialsValue credentialsValue, string? password)
    {
        KeyCredentials credentials = new(description, credentialsValue, password == null ? null : Encoding.UTF8.GetBytes(password));
        JsonSerializerOptions options = new()
        {

        };
        options.Converters.Add(new KeyCredentialsJsonConverter(false));
        var json = JsonSerializer.Serialize(credentials, options);
        var jsonCredentials = JsonSerializer.Deserialize<KeyCredentials>(json, options);
        jsonCredentials.Should().NotBeNull();
        jsonCredentials?.Description.Label.Should().Be(description.Label);
        jsonCredentials?.Description.Details.Should().Be(description.Details);
        jsonCredentials?.Value.Should().BeEmpty();
    }
}