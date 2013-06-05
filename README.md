Gendarme.Rules.Xamarin
======================

Gendarme static analysis rules for Xamarin.

##Getting Started

This project currently requires .NET 4.x/Mono 3.x.
A release build of the Gendarme 2.11 and the standard 
rules assemblies are included for convenience.

`cd src`  
`xbuild`

The `Gendarme.Rules.Xamarin.dll` assembly will be output into `bin`.

To run:

`mono bin/gendarme.exe [Some.Assembly.To.Analyze.dll]`

For example, to run only the Gendarme.Rules.Xamarin rules, and have the
report show only `high` and `critical` violations saved to an HTML file on
your Desktop, issue:

`mono gendarme.exe --set xamarin --severity high --html ~/Desktop/Gendarme-Report.html /Users/Demo/Projects/Test/bin/Some.Assembly.To.Analyze.dll`
