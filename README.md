# Limbo Signatur

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/limbo-works/Limbo.Umbraco.Signatur/blob/v1/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/vpre/Limbo.Umbraco.Signatur.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Signatur)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.Signatur.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Signatur)
<!--[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.signatur)-->

**Limbo.Umbraco.Signatur** is a package that integrates with [**Signatur**](https://www.signatur.dk/) RSS job feeds, and allows importing jobs as content in Umbraco.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/limbo-works/Limbo.Umbraco.Signatur/blob/v10/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>
      Umbraco 10, 11 and 12
    </td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>
      .NET 6
    </td>
  </tr>
</table>






<br /><br />

## Installation

**Umbraco 10+**  

Version 10 of this package supports Umbraco version 10, 11, and 12. The package is only available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.Signatur).

To install the package, you can use either the .NET CLI:

```
dotnet add package Limbo.Umbraco.Signatur --version 10.0.0-alpha005
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.Signatur -Version 10.0.0-alpha005
```




<br /><br /><br />

## Configuration

The package can be configured via appsettings and the `Limbo:Signatur` section. This primarily let's you set up the Signatur RSS feeds to be imported, but there is also a few other settings as well:

```json
{
  "Limbo": {
    "Signatur": {
      "ImportUserId": -1,
      "LogResults": true,
      "Feeds": [
        {
          "Url": "https://portal.signatur.dk/ExtJobs/Rss.aspx?ClientId=0000",
          "ParentContentKey": "a8c8f679-63bb-4b5b-a52a-19f6117344c7",
          "ContentTypeAlias": "mySignaturJobPage"
        }
      ],
      "Scheduling": {
        "Delay": 7,
        "Interval": "PT1H30M"
      }
    }
  }
}
```

The available configuration options are as following:

- `ImportUserId` (int)  
By the default, actions (save, publish and delete) made by the import will be attributed to the default super user in Umbraco (ID: `-1`). If you wish to attribute these actions to another user, you can specify it's numeric ID via this setting.

- `LogResults` (bool)  
*Currently not used.*

- `Feeds` (array)  
An array of the feeds to be imported.

  - `Url` (string)  
  The URL of the Signatur RSS feed.

  - `ParentContentKey` (guid)  
  The GUID key of the parent code node under which new jobs should be created.

  - `ContentTypeAlias` (string)  
  The alias of the content type which should be used for new jobs.

- `Scheduling` (object)  
The package adds a background task for running the import. That task may be configured through this object.

  - `Delay` (duration)  
  An initial delay after startup the task should wait before beginning the import. Default is five minutes.

  - `Interval` (duration)  
  The interval between each time the task is running the import. Default is one hour.

Durations may be specified as an integer, in which case the value is treated as minutes - or as a string using the ISO 8601 duration format such as `PT1H30M` (one and a half hour).









<br /><br /><br />

## Property Editors

- **Limbo Signatur Job ID** (alias: `Limbo.Umbraco.Signatur.JobId`)
Dedeicated property editor for storing the ID of each job. This is used by the import to keep track of which jobs to add, update or delete, so your job content type must have a property using this property editor. Returns an `int` value.

- **Limbo Signatur Job Data** (alias: `Limbo.Umbraco.Signatur.JobData`)
Dedicated property editor for storing the job data as received from Signatur. Your job content type must have a property using this property editor. Returns an `ISignaturItem` value by default.

- **Limbo Signatur Last Updated** (alias: `Limbo.Umbraco.Signatur.LastUpdated`)
Property editor for storing when the import last updated a job in Umbraco. The property editor is optional for the job content type. Returns an `EssentialsTime` value.







<br /><br /><br />

## Extending

The package contains several extension points that let's you extend and control how the package and the import is working.



### Custom Properties

Jobs in the Signatur RSS feeds contain a number of default fields, and then typically also some client/feed specific. The parsing of the RSS feed is handled by our underlying [**Limbo.Integrations.Signatur** package](https://github.com/limbo-works/Limbo.Integrations.Signatur), which features `ISignaturFeedParser` service that you may replace with your own parser.

By default each RSS item is parsed and returned as `ISignaturItem`, which contains properties for the default fields. If you wish to support custom fields, you can create your own class that implements the `ISignaturItem` interface (or the `SignaturItem` concrete class):

```csharp
public class MySignaturItem : SignaturItem {

    public string? Location { get; }

    public string? Teaser { get; }

    public MySignaturItem(SyndicationItem item, Dictionary<string, string> fields) : base(item, fields) {
        Location = GetString("companyInformation");
        Teaser = GetString("teaser");
    }

}
```

and then a matching feed class:

```csharp
public class MySignaturFeed : SignaturFeed<MySignaturItem> {

    public MySignaturFeed(SyndicationFeed feed, IReadOnlyList<MySignaturItem> items) : base(feed, items) { }

}
```

This ensures a strongly typed approach to working with the feed and it's items - also supporting the custom fields handled by the `MySignaturItem` class.

With these to classes in place, you can now set up the custom feed parser, could look like:

```csharp
public class MySignaturFeedParser : SignaturFeedParser<MySignaturFeed, MySignaturItem> {

    public override MySignaturFeed ParseFeed(SyndicationFeed feed) {
        return new MySignaturFeed(feed, ParseItems(feed));
    }

    protected override MySignaturItem ParseItem(SyndicationItem item, Dictionary<string, string> fields) {
        return new MySignaturItem(item, fields);
    }

}
```

and then add a composer to replace the default implementation of `ISignaturFeedParser`:

```csharp
[ComposeAfter(typeof(SignaturComposer))]
public class MySignaturComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<ISignaturFeedParser, RaSignaturFeedParser>();
    }

}
```

Notice that your `MySignaturComposer` should run after the package's `SignaturComposer` to ensure that things are set up in the correct order.



<br /><br />

### SignaturJobsService

The package contains the `SignaturJobsService` class, which is responsible for the import. The class contains a number of virtual methods than can be overridden by creating a custom class extending `SignaturJobsService`. Eg. if the RSS items contain a custom teaser field, the package doesn't know what to do with this out of the box. But by overriding the `UpdateProperties` method, you can set the corresponding `teaser´ property in Umbraco:

```csharp
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Models.Import;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace UmbracoTen.Packages.Signatur;

public class MySignaturJobsService : SignaturJobsService {

    public MySignaturJobsService(IOptions<SignaturSettings> settings, IWebHostEnvironment webHostEnvironment, IContentTypeService contentTypeService, IContentService contentService, ISignaturFeedParser signaturFeedParser) : base(settings, webHostEnvironment, contentTypeService, contentService, signaturFeedParser) { }

    protected override bool UpdateProperties(ISignaturItem item, IContent content, ImportTask task, SignaturImportJobsSettings settings, bool isNew) {

        // Call the base method so the package does what it should by default
        bool modified = base.UpdateProperties(item, content, task, settings, isNew);

        // Get the old and new teaser values
        string? oldTeaser = isNew ? null : content.GetValue<string>("teaser");
        string? newTeaser = item is MySignaturItem my ? my.Teaser : null;
        SetValueIfModified(content, "teaser", oldTeaser, newTeaser, ref modified);

        // Return whether the content item properties were modified
        return modified;

    }

}
```


<br /><br />

### SignaturModelFactory

By default properties using the **Limbo Signatur Job Data** property editor will return a `ISignaturItem` value. If you want it to return something else, you can replace the `SignaturModelFactory` factory with your own implementation - eg. if you've implemented a custom feed converter, you may wan't to return your concrete type instead:

```csharp
public class MySignaturModelFactory : SignaturModelFactory {

    public MySignaturModelFactory(ISignaturFeedParser signaturFeedParser) : base(signaturFeedParser) { }

    public override Type GetJobDataValueType(IPublishedPropertyType propertyType) {
        return typeof(MySignaturItem);
    }

    public override object ConvertJobData(IPublishedElement owner, IPublishedPropertyType propertyType, ISignaturItem item) {
        if (item is not MySignaturItem my) throw new ComputerSaysNoException("This shouldn't happen!!!");
        return my;
    }

}
```

In this case, `MySignaturItem` implements the `ISignaturItem` interface, but you could also return your own type that is based on `ISignaturItem` or `MySignaturItem` instead.
