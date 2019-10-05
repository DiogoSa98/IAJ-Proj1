using UnityEngine;
using System.Collections;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        public float AvoidDistance { get; set; }
        public float LookAhead { get; set; }
        public GameObject Obstacle { get; set; }

        public override string Name
        {
            get
            {
                return "AvoidObstacle";
            }
        }

        public DynamicAvoidObstacle(GameObject obstacle)
        {
            this.Obstacle = obstacle;
        }
        public override MovementOutput GetMovement()
        {


            if (Character.velocity.sqrMagnitude > 0)
            {

                Vector3 rayVector = Character.velocity.normalized * LookAhead;
                Vector3 leftWhisker = Util.MathHelper.Rotate2D(rayVector, Util.MathConstants.MATH_PI_4) * 1 / 2;
                Vector3 rightWhisker = Util.MathHelper.Rotate2D(rayVector, -Util.MathConstants.MATH_PI_4) * 1 / 2;

                Debug.DrawRay(Character.Position, rayVector, Color.red);
                Debug.DrawRay(Character.Position, leftWhisker, Color.blue);
                Debug.DrawRay(Character.Position, rightWhisker, Color.green);

                //collision
                RaycastHit hitRay;
                RaycastHit hitLeft;
                RaycastHit hitRight;

                bool collisionRay = Obstacle.GetComponent<Collider>().Raycast(new Ray(Character.Position, rayVector), out hitRay, LookAhead);
                bool collisionLeft = Obstacle.GetComponent<Collider>().Raycast(new Ray(Character.Position, leftWhisker), out hitLeft, LookAhead);
                bool collisionRight = Obstacle.GetComponent<Collider>().Raycast(new Ray(Character.Position, rightWhisker), out hitRight, LookAhead);

                if (collisionRay) base.Target.Position = hitRay.point + hitRay.normal * AvoidDistance;
                else if (collisionLeft) base.Target.Position = hitLeft.point + hitLeft.normal * AvoidDistance;
                else if (collisionRight) base.Target.Position = hitRight.point + hitRight.normal * AvoidDistance;
                else return new MovementOutput();

                return base.GetMovement();
            }
            return new MovementOutput();
        }

    }
}
