# kvpbase C# SDK

[![][nuget-img]][nuget]

[nuget]:     https://www.nuget.org/packages/KvpbaseSDK
[nuget-img]: https://badge.fury.io/nu/Object.svg

kvpbase is a RESTful object storage platform.  This SDK is intended to help application developers integrate object storage using kvpbase into their C# applications.

## help or feedback
first things first - do you need help or have feedback?  Contact me at joel at maraudersoftware.com dot com or file an issue here!

## install with NuGet
```
PM> Install-Package kvpbase-sdk-csharp
```

## simple example
Initialize the SDK and define commonly-used variables
```
using KvpbaseSDK;

// User GUID, API Key, Endpoint
Client kvp = new Client("default", "default", "http://localhost:8080"); 
```

### Create an object
```
if (!kvp.CreateObjectWithoutName(
   "/path/to/container", 
   "text/plain", 
   Encoding.UTF8.GetBytes("Hello!"), 
   out url)) 
{ // handle errors }
else 
{ Console.WriteLine("Stored at: " + url); }

if (!kvp.CreateObject(
   "/path/to/container",
   "hello.txt",
   "text/plain",
   Encoding.UTF8.GetBytes("Hello!"),
   out url))
{ // handle errors }
else
{ Console.WriteLine("Stored at: " + url); }
```

### Retrieve an object
```
if (!kvp.GetObject(
   "/path/to/container/hello.txt",
   out data))
{ // handle errors }
else
{ Console.WriteLine("Data: " + Encoding.UTF8.GetString(data)); }
```

### Verify object existence
```
if (!kvp.ObjectExists("/path/to/container/hello.txt")) Console.WriteLine("Does not exist");
else Console.WriteLine("Exists!");
```

### Move or rename an object
```
if (!kvp.MoveObject(
   "/path/to/container", 
   "hello.txt",
   "/path/to/newcontainer",
   "hello2.txt"))
{ // handle errors }
else
{ Console.WriteLine("Success"); }

if (!kvp.RenameObject(
   "/path/to/container",
   "hello2.txt",
   "hello.txt"))
{ // handle errors }
else
{ Console.WriteLine("Success"); }
```

### Delete an object
```
if (!kvp.DeleteObject("/path/to/container/hello.txt")) Console.WriteLine("Failed");
else Console.WriteLine("Deleted!");
```

### Create a container
```
if (!kvp.CreateContainer(
   "/path/to/container", 
   out url)) 
{ // handle errors }
else
{ Console.WriteLine("Created at: " + url); }

if (!kvp.GetObject(
   "/path/to/container",
   out data))
{ // handle errors }
else
{ Console.WriteLine("Container contents: " + Encoding.UTF8.GetString(data)); }
```

### Verify container existence
```
if (!kvp.ContainerExists("/path/to/container")) Console.WriteLine("Does not exist");
else Console.WriteLine("Exists!");
```

### Move or rename a container
```
if (!kvp.MoveContainer(
   "/path/to/container", 
   "container1",
   "/path/to/newcontainer",
   "container2"))
{ // handle errors }
else
{ Console.WriteLine("Success"); }

if (!kvp.RenameContainer(
   "/path/to/newcontainer",
   "container2",
   "container1"))
{ // handle errors }
else
{ Console.WriteLine("Success"); }
```

### Delete a container
```
if (!kvp.DeleteContainer("/path/to/newcontainer/container1", false)) Console.WriteLine("Failed");
else Console.WriteLine("Deleted!");
```

## running under Mono
kvpbase works well in Mono environments to the extent that we have tested it.  It is recommended that when running under Mono, you use the Mono Ahead-of-Time Compiler (AOT).
```
mono --aot=nrgctx-trampolines=8096,nimt-trampolines=8096,ntrampolines=4048 --server <yourapp>.exe
mono --server <yourapp>.exe
```