using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

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

        public MainWindow()
        {
            InitializeComponent();
            
            _camera = new PerspectiveCamera
            {
                Position = new Point3D(5, 5, 5),
                LookDirection = new Vector3D(-5, -5, -4),
                UpDirection = new Vector3D(0, 1, 0)
            };
            MyViewPort3D.Camera = _camera;
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
            if (_mousePressed)
            {
                var currentPosition = e.GetPosition(this);
                var dx = currentPosition.X - _lastMousePosition.X;
                var dy = currentPosition.Y - _lastMousePosition.Y;

                // Update camera position based on dx and dy
                // ...

                _lastMousePosition = currentPosition;
            }
        }
    }
}