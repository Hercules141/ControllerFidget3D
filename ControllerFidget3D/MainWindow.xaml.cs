using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BulletSharp.Math;
using HelixToolkit.Wpf;
using Timer = System.Timers.Timer;

namespace ControllerFidget3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private XBoxController controller = new XBoxController();
        
        // physics world 
        private PhysicsSimulation physicsWorld;
        
        // timers
        private Timer controllerTimer;
        private Timer renderTimer;

        // objects
        private Model3DGroup gameBoard;
        private AxisAngleRotation3D gameBoardRotationX;
        private AxisAngleRotation3D gameBoardRotationY;

        private ModelVisual3D ballModelVisual;
        private Transform3D ballTransform;

        public MainWindow()
        {
            InitializeComponent();

            setUpHelix();

            setUpPhysics();
            
            // set Controller Event Handler
            controllerTimer = new Timer(16);
            controllerTimer.Elapsed += (sender, args) => OnControllerPoll();
            controllerTimer.Start();
            
            // set Render Timer
            renderTimer = new Timer(16);
            renderTimer.Elapsed += (sender, args) =>
            {
                renderTick();
            };
            renderTimer.Start();

        }

        private void renderTick()
        {
            Dispatcher.Invoke(() => {
                // combine physics pos downstream to visual ball
                var ballPosX = physicsWorld.ballPosition.X;
                var ballPosY = physicsWorld.ballPosition.Y;
                var ballPosZ = physicsWorld.ballPosition.Z;
            
                ballModelVisual.Transform = new TranslateTransform3D(new Vector3D(ballPosX, ballPosY, ballPosZ));
            });
        }

        private void setUpPhysics()
        {
            physicsWorld = new PhysicsSimulation();
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

            // move ball
            physicsWorld.applyForceToBall(new Vector3(controller.leftThumb.X, 0, controller.leftThumb.Y) * 100);
        }


        private void setUpHelix()
        {
            // import blender model board
            var reader = new ObjReader();
            
            var model3DGroup = new Model3DGroup();
            var gameBoardModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "field.obj");
            
            gameBoard = reader.Read(gameBoardModelPath);
            model3DGroup.Children.Add(gameBoard);
            var modelVisual3DModel = new ModelVisual3D { Content = model3DGroup };
            MyHelixViewport.Children.Add(modelVisual3DModel);
            
            // import ball blender
            var ballReader = new ObjReader();
            var ballModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "ball.obj");
            var tempModelGroup = ballReader.Read(ballModelPath);
            ballModelVisual = new ModelVisual3D { Content = tempModelGroup };
            MyHelixViewport.Children.Add(ballModelVisual);
            // // import ball blender
            // var ballModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "ball.obj");
            // ball = reader.Read(ballModelPath);
            // var ballVisual3DModel = new ModelVisual3D { Content = ball };
            // MyHelixViewport.Children.Add(ballVisual3DModel);

            
            
            // // test ball mirroring in viewPort
            // // setup ball in viewport
            // var material = MaterialHelper.CreateMaterial(Colors.Blue);
            // var sphere = new MeshBuilder(true, true);
            // sphere.AddSphere(new Point3D(0, 100, 0), 10);
            // var sphereModel = new GeometryModel3D(sphere.ToMesh(), material);
            // ballModelVisual = new ModelVisual3D();  // ModelVisual allows setting Transforms
            // ballModelVisual.Content = sphereModel;
            // MyHelixViewport.Children.Add(ballModelVisual);

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