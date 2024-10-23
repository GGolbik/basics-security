using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using GGolbik.SecurityTools;
using GGolbik.SecurityTools.Credentials;
using Serilog;

namespace GGolbik.SecurityToolsApp.Credentials;

public class CredentialsService : ICredentialsService
{
    private class CredentialsServiceConfig
    {
        public IDictionary<string, KeyCredentials>? Credentials { get; set; }
        public CredentialsServiceConfig()
        {

        }
    }

    private readonly Serilog.ILogger Logger = Log.ForContext<CredentialsService>();

    private CredentialsServiceConfig _config;

    public CredentialsService()
    {
        _config = this.LoadConfig();
    }

    private CredentialsServiceConfig LoadConfig()
    {
        string path = Path.Combine(Settings.ApplicationDataDirectory, "credentials.json");
        CredentialsServiceConfig? config = null;
        if (File.Exists(path))
        {
            try
            {
                using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    config = JsonSerializer.Deserialize<CredentialsServiceConfig>(stream);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load config from '{path}'.");
            }
        }
        return config ?? new CredentialsServiceConfig();
    }

    private void SaveConfig(CredentialsServiceConfig config)
    {
        string path = Path.Combine(Settings.ApplicationDataDirectory, "credentials.json");
        string pathTmp = Path.Combine(Settings.ApplicationDataDirectory, "credentials.json.tmp");
        using (Stream stream = new FileStream(pathTmp, FileMode.Create))
        {
            JsonSerializer.Serialize(stream, config);
        }
        File.Move(pathTmp, path, true);
    }

    public void Dispose()
    {
    }

    public string AddCredentials(KeyCredentials credentials)
    {
        lock (this)
        {
            _config.Credentials ??= new Dictionary<string, KeyCredentials>();
            string id;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (_config.Credentials.ContainsKey(id));
            _config.Credentials.Add(id, (KeyCredentials)credentials.Clone());
            try
            {
                this.SaveConfig(_config);
            }
            catch
            {
                _config.Credentials.Remove(id);
                throw;
            }
            return id;
        }
    }

    public IDictionary<string, KeyCredentials> GetCredentials()
    {
        lock (this)
        {
            _config.Credentials ??= new Dictionary<string, KeyCredentials>();
            Dictionary<string, KeyCredentials> result = new();
            foreach (var entry in _config.Credentials)
            {
                result.Add(entry.Key, (KeyCredentials)entry.Value.Clone());
            }
            return result;
        }
    }

    public KeyCredentials GetCredentials(string id)
    {
        lock (this)
        {
            if (this.TryGetCredentials(id, out var credentials))
            {
                return credentials;
            }
            throw new ArgumentException($"Invalid ID '{id}'.");
        }
    }

    public bool TryGetCredentials(string id, [NotNullWhen(true)] out KeyCredentials? credentials)
    {
        lock (this)
        {
            _config.Credentials ??= new Dictionary<string, KeyCredentials>();
            if (_config.Credentials.TryGetValue(id, out credentials))
            {
                credentials = (KeyCredentials)credentials.Clone();
                return true;
            }
            return false;
        }
    }

    public void UpdateCredentials(string id, KeyCredentials credentials)
    {
        lock (this)
        {
            _config.Credentials ??= new Dictionary<string, KeyCredentials>();
            if (!_config.Credentials.Remove(id, out var current))
            {
                throw new ArgumentException($"Invalid ID '{id}'.");
            }
            try
            {
                _config.Credentials.Add(id, (KeyCredentials)credentials.Clone());
                this.SaveConfig(_config);
            }
            catch
            {
                _config.Credentials.Add(id, current);
                throw;
            }
        }
    }

    public void DeleteCredentials(string id)
    {
        lock (this)
        {
            _config.Credentials ??= new Dictionary<string, KeyCredentials>();
            if (_config.Credentials.Remove(id, out var current))
            {
                try
                {
                    this.SaveConfig(_config);
                }
                catch
                {
                    _config.Credentials.Add(id, current);
                    throw;
                }
            }
        }
    }
}