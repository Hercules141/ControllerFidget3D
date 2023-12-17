using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Timer = System.Timers.Timer;

namespace ControllerFidget3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Point _lastMousePosition;
        private bool _mousePressed;
        private PerspectiveCamera _camera;
        private XBoxController controller = new XBoxController();

        private Timer controllerTimer;
        private int pollCounter;
        
        public MainWindow()
        {
            InitializeComponent();
            
           
            
            _camera = new PerspectiveCamera
            {
                Position = new Point3D(5, 5, 5),
                LookDirection = new Vector3D(-5, -5, -5),
                UpDirection = new Vector3D(0, 0, 1)
            };
            MyViewPort3D.Camera = _camera;
            
            // set Controller Event Handler
            controllerTimer = new Timer(16);
            controllerTimer.Elapsed += (sender, args) => OnControllerPoll();
            controllerTimer.Start();
            
        }

        private void Viewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            _mousePressed = true;
            MyViewPort3D.CaptureMouse();
        }

        private void Viewport3D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mousePressed = false;
            MyViewPort3D.ReleaseMouseCapture();
        }

        private void Viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mousePressed) return;
            
            var currentPosition = e.GetPosition(this);
            var dx = currentPosition.X - _lastMousePosition.X;
            var dy = currentPosition.Y - _lastMousePosition.Y;


            if (e.RightButton == MouseButtonState.Pressed)
            {
                // adjust camera
                var cameraDx = dx / 100;
                var cameraDy = dy / 100;
                
                _camera.Position = Point3D.Add(_camera.Position, new Vector3D(cameraDx, cameraDy, 0));
                
            } else if (e.LeftButton == MouseButtonState.Pressed)
            {
                // rotate cube
                cubeRotation.Angle += 5;

            }

            // controllerConnectedTextBlock.Text = controller.rightThumb.X.ToString();
            
            _lastMousePosition = currentPosition;
        }
        
        // controller event handler
        public void OnControllerPoll()
        {
            Dispatcher.Invoke(() =>
            {
                controller.Update();
                updateControllerInfoDisplay();
                
                // update camera position
                _camera.Position =  Point3D.Add(
                    _camera.Position,
                    new Vector3D(controller.leftThumb.X, controller.leftThumb.Y, controller.leftTrigger/255 - controller.rightTrigger/255)
                );
                // update camera angle, actually vector..
                _camera.LookDirection += new Vector3D(controller.rightThumb.X/10*-1, controller.rightThumb.Y/10*-1, 0);
                
            });
        }

        
        
        
        // helper functions
        private void updateControllerInfoDisplay()
        {
            controllerConnectedTextBlock.Text = controller.connected.ToString();
            controllerInfoTextBlock.Text = "Controller Info:    "
                                           + "\nLeft Thumb X:   " + controller.leftThumb.X
                                           + "\nLeft Thumb Y:   " + controller.leftThumb.Y
                                           + "\nRight Thumb X:  " + controller.rightThumb.X
                                           + "\nRight Thumb Y:  " + controller.rightThumb.Y

                                           + "\nL Trigger:      " + controller.leftTrigger
                                           + "\nR Trigger:      " + controller.rightTrigger

                                           + "\ndead thing:     " + controller.deadZone;
        }
    }
}