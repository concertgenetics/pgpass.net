namespace Pgpass.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class Pgpass
    {
        private static readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private readonly PgpassConfig _config;
        private readonly IEnumerable<PgpassEntry> _entries;
        private readonly string _host;
        private readonly int _port;
        private readonly string _path;

        public Pgpass(string dbServer, PgpassConfig config = null)
        {
            _config = config ?? PgpassConfig.Default;
            _path = Environment.GetEnvironmentVariable(_config.PathEnvironmentVariable);
            if (string.IsNullOrEmpty(_path))
                _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"postgresql\pgpass.conf");
            var fields = dbServer.Split(':');
            _host = fields[0];
            _port = fields.Length < 2 ? 5432 : int.Parse(fields[1]);
            _entries = ReadConfig();
        }

        public Pgpass(string dbServer, string path, PgpassConfig config = null)
        {
            _path = path;
            _config = config ?? PgpassConfig.Default;
            var fields = dbServer.Split(':');
            _host = fields[0];
            _port = fields.Length < 2 ? 5432 : int.Parse(fields[1]);
            _entries = ReadConfig();
        }

        private IEnumerable<PgpassEntry> ReadConfig()
        {
            if (!File.Exists(_path))
            {
                if (!_config.AllowMissingConfig)
                    throw new FileNotFoundException(@"Pgpass file does not exist: ""{0}""", _path);
                return new List<PgpassEntry>();
            }
            return File.ReadAllLines(_path).Select(l => new PgpassEntry(l, _config)).Where(e => e.Valid && !e.IsEmpty);
        }

        public string GetConnectionString(string database, string user, string template = null)
        {
            var key = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", _path, _host, _port, database, user);
            if (_config.UseStaticCache && _cache.ContainsKey(key)) return _cache[key];
            var entry = _entries.FirstOrDefault(e => e.Matches(_host, _port, database, user));
            var connectionString = entry == null
                ? null
                : string.Format(template ?? _config.DefaultConnectionStringTemplate, _host, _port, database, user,
                    entry.Password);
            if(_config.UseStaticCache) _cache.Add(key, connectionString);
            return connectionString;
        }
    }
}
