ScreenOffAndroid

Open ScreenOffAndroid.sln in Visual Studio with the .NET Android workload.
The project is independent of cctt-test-tool and can be moved with this whole folder.
Requires .NET 11 Android workload and Android 7.0 (API 24) or newer.

Install and launch ScreenOff on an Android device. On first launch, explicitly
activate its device administrator permission. The only requested policy is
force-lock. After activation, launching the application locks the device and
turns off its screen, removes the activity from recents and kills its own process.
If permission is denied, the application explains why it cannot lock and closes.

This locks the device; it is not just a brightness change. Depending on Android
and device policy, unlocking may require the PIN/password rather than biometrics.
To uninstall, first deactivate ScreenOff under Settings > Security > Device admin
apps (the exact settings path varies by manufacturer).
Android may start the process again for system-delivered administrator events;
this application does not install a background service, alarm or boot receiver.

Build:
  dotnet build ScreenOff\ScreenOff.csproj

Device behavior must be verified on a connected Android device.
