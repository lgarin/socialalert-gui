using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace Socialalert
{
    [Activity(Label = "android_app", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const String TAG = nameof(MainActivity);

        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Text = "Click me";
            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
            Log.Info(TAG, nameof(OnCreate));
        }

        protected override void OnPostResume()
        {
            base.OnPostResume();
            Log.Info(TAG, nameof(OnPostResume));
        }

        protected override void OnStart()
        {
            base.OnStart();
            Log.Info(TAG, nameof(OnStart));
        }
    }
}

