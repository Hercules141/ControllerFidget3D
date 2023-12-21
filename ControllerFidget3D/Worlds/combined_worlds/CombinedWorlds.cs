using System.Collections.Generic;
using System.Timers;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;

namespace ControllerFidget3D
{
    public class CombinedWorlds
    {
        private Timer renderTimer;
        private Timer simulationTimer;

        private GraphicsWorld _graphicsWorld;
        private PhysicsWorld _physicsWorld;
        

        private List<ICombinedBody> objects;

        public CombinedWorlds()
        {

            setupGraphicsWorld();
            setupPhyicsWorld();

        }

        private void setupPhyicsWorld()
        {
            _physicsWorld = new PhysicsWorld();
        }

        private void setupGraphicsWorld()
        {
            _graphicsWorld = new GraphicsWorld();
        }


        private void renderTick()
        {
            // Dispatcher.Invoke(() => {
                // combine physics pos downstream to visual ball
                // var ballPosX = physicsWorld.ballPosition.X;
                // var ballPosY = physicsWorld.ballPosition.Y;
                // var ballPosZ = physicsWorld.ballPosition.Z;
            
                // ballModelVisual.Transform = new TranslateTransform3D(new Vector3D(ballPosX, ballPosY, ballPosZ));
            // });
        }

        private void setupSimulationTimer()
        {
            throw new System.NotImplementedException();
        }
    }
}