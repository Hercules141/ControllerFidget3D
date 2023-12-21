using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows.Media.Media3D;
using Assimp;
using HelixToolkit.Wpf;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

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
        
        public MeshGeometry3D ConvertAssimpMeshToMeshGeometry3D(Mesh assimpMesh)
        {
            var meshGeometry3D = new MeshGeometry3D();

            // Extract vertices
            foreach (var vertex in assimpMesh.Vertices)
            {
                meshGeometry3D.Positions.Add(new Point3D(vertex.X, vertex.Y, vertex.Z));
            }

            // Extract normals (if available)
            if (assimpMesh.HasNormals)
            {
                foreach (var normal in assimpMesh.Normals)
                {
                    meshGeometry3D.Normals.Add(new Vector3D(normal.X, normal.Y, normal.Z));
                }
            }

            // Extract indices
            foreach (var face in assimpMesh.Faces)
            {
                foreach (var index in face.Indices)
                {
                    meshGeometry3D.TriangleIndices.Add(index);
                }
            }

            return meshGeometry3D;
        }
    }

}