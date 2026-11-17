using Android.App.Admin;
using Android.Content;
using Android.Util;

namespace ScreenOff;

[Activity(Label = "@string/app_name", MainLauncher = true, Exported = true,
    ExcludeFromRecents = true, Theme = "@style/ScreenOffTheme")]
public class MainActivity : Activity
{
    private const int ActivateAdminRequest = 1;
    private bool _hasResumed;
    private bool _lockRequested;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var policyManager = (DevicePolicyManager)GetSystemService(DevicePolicyService)!;
        var admin = new ComponentName(this, Java.Lang.Class.FromType(typeof(LockDeviceAdminReceiver)));
        if (policyManager.IsAdminActive(admin))
        {
            LockAndExit(policyManager);
        }
        else if (savedInstanceState is null)
        {
            var intent = new Intent(DevicePolicyManager.ActionAddDeviceAdmin);
            intent.PutExtra(DevicePolicyManager.ExtraDeviceAdmin, admin);
            intent.PutExtra(DevicePolicyManager.ExtraAddExplanation, GetString(Resource.String.admin_explanation));
            StartActivityForResult(intent, ActivateAdminRequest);
        }
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (_hasResumed && _lockRequested && !((KeyguardManager)GetSystemService(KeyguardService)!).IsKeyguardLocked)
        {
            FinishAndRemoveTask();
        }

        _hasResumed = true;
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != ActivateAdminRequest)
        {
            return;
        }

        var policyManager = (DevicePolicyManager)GetSystemService(DevicePolicyService)!;
        var admin = new ComponentName(this, Java.Lang.Class.FromType(typeof(LockDeviceAdminReceiver)));
        if (policyManager.IsAdminActive(admin))
        {
            LockAndExit(policyManager);
        }
        else
        {
            Toast.MakeText(this, Resource.String.permission_required, ToastLength.Long)!.Show();
            FinishAndRemoveTask();
        }
    }

    private void LockAndExit(DevicePolicyManager policyManager)
    {
        try
        {
            policyManager.LockNow();
        }
        catch (Java.Lang.SecurityException exception)
        {
            Log.Error(nameof(ScreenOff), exception.ToString());
            new AlertDialog.Builder(this)!
                .SetTitle(Resource.String.lock_failed)!
                .SetMessage(Resource.String.permission_required)!
                .SetPositiveButton(Android.Resource.String.Ok, (_, _) => FinishAndRemoveTask())!
                .SetOnCancelListener(new CloseListener(this))!
                .Show();
            return;
        }

        _lockRequested = true;
    }

    private sealed class CloseListener(MainActivity activity) : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        public void OnCancel(IDialogInterface? dialog) => activity.FinishAndRemoveTask();
    }
}
