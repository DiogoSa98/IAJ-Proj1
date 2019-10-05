using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidCharacter : DynamicMovement
    {
        public KinematicData Character { get; set; }
        public KinematicData Target { get; set; }
        public float MaxAcceleration { get; set; }
        public float CollisionRadius { get; set; }
        public float LookAhead { get; set; }

        public DynamicAvoidCharacter(KinematicData target)
        {
            Target = target;
            this.Output = new MovementOutput();
            LookAhead = 2.0f;
        }

        public override string Name
        {
            get
            {
                return "AvoidCharacter";
            }
        }


        public override MovementOutput GetMovement()
        {
            Vector3 deltaPos = this.Target.Position - this.Character.Position;
            Vector3 deltaVel = this.Target.velocity - this.Character.velocity;
            float deltaSqrSpeed = deltaVel.sqrMagnitude;

            if (deltaSqrSpeed == 0) return new MovementOutput();

            float timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / deltaSqrSpeed;

            if (timeToClosest > LookAhead) return new MovementOutput();

            Vector3 futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            float futureDistance = futureDeltaPos.magnitude;

            if (futureDistance > 2 * CollisionRadius) return new MovementOutput();
            if (futureDistance <= 0 || deltaPos.magnitude < 2 * this.CollisionRadius)
                Output.linear = this.Character.Position - this.Target.Position;
            else
                Output.linear = -futureDeltaPos;
            Output.linear = Output.linear.normalized * MaxAcceleration;
            return Output;
        }
    }
}
