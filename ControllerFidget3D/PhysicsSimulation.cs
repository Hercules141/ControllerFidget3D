

using System;
using System.Timers;
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

        // test objects
        RigidBody groundBody;
        
        public RigidBody ball;
        public Vector3 ballPosition;


        public PhysicsSimulation()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            broadphase = new DbvtBroadphase();
            solver = new SequentialImpulseConstraintSolver();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, collisionConfiguration);
            
            // start simulation loop
            simulationTimer = new Timer(1000/60);
            simulationTimer.Elapsed += (sender, args) =>
            {
                simulationTick();
            };
            simulationTimer.Start();
            
            addTestObject();
        }

        private void addTestObject()
        {
            var groundShape = new BoxShape(50, 1, 50);
            // var groundTransform = Matrix.Translation(0, -1, 0);
            var groundMotionState = new DefaultMotionState();
            var groundMass = 0.0f; // Mass of 0 means static object
            var constructionInfo = new RigidBodyConstructionInfo(groundMass, groundMotionState, groundShape);
            groundBody = new RigidBody(constructionInfo);
            dynamicsWorld.AddRigidBody(groundBody);
            
            //ball init
            var ballShape = new SphereShape(50);
            var ballMotionState = new DefaultMotionState();
            var ballMass = 1;
            var ballConstructionInfo = new RigidBodyConstructionInfo(ballMass, ballMotionState, ballShape);
            ball = new RigidBody(ballConstructionInfo);
            dynamicsWorld.AddRigidBody(ball);
        }

        private void simulationTick()
        {
            dynamicsWorld.StepSimulation(1 / 60f);
            // Console.WriteLine("World User Info: " + dynamicsWorld.WorldUserInfo);
            // Console.WriteLine("ground Motion State: " + groundBody.MotionState);
            // Console.WriteLine("ball world transfor basis: " + ball.MotionState.WorldTransform.Basis);
            // Console.WriteLine("ball info" + ball.);
            
            // test
            Matrix ballTransform;
            ball.MotionState.GetWorldTransform(out ballTransform);
            ballPosition = ballTransform.Origin;

            Console.WriteLine("Ball Pos: " + ballPosition);
        }
    }
}