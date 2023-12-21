using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace ControllerFidget3D
{
    public class GraphicsWorld : IGraphicsWorld
    {
        private HelixViewport3D viewport;
        private Timer renderTimer;

        private List<GraphicsBody> bodies;

        public GraphicsWorld()
        {
            setupHelix();
            setupRenderTimer();
        }

        private void setupHelix()
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
            // viewport.Children.Add(modelVisual3DModel);
            //
            // // import ball blender
            // var ballReader = new ObjReader();
            // var ballModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "ball.obj");
            // var tempModelGroup = ballReader.Read(ballModelPath);
            // ballModelVisual = new ModelVisual3D { Content = tempModelGroup };
            // viewport.Children.Add(ballModelVisual);

        }

        public void addGraphicsBody(GraphicsBody body)
        {
            bodies.Add(body);
            // have the body heava a mesh somehow
            // var modelVisual3DModel = new ModelVisual3D { Content = /*have some uniform mesh and pos data please*/ };
            // viewport.Children.Add(modelVisual3DModel);
        }

        private void setupRenderTimer()
        {
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
            
        }

        // public void drawGraphicsBody()
        // {
        //     // check maybe just have the timer do this port
        //     throw new NotImplementedException();
        // }
    }
}