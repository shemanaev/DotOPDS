# DotOPDS: the lightweight OPDS server

DotOPDS is an [OPDS][1] server designed for large libraries like
full archive of [lib.rus.ec][2] or [Flibusta][3].

## Features

* Full-text search through Lucene.net
* Plugins support
* Support for external converters
* OPDS catalog localization
* Cover and annotation extraction *(experimental)*
* Web interface

## Getting started

Download [latest release][4] and extract somewhere.
Copy `appsettings.json` to `appsettings.Production.json` and customize it or use any other method to configure [ASP.NET Core Application][6]

Settings description:

```js
{
  "Presentation": {
    "DefaultLanguage": "en", // language used to index texts
    "PageSize": 10, // how many items per page to show
    "Title": "DotOPDS Library", // OPDS title
    "LazyInfoExtract": false, // try to extract cover and annotation from book before displaying
    "Converters": [
      {
        "From": "fb2", // file extension
        "To": "epub", // file extension
        "Command": "fb2epub.exe", // path to converter
        "Arguments": "{0} {1}" // {0} - from, {1} - to
      }
    ],
  },
  "IndexStorage": {
    "Path": "./database" // path to database storage
  }
}
```

Import library index from `.inpx` file:

```shell
DotOPDS import inpx C:\library C:\lib.inpx
```

To see available import plugins type:

```shell
DotOPDS import help
DotOPDS import help inpx # plugin help
```

Now just start server:

```shell
DotOPDS.Server
```

and OPDS will be available at [http://localhost:5000/opds](http://localhost:5000/opds)

You always can use `help` command to get more info.

## Docker

### Build the image

```shell
docker build . -t dotopds
```

### Run the image

```shell
docker run -it -p 5000:80 -v databasePath:/app/database dotopds
```

### Manage data in docker

Find container id with `docker ps` and get into it

```shell
docker exec -it CONTAINER_ID /bin/bash
```

## Included plugins

* `BookProvider.Inpx` - import `.inpx` files
* `FileFormat.Fb2` - extract annotation and cover from fb2 books

## Preparing release

* Bump version in `appveyor.yml`
* Make tag with version number
* Wait for ci build completed and edit draft description
* Publish

[1]: https://en.wikipedia.org/wiki/OPDS
[2]: http://lib.rus.ec
[3]: http://flibusta.is
[4]: https://github.com/shemanaev/DotOPDS/releases
[5]: https://nssm.cc
[6]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#configuration-providers
