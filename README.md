# Pgpass.Net

This is a simple library that builds Postgresql connection strings using your pgpass.conf file.

## Features

- connection string caching
- PGPASSFILE environment variable support
- custom connection string template support

## Installation

You can install the [NuGet package](https://www.nuget.org/packages/Pgpass.Net/) using the package manager console:

    PM> Install-Package Pgpass.Net

## Basic Usage

    var pgpass = new Pgpass("localhost:5432");
    var connString = pgpass.GetConnectionString("mydatabase", "myuser");

## Advanced Usage

### Example 1

    var config = new PgpassConfig
    {
        UseStaticCache = false,
        IgnoreBadConfigLines = true,
        PathEnvironmentVariable = "PGPASSFILE_PROD",
        DefaultConnectionStringTemplate =
            "Server={0};Port={1};Database={2};User={3};Password={4};Timeout=1000;"
    };
    var pgpass = new Pgpass("localhost:5432", config);
    var connString = pgpass.GetConnectionString("mydatabase", "myuser");

### Example 2

    var pgpass = new Pgpass("localhost:5432", @"c:\path\to\pgpass.conf");
    var connString = pgpass.GetConnectionString("mydatabase", "myuser",
        "Server={0};Port={1};Database={2};User={3};Password={4};Timeout=1000;");
