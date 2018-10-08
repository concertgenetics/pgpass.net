namespace Pgpass.Net.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Xunit;

    public class PgpassTests
    {
        private string BaseDir
        {
            get { return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath); }
        }

        private string ConfigPath
        {
            get { return Path.Combine(BaseDir, "pgpass.conf"); }
        }

        private string ExpectedConnectionString(string host, int port, string database, string user, string password)
        {
            return string.Format(PgpassConfig.Default.DefaultConnectionStringTemplate, host, port, database, user,
                password);
        }

        [Fact]
        public void CanMatchAllValues()
        {
            var pgpass = new Pgpass("localhost", ConfigPath);
            var str = pgpass.GetConnectionString("mydatabase", "myuser");
            Assert.Equal(ExpectedConnectionString("localhost", 5432, "mydatabase", "myuser", "password1"), str);
        }

        [Fact]
        public void CanMatchWithoutDatabase()
        {
            var pgpass = new Pgpass("localhost", ConfigPath);
            var str = pgpass.GetConnectionString("notinfile", "myuser");
            Assert.Equal(ExpectedConnectionString("localhost", 5432, "notinfile", "myuser", "password2"), str);
        }

        [Fact]
        public void CanMatchWithoutPort()
        {
            var pgpass = new Pgpass("localhost:9999", ConfigPath);
            var str = pgpass.GetConnectionString("mydatabase", "myuser");
            Assert.Equal(ExpectedConnectionString("localhost", 9999, "mydatabase", "myuser", "password3"), str);
        }

        [Fact]
        public void CanMatchWithoutUser()
        {
            var pgpass = new Pgpass("localhost", ConfigPath);
            var str = pgpass.GetConnectionString("mydatabase", "notinfile");
            Assert.Equal(ExpectedConnectionString("localhost", 5432, "mydatabase", "notinfile", "password4"), str);
        }

        [Fact]
        public void CanMatchWithoutPortAndDatabase()
        {
            var pgpass = new Pgpass("localhost:9999", ConfigPath);
            var str = pgpass.GetConnectionString("notinfile", "myuser");
            Assert.Equal(ExpectedConnectionString("localhost", 9999, "notinfile", "myuser", "password5"), str);
        }

        [Fact]
        public void CanMatchWithoutPortAndDatabaseAndUser()
        {
            var pgpass = new Pgpass("localhost:9999", ConfigPath);
            var str = pgpass.GetConnectionString("notinfile1", "notinfile2");
            Assert.Equal(ExpectedConnectionString("localhost", 9999, "notinfile1", "notinfile2", "password6"), str);
        }

        [Fact]
        public void CanMatchWithoutHostAndPort()
        {
            var pgpass = new Pgpass("otherserver.local:9999", ConfigPath);
            var str = pgpass.GetConnectionString("mydatabase", "myuser");
            Assert.Equal(ExpectedConnectionString("otherserver.local", 9999, "mydatabase", "myuser", "password7"), str);
        }

        [Fact]
        public void CanMatchWithoutHostAndDatabase()
        {
            var pgpass = new Pgpass("otherserver.local:3333", ConfigPath);
            var str = pgpass.GetConnectionString("notinfile", "myuser");
            Assert.Equal(ExpectedConnectionString("otherserver.local", 3333, "notinfile", "myuser", "password8"), str);
        }

        [Fact]
        public void CanMatchWithoutHostAndDatabaseAndUser()
        {
            var pgpass = new Pgpass("otherserver.local:3333", ConfigPath);
            var str = pgpass.GetConnectionString("notinfile1", "notinfile2");
            Assert.Equal(ExpectedConnectionString("otherserver.local", 3333, "notinfile1", "notinfile2", "password9"), str);
        }

        [Fact]
        public void CanMatchWildcardEntry()
        {
            var pgpass = new Pgpass("otherserver.local:9999", ConfigPath);
            var str = pgpass.GetConnectionString("notinfile1", "notinfile2");
            Assert.Equal(ExpectedConnectionString("otherserver.local", 9999, "notinfile1", "notinfile2", "password10"), str);
        }

        [Fact]
        public void ReturnsNullOnNoMatch()
        {
            var path = Path.Combine(BaseDir, "pgpass_nocatchall.conf");
            var pgpass = new Pgpass("otherserver.local:9999", path);
            var str = pgpass.GetConnectionString("notinfile1", "notinfile2");
            Assert.Null(str);
        }
    }
}
