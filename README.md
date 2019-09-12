# GitScraping
This cross-platform dotnet command-line tool scrapes sensitive data from public repositories. 

## Requirements
To run this tool, DotNet Core is required. Version 2.2 of DotNet Core was used to test this tool.

Download chromedriver version 76 and save it under `/bin/Debug/netcoreapp2.2/`

## Build and Run
To build the project, go to the root folder (which contains .sln file), and run:

`dotnet build`

To run the tool, use:

`dotnet run`

## Usage
To use this tool, the following data is required:
+ Github username (github requires login to search)
+ Github password
+ Search query
+ File name

Example:

`dotnet run --username=USERNAME --password=PASSWORD --query="filename=web.config" --file="web.config"`

## Output
The scraped data is saved under current working directory with as the `file` parameter.
