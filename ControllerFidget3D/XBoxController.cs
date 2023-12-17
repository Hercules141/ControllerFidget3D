using System;
using System.Windows;
using SharpDX.XInput;

namespace ControllerFidget3D
{
    public class XBoxController
    {
        private Controller controller;
        private Gamepad gamepad;
        public bool connected = false;
        public int deadZone = 2500;
        public Point leftThumb, rightThumb = new Point(0, 0);
        public float leftTrigger, rightTrigger;

        public XBoxController()
        {
            tryConnect();
        }

        public void Update()
        {
            if (!connected)
                return;

            gamepad = controller.GetState().Gamepad;

            leftThumb.X = (Math.Abs((float)gamepad.LeftThumbX) < deadZone) ? 0 : (float)gamepad.LeftThumbX / short.MaxValue;
            leftThumb.Y = (Math.Abs((float)gamepad.LeftThumbY) < deadZone) ? 0 : (float)gamepad.LeftThumbY / short.MaxValue;
            rightThumb.X = (Math.Abs((float)gamepad.RightThumbX) < deadZone) ? 0 : (float)gamepad.RightThumbX / short.MaxValue;
            rightThumb.Y = (Math.Abs((float)gamepad.RightThumbY) < deadZone) ? 0 : (float)gamepad.RightThumbY / short.MaxValue;

            leftTrigger = gamepad.LeftTrigger;
            rightTrigger = gamepad.RightTrigger;
        }

        public void tryConnect()
        {
            controller = new Controller(UserIndex.One);
            connected = controller.IsConnected;
        }
    }
}