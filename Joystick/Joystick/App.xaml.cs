using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using Joystick.Interfaces;
using Joystick.Services;
using nexus.protocols.ble;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace Joystick
{
    public partial class App
    {
        public App(IBluetoothLowEnergyAdapter bluetooth)
        {
            InitializeComponent();

            ServiceLocator.Current.GetInstance<IBluetoothService>().Adapter = bluetooth;

            MainPage = new NavigationPage(new Views.MainPage());

            MessagingCenter.Subscribe<CustomDisplayAlert, string[]>(this, "DisplayAlert", (sender, values) => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MainPage.DisplayAlert(values[0], values[1], "Ok");
                });
            });
            
            CheckPermission(Permission.Location);
            CheckPermission(Permission.LocationAlways);
            CheckPermission(Permission.LocationWhenInUse);
            CheckPermission(Permission.Storage);
        }
        
        private async void CheckPermission(Permission permission)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
                    {
                        await MainPage.DisplayAlert("Need " + permission, "Gunna need that " + permission, "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(permission))
                        status = results[permission];
                }
                if (status != PermissionStatus.Unknown && status != PermissionStatus.Granted)
                {
                    await MainPage.DisplayAlert(permission+ " Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
