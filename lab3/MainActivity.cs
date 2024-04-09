using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Xamarin.Essentials;
using Android.Graphics;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Provider;
using static Android.Renderscripts.Sampler;

namespace lab3
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ImageView img;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var btnTakePhoto = FindViewById<Button>(Resource.Id.btnTakePhoto);
            btnTakePhoto.Click += TakePhotoAsync;

            var btnChoosePhoto = FindViewById<Button>(Resource.Id.btnChoosePhoto);
            btnChoosePhoto.Click += GetPhotoAsync;

            var btnSavePhoto = FindViewById<Button>(Resource.Id.btnSavePhoto);
            btnSavePhoto.Click += SavePhotoAsync;

            img = FindViewById<ImageView>(Resource.Id.imgView);
        }

        async void TakePhotoAsync(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    var bitmap = await BitmapFactory.DecodeStreamAsync(await photo.OpenReadAsync());
                    if (bitmap != null)
                    {
                        img.SetImageBitmap(bitmap);
                    }
                }
            }
            catch (Exception exp)
            {
                Toast.MakeText(Application.Context, exp.Message, ToastLength.Long).Show();
            }
        }

        async void GetPhotoAsync(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();

                if(photo != null)
                {
                    var bitmap = await BitmapFactory.DecodeStreamAsync(await photo.OpenReadAsync());
                    img.SetImageBitmap(bitmap);
                }
   
            }
            catch (Exception exp)
            {
                Toast.MakeText(Application.Context, exp.Message, ToastLength.Long).Show();
            }
        }

        async void SavePhotoAsync(object sender, EventArgs e)
        {
            try
            {
                var bitmap = ((BitmapDrawable)img.Drawable).Bitmap;
                if (bitmap != null)
                {

                    var name = "my_image_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
                    var values = new ContentValues();
                    values.Put(MediaStore.Images.Media.InterfaceConsts.DisplayName, name);
                    values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, "image/jpeg");

                    var uri = ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);
                    using (var outputStream = ContentResolver.OpenOutputStream(uri))
                    {
                        await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);
                    }

                    Toast.MakeText(Application.Context, "Image saved to gallery", ToastLength.Long).Show();
                }
            }
            catch (Exception exp)
            {
                Toast.MakeText(Application.Context, exp.Message, ToastLength.Long).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

