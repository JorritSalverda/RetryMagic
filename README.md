# RetryMagic

A c# library to retry code using a truncated binary exponential backoff algorithm

[![Build Status .NET](https://ci.appveyor.com/api/projects/status/github/JorritSalverda/RetryMagic?svg=true)](https://ci.appveyor.com/project/JorritSalverda/RetryMagic/)
[![Version](https://img.shields.io/nuget/v/RetryMagic.svg)](https://www.nuget.org/packages/RetryMagic)
[![License](https://img.shields.io/github/license/JorritSalverda/RetryMagic.svg)](https://github.com/JorritSalverda/RetryMagic/blob/master/LICENSE)

Why?
--------------------------------
To make more robust applications make sure to retry any external call, whether it's a database call or a web request. The truncated [binary exponential backoff](https://en.wikipedia.org/wiki/Exponential_backoff) algorithm is used to avoid congestion and is truncated/capped by default to keep the maximum interval within reason

Usage
--------------------------------
You can retry either an `Action` or a `Func<T>`

```csharp
Retry.Action(() => { _databaseRepository.Update(databaseObject); });
```

```csharp
return Retry.Function(() => { return _databaseRepository.Get(id); });
```

### Changing defaults

The following default settings are used and can be changed by using the following code with different values

```csharp
Retry.UpdateSettings(new RetrySettings(
	jitterSettings: new JitterSettings(percentage: 25), 
	maximumNumberOfAttempts: 5, 
	millisecondsPerSlot: 32, 
	truncateNumberOfSlots: true, 
	maximumNumberOfSlotsWhenTruncated: 16));
```

Validation of settings always takes place during construction of the object so it fails as early as possible.

### Non-static usage

If you wish to be able to inject it - for example for having different settings in different places - you can use the `RetryInstance` class:

```csharp
IRetryInstance instance = new RetryInstance(new RetrySettings(
	jitterSettings:new JitterSettings(percentage: 25), 
	maximumNumberOfAttempts: 5, 
	millisecondsPerSlot: 32, 
	truncateNumberOfSlots: true, 
	maximumNumberOfSlotsWhenTruncated: 16));
```

This interface and class only has the Action and Function methods without the settings parameter besides the `Action` or `Func<T>`, because you provide those during construction.

```csharp
instance.Action(() => { _databaseRepository.Update(databaseObject); });
```

```csharp
return instance.Function(() => { return _databaseRepository.Get(id); });
```

Get it
--------------------------------
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [RetryMagic](https://www.nuget.org/packages/RetryMagic/) from the package manager console:

    PM> Install-Package RetryMagic

RetryMagic is Copyright &copy; 2015 [Jorrit Salverda](http://blog.jorritsalverda.com/) and other contributors under the [MIT license](https://github.com/JorritSalverda/RetryMagic/blob/master/LICENSE).
