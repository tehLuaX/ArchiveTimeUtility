# Archive Time Utility

Archive Time Utility (ATU) is a simple command line program that stores the creation, modification and access times of all files and directories in a specified directory so that you can restore the file and directory timestamps after making a backup.

Certain archive utilities that are used for backup are unable to store the creation time of files. Archive Time Utility solves that problem by saving (before archiving) timestamps and then allowing you to overwrite the creation, modification and/or access timestamps of the extracted files.

This software is FOSS (free and open source software) and was written using C# and was currently only tested on Windows.

## Disclaimer

WARNING! By running this program with the store job, you acknowledge that it may overwrite the creation, modification and access time of the provided files and/or directories. After running the store job, **there is no going back**.

## Running ATU

### Standalone build

There is only one standalone build available, and it's for Windows (x86). Just download the standalone version from the releases tab on GitHub and run the `.exe` file.

### Framework-dependent build

In order to run framework-dependent builds, just run the software with the following command: `dotnet ArchiveTimeUtility.dll`.

## Contributing

If you'd like to contribute to this project, please check [CONTRIBUTING.md](CONTRIBUTING.md) for more info.