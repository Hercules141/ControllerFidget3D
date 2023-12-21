using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
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
        
        // worlds 
        private PhysicsSimulation physicsWorld;
        private GraphicsWorld graphicsWorld;
        
        // timers
        private Timer controllerTimer;

        // objects
        // private Model3DGroup gameBoard;
        // private AxisAngleRotation3D gameBoardRotationX;
        // private AxisAngleRotation3D gameBoardRotationY;
        //
        // private ModelVisual3D ballModelVisual;
        // private Transform3D ballTransform;

        public MainWindow()
        {
            InitializeComponent();
            
            setupControllerHandling();
            
            setUpPhysicsSimulation();

            setupGraphicsWorld();


            /*test*/
            var index = 0;
            var mass = 0;
            var startingPos = new Vector3(0, 0, 0);

            var importer = new AssimpContext();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "game.obj");
            var scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            // add meshes to simulation
            var listOfTriangleMeshes = physicsWorld.getTriangleMeshesFromScene(scene); // non static reference, just test
            listOfTriangleMeshes.ForEach( mesh =>
                {
                    /* these if statements are only to set certain properties bc .obj might need to be imported differently to find by name.
                        todo remember: mesh is single object with origin at 0,0,0 but mesh position is persisted after blender export (maybe retrieveble by node transform)
                        starting index only applied to physical object here, graphical object needs seperate transform if this is the route to go
                    */
                    if (index == 0) {
                        mass = 0; // first object, disable gravity
                        startingPos = new Vector3(0, 0, 0);

                    }
                    else {
                        mass = 1;
                        // startingPos = new Vector3(0, 100, 0);
                        startingPos = new Vector3(0, 0, 0);

                    }

                    var startingTransform = Matrix.Translation(startingPos);
                    physicsWorld.addRigidBody(new RigidBody(new RigidBodyConstructionInfo(mass, new DefaultMotionState(startingTransform), mesh)));
                    index++;
                    Console.WriteLine("Added Mesh to physics simulation");
                       
                }
            );
            
            // add visual meshes
            var translatedMeshes = scene.Meshes.Select(mesh => graphicsWorld.ConvertAssimpMeshToMeshGeometry3D(mesh)).ToList(); 

            translatedMeshes.ForEach(mesh =>
            {
                var material = new DiffuseMaterial(new SolidColorBrush(Colors.Yellow));
                graphicsWorld.addObject(new ModelVisual3D{Content = new GeometryModel3D(mesh, material)}); // todo fixme mesh is added as object without setting transform, ignoring scene location (mesh data not enough, origin!)
            });
            
        }

        private void setupControllerHandling()
        {
            // set Controller Event Handler
            controllerTimer = new Timer(1000/60); // ca. 60 Polls/s
            controllerTimer.Elapsed += (sender, args) => OnControllerPoll();
            controllerTimer.Start();       
        }

        // private void renderTick()
        // {
        //     Dispatcher.Invoke(() => {
        //         // // combine physics pos downstream to visual ball
        //         // var ballPosX = physicsWorld.ballPosition.X;
        //         // var ballPosY = physicsWorld.ballPosition.Y;
        //         // var ballPosZ = physicsWorld.ballPosition.Z;
        //         //
        //         // ballModelVisual.Transform = new TranslateTransform3D(new Vector3D(ballPosX, ballPosY, ballPosZ));
        //
        //         updateGraphicsWithPhysics();
        //     });
        // }

        private void updateGraphicsWithPhysics()
        {
                
            var bulletObjects = physicsWorld.getRigidBodies();
            var helixobjects = graphicsWorld.GetObjects();
            
            if (bulletObjects == null || helixobjects == null || bulletObjects.Count != helixobjects.Count) return;
            Console.WriteLine("updating positions");
            Dispatcher.Invoke( () => {
                for (var i = 0; i < bulletObjects.Count; i++)
                {
                    // Physics Postition
                    Matrix bulletTransform;
                    bulletObjects[i].MotionState.GetWorldTransform(out bulletTransform);    Console.WriteLine("bullet Object index: "  + i + ", Position: " + bulletTransform.Origin);

                    // Convert Bullet transform to Helix transform
                    var helixTransform = new MatrixTransform3D(ToMedia3DMatrix(bulletTransform));
                    helixobjects[i].Transform = helixTransform;                         Console.WriteLine("helix Object index: "  + i + ", Position: " + helixTransform.Value);
                }
            });
        }

        private Matrix3D ToMedia3DMatrix(Matrix bulletTransform)
        {
            return new Matrix3D(
                bulletTransform.M11, bulletTransform.M12, bulletTransform.M13, bulletTransform.M14, // x
                bulletTransform.M21, bulletTransform.M22, bulletTransform.M23, bulletTransform.M24, // y
                bulletTransform.M31, bulletTransform.M32, bulletTransform.M33, bulletTransform.M34,// z
                bulletTransform.M41, bulletTransform.M42, bulletTransform.M43, bulletTransform.M44); // standard transformation (perspectives)

        }

        private void setUpPhysicsSimulation()
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
            // gameBoardRotationX.Angle = xAngle;
            // gameBoard.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1,0,0), xAngle));

            // handle y rotation
            var yAngle = controller.rightThumb.Y * 30;
            // gameBoardRotationY.Angle = yAngle;
            // gameBoard.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,1,0), yAngle));

            // move ball
            // physicsWorld.applyForceToBall(new Vector3(controller.leftThumb.X, 0, controller.leftThumb.Y) * 100);
        }


        private void setupGraphicsWorld()
        {
            graphicsWorld = new GraphicsWorld(MyHelixViewport, updateGraphicsWithPhysics);
            
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
            
            
            
            // // set Render Timer
            // renderTimer = new Timer(16);
            // renderTimer.Elapsed += (sender, args) =>
            // {
            //     renderTick();
            // };
            // renderTimer.Start();
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