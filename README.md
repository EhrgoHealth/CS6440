[![Build status](https://ci.appveyor.com/api/projects/status/gmt8b2ot3nk5p7e7?svg=true)](https://ci.appveyor.com/project/tparnell8/cs6440)

## Getting started

To get started you must install Visual Studio 2015. You can download the community edition for free.


Make sure you have .NET 4.6 installed, open the visual studio project, and just launch the web project (you can right click on the web project and click debug)

#### VS Extensions

Visual Studio extensions provide code quality, and productivity tools. If you install the [suggested extensions](https://visualstudiogallery.msdn.microsoft.com/3be88243-8bf1-407a-a7ca-a968d0de2d59) extension, VS should prompt you once you open this project with some various extensions that are recommended (web compilers, improved refactors, etc)

If you don't get prompted, just click on the down arrow next to the purple speach bubble at the top of VS, and click `Solution Specific Extensions` then click install
## Testing

You can write unit tests in one of the two unit tests projects. Appveyor will automatically build and run these tests
