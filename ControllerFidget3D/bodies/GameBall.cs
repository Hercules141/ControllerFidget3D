using System;
using System.IO;
using System.Windows.Media.Media3D;
using BulletSharp.Math;
using HelixToolkit.Wpf;

namespace ControllerFidget3D
{
    public class GameBall : ICombinedBody
    {
        private Vector3 position;
        private Vector3 orientation;

        private ModelVisual3D ball;
        
        GameBall(String path, Vector3 position, Vector3 orientation)
        {
            position = this.position;
            orientation = this.orientation;

            loadGameBall(path);
        }

        private void loadGameBall(String path)
        {
           
        }

        
        // Overrides
        public Vector3 getPosition()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 getOrientation()
        {
            throw new System.NotImplementedException();
        }

        public void toString()
        {
            
        }
    }
}