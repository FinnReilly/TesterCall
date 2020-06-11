# TesterCall
------------
## A Powershell Module for automation testing of OpenApi 3 (Swagger) RESTful APIs

TesterCall is a Powershell module for the import and automated testing of OpenApi specifications.

It is designed to be a cross-platform tool for use with either Powershell 5 or Powershell 7 (Core)
and as such has been built on the portable .Net standard framework.

## Setting up

### Importing the module

In order to use TesterCall, you will need to place the following dll files from this
project into your module directory (currently located in bin/debug/netstandard2.0):

* TesterCall.dll
* Newtonsoft.Json.dll
* YamlDotNet.dll

You should then run Import-Module from Powershell as follows:

	Import-Module *Path to Module Directory*/TesterCall.dll

### Creating a Test Environment

The test environment will provide a Base Url for testing Api endpoints against and is
created using New-TestEnvironment. This requires a Host name and protocol (Http|Https)
and can either be stored in a variable or set as the default environment by using the -Default
flag:

	New-TestEnvironment -Protocol HTTPS -Host localhost:5001 -Default

The default environment will be used by commands such as Invoke-Endpoint when a test environment
is not supplied, and can be viewed by using `Get-DefaultTestEnvironment`

### Importing a spec

To import all the endpoints specified in an OpenApi 3 json or yaml file, run 
Import-OpenApiSpecification:

	Import-OpenApiSpecification -Path *Path to Yaml/Json spec file*

This command will return all Endpoint objects to the Powershell pipeline as well
as storing them in memory.  If you don't need them immediately and don't want them
in your console output, you can do this without losing the information:

	Import-OpenApiSpecification -Path *File path* | Out-Null

Endpoints can be retrieved later using `Get-Endpoint` (see below)

### Configuring Authorisation Strategies

TesterCall offers multiple commands for creating an Authorisation strategy which
can be used to populate the Authorisation header used when calling Endpoints - these
straegy objects should be stored in a variable and passed as a parameter to `Invoke-Endpoint`.
These are as follows:

* New-Oauth2ClientCredentials
* New-Oauth2PasswordCredentials
* New-BasicAuthCredentials
* New-BearerToken

The details of these commands can be found by running `Get-Help *Command Name*`
