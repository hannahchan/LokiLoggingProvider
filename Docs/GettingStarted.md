# Getting Started

If you are intending to help develop and contribute to the Loki Logging Provider package, this guide is meant to help you setup your development environment.

## Local Development

The local development experience is what most developers will be familiar with. This is where you clone and work off a copy of a repository on your local machine using the tools that you have installed. To get started with local development, you will need to have the following installed on your local machine;

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/engine/install)
- [Docker Compose](https://docs.docker.com/compose/install)
- [Visual Studio Code](https://code.visualstudio.com) (Recommended)

## Remote Development

This solution includes configuration which you can use with [Visual Studio Code Remote - Containers](https://code.visualstudio.com/docs/remote/containers) or [GitHub Codespaces](https://code.visualstudio.com/docs/remote/codespaces) to spin up a remote development environment. This is ideal for those who want to;

- Use a sandboxed development environment.
- Get started faster with a consistent development environment.

## Building and Testing the Package

Once you have cloned the repository but before you start working on it, it is always a good idea to make sure that you are able to successfully build the solution and run the tests.

From the root of the solution, to build the package run;

    dotnet build

Likewise to run the tests, run;

    dotnet test

### Creating a Release

To help different developers build and test the package deterministically on different machines, a [Cake script](../build.cake) has been included at the root of this solution. To run the script, run the command;

    dotnet cake

You may need to run `dotnet tool restore` first if you don't have the dotnet tool `Cake.Tool` installed. By default, the Cake script will build, test and publish the package and copy all related artifacts into the _artifacts_ directory. This directory is where you will find your release ready build output and test coverage reports.

For more information about Cake, please visit <https://cakebuild.net>.

## Running and Debugging the Package

During development of the Loki Logging Provider, you may wish to experience how the library actually works. Included in the solution is a [`docker-compose.yml`](../docker-compose.yml) file which you can use to spin up an instance Grafana and Loki. Once spun up you can use the two included example applications which reference the Loki Logging Provider package to send logs to Loki.

To spin up a Grafana and Loki instance, run;

    docker-compose up -d

Run one of the example applicatons to send logs to Loki;

- [Example.ConsoleApp](../Sources/Example.ConsoleApp)
- [Example.WebApp](../Sources/Example.WebApp)

In a web browser, load up Grafana and view the logs that were sent at;

    http://localhost:3000

To stop the Grafana and Loki instance, run;

    docker-compose down
