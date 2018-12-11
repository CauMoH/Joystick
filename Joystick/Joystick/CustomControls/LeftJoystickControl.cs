using System;
using Xamarin.Forms;

namespace Joystick.CustomControls
{
    public class LeftJoystickControl : View
    {
        public static readonly BindableProperty RawYpositionProperty =
            BindableProperty.Create(
                nameof(RawYposition),
                typeof(Tuple<int, int>),
                typeof(Tuple<int, int>),
                new Tuple<int, int>(0, 0)
            );

        public Tuple<int, int> RawYposition
        {
            get => (Tuple<int, int>)GetValue(RawYpositionProperty);
            set => SetValue(RawYpositionProperty, value);
        }

    }
}
