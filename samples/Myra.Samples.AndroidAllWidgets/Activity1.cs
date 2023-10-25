using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Myra.Samples.AllWidgets
{
	[Activity(
		Label = "@string/app_name",
		MainLauncher = true,
		Icon = "@drawable/icon",
		AlwaysRetainTaskState = true,
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.FullUser,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
	)]
	public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			var g = new AllWidgetsGame();
			SetContentView((View)g.Services.GetService(typeof(View)));
			g.Run();
		}
	}
}

