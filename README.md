# Beavis CLI

Beavis CLI is a library which enables you to use a web-cli/terminal/console with your ASP.NET Core applications. It is very easy to setup and implement your own custom commands.

## Install

You can get the library from <a href="https://www.nuget.org/packages/Ooaavee.Xxxx">NuGet</a>.

```
PM> Install-Package Ooaavee.BeavisCli
```

## Setup

Add services to the container.

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddBeavisCli();
}
```

Configure HTTP request

```cs
public void Configure(IApplicationBuilder app)
{
    app.UseBeavisCli();
}

```

