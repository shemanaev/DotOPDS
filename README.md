# DotOPDS: the lightweight* OPDS server

**This is a WIP project.**

DotOPDS is an [OPDS][1] server designed for large libraries like
full archive of [lib.rus.ec][2] or [Flibusta][3].

_* maybe not so :wink: it consumes up to 100mb RAM on my typical usage
with no more 10 users_

## Features

* Full-text search through Lucene.net
* Importing `.inpx` index files
* Support for external converters
* OPDS catalog localization
* Basic authentication support
* Works on Windows (.NET 4.5.2) and Linux (mono 4.2.3) *(not tested on OS X)*
* Cover and annotation extraction *(experimental)*
* Web interface support (not included)

## Limitations

* Works only with archived `fb2` libraries for now

## Getting started

Download [latest build][4] from CI server, extract somewhere.
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
        "attempts": 3, // how many wrong auth attempts before ban
        "users": {
            "user": "pass"
        },
        "banned": [] // banned ips will be here
    },
    "pagination": 10, // how many books per page
    "converters": [
        {
            "from": "fb2", // file extension
            "to": "epub",
            "command": "Fb2ePub {0} {1}" // {0} - from, {1} - to
        }
    ]
}
```

Import library index:

    DotOPDS import D:\library lib.inpx

Now just start server:

    DotOPDS serve

and OPDS will be available at [http://localhost:8080/opds](http://localhost:8080/opds)

You always can use `help` command to get more info.

### Install as windows service

Use [NSSM][5], Luke!

## TODO

* producing `.inpx` diff for index updates
* support for more file formats (epub)
* support for not archived libraries

## License

[MIT](LICENSE)

[1]: https://en.wikipedia.org/wiki/OPDS
[2]: http://lib.rus.ec
[3]: http://flibusta.is
[4]: https://ci.appveyor.com/api/projects/nis/dotopds/artifacts/DotOPDS/bin/Release/DotOPDS.exe
[5]: https://nssm.cc
