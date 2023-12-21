using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Assimp;
using BulletSharp;
using BulletSharp.Math;
using HelixToolkit.Wpf;
using Matrix = BulletSharp.Math.Matrix;
using Timer = System.Timers.Timer;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

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

            setupControllerHandling();
            
            /*test*/
            
            var importer = new AssimpContext();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "game.obj");
            var scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            // add meshes to simulation
            var listOfTriangleMeshes = physicsWorld.getTriangleMeshesFromScene(scene); // non static reference, just test
            listOfTriangleMeshes.ForEach( mesh =>
                {
                    physicsWorld.addRigidBody(new RigidBody(new RigidBodyConstructionInfo(1, new DefaultMotionState(), mesh)));
                    Console.WriteLine("Added Mesh to physics simulation");
                }
            );
            
            // add visual meshes
            var translatedMeshes = scene.Meshes.Select(mesh => new GraphicsWorld().ConvertAssimpMeshToMeshGeometry3D(mesh)).ToList();

            translatedMeshes.ForEach(mesh =>
            {
                var material = new DiffuseMaterial(new SolidColorBrush(Colors.Yellow));
                MyHelixViewport.Children.Add(new ModelVisual3D{Content = new GeometryModel3D(mesh, material)});
            });
            
        }

        private void setupControllerHandling()
        {
            // set Controller Event Handler
            controllerTimer = new Timer(16);
            controllerTimer.Elapsed += (sender, args) => OnControllerPoll();
            controllerTimer.Start();       
        }

        private void renderTick()
        {
            Dispatcher.Invoke(() => {
                // // combine physics pos downstream to visual ball
                // var ballPosX = physicsWorld.ballPosition.X;
                // var ballPosY = physicsWorld.ballPosition.Y;
                // var ballPosZ = physicsWorld.ballPosition.Z;
                //
                // ballModelVisual.Transform = new TranslateTransform3D(new Vector3D(ballPosX, ballPosY, ballPosZ));

                updateGraphicsWithPhysics();
            });
        }

        private void updateGraphicsWithPhysics()
        {
            var bulletObjects = physicsWorld.Objects;
            var helixobjects = GraphicsWorld.obj
            foreach (var bulletObject in bulletObjects)
            {
                // Physics Postition
                Matrix bulletTransform;
                bulletObject.MotionState.GetWorldTransform(out bulletTransform);

                // Convert Bullet transform to Helix transform
                var transform = new MatrixTransform3D(ToMedia3DMatrix(bulletTransform));
                visual.Transform = transform;
            }
        }

        private Matrix3D ToMedia3DMatrix(Matrix bulletTransform)
        {
            return new Matrix3D(
                bulletMatrix.M11, bulletMatrix.M12, bulletMatrix.M13, bulletMatrix.M14,
                bulletMatrix.M21, bulletMatrix.M22, bulletMatrix.M23, bulletMatrix.M24,
                bulletMatrix.M31, bulletMatrix.M32, bulletMatrix.M33, bulletMatrix.M34,
                bulletMatrix.M41, bulletMatrix.M42, bulletMatrix.M43, bulletMatrix.M44);

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
            // var reader = new ObjReader();
            //
            // var model3DGroup = new Model3DGroup();
            // var gameBoardModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "field.obj");
            //
            // gameBoard = reader.Read(gameBoardModelPath);
            // model3DGroup.Children.Add(gameBoard);
            // var modelVisual3DModel = new ModelVisual3D { Content = model3DGroup };
            // MyHelixViewport.Children.Add(modelVisual3DModel);
            //
            // // import ball blender
            // var ballReader = new ObjReader();
            // var ballModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "ball.obj");
            // var tempModelGroup = ballReader.Read(ballModelPath);
            // ballModelVisual = new ModelVisual3D { Content = tempModelGroup };
            // MyHelixViewport.Children.Add(ballModelVisual);
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
            // var transformGroup = new Transform3DGroup();
            // gameBoardRotationX = new AxisAngleRotation3D(new Vector3D(1,0,0), 0);   // global rotation "handle"
            // gameBoardRotationY = new AxisAngleRotation3D(new Vector3D(0,1,0), 0); // global rotation "handle"
            // transformGroup.Children.Add(new RotateTransform3D(gameBoardRotationX));
            // transformGroup.Children.Add(new RotateTransform3D(gameBoardRotationY));
            // gameBoard.Transform = transformGroup;
            
            
            
            // set Render Timer
            renderTimer = new Timer(16);
            renderTimer.Elapsed += (sender, args) =>
            {
                renderTick();
            };
            renderTimer.Start();
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