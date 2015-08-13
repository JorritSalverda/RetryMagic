# RetryMagic

A c# library to retry code using a truncated binary exponential backoff algorithm

[![Build Status .NET](https://ci.appveyor.com/api/projects/status/github/JorritSalverda/RetryMagic?svg=true)](https://ci.appveyor.com/project/JorritSalverda/RetryMagic/)
[![Build Status Mono](https://api.travis-ci.org/JorritSalverda/RetryMagic.svg)](https://travis-ci.org/JorritSalverda/RetryMagic/)
[![NuGet downloads](https://img.shields.io/nuget/dt/RetryMagic.svg)](https://www.nuget.org/packages/RetryMagic)
[![Version](https://img.shields.io/nuget/v/RetryMagic.svg)](https://www.nuget.org/packages/RetryMagic)
[![Issues](https://img.shields.io/github/issues/JorritSalverda/RetryMagic.svg)](https://github.com/JorritSalverda/RetryMagic/issues)
[![License](https://img.shields.io/github/license/JorritSalverda/RetryMagic.svg)](https://github.com/JorritSalverda/RetryMagic/blob/master/LICENSE)

Why?
--------------------------------
To make more robust applications make sure to retry any external call, whether it's a database call or a web request. The truncated [binary exponential backoff](https://en.wikipedia.org/wiki/Exponential_backoff) algorithm is used to avoid congestion and is truncated/capped by default to keep the maximum interval within reason

Usage
--------------------------------
You can retry either an Action or a Func<T>

```csharp
Retry.Action(() => { _databaseRepository.Update(databaseObject); });
```

```csharp
return Retry.Function(() => { return _databaseRepository.Get(id); });
```

### Changing defaults

The following defaults are used and can be changed by using the following code with different values

```csharp
Retry.DefaultMaximumNumberOfAttempts = 8;
```

```csharp
Retry.MillisecondsPerSlot = 32;
```

```csharp
Retry.TruncateNumberOfSlots = true;
```

```csharp
Retry.MaximumNumberOfSlotsWhenTruncated = 16;
```

```csharp
Retry.JitterPercentage = 25;

```

### Non-static usage

If you wish to be able to inject it - for example for having different settings in different places - you can use the RetryInstance class:

```csharp
IRetryInstance instance = new RetryInstance(8, 32, true, 16, 25);

This interface and class only has the Action and Function methods without extra parameters besides the Action or Func<T>, because you provide those during construction.

You can retry either an Action or a Func<T>

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
