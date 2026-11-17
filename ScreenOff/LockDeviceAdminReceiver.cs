using Android.App.Admin;
using Android.Content;

namespace ScreenOff;

[BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", Exported = true)]
[MetaData("android.app.device_admin", Resource = "@xml/device_admin")]
[IntentFilter(["android.app.action.DEVICE_ADMIN_ENABLED"])]
public class LockDeviceAdminReceiver : DeviceAdminReceiver
{
}
