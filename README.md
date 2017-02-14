# DotOPDS: the lightweight OPDS server

DotOPDS is an [OPDS][1] server designed for large libraries like
full archive of [lib.rus.ec][2] or [Flibusta][3].

## Features

* Full-text search through Lucene.net
* Plugins support
* Support for external converters
* OPDS catalog localization
* Basic authentication support
* Works on Windows (.NET 4.5.2) and Linux (mono 4.2.3) *(not tested on OS X)*
* Cover and annotation extraction *(experimental)*
* Web interface support (not included, see example [here][6])

## Included plugins

* `BookProvider.Inpx` - import `.inpx` files
* `FileFormat.Fb2` - extracts annotation and cover from fb2 books

## Third-party plugins

* [Pdf tree import](https://github.com/GerritV/DotOPDS) - import `.pdf` files with genres tree from filesystem

## Getting started

Download [latest release][4] and extract somewhere.
Now create default configuration file:

    DotOPDS init -c path/to/config

`-c` parameter is optional and can be used with any command.
By default configuration stored in
`%APPDATA%\DotOPDS\default.json` on Windows
and in `$HOME/.config/DotOPDS/default.json` on Linux.

Now edit configuration file if needed:

```js
{
    "port": 8080,
    "title": "DotOPDS Library", // OPDS title
    "language": "en", // OPDS language
    "lazyInfoExtract": false, // should server try to extract cover and annotation from book?
    "database": "%APPDATA%/DotOPDS/database", // path to database storage
    "web": "", // path to web interface files
    "log": {
        "level": "info", // log level
        "enabled": true, // should server write log to files?
        "path": "%APPDATA%/DotOPDS/logs"
    },
    "authentication": {
        "enabled": false, // enable basic authentication
        "attempts": 3, // how many wrong auth attempts before ban. Banned ips stored in banned.json near config file
        "users": {
            "user": "pass"
        }
    },
    "pagination": 10, // how many books per page
    "converters": [
        {
            "from": "fb2", // file extension
            "to": "epub",
            "command": "Fb2ePub",
            "arguments": "{0} {1}" // {0} - from, {1} - to
        }
    ]
}
```

Import library index from `.inpx` file:

    DotOPDS import inpx D:\library D:\lib.inpx

To see available import plugins type:

    DotOPDS import help
    DotOPDS import help inpx # plugin help

Now just start server:

    DotOPDS serve

and OPDS will be available at [http://localhost:8080/opds](http://localhost:8080/opds)

You always can use `help` command to get more info.

### Install as windows service

Use [NSSM][5], Luke!

## TODO

* support for more file formats (epub)

## License

[MIT](LICENSE)

## Preparing release
 * Bump version in `appveyor.yml`
 * Make tag with version number
 * Wait for ci build completed and edit draft description
 * Publish

[1]: https://en.wikipedia.org/wiki/OPDS
[2]: http://lib.rus.ec
[3]: http://flibusta.is
[4]: https://github.com/DeniSix/DotOPDS/releases
[5]: https://nssm.cc
[6]: https://github.com/DeniSix/DotOPDS-web
