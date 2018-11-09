using Xamarin.Forms;

namespace Joystick.CustomControls
{
    public class RightJoystickControl : View
    {
        public static readonly BindableProperty XpositionProperty =
            BindableProperty.Create(
                propertyName: nameof(Xposition),
                returnType: typeof(int),
                declaringType: typeof(int),
                defaultValue: 0
            );

        public int Xposition
        {
            get => (int)GetValue(XpositionProperty);
            set => SetValue(XpositionProperty, value);
        }
    }
}
