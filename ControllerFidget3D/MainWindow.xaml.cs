using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
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

        private Model3DGroup gameBoard;
        private AxisAngleRotation3D gameBoardRotationX;
        private AxisAngleRotation3D gameBoardRotationY;

        public MainWindow()
        {
            InitializeComponent();

            setUpHelix();

            // set Controller Event Handler
            controllerTimer = new Timer(16);
            controllerTimer.Elapsed += (sender, args) => OnControllerPoll();
            controllerTimer.Start();
            
        }

        // controller event handler
        public void OnControllerPoll()
        {
            Dispatcher.Invoke(() =>
            {
                if (controller.connected)
                {
                    controller.Update();
                    //visual transforms to objects
                    updateObjectsByControllerInput();
                    updateControllerInfoDisplay();
                } else
                {
                    controller.tryConnect();
                }
            });
        }

        private void updateObjectsByControllerInput()
        {
            // handle x rotation
            var xAngle = controller.rightThumb.X * 30;   // 30 deg tiltable table
            // gameBoard.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1,0,0), xAngle));
            gameBoardRotationX.Angle = xAngle;
            
            // handle y rotation
            var yAngle = controller.rightThumb.Y * 30;
            // gameBoard.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,1,0), yAngle));
            gameBoardRotationY.Angle = yAngle;

            
        }


        private void setUpHelix()
        {
            // import blender model
            var reader = new ObjReader();
            var model3DGroup = new Model3DGroup();
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "field.obj");
            
            gameBoard = reader.Read(modelPath);
            model3DGroup.Children.Add(gameBoard);
            var modelVisual3DModel = new ModelVisual3D { Content = model3DGroup };
            MyHelixViewport.Children.Add(modelVisual3DModel);

            // set the rotation axis for the game Board model
            var transformGroup = new Transform3DGroup();
            gameBoardRotationX = new AxisAngleRotation3D(new Vector3D(1,0,0), 0);   // global rotation "handle"
            gameBoardRotationY = new AxisAngleRotation3D(new Vector3D(0,1,0), 0); // global rotation "handle"
            transformGroup.Children.Add(new RotateTransform3D(gameBoardRotationX));
            transformGroup.Children.Add(new RotateTransform3D(gameBoardRotationY));
            gameBoard.Transform = transformGroup;
        }
        
        
        
        
        // helper functions
        private void updateControllerInfoDisplay()
        {
            controllerConnectedTextBlock.Text = "Controller Connected: " + controller.connected;
            controllerInfoTextBlock.Text = controller.getControllerInfo();
        }

        // this belongs here
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {   
            if (e.Key == Key.Escape) Close();
        }
    }
}