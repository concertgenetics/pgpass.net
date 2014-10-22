namespace Pgpass.Net
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class PgpassEntry
    {
        private static readonly Regex Splitter = new Regex(@"(?<![^\\](\\|\\{3}|\\{5}|\\{7}|\\{9})):",
            RegexOptions.Compiled);

        private static readonly Regex Empty = new Regex(@"(^\s*$)|(^\s*#)", RegexOptions.Compiled);

        public PgpassEntry(string configLine, PgpassConfig config)
        {
            Valid = true;

            // identify comments and empty lines
            if (Empty.IsMatch(configLine))
            {
                IsEmpty = true;
                return;
            }

            // parse fields
            string[] fields;
            try
            {
                fields =
                    Splitter.Split(configLine)
                        .Select(l => l == "*" ? null : l.PgpassUnescape())
                        .ToArray();
            }
            catch (FormatException)
            {
                if (config.IgnoreBadConfigLines)
                {
                    Valid = false;
                    return;
                }
                throw;
            }
            if (fields.Length == 5)
            {
                Host = fields[0];
                try
                {
                    Port = fields[1] == null ? null : (int?) int.Parse(fields[1]);
                }
                catch (FormatException e)
                {
                    if (!config.IgnoreBadConfigLines)
                        throw new FormatException(string.Format(@"Cannot parse config line: ""{0}""", configLine), e);
                    Valid = false;
                }
                Database = fields[2];
                User = fields[3];
                Password = fields[4];
            }
            else
            {
                if (!config.IgnoreBadConfigLines)
                    throw new FormatException(string.Format(@"Cannot parse config line: ""{0}""", configLine));
                Valid = false;
            }
        }

        public bool Valid { get; private set; }
        public bool IsEmpty { get; private set; }
        public string Host { get; private set; }
        public int? Port { get; private set; }
        public string Database { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
    }
}