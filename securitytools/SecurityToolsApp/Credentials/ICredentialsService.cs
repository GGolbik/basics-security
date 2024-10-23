
using System.Diagnostics.CodeAnalysis;
using GGolbik.SecurityTools.Credentials;

namespace GGolbik.SecurityToolsApp.Credentials;

public interface ICredentialsService : IDisposable
{
    /// <summary>
    /// Returns all credentials
    /// </summary>
    /// <returns>The credentials.</returns>
    IDictionary<string, KeyCredentials> GetCredentials();

    /// <summary>
    /// Returns the credentials for the ID.
    /// </summary>
    /// <param name="id">The ID of the credentials.</param>
    /// <returns>The credentials.</returns>
    KeyCredentials GetCredentials(string id);

    /// <summary>
    /// Tries to find the credentials for the ID.
    /// </summary>
    /// <param name="id">The ID of the credentials.</param>
    /// <param name="credentials">The credentials.</param>
    /// <returns>true if the config exists.</returns>
    bool TryGetCredentials(string id, [NotNullWhen(true)] out KeyCredentials? credentials);

    /// <summary>
    /// Adds new credentials.
    /// </summary>
    /// <param name="credentials">The new credentials.</param>
    /// <returns>The credentials ID.</returns>
    string AddCredentials(KeyCredentials credentials);

    /// <summary>
    /// Updates existing credentials.
    /// </summary>
    /// <param name="id">The ID of the credentials.</param>
    /// <param name="credentials">The updated credentials.</param>
    void UpdateCredentials(string id, KeyCredentials credentials);

    /// <summary>
    /// Deletes the credentials.
    /// </summary>
    /// <param name="id">The ID of the credentials.</param>
    void DeleteCredentials(string id);
}