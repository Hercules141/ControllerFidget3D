using System;
using System.IO;
using System.Windows;
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

        public MainWindow()
        {
            InitializeComponent();

            setUpHelixModel();

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
                    updateControllerInfoDisplay();
                } else
                {
                    controller.tryConnect();
                }
            });
        }


        private void setUpHelixModel()
        {
            // HelixViewPort3D setup
            // var meshBuilder = new MeshBuilder(false, false);
            // meshBuilder.AddBox(new Point3D(0, 0, 0), 2, 2, 2);
            // var mesh = meshBuilder.ToMesh();
            //
            // var blueMaterial = MaterialHelper.CreateMaterial(Colors.Blue);
            // var modelVisual3D = new ModelVisual3D
            // {
            //     Content = new GeometryModel3D { Geometry = mesh, Material = blueMaterial }
            // };
            //
            // MyHelixViewport.Children.Add(modelVisual3D);
            
            // import blender model
            var reader = new ObjReader();
            var model3DGroup = new Model3DGroup();
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "field.obj");
            var model = reader.Read(modelPath);
            // var model = reader.Read("X:\\Home\\Projects\\Rider\\ControllerFidget3D\\ControllerFidget3D\\res\\field.obj");
            // var model = reader.Read(@"ControllerFidget3D/res/field.obj");
            model3DGroup.Children.Add(model);
            // MyHelixViewport.Children.Add(new ModelVisual3D{Content = model3DGroup});
            MyHelixViewport.Children.Add(new ModelVisual3D { Content = model3DGroup });
        }
        
        
        
        
        // helper functions
        private void updateControllerInfoDisplay()
        {
            controllerConnectedTextBlock.Text = "Controller Connected: " + controller.connected;
            controllerInfoTextBlock.Text = "\nController Info:    "
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