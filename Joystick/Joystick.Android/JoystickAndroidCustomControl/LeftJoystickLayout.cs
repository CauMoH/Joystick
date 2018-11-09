using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace Joystick.Droid.JoystickAndroidCustomControl
{
    internal class LeftJoystickLayout : LinearLayout, View.IOnTouchListener
    {
        public LeftJoystickLayout(Context context) : base(context)
        {
            SetContent(context);
        }
        public LeftJoystickLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetContent(context);
        }
        public LeftJoystickLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            SetContent(context);
        }
        public LeftJoystickLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            SetContent(context);
        }

        private readonly int _resolution = 255;

        private LinearLayout _leftJoyStick;
        private Action<int> _updateValue;

        private float _yInView;

        private float _originalX;
        private float _originalY;

        public int Yposition { get; private set; }

        private void SetContent(Context context)
        {
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.leftJoystickMainLayout, this);

            SetMinimumWidth(200);
            SetMinimumHeight(500);

            _leftJoyStick = FindViewById<LinearLayout>(Resource.Id.acceleratorStickView);

            SetGravity(GravityFlags.Center);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Up:
                    v.Layout((int)_originalX, (int)_originalY, (int)_originalX + v.Width, (int)_originalY + v.Height);
                    UpdateY((int)v.GetY());
                    break;
                case MotionEventActions.Down:
                    _originalX = v.GetX();
                    _originalY = v.GetY();
                    _yInView = e.GetY();
                    break;
                case MotionEventActions.Move:
                    var parent = v.Parent as View;
                    var parentCoordinates = new[] { 0, 0 };
                    parent?.GetLocationOnScreen(parentCoordinates);

                    var newTop = (int)(e.RawY - (_yInView)) - parentCoordinates[1];
                    var newBottom = newTop + v.Height;
                    var newViewY = (newTop - (int)_originalY) * -1;

                    if (Math.Abs(newViewY) > _originalY)
                    {
                        if (newViewY < 0)
                        {
                            newTop = (int)_originalY * 2;
                            newBottom = newTop + v.Height;
                        }
                        else
                        {
                            newTop = 0;
                            newBottom = newTop + v.Height;
                        }

                    }

                    v.Layout((int)_originalX, newTop, (int)_originalX + v.Width, newBottom);
                    UpdateY((int)v.GetY());
                    break;
            }
            return true;
        }

        private void UpdateY(int y)
        {
            var totalAxisLength = (int)_originalY * 2;
            Yposition = ((y * _resolution / totalAxisLength) - (_resolution / 2)) * -1;

            _updateValue?.Invoke(Yposition);
        }

        public void AddTouchListener(Action<int> updateValue)
        {
            _leftJoyStick.SetOnTouchListener(this);
            _updateValue = updateValue;
        }

        public void RemoveTouchListener()
        {
            _leftJoyStick.SetOnTouchListener(null);
            _updateValue = null;
        }
    }
}