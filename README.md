# Beavis CLI

Beavis CLI library adds a web-cli (terminal in web browser) support for ASP.NET Core applications. It's very easy to configure and develope new commands with .NET Core.

## Install

You can get the library from <a href="https://www.nuget.org/packages/BeavisCLI/">NuGet</a>.

```
PM> Install-Package BeavisCLI -Version 0.9.6-beta1
```
or
```
> dotnet add package BeavisCLI --version 0.9.6-beta1
```


## Basic configuration

To configure your ASP.NET Core application to use the Beavis CLI library: Just add services to the container and configure the HTTP request pipeline in the application `Startup` code.

```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBeavisCli();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseBeavisCli();
    }
}
```


That's all, voilà! After this, just start your application and open the address `/terminal` in your web browser and the terminal will open.

<img src="/doc/images/empty_terminal.jpg" style="width: 100%">


## Developing commands

By the default, there is a small set of built-in commands available in the Beavis CLI library (you can see them by executing the `help` command on the terminal), but if you want to get much out of this library, then you should develop your own custom commands - luckily it's a pretty straightforward process.

First you create a new class that implements the `ICommand` interface and use the `CommandAttribute` to give a name and a description for your command. 

```cs
using System.Threading.Tasks;
using BeavisCli;

[Command("hello", "This demo say Hello World!")]
public class Hello : ICommand
{
    public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
    {
        await context.OnExecuteAsync(async () =>
        {
            context.WriteText("Hello World");
            return await context.ExitAsync();
        });
    }
}
```

...and register `ICommand` service with the concrete type `Hello`.

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddBeavisCli();
    services.AddScoped<ICommand, Hello>();
}

```

<img src="/doc/images/hello_terminal.jpg" style="width: 100%">


...to be continued :)
