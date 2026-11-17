ScreenOffAndroid

Open ScreenOffAndroid.sln in Visual Studio with the .NET Android workload.
The project is independent of cctt-test-tool and can be moved with this whole folder.
Requires .NET 11 Android workload and Android 7.0 (API 24) or newer.

Locking requires Android 9 (API 28) or newer. On older supported installations,
the application explains the limitation and does not fall back to administrator locking.

On first launch, read the disclosure and enable ScreenOff under Settings >
Accessibility > Installed apps, then return to ScreenOff. If Android restricts
this setting for a sideloaded APK, review App info > menu > Allow restricted settings
before enabling it. Setting names vary by manufacturer. If the service is still
connecting, reopen ScreenOff after a moment. Denying access leaves the device unlocked.

The accessibility service only performs GLOBAL_ACTION_LOCK_SCREEN when ScreenOff
is opened. It does not subscribe to accessibility events or request access to
screen content. Android keeps this service connected in the background. The
transparent launcher activity closes after unlocking; no alarm or boot receiver is used.

This replaces DevicePolicyManager.LockNow(), which forced PIN/password/pattern
entry with a device-policy message. The new action preserves normal biometric
unlocking; Android can still require a PIN after restart, timeouts, failed attempts,
or other system/security policies. Unlock once with the PIN after upgrading.
On the first successful setup after an upgrade, ScreenOff removes its own old
device administrator permission. The old receiver is retained only for migration.
If uninstalling before setup, deactivate ScreenOff under Settings > Security >
Device admin apps first. Disable the accessibility service to revoke its new access.

Build:
  dotnet build ScreenOff\ScreenOff.csproj

Device behavior must be verified on a connected Android device.
