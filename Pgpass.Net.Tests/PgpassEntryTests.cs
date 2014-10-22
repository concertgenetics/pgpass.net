namespace Pgpass.Net.Tests
{
    using System;
    using Xunit;

    public class PgpassEntryTests
    {
        [Fact]
        public void CanParseValidEntries()
        {
            var config = PgpassConfig.Default;
            config.IgnoreBadConfigLines = false;
            config.UseStaticCache = false;

            const string host = "localhost";
            const int port = 9999;
            const string database = "mydatabase";
            const string user = "myuser";
            const string password = "mypassword";

            var e1 = new PgpassEntry(string.Format("{0}:{1}:{2}:{3}:{4}", host, port, database, user, password), config);
            var e2 = new PgpassEntry(string.Format("*:{0}:{1}:{2}:{3}", port, database, user, password), config);
            var e3 = new PgpassEntry(string.Format("*:*:{0}:{1}:{2}", database, user, password), config);
            var e4 = new PgpassEntry(string.Format("*:*:*:{0}:{1}", user, password), config);
            var e5 = new PgpassEntry(string.Format("*:*:*:*:{0}", password), config);
            var e6 = new PgpassEntry(string.Format("{0}:*:{1}:{2}:{3}", host, database, user, password), config);
            var e7 = new PgpassEntry(string.Format("{0}:{1}:*:*:{2}", host, port, password), config);
            var e8 = new PgpassEntry(string.Format("{0}:*:{1}:*:{2}", host, database, password), config);

            Assert.True(e1.Valid);
            Assert.False(e1.IsEmpty);
            Assert.Equal(host, e1.Host);
            Assert.Equal(port, e1.Port);
            Assert.Equal(database, e1.Database);
            Assert.Equal(user, e1.User);
            Assert.Equal(password, e1.Password);

            Assert.True(e2.Valid);
            Assert.False(e2.IsEmpty);
            Assert.Null(e2.Host);
            Assert.Equal(port, e2.Port);
            Assert.Equal(database, e2.Database);
            Assert.Equal(user, e2.User);
            Assert.Equal(password, e2.Password);

            Assert.True(e3.Valid);
            Assert.False(e3.IsEmpty);
            Assert.Null(e3.Host);
            Assert.Null(e3.Port);
            Assert.Equal(database, e3.Database);
            Assert.Equal(user, e3.User);
            Assert.Equal(password, e3.Password);

            Assert.True(e4.Valid);
            Assert.False(e4.IsEmpty);
            Assert.Null(e4.Host);
            Assert.Null(e4.Port);
            Assert.Null(e4.Database);
            Assert.Equal(user, e4.User);
            Assert.Equal(password, e4.Password);

            Assert.True(e5.Valid);
            Assert.False(e5.IsEmpty);
            Assert.Null(e5.Host);
            Assert.Null(e5.Port);
            Assert.Null(e5.Database);
            Assert.Null(e5.User);
            Assert.Equal(password, e5.Password);

            Assert.True(e6.Valid);
            Assert.False(e6.IsEmpty);
            Assert.Equal(host, e6.Host);
            Assert.Null(e6.Port);
            Assert.Equal(database, e6.Database);
            Assert.Equal(user, e6.User);
            Assert.Equal(password, e6.Password);

            Assert.True(e7.Valid);
            Assert.False(e7.IsEmpty);
            Assert.Equal(host, e7.Host);
            Assert.Equal(port, e7.Port);
            Assert.Null(e7.Database);
            Assert.Null(e7.User);
            Assert.Equal(password, e7.Password);

            Assert.True(e8.Valid);
            Assert.False(e8.IsEmpty);
            Assert.Equal(host, e8.Host);
            Assert.Null(e8.Port);
            Assert.Equal(database, e8.Database);
            Assert.Null(e8.User);
            Assert.Equal(password, e8.Password);
        }

        [Fact]
        public void CanParseValidEntriesWithEscapedCharacters()
        {
            var config = PgpassConfig.Default;
            config.IgnoreBadConfigLines = false;
            config.UseStaticCache = false;

            const string host = "localhost";
            const int port = 9999;
            const string database = @"my\\data\:base";
            const string user = @"my\:us\\er";
            const string password = @"abc\\\:12#$! has *spaces\:\\-_ye\:ah\\*";

            var e1 = new PgpassEntry(string.Format("{0}:{1}:{2}:{3}:{4}", host, port, database, user, password), config);
            var e2 = new PgpassEntry(string.Format("{0}:{1}:*:{2}:{3}", host, port, user, password), config);

            Assert.Equal(host, e1.Host);
            Assert.Equal(port, e1.Port);
            Assert.Equal(database.PgpassUnescape(), e1.Database);
            Assert.Equal(user.PgpassUnescape(), e1.User);
            Assert.Equal(password.PgpassUnescape(), e1.Password);

            Assert.Equal(host, e2.Host);
            Assert.Equal(port, e2.Port);
            Assert.Null(e2.Database);
            Assert.Equal(user.PgpassUnescape(), e2.User);
            Assert.Equal(password.PgpassUnescape(), e2.Password);
        }

        [Fact]
        public void CanParseCommentsAndEmptyLines()
        {
            var config = PgpassConfig.Default;
            config.IgnoreBadConfigLines = false;
            config.UseStaticCache = false;

            const string host = "localhost";
            const int port = 9999;
            const string database = @"my\\data\:base";
            const string user = @"my\:us\\er";
            const string password = @"abc\\\:12#$! has *spaces\:\\-_ye\:ah\\*";

            var e1 = new PgpassEntry(string.Format("# {0}:{1}:{2}:{3}:{4}", host, port, database, user, password), config);
            var e2 = new PgpassEntry(string.Format(" # {0}:{1}:{2}:{3}:{4}", host, port, database, user, password), config);
            var e3 = new PgpassEntry(string.Format("#{0}:{1}:{2}:{3}:{4}", host, port, database, user, password), config);
            var e4 = new PgpassEntry("", config);
            var e5 = new PgpassEntry("        ", config);

            Assert.True(e1.Valid);
            Assert.True(e1.IsEmpty);
            Assert.True(e2.Valid);
            Assert.True(e2.IsEmpty);
            Assert.True(e3.Valid);
            Assert.True(e3.IsEmpty);
            Assert.True(e4.Valid);
            Assert.True(e4.IsEmpty);
            Assert.True(e5.Valid);
            Assert.True(e5.IsEmpty);
        }

        [Fact]
        public void CanIdentifyInvalidEntries()
        {
            var config = PgpassConfig.Default;
            config.IgnoreBadConfigLines = false;
            config.UseStaticCache = false;

            Assert.Throws<FormatException>(() => new PgpassEntry("too:2:many:chefs:in:the:kitchen", config));
            Assert.Throws<FormatException>(() => new PgpassEntry("host:nonumber:database:user:password", config));
            Assert.Throws<FormatException>(() => new PgpassEntry(@"wrong:9999:escape:sequence:slashed\\\wrong", config));
        }

        [Fact]
        public void ConfigCanIgnoreInvalidEntries()
        {
            var config = PgpassConfig.Default;
            config.IgnoreBadConfigLines = true;
            config.UseStaticCache = false;

            var e1 = new PgpassEntry("too:2:many:chefs:in:the:kitchen", config);
            var e2 = new PgpassEntry("host:nonumber:database:user:password", config);
            var e3 = new PgpassEntry(@"wrong:9999:escape:sequence:slashed\\\wrong", config);

            Assert.False(e1.Valid);
            Assert.False(e2.Valid);
            Assert.False(e3.Valid);
        }
    }
}
