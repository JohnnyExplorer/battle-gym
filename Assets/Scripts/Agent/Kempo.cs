using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Agent.Controllers;
namespace Agent {

    public class Kempo : Unity.MLAgents.Agent
    {
        public int goalCount;
        public AgentInputController agentInputController;
        public Rigidbody rBody;
        public float rewardGoal = 2f;
        public float rewardFinished = 2;
        public float penaltyFall = -2;
        public float penaltyTimeOut = -1;

        public override void Initialize() {
            // Debug.Log("Agent - Initialize");
            agentInputController = GetComponent<AgentInputController>();
            rBody = GetComponent<Rigidbody>();
        }
        public void EngineReset() {
            // Debug.Log("Agent - EngineReset");
            EndEpisode();
        }

        public void RewardGoal() {
            // Debug.Log("Agent - RewardGoal");
            AddReward(rewardGoal/goalCount);
            EndEpisode();
        }

        public void UpdateGoalCount(int count) {
            // Debug.Log("Agent - UpdateGoalCount");
            goalCount = count;

        }

        public void RewardFinished() {
            // Debug.Log("Agent - RewardFinished");
            AddReward(rewardFinished);
        }

        public void Dead() {
            // Debug.Log("Agent - Dead");
            AddReward(penaltyFall);
           
        }

        private void TimeUp() {
            // Debug.Log("Agent - Time");
            AddReward(penaltyFall);
        }
        public override void OnEpisodeBegin()
        {
            // Debug.Log("Agent - OnEpisodeBeing");
        }

        public override void CollectObservations(VectorSensor sensor)
        {

            // Debug.Log("Agent - CollectObservations");
            // Target and Agent positions & Agent velocity
            sensor.AddObservation(this.transform.localPosition);
            sensor.AddObservation(rBody.velocity);
            sensor.AddObservation(goalCount);
        }
        public override void OnActionReceived( Unity.MLAgents.Actuators.ActionBuffers vectorAction)
        {
            // Debug.Log("Agent - action : vectorAction.ContinuousActions " + vectorAction);
            // Debug.Log("Agent - action : vectorAction.ContinuousActions " + vectorAction.ContinuousActions);
            // Debug.Log("Agent - action : vectorAction.DiscreteActions " + vectorAction.DiscreteActions);
            agentInputController.inputVertical = vectorAction.DiscreteActions[0] - 1;
            // Debug.Log("Agent - Control vectorAction[0] " + vectorAction.DiscreteActions[0].GetType());
            agentInputController.inputHorizontal = vectorAction.DiscreteActions[1] - 1;
            // Debug.Log("Agent - Control vectorAction[1] " + vectorAction.DiscreteActions[1]);
                        
        }
    }

}
