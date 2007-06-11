WikiAccess is the library for .NET Framework v2.0. Here is some short notes to it.

== Notes ==
* WikiAccess uses IE engine so you can't login as two users in one time. Please, remember it.
* Because this library uses ActiveX, you have to put [STAThread] attribute to the Main() function.
* Use Utils.Wait() method for timers to avoid context deadlock
* Message cache is usually saved in "." directory, but you can change it using second parameter
  of Wiki object constructor.
* Properties like Page.Images are cached
== Build ===
If you uploaded source code, and want to build it:
* Open .NET 2.0 SDK Command prompt