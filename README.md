# Tretton37 Crawler

Simple web crawler.

## Features and Highlights
 - Runs in parallel.
 - No third party dependencies.
 - There is a default file handler and download folder can be specified.
 - It can be specified whether to clean the download folder before scanning.
 - Different resource handler can be specified.
 - Summary after scanning ends. (e.g. total downloaded url count and total size)
 - Logs before and after some specific actions (e.g. downloading, saving etc.)
 - Written as a library so it can be used in different type of projects.

## Limitations
 - Not well tested outside of https://tretton37.com
 - Resources have no path editing to work properly offline.
 - Scans only urls for the specified domain.
 - Not extensively unit tested.

## Requirements
- .NET SDK 6.0 or newer (https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Quick Start
__Windows & Unix/GNU Linux__

By default the crawler runs for https://tretton37.com
You can change this domain by changing 'Domain' property in src/Tretton37Crawler.ConsoleApp/appsettings.json

To run console application you can run the following command on the root folder:
```
$ dotnet run --project src/Tretton37Crawler.ConsoleApp
```

To run tests you can run the following command on the root folder:
```
$ dotnet test
```