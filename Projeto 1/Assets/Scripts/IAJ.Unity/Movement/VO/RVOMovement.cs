//class adapted from the HRVO library http://gamma.cs.unc.edu/HRVO/
//adapted to IAJ classes by João Dias

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.VO
{
    public class RVOMovement : DynamicMovement.DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "RVO"; }
        }

        protected List<KinematicData> Characters { get; set; }
        protected List<StaticData> Obstacles { get; set; }
        public float CharacterSize { get; set; }
        public float IgnoreDistance { get; set; }
        public float MaxSpeed { get; set; }
        //create additional properties if necessary

        protected DynamicMovement.DynamicMovement DesiredMovement { get; set; }

        public RVOMovement(DynamicMovement.DynamicMovement goalMovement, List<KinematicData> movingCharacters, List<StaticData> obstacles)
        {
            this.DesiredMovement = goalMovement;
            this.Characters = movingCharacters;
            this.Obstacles = obstacles;
            this.CharacterSize = 0.8f;
            this.IgnoreDistance = 3.5f;
            base.Target = new KinematicData();
            //initialize other properties if you think is relevant
        }

        public Vector3 GetBestSample(Vector3 desiredVelocity, List<Vector3> samples)
        {
            Vector3 bestSample = new Vector3(0.0f, 0.0f, 0.0f);
            float minimumPenalty = float.PositiveInfinity;

            float distancePenalty;
            float maximumTimePenalty;
            float maximumTimePenaltyObstacle;
            Vector3 deltaP;
            Vector3 rayVector;
            float tc;
            float timePenalty;
            float penalty;
            float w = 5.0f;

            foreach (Vector3 sample in samples)
            {
                distancePenalty = (desiredVelocity - sample).magnitude;
                maximumTimePenalty = 0;
                maximumTimePenaltyObstacle = 0;
                foreach (KinematicData b in Characters)
                {
                    if (b == this.Character) continue;

                    deltaP = b.Position - this.Character.Position;

                    if (deltaP.magnitude > IgnoreDistance) continue;

                    rayVector = 2 * sample - Character.velocity - b.velocity;
                    tc = MathHelper.TimeToCollisionBetweenRayAndCircle(Character.Position, rayVector, b.Position, CharacterSize * 2);

                    if (tc > 0)
                        timePenalty = w / tc;
                    else if (tc == 0)
                        timePenalty = float.PositiveInfinity;
                    else
                        timePenalty = 0;

                    if (timePenalty > maximumTimePenalty)
                        maximumTimePenalty = timePenalty;
                }

                foreach (StaticData obstacle in Obstacles)
                {
                    deltaP = obstacle.Position - this.Character.Position;

                    if (deltaP.magnitude > IgnoreDistance) continue;

                    rayVector = 2 * sample - Character.velocity;
                    tc = MathHelper.TimeToCollisionBetweenRayAndCircle(Character.Position, rayVector, obstacle.Position, CharacterSize * 2);

                    if (tc > 0)
                        timePenalty = w / tc;
                    else if (tc == 0)
                        timePenalty = float.PositiveInfinity;
                    else
                        timePenalty = 0;

                    if (timePenalty > maximumTimePenaltyObstacle)
                        maximumTimePenaltyObstacle = timePenalty;
                }
                penalty = distancePenalty + maximumTimePenalty + maximumTimePenaltyObstacle;

                if (penalty < minimumPenalty)
                {
                    minimumPenalty = penalty;
                    bestSample = sample;
                }

            }
            return bestSample;
        }
        public override MovementOutput GetMovement()
        {

            MovementOutput desiredOutput = this.DesiredMovement.GetMovement();
            Vector3 desiredVelocity = this.Character.velocity + desiredOutput.linear;
            //Debug.Log("desiredOutput.linear = " + desiredOutput.linear);
            if (desiredVelocity.magnitude > this.MaxSpeed)
            {
                desiredVelocity = desiredVelocity.normalized;
                desiredVelocity *= this.MaxSpeed;
            }

            List<Vector3> samples = new List<Vector3>();
            samples.Add(desiredVelocity);

            float angle;
            float magnitude;
            Vector3 velocitySample;
            for (var i = 0; i < 30; i++)
            {
                angle = Random.Range(0, Util.MathConstants.MATH_2PI);
                magnitude = Random.Range(0, MaxSpeed);
                velocitySample = Util.MathHelper.ConvertOrientationToVector(angle) * magnitude;
                samples.Add(velocitySample);
                //Debug.DrawRay(Character.Position, velocitySample, Color.gray);
            }
            base.Target.velocity = GetBestSample(desiredVelocity, samples);
            return base.GetMovement();
        }
    }
}
