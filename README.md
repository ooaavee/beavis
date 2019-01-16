# Beavis CLI

Beavis CLI library enables a web-cli support for ASP.NET Core applications. It is very easy to configure and of course develope new custom commands with .NET Core.

## Install

You can get the library from <a href="https://www.nuget.org/packages/Ooaavee.Xxxx">NuGet</a>.

```
PM> Install-Package Ooaavee.BeavisCli
```

## Basic configuration

To configure your ASP.NET Core application to use the Beavis CLI library: Just add services to the container and configure the HTTP request pipeline in the application `Startup` code.

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddBeavisCli();

    ...
}

public void Configure(IApplicationBuilder app)
{
    app.UseBeavisCli();
    
    ...
}
```


That's all, voilà! After this, just start your application and open the address `/terminal` in your web browser and the terminal is shown.

<img src="/doc/images/empty_terminal.jpg" style="width: 100%">


## Develope custom commands

xx
