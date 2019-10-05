using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{

    public class DynamicArrive : DynamicVelocityMatch
    {
        public float MaxSpeed { get; set; }
        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }
        public KinematicData DestinationTarget { get; set; }

        public DynamicArrive()
        {
            this.Target = new KinematicData();
        }

        public override string Name
        {
            get { return "Arrive"; }
        }

        public override MovementOutput GetMovement()
        {
            //TODO implement Dynamic Arrive movement



            Vector3 direction = this.DestinationTarget.Position - this.Character.Position;
            float distance = direction.magnitude;

            float DesiredSpeed;

            if (distance < StopRadius)
            {
                DesiredSpeed = 0;
            }
            else if (distance > SlowRadius)
            {
                DesiredSpeed = MaxSpeed;
            }
            else
            {
                DesiredSpeed = MaxSpeed * (distance / SlowRadius);
            }
            base.Target.velocity = direction.normalized * DesiredSpeed;

            return base.GetMovement();
        }
    }
}