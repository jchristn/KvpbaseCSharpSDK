# Kvpbase C# SDK

[![][nuget-img]][nuget]

[nuget]:     https://www.nuget.org/packages/KvpbaseSDK
[nuget-img]: https://badge.fury.io/nu/Object.svg

Kvpbase is a RESTful object storage platform.  This SDK is intended to help application developers integrate object storage using Kvpbase into their C# applications.
 
## Help or Feedback

First things first - do you need help or have feedback?  Contact me at joel dot christner at gmail dot com or file an issue here!

## Classes

Ensure you have a ```using``` statement for the SDK.
```
using KvpbaseSDK;
```
Then instantiate the ```KvpbaseClient``` class.
```
KvpbaseClient client = new KvpbaseClient(
	"your user GUID",
	"your API key",
	"your endpoint"   // i.e. http://localhost:8001
	);
```
Or...
```
KvpbaseClient client = new KvpbaseClient(
	"your user GUID",
	"your email",
	"your password",
	"your endpoint"   // i.e. http://localhost:8001
	);
```
```KvpbaseClient``` provides core API methods for reading, writing, and other metadata related methods.

If you will be using the SDK for interacting with objects stored on Kvpbase using a stream:
```
KvpbaseStream stream = new KvpbaseStream(
	client, 					// as above
	"your container name",
	"object key"
	);
``` 
From there, you can use ```Stream``` methods such as ```Seek```, ```Read```, ```Write```, and so on.

## Install with NuGet
```
PM> Install-Package KvpbaseSDK
```

## Full Example

Refer to the ```Test``` project for a full example using the API primitives.  For stream examples, refer to ```KvpbaseStreamTest``` (using ```KvpbaseStream```) or ```StreamTest``` (for uploading/downloading files to/from your filesystem).

