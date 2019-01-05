# Distancify.LitiumAddOns.MediaMapper

[![Build status](https://ci.appveyor.com/api/projects/status/acu377nabtjj1fas?svg=true)](https://ci.appveyor.com/project/KristofferLindvall/distancify-litiumaddons-mediamapper)

Litium Add-on to programmatically organize media in the media archive and link media to other entities.

This add-on:

- Automatically puts uploaded files in the correct folder
- Automatically link files to any product fields, including your custom ones
- Allows you to specify a sort order for a product's display images
- Allows you to update a file's fields/meta data as it is mapped

Requires Litium 7.

## Getting Started

### Prerequisites

This library aims at extending the e-comemrce platform [Litium](https://www.litium.se/). In order to use and develop the project, you need to fulfill their [development system requirements](https://docs.litium.com/documentation/get-started/system-requirements#DevEnv).

### Install

Install the add-on by adding the NuGet package to your project. Then add the scheduled task needed to perform the actual mapping. Note that if you have a multi-node cluster, run this task on one of your machines, or your application server.

```xml
<scheduledTask type="Distancify.LitiumAddOns.MediaMapper.ScheduledTask, Distancify.LitiumAddOns.MediaMapper" startTime="00:05" interval="5m" />
```

#### Logging

This add-on uses the [Serilog framework](https://medium.com/@kristoffer.lindvall/why-you-should-try-out-serilog-4f95b82ea5a0) for logging. Make sure you forward Serilog messages to Litium's logging framework if you're still using that.

### Configure

You need to create the class that is responsible for classifying files by implementing the `IMediaProfiler` interface. This is easily done by inheriting the BaseMediaProfiler class. You don't need to register your class with Litiums Dependency Injector. This is handled automatically for you.

```csharp
public class MyMediaProfiler : BaseMediaProfiler
{
    private Regex _imagePattern = new Regex(@"^(?<articleNumber>.+?)\.jpeg$");

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

Next, you need to register at least one media mapper in the dependency injection container, using your newly created profiler:

```csharp
public class MediaMapperInstaller : IComponentInstaller
{
    public void Install(IIoCContainer container, Assembly[] assemblies)
    {
        container.RegisterMediaMapper(new MyMediaProfiler(), "/upload");
    }
}
```

### Maping to Custom Fields

This add-on comes with the ability to map to Litium's built-in MediaPointerFile and MediaPointerImageArray field types. You can easily add support for your custom fields by implemeting the `IFieldSetter<>` interface.

### Customized Image Sorting

By decorating the built-in MediaPointerImageArrayFieldMediaSetter class, we can implement custom sorting rules for images mapped to products. Here's an example:

```csharp

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