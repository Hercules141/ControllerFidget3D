using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Timers;
using System.Windows.Media.Media3D;
using Assimp;
using HelixToolkit.Wpf;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace ControllerFidget3D
{
    public class GraphicsWorld// : IGraphicsWorld
    {
        private HelixViewport3D viewport;
        private Timer renderTimer;

        // private List<GraphicsBody> bodies;
        private List<ModelVisual3D> models;
        
        private Action renderTick;
        
        public GraphicsWorld(HelixViewport3D viewport, Action renderTick) // stupid but I need more architecture to be able to cross access phsics and graphics objects for updating and synchronizing
        {
            this.renderTick = renderTick;
            this.viewport = viewport;
            
            models = new List<ModelVisual3D>();
            
            setupHelix();
        }

        private void setupHelix()
        {
            setupRenderTimer();
        }

        // public void addGraphicsBody(GraphicsBody body)
        // {
        //     bodies.Add(body);
        //     // have the body heava a mesh somehow
        //     // var modelVisual3DModel = new ModelVisual3D { Content = /*have some uniform mesh and pos data please*/ };
        //     // viewport.Children.Add(modelVisual3DModel);
        // }

        public void addObject(ModelVisual3D model)
        {
            viewport.Children.Add(model);
            
            models.Add(model);
        }
        
        private void setupRenderTimer()
        {
            // set Render Timer
            renderTimer = new Timer(1000/40);
            renderTimer.Elapsed += (sender, args) =>
            {
                renderTick();
            };
            renderTimer.Start();
        }

        // private renderTick()
        // {
        //     
        // }

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

        public List<ModelVisual3D> GetObjects()
        {
            return models;
        }
    }

}