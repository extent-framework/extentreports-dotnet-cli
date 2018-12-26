![](https://img.shields.io/github/license/extent-framework/extentreports-csharp.svg)

## Extent .NET CLI

The extentreports-dotnet-cli deprecates [ReportUnit](https://github.com/reportunit/reportunit). Extent Framework is actively maintained and allows using a host of reporters making it very simple to generate test reports. [Klov](http://klov.herokuapp.com/) is the framework's report server and can also be integrated as part of this CLI.

### Example

A sample created from NUnit results.xml available [here](http://extentreports.com/docs/versions/4/net/files/dotnetcli/index.html).

### CLI Args

| Arg | Details |
|----|--------------------------------------------------------------------|
| -i | TestRunner results file |
| -o | Report output directory |
| -r | List of Reporters (avent, bdd, cards, tabular, html, v3html, klov) |
| -p | TestRunner (NUnit) |


### Usage

The default usage creates a HTML report using the default version 4 `ExtentHtmlReporter`:

```
extent -i result.xml -o reports/
```

It is possible to specify the reporters or outputs by specifying them using `-r`:

```
extent i- results.xml -o reports/ -r html -r klov -r v3html
```


### License

Apache-2.0
