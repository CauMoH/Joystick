using Xamarin.Forms.Platform.Android;
using Joystick.CustomControls;
using Joystick.Droid.JoystickAndroidCustomControl;
using Xamarin.Forms;
using Joystick.Droid.CustomRenderers;
using Android.Content;

[assembly: ExportRenderer(typeof(RightJoystickControl), typeof(RightJoystickRenderer))]

namespace Joystick.Droid.CustomRenderers
{
    class RightJoystickRenderer : ViewRenderer<RightJoystickControl, RightJoystickLayout>
    {
        private RightJoystickLayout _rightJoystickLayout;

        public RightJoystickRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RightJoystickControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _rightJoystickLayout = new RightJoystickLayout(Context);

                SetNativeControl(_rightJoystickLayout);
            }

            if (e.OldElement != null)
            {
                _rightJoystickLayout.RemoveTouchListener();
            }

            if (e.NewElement != null)
            {
                _rightJoystickLayout.AddTouchListener(xposition =>
                {
                    Element.RawXposition = xposition;
                });
            }
        }
    }
}