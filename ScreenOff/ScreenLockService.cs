using Android.AccessibilityServices;
using Android.Content;
using Android.Views.Accessibility;

namespace ScreenOff;

[Service(Name = "AppsTheater.ScreenOff.ScreenLockService",
	Label = "@string/app_name", Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE",
	Exported = true)]
[IntentFilter(["android.accessibilityservice.AccessibilityService"])]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service")]
public class ScreenLockService : AccessibilityService
{
	internal static ScreenLockService? Current { get; private set; }

	protected override void OnServiceConnected()
	{
		base.OnServiceConnected();
		Current = this;
	}

	public override bool OnUnbind(Intent? intent)
	{
		if (Current == this)
		{
			Current = null;
		}
		return base.OnUnbind(intent);
	}

	public override void OnDestroy()
	{
		if (Current == this)
		{
			Current = null;
		}
		base.OnDestroy();
	}

	public override void OnAccessibilityEvent(AccessibilityEvent? e)
	{
	}

	public override void OnInterrupt()
	{
	}
}
