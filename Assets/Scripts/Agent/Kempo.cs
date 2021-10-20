using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Agent.Controllers;
namespace Agent {

    public class Kempo : Unity.MLAgents.Agent
    {
        public float goalCount;
        public AgentInputController agentInputController;
        public Rigidbody rBody;
        public float rewardGoal = 1f;
        public float rewardFinished = 2f;
        public float penaltyFall = -2f;
        public float penaltyTimeOut = -1f;

        private string rootName = "";

        private float totalRewards = 0;

        public override void Initialize() {
            // Debug.Log(rootName + " Agent - Initialize");
            agentInputController = GetComponent<AgentInputController>();
            rBody = GetComponent<Rigidbody>();
            rootName = transform.root.gameObject.name;
        }
        public void EngineReset() {
            Debug.Log(rootName + "Agent - EngineReset - finalScore" + totalRewards);
            AgentReset();
            EndEpisode();
        }

        public void AgentReset() {
            totalRewards = 0;
        }

        public void RewardGoal() {
            Debug.Log(rootName + " rewardGoal " + rewardGoal);
            Debug.Log(rootName + " goalCount " + goalCount);
            float reward = rewardGoal / (goalCount + 0.1f);
            Debug.Log(rootName + " Agent - RewardGoal" + reward);
            AddReward(reward);
            totalRewards += reward;
            Debug.Log(rootName + " Agent - TOTAL REWARD" + totalRewards);
            EndEpisode();
        }

        public void UpdateGoalCount(int count) {
            // Debug.Log(rootName + " Agent - UpdateGoalCount");
            goalCount = count;

        }

        public void RewardFinished() {
            Debug.Log(rootName + " Agent - RewardFinished" + rewardFinished);
            AddReward(rewardFinished);
            totalRewards += rewardFinished;
        }

        public void Dead() {
            Debug.Log(rootName + " Agent - Dead" + penaltyFall);
            AddReward(penaltyFall);
            totalRewards += penaltyFall;
           
        }

        private void TimeUp() {
            Debug.Log(rootName + " Agent - Time" + penaltyFall);
            AddReward(penaltyFall);
            totalRewards += penaltyFall;
        }
        public override void OnEpisodeBegin()
        {
            // Debug.Log(rootName + " Agent - OnEpisodeBeing");
        }

        public override void CollectObservations(VectorSensor sensor)
        {

            // Debug.Log(rootName + " Agent - CollectObservations");
            // Target and Agent positions & Agent velocity
            sensor.AddObservation(this.transform.localPosition);
            sensor.AddObservation(rBody.velocity);
            sensor.AddObservation(goalCount);
        }
        public override void OnActionReceived( Unity.MLAgents.Actuators.ActionBuffers vectorAction)
        {
            // Debug.Log(rootName + " Agent - action : vectorAction.ContinuousActions " + vectorAction);
            // Debug.Log(rootName + " Agent - action : vectorAction.ContinuousActions " + vectorAction.ContinuousActions);
            // Debug.Log(rootName + " Agent - action : vectorAction.DiscreteActions " + vectorAction.DiscreteActions);
            agentInputController.inputVertical = vectorAction.DiscreteActions[0] - 1;
            // Debug.Log(rootName + " Agent - Control vectorAction[0] " + vectorAction.DiscreteActions[0].GetType());
            agentInputController.inputHorizontal = vectorAction.DiscreteActions[1] - 1;
            // Debug.Log(rootName + " Agent - Control vectorAction[1] " + vectorAction.DiscreteActions[1]);
                        
        }

        public float getTotalRewards() {
            return totalRewards;
        }
    }

}
