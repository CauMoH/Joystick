using System;
using Xamarin.Forms;

namespace Joystick.CustomControls
{
    public class RightJoystickControl : View
    {
        public static readonly BindableProperty RawXpositionProperty =
            BindableProperty.Create(
                nameof(RawXposition),
                typeof(Tuple<int, int>),
                declaringType: typeof(Tuple<int, int>),
                defaultValue: new Tuple<int, int>(0, 0)
            );

        public Tuple<int, int> RawXposition
        {
            get => (Tuple<int, int>)GetValue(RawXpositionProperty);
            set => SetValue(RawXpositionProperty, value);
        }
    }
}
