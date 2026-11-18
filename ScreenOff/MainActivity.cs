using Android.AccessibilityServices;
using Android.App.Admin;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Util;

namespace ScreenOff;

[Activity(Label = "@string/app_name", MainLauncher = true, Exported = true,
    ExcludeFromRecents = true, Theme = "@style/ScreenOffTheme")]
public class MainActivity : Activity
{
    private bool _lockRequested;
    private bool _dialogShown;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _lockRequested = savedInstanceState?.GetBoolean("lockRequested") ?? false;
    }

    protected override void OnSaveInstanceState(Bundle outState)
    {
        outState.PutBoolean("lockRequested", _lockRequested);
        base.OnSaveInstanceState(outState);
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (_lockRequested)
        {
            if (!((KeyguardManager)GetSystemService(KeyguardService)!).IsKeyguardLocked)
            {
                FinishAndRemoveTask();
            }
            return;
        }

        if (_dialogShown)
        {
            return;
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            ShowError(Resource.String.android_version_required);
            return;
        }

        var service = ScreenLockService.Current;
        if (service is null)
        {
            _dialogShown = true;
            new AlertDialog.Builder(this)!
                .SetTitle(Resource.String.app_name)!
                .SetMessage(Resource.String.accessibility_explanation)!
                .SetPositiveButton(Resource.String.open_settings, (_, _) =>
                {
                    // Keep system settings out of the task reopened by the launcher icon.
                    StartActivity(new Intent(Settings.ActionAccessibilitySettings)
                        .AddFlags(ActivityFlags.NewTask));
                    FinishAndRemoveTask();
                })!
                .SetNegativeButton(Android.Resource.String.Cancel, (_, _) => FinishAndRemoveTask())!
                .SetOnCancelListener(new CloseListener(this))!
                .Show();
            return;
        }

        try
        {
            // Retain the old receiver only to revoke administrator access after an upgrade.
            var policyManager = (DevicePolicyManager)GetSystemService(DevicePolicyService)!;
            var admin = new ComponentName(this, Java.Lang.Class.FromType(typeof(LockDeviceAdminReceiver)));
            if (policyManager.IsAdminActive(admin))
            {
                policyManager.RemoveActiveAdmin(admin);
            }

            if (!service.PerformGlobalAction(GlobalAction.LockScreen))
            {
                Log.Error(nameof(ScreenOff), "Android rejected the accessibility lock-screen action.");
                ShowError(Resource.String.lock_action_failed);
                return;
            }

            _lockRequested = true;
        }
        catch (Java.Lang.SecurityException exception)
        {
            Log.Error(nameof(ScreenOff), exception.ToString());
            ShowError(Resource.String.lock_action_failed);
        }
    }

    private void ShowError(int message)
    {
        _dialogShown = true;
        new AlertDialog.Builder(this)!
            .SetTitle(Resource.String.lock_failed)!
            .SetMessage(message)!
            .SetPositiveButton(Android.Resource.String.Ok, (_, _) => FinishAndRemoveTask())!
            .SetOnCancelListener(new CloseListener(this))!
            .Show();
    }

    private sealed class CloseListener(MainActivity activity) : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        public void OnCancel(IDialogInterface? dialog) => activity.FinishAndRemoveTask();
    }
}
