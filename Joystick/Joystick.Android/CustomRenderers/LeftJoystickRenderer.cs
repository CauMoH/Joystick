using Xamarin.Forms.Platform.Android;
using Joystick.CustomControls;
using Joystick.Droid.JoystickAndroidCustomControl;
using Xamarin.Forms;
using Joystick.Droid.CustomRenderers;
using Android.Content;

[assembly: ExportRenderer(typeof(LeftJoystickControl), typeof(LeftJoystickRenderer))]

namespace Joystick.Droid.CustomRenderers
{
    class LeftJoystickRenderer : ViewRenderer<LeftJoystickControl, LeftJoystickLayout>
    {
        private LeftJoystickLayout _leftJoystickLayout;

        public LeftJoystickRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<LeftJoystickControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _leftJoystickLayout = new LeftJoystickLayout(Context);

                SetNativeControl(_leftJoystickLayout);
            }

            if (e.OldElement != null)
            {
                _leftJoystickLayout.RemoveTouchListener();
            }

            if (e.NewElement != null)
            {
                _leftJoystickLayout.AddTouchListener(yposition =>
                {
                    Element.Yposition = yposition;
                });
            }
        }
    }
}