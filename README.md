# RetryMagic

<<<<<<< HEAD
What is RetryMagic?
--------------------------------
A c# library to retry code with a truncated binary exponential backoff algorithm
=======
A c# library to retry code using a truncated binary exponential backoff algorithm
>>>>>>> 8b4e149b7f68521865111ea9bc19fc26cbfaf21f

[![Build Status](https://img.shields.io/appveyor/ci/JorritSalverda/RetryMagic.svg)](https://ci.appveyor.com/project/JorritSalverda/RetryMagic/)
[![NuGet downloads](https://img.shields.io/nuget/dt/RetryMagic.svg)](https://www.nuget.org/packages/RetryMagic)
[![Version](https://img.shields.io/nuget/v/RetryMagic.svg)](https://www.nuget.org/packages/RetryMagic)
[![Issues](https://img.shields.io/github/issues/JorritSalverda/RetryMagic.svg)](https://github.com/JorritSalverda/JitterMagic/issues)
[![License](https://img.shields.io/github/license/JorritSalverda/RetryMagic.svg)](https://github.com/JorritSalverda/RetryMagic/blob/master/LICENSE)

Why?
--------------------------------
<<<<<<< HEAD
To make more robust applications make sure to retry any external call, whether it's a database call or a web request. The truncated binary exponential backoff algorithm is used to avoid congestion and is truncated/capped by default to keep the maximum interval within reason
=======
To make more robust applications make sure to retry any external call, whether it's a database call or a web request. The truncated [binary exponential backoff](https://en.wikipedia.org/wiki/Exponential_backoff) algorithm is used to avoid congestion and is truncated/capped by default to keep the maximum interval within reason
>>>>>>> 8b4e149b7f68521865111ea9bc19fc26cbfaf21f

Usage
--------------------------------
You can retry either an Action or a Func<T>

```csharp
Retry.Action(() => { _databaseRepository.Update(databaseObject); });
```

```csharp
<<<<<<< HEAD
Retry.Function(() => { return _databaseRepository.Get(id); });
=======
return Retry.Function(() => { return _databaseRepository.Get(id); });
>>>>>>> 8b4e149b7f68521865111ea9bc19fc26cbfaf21f
```

### Changing defaults

The following defaults are used and can be changed by using the following code with different values

```csharp
Retry.DefaultMaximumNumberOfAttempts = 8;
```

```csharp
Retry.MilliSecondsPerSlot = 32;
```

```csharp
Retry.Truncate = true;
```

```csharp
Retry.MaximumNumberOfSlotsWhenTruncated = 16;
```

Get it
--------------------------------
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [RetryMagic](https://www.nuget.org/packages/RetryMagic/) from the package manager console:

    PM> Install-Package RetryMagic

<<<<<<< HEAD
RetryMagic is Copyright &copy; 2015 [Jorrit Salverda](http://blog.jorritsalverda.com/) and other contributors under the [MIT license](https://github.com/JorritSalverda/RetryMagic/blob/master/LICENSE).
=======
RetryMagic is Copyright &copy; 2015 [Jorrit Salverda](http://blog.jorritsalverda.com/) and other contributors under the [MIT license](https://github.com/JorritSalverda/RetryMagic/blob/master/LICENSE).
>>>>>>> 8b4e149b7f68521865111ea9bc19fc26cbfaf21f
