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
        private float rewardGoal = 8f;
        private float rewardFinished = 2f;
        private float penaltyFall = -3f;
        private float penaltyTimeOut = -2f;

        private float iterationPenalty = -0.001f;

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
            float reward = rewardGoal / (goalCount + 1);
            Debug.Log(string.Format("Agent({0}) - Reward: {1} for spot {2}/{3}",
            rootName,reward,goalCount,rewardGoal));
            AddReward(reward);
            totalRewards += reward;
            Debug.Log(rootName + " Agent - TOTAL REWARD" + totalRewards);
        }

        public void UpdateGoalCount(float count) {
            Debug.Log(string.Format(" Agent({0}) - UpdateGoalCount {1}",rootName,count));
            goalCount = count;

        }

        public void IterationPenalty() {
            Debug.Log(rootName + " Agent - IterationPenalty" + iterationPenalty);
            AddReward(iterationPenalty);
            totalRewards += iterationPenalty;
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
            Debug.Log(rootName + " Agent - Time" + penaltyTimeOut);
            AddReward(penaltyTimeOut);
            totalRewards += penaltyTimeOut;
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
            //sensor.AddObservation(rBody.velocity);
            sensor.AddObservation(goalCount);
        }
        public override void OnActionReceived( Unity.MLAgents.Actuators.ActionBuffers vectorAction)
        {
            // Debug.Log(rootName + " Agent - action : vectorAction.ContinuousActions 0" + vectorAction.ContinuousActions[0]);
            // Debug.Log(rootName + " Agent - action : vectorAction.ContinuousActions 1" + vectorAction.ContinuousActions[1]);

            
             agentInputController.inputVertical = vectorAction.ContinuousActions[0];
             agentInputController.inputHorizontal = vectorAction.ContinuousActions[1];
            //agentInputController.inputVertical = vectorAction.DiscreteActions[0] - 1;
            //agentInputController.inputHorizontal = vectorAction.DiscreteActions[1] - 1;

        }

        public float getTotalRewards() {
            return totalRewards;
        }
    }

}
