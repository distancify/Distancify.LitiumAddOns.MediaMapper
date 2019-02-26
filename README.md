# Distancify.LitiumAddOns.MediaMapper

[![Build status](https://ci.appveyor.com/api/projects/status/acu377nabtjj1fas?svg=true)](https://ci.appveyor.com/project/DistancifyAB/distancify-litiumaddons-mediamapper)

Litium Add-on to programmatically organize media in the media archive and link media to other entities.

This add-on:

- Automatically puts uploaded files in the correct folder
- Automatically link files to any product fields, including your custom ones
- Allows you to map files based on more than just filename, including file content and EXIF data
- Allows you to specify a sort order for a product's display images
- Allows you to update a file's fields/meta data as it is mapped

Requires Litium 7.

## Getting Started

### Prerequisites

This library aims at extending the e-comemrce platform [Litium](https://www.litium.se/). In order to use and develop the project, you need to fulfill their [development system requirements](https://docs.litium.com/documentation/get-started/system-requirements#DevEnv).

### Install

```
Install-Package Distancify.LitiumAddOns.MediaMapper
```

To enable the mapper, add the following app setting in web.config. Note that if you have a multi-node cluster, only enable the mapper on one of your machines to avoid concurrency issues.

```xml
<configuration>
  <appSettings>
    <add key="MediaMapperEnabled" value="true"/>
  </appSettings>
</configuration>
```

#### Logging

This add-on uses the [Serilog framework](https://medium.com/@kristoffer.lindvall/why-you-should-try-out-serilog-4f95b82ea5a0) for logging. Make sure you forward Serilog messages to Litium's logging framework if you're still using that.

### Configure

You need to create the class that is responsible for classifying files by implementing the `IMediaProfiler` interface. This is easily done by inheriting the BaseMediaProfiler class.

```csharp
using Distancify.LitiumAddOns.MediaMapper;
using System.Text.RegularExpressions;
using Litium.Media;
using Litium.FieldFramework;

public class MyMediaProfiler : BaseMediaProfiler
{
    private Regex _imagePattern = new Regex(@"^(?<articleNumber>.+?)\.jpe?g$");

    protected override MediaProfile CreateMediaProfile(File file)
    {
        var match = _imagePattern.Match(file.Name);
        if (match.Success)
        {
            return CreateMediaProfile(file, match.Groups);
        }

        return null;
    }

    public override bool HasMatchingProfile(string fileName)
    {
        return _imagePattern.IsMatch(fileName);
    }

    private MediaProfile CreateMediaProfile(File file, GroupCollection groups)
    {
        var articleNumber = groups["articleNumber"].Value;
        var productIds = new[] { (articleNumber, false) };

        var archivePath = GetArchivePath(articleNumber);

        return new MediaProfile(file, productIds, SystemFieldDefinitionConstants.Images, null, archivePath);
    }

    private string GetArchivePath(string articleNumber)
    {
        return $"{articleNumber.Substring(0, 2)}/{articleNumber}";
    }
}
```

Next, you need to register at least one media mapper in the dependency injection container, using your newly created profiler. Each Mapper instance monitors a folder in the Media Archive module:

```csharp
public class MediaMapperConfig : IComponentInstaller
{
    public void Install(IIoCContainer container, Assembly[] assemblies)
    {
        container.RegisterMediaMapper(new MyMediaProfiler(), "/upload");
    }
}
```

### Mapping to Custom Fields

This add-on comes with the ability to map to Litium's built-in MediaPointerFile and MediaPointerImageArray field types. You can easily add support for your custom fields by implemeting the `IFieldSetter` interface.

### Customized Image Sorting

The `MediaPointerImageArrayFieldSetter` comes with a static configuration that can be used to implement custom sorting logic for when setting the media order to a field. Here's an example:

```csharp
MediaPointerImageArrayFieldSetter.Sort = (entity, field, images) =>
{
    images.Sort((a, b) => string.Compare(a.Name, b.Name));
};
```

## Publishing

The project is built on AppVeyor and set up to automatically push any release branch to NuGet.org. To create a new release, create a new branch using the following convention: `release/v<Major>.<Minor>`. AppVeyor will automatically append the build number.

## Running the tests

The tests are built using xUnit and does not require any setup in order to run inside Visual Studio's standard test runner.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning.

## Authors

See the list of [contributors](https://github.com/distancify/Distancify.LitiumAddOns.MediaMapper/graphs/contributors) who participated in this project.

## License

This project is licensed under the LGPL v3 License - see the [LICENSE](LICENSE) file for details