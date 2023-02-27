# SrvAny
SrvAny is a program that allows you to run any program as a service on Windows. It is similar to the srvany.exe program that was included in the Windows Resource Kit.

# Installation
To install SrvAny, simply download the executable file and place it in a directory on your Windows system. There is no need to install any additional software or drivers.

# Usage
You can use the sc.exe command-line tool to create the service. For example, to create a service called "MyService" that runs the program "C:\MyProgram.exe", you would run the following command:

```
sc create SrvAnyTest binPath= """C:\Program Files\SrvAny\SrvAny.exe"" ""C:\Program Files\SomeApp\SomeApp.exe"" C:\test.txt" type= own DisplayName= "SomeApp Service" start= auto
```

After creating the service, you can start, stop, and manage it just like any other Windows service.

# License
SrvAny is released under the MIT license. See the LICENSE file for more information.

# Disclaimer
SrvAny is provided as-is, without any warranty or support. Use at your own risk.
