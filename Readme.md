![](https://img.shields.io/github/license/extent-framework/extentreports-csharp.svg)

## Extent .NET CLI

The extentreports-dotnet-cli deprecates [ReportUnit](https://github.com/reportunit/reportunit). Extent Framework is actively maintained and allows using a host of reporters making it very simple to generate test reports. [Klov](http://klov.herokuapp.com/) is the framework's report server and can also be integrated as part of this CLI.

### Example

A sample created from NUnit results.xml available [here](http://extentreports.com/docs/versions/4/net/files/dotnetcli/index.html).

### CLI Args

| Arg | Details |
|----|--------------------------------------------------------------------|
| -i | TestRunner results file |
| -d | TestRunner results directory to process multiple files at once |
| -o | Report output directory |
| -r | List of Reporters [html, v3html], html (default)|
| -p | TestRunner [NUnit] |


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

### Specifying the reporter to use:

It is possible to specify the reporter or outputs by specifying them using `-r`:

```
extent -i results/nunit.xml -o results/ -r v3html
```

Do you use `html` and `v3html` reporters at once as this may result in clashes.

### License

Apache-2.0
