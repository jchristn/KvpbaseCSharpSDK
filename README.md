# kvpbase C# SDK
kvpbase is a RESTful object storage platform.  This SDK is intended to help application developers integrate object storage using kvpbase into their C# applications.

## help or feedback
first things first - do you need help or have feedback?  Contact me at joel at maraudersoftware.com dot com or file an issue here!

## simple example
```
using KvpbaseSDK;
...
//
// Start the client
// User GUID, API Key, Endpoint
//
Client kvp = new Client("default", "default", "http://localhost:8080"); 

//
// Perform object operations
//
string url = "";
byte data[];

if (!kvp.CreateObjectWithoutName(
   "/path/to/container", 
   "text/plain", 
   Encoding.UTF8.GetBytes("Hello!"), 
   out url)) 
{
   // handle errors  
}
else
{
   Console.WriteLine("Stored at: " + url);
}

if (!kvp.CreateObject(
   "/path/to/container",
   "hello.txt",
   "text/plain",
   Encoding.UTF8.GetBytes("Hello!"),
   out url))
{
   // handle errors  
}
else
{
   Console.WriteLine("Stored at: " + url);
}

if (!kvp.GetObject(
   "/path/to/container/hello.txt",
   out data))
{
   // handle errors
}
else
{
   Console.WriteLine("Data: " + Encoding.UTF8.GetString(data));
}

if (!kvp.ObjectExists("/path/to/container/hello.txt")) Console.WriteLine("Does not exist");
else Console.WriteLine("Exists!");

if (!kvp.MoveObject(
   "/path/to/container", 
   "hello.txt",
   "/path/to/newcontainer",
   "hello2.txt"))
{
   // handle errors
}
else
{
   Console.WriteLine("Success");
}

if (!kvp.RenameObject(
   "/path/to/container",
   "hello2.txt",
   "hello.txt"))
{
   // handle errors
}
else
{
   Console.WriteLine("Success");
}

if (!kvp.DeleteObject("/path/to/container/hello.txt")) Console.WriteLine("Failed");
else Console.WriteLine("Deleted!");

// 
// Perform container operations
//
string url = "";
byte data[];

if (!kvp.CreateContainer(
   "/path/to/container", 
   out url)) 
{
   // handle errors  
}
else
{
   Console.WriteLine("Created at: " + url);
}

if (!kvp.GetObject(
   "/path/to/container",
   out data))
{
   // handle errors
}
else
{
   Console.WriteLine("Container contents: " + Encoding.UTF8.GetString(data));
}

if (!kvp.ContainerExists("/path/to/container")) Console.WriteLine("Does not exist");
else Console.WriteLine("Exists!");

if (!kvp.MoveContainer(
   "/path/to/container", 
   "container1",
   "/path/to/newcontainer",
   "container2"))
{
   // handle errors
}
else
{
   Console.WriteLine("Success");
}

if (!kvp.RenameContainer(
   "/path/to/newcontainer",
   "container2",
   "container1"))
{
   // handle errors
}
else
{
   Console.WriteLine("Success");
}

if (!kvp.DeleteContainer("/path/to/newcontainer/container1", false)) Console.WriteLine("Failed");
else Console.WriteLine("Deleted!");
```

## running under Mono
kvpbase works well in Mono environments to the extent that we have tested it.  It is recommended that when running under Mono, you use the Mono Ahead-of-Time Compiler (AOT).
```
mono --aot=nrgctx-trampolines=8096,nimt-trampolines=8096,ntrampolines=4048 --server <yourapp>.exe
mono --server <yourapp>.exe
```