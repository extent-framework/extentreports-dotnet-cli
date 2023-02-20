[![NuGet](https://img.shields.io/nuget/v/extent.svg)](https://www.nuget.org/packages/extent)
![](https://img.shields.io/github/license/extent-framework/extentreports-csharp.svg)

## Extent .NET CLI

The extentreports-dotnet-cli deprecates [ReportUnit](https://github.com/reportunit/reportunit). Extent Framework is actively maintained and allows using a host of reporters making it very simple to generate test reports. [Klov](http://klov.herokuapp.com/) is the framework's report server and can also be integrated as part of this CLI.

### Example

A sample created from NUnit results.xml available [here](http://extentreports.com/docs/versions/4/net/files/dotnetcli/index.html).

### Screenshots

<img src="http://extentreports.com/docs/versions/4/net-cli/img/net-cli-spark-main.png" width="70%" />
<img src="http://extentreports.com/docs/versions/4/net-cli/img/net-cli-spark-dashboard.png" width="70%" />

### CLI Args

| Arg | Details |
|----|--------------------------------------------------------------------|
| -i | TestRunner results file |
| -d | TestRunner results directory to process multiple files at once |
| -o | Report output directory |
| -r | List of Reporters, html (default)|
| -p | TestRunner [NUnit] |
| --merge | Merge multiple results file into a single report |


### Processing a single file

The default usage creates a HTML report using the default version 4 `ExtentHtmlReporter`:

```
extent -i results/nunit.xml -o results/
```

### Processing multiple files at once

To process multiple files at once, use `-d` to specify the directory where the files are present.

```
extent -d results/ -o results/
```

The command above creates multiple HTML outputs, one for each results file. To combine all results into a single HTML file, use the `--merge` option:

```
extent -d results/ -o results/ --merge
```

### Specifying the reporter to use:

It is possible to specify the reporter or outputs by specifying them using `-r`:

```
extent -i results/nunit.xml -o results/ -r html
```

### License

Apache-2.0
