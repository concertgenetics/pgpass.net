namespace Pgpass.Net
{
    public class PgpassConfig
    {
        public static PgpassConfig Default
        {
            get
            {
                return new PgpassConfig
                {
                    UseStaticCache = true,
                    AllowMissingConfig = false,
                    IgnoreBadConfigLines = false,
                    PathEnvironmentVariable = "PGPASSFILE",
                    DefaultConnectionStringTemplate = "Host={0};Port={1};Database={2};Username={3};Password={4};"
                };
            }
        }

        public bool UseStaticCache { get; set; }
        public bool AllowMissingConfig { get; set; }
        public bool IgnoreBadConfigLines { get; set; }
        public string PathEnvironmentVariable { get; set; }
        public string DefaultConnectionStringTemplate { get; set; }
    }
}
