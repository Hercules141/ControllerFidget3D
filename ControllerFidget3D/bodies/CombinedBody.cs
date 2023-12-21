
using BulletSharp.Math;

namespace ControllerFidget3D
{
    public interface ICombinedBody
    {

        Vector3 getPosition();
        
        Vector3 getOrientation();

        // Vector3 getSpeed();

        // Vector3 getMotionState();


        void toString();
    }
}