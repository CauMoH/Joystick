using Xamarin.Forms;

namespace Joystick.CustomControls
{
    public class LeftJoystickControl : View
    {
        public static readonly BindableProperty YpositionProperty =
            BindableProperty.Create(
                propertyName: nameof(Yposition),
                returnType: typeof(int),
                declaringType: typeof(int),
                defaultValue: 0
            );

        public int Yposition
        {
            get => (int)GetValue(YpositionProperty);
            set => SetValue(YpositionProperty, value);
        }

    }
}
