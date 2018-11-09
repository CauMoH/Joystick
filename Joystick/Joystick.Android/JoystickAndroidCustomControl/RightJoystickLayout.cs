using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Joystick.Droid.JoystickAndroidCustomControl
{
    internal class RightJoystickLayout : LinearLayout, View.IOnTouchListener
    {
        public RightJoystickLayout(Context context) : base(context)
        {
            SetContent(context);
        }
        public RightJoystickLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetContent(context);
        }
        public RightJoystickLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            SetContent(context);
        }
        public RightJoystickLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            SetContent(context);
        }

        private readonly int _resolution = 255;

        private LinearLayout _rightJoyStick;
        private Action<int> _updateValue;

        private float _xInView;

        private float _originalX;
        private float _originalY;

        public int Xposition { get; private set; }

        private void SetContent(Context context)
        {
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.rightJoystickMainLayout, this);

            _rightJoyStick = FindViewById<LinearLayout>(Resource.Id.directionStickView);

            SetGravity(GravityFlags.Center);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Up:
                    v.Layout((int)_originalX, (int)_originalY, (int)_originalX + v.Width, (int)_originalY + v.Height);
                    UpdateX((int)v.GetX());
                    break;
                case MotionEventActions.Down:
                    _originalX = v.GetX();
                    _originalY = v.GetY();
                    _xInView = e.GetX();
                    break;
                case MotionEventActions.Move:
                    var parent = v.Parent as View;
                    var parentCoordinates = new[] { 0, 0 };
                    parent?.GetLocationOnScreen(parentCoordinates);

                    var newLeft = (int)(e.RawX - (_xInView)) - parentCoordinates[0];
                    var newRight = newLeft + v.Width;
                    var newViewX = newLeft - (int)_originalX;

                    if (Math.Abs(newViewX) > _originalX)
                    {
                        if (newViewX < 0)
                        {
                            newLeft = 0;
                            newRight = newLeft + v.Width;
                        }
                        else
                        {
                            newLeft = (int)_originalX * 2;
                            newRight = newLeft + v.Width;
                        }

                    }

                    v.Layout(newLeft, (int)_originalY, newRight, (int)_originalY + v.Height);
                    UpdateX((int)v.GetX());
                    break;
            }
            return true;
        }

        private void UpdateX(int x)
        {
            var totalAxisLength = (int)_originalX * 2;
            Xposition = x * _resolution / totalAxisLength - _resolution / 2;

            _updateValue?.Invoke(Xposition);
        }

        public void AddTouchListener(Action<int> updateValue)
        {
            _rightJoyStick.SetOnTouchListener(this);
            _updateValue = updateValue;
        }

        public void RemoveTouchListener()
        {
            _rightJoyStick.SetOnTouchListener(null);
            _updateValue = null;
        }
    }
}