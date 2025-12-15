# Swesim Flight Form Controls

[![NuGet](https://img.shields.io/nuget/v/SwesimFlightFormControls.svg)](https://www.nuget.org/packages/SwesimFlightFormControls) [![Build Status](<CI_BUILD_BADGE_URL>)](<CI_URL>) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](<LICENSE_URL>)

A concise one-line description of what this library provides.

## Highlights
- Focused UI controls for instrument panels and input calibration.
- First-class support for .NET 8.
- XML docs and Source Link recommended for best IDE experience.

## Supported frameworks
- .NET 8

## Install
Install from NuGet. Run the following command in the Package Manager Console:
```
powershell Install-Package SwesimFlightFormControls
```

## Notes:
- `AxisCalibrationControl` exposes `ObservedMin`, `ObservedMax`, nullable calibration points (`CalMin`, `CalCenter`, `CalMax`), `ResetObserved()`, and `GetNormalizedMinus1ToPlus1(double)`.
- `AdfIndicatorControl` provides the `BearingDegrees` property; set to a 0–360 value and the control repaints.

## API & documentation
- XML docs are included in the package to provide IntelliSense.

## Symbols & Source Link
Symbol package (.snupkg) published to NuGet.org.
