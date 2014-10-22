namespace Pgpass.Net
{
    using System;
    using System.Text.RegularExpressions;

    public static class PgpassExtensions
    {
        private static readonly Regex Colons = new Regex(@"(?<![^\\](\\|\\{3}|\\{5}|\\{7}|\\{9}))\\:",
            RegexOptions.Compiled);

        private static readonly Regex BadEscapes =
            new Regex(@"(?<![^\\](\\|\\{3}|\\{5}|\\{7}|\\{9}))\\(?!(\\|\\{3}|\\{5}|\\{7}|\\{9})([^\\]|$))",
                RegexOptions.Compiled);

        public static string PgpassUnescape(this string s)
        {
            var slashed = Colons.Replace(s, ":");

            // look for bad escape sequences
            if (BadEscapes.IsMatch(slashed))
                throw new FormatException(string.Format(@"Bad escape sequence: ""{0}""", s));

            return slashed.Replace(@"\\", @"\");
        }

        public static bool IsNullOrEqual(this string s1, string s2)
        {
            return s1 == null || s1.Equals(s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrEqual(this int? i1, int i2)
        {
            return i1 == null || i1 == i2;
        }

        public static bool Matches(this PgpassEntry entry, string host, int port, string database, string user)
        {
            return entry.Host.IsNullOrEqual(host) && entry.Port.IsNullOrEqual(port) &&
                   entry.Database.IsNullOrEqual(database) && entry.User.IsNullOrEqual(user);
        }
    }
}
