

using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Assimp;
using BulletSharp;
using BulletSharp.Math;


namespace ControllerFidget3D{
    
    public class PhysicsSimulation
    {
        DefaultCollisionConfiguration collisionConfiguration;
        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        SequentialImpulseConstraintSolver solver;
        DiscreteDynamicsWorld dynamicsWorld;

        private Timer simulationTimer;

        private List<RigidBody> objects;

        // test objects
        // RigidBody groundBody;
        //
        // private RigidBody testBoard;
        // public RigidBody ball;
        // public Vector3 ballPosition;


        public PhysicsSimulation()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            broadphase = new DbvtBroadphase();
            solver = new SequentialImpulseConstraintSolver();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, collisionConfiguration);

            objects = new List<RigidBody>();
        // addTestScene();
            
            // start simulation loop
            simulationTimer = new Timer(1000/60);
            simulationTimer.Elapsed += (sender, args) =>
            {
                simulationTick();
            };
            simulationTimer.Start();
        }

        public void addRigidBody(RigidBody rigidBody)
        {
            dynamicsWorld.AddRigidBody(rigidBody);
            objects.Add(rigidBody);
        }

        public List<RigidBody> getRigidBodies()
        {
            return objects;
        }
        
        private void addTestScene()
        {
        //     var importer = new AssimpContext();
        //     var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "game.obj");
        //     var scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
        //
        //     var listOfTriangleMeshes = getTriangleMeshesFromScene(scene);
        //     
        //     listOfTriangleMeshes.ForEach( mesh =>
        //         {
        //             dynamicsWorld.AddRigidBody(new RigidBody(new RigidBodyConstructionInfo(1, new DefaultMotionState(), mesh)));
        //             Console.WriteLine("Added Mesh to physics simulation");
        //         }
        //     );
        }

        
        
        private void addTestObject()
        {
            // var groundShape = new BoxShape(50, 1, 50);
            // // var groundTransform = Matrix.Translation(0, -1, 0);
            // var groundMotionState = new DefaultMotionState();
            // var groundMass = 0.0f; // Mass of 0 means static object
            // var constructionInfo = new RigidBodyConstructionInfo(groundMass, groundMotionState, groundShape);
            // groundBody = new RigidBody(constructionInfo);
            // dynamicsWorld.AddRigidBody(groundBody);
            
            // // test load blender board mesh
            // var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "field.obj");
            // var testBoardMesh = LoadObjMeshAsTriangleMesh(modelPath);
            //
            // var testBoardConstructionInfo = new RigidBodyConstructionInfo(0, new DefaultMotionState(), testBoardMesh);
            // testBoard = new RigidBody(testBoardConstructionInfo);
            // dynamicsWorld.AddRigidBody(testBoard);
            //
            // // load blender ball
            // var ballModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "ball.obj");
            // var ballMesh = LoadObjMeshAsTriangleMesh(ballModelPath);
            //
            // var ballConstructionInfo = new RigidBodyConstructionInfo(1, new DefaultMotionState(), ballMesh);
            // ball = new RigidBody(ballConstructionInfo);
            // ball.Translate(new Vector3(0, 20, 0)); // starting pos
            // dynamicsWorld.AddRigidBody(ball);
        }

        private void simulationTick()
        {
            dynamicsWorld.StepSimulation(1000/60);
            // Console.WriteLine("World User Info: " + dynamicsWorld.WorldUserInfo);
            // Console.WriteLine("ground Motion State: " + groundBody.MotionState);
            // Console.WriteLine("ball world transfor basis: " + ball.MotionState.WorldTransform.Basis);
            // Console.WriteLine("ball info" + ball.);
            
            // test
            // Matrix ballTransform;
            // ball.MotionState.GetWorldTransform(out ballTransform);
            // ballPosition = ballTransform.Origin;
            //
            // Console.WriteLine("Ball Pos: " + ballPosition);
        }

        // public void applyForceToBall(Vector3 forceVector)
        // {
        //     ball.ApplyForce(forceVector, ball.CenterOfMassPosition);
        // }
        
        // helper functions
       public List<BvhTriangleMeshShape> getTriangleMeshesFromScene(Scene scene)
       {
           return scene.Meshes.Select(m => MeshToTriangleMesh(m)).ToList();
       }

       BvhTriangleMeshShape MeshToTriangleMesh(Mesh mesh)
       {
           var vertices = mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList();
           var indices = mesh.GetIndices().ToList();

           // to triangle mesh
           var triangleMesh = new TriangleMesh();

           for (int i = 0; i < indices.Count; i += 3)
           {
               Vector3 vertex1 = vertices[indices[i]];
               Vector3 vertex2 = vertices[indices[i + 1]];
               Vector3 vertex3 = vertices[indices[i + 2]];

               triangleMesh.AddTriangle(vertex1, vertex2, vertex3);
           }

           return new BvhTriangleMeshShape(triangleMesh, true);
       }
    }
}