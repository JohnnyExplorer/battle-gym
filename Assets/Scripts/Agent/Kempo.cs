using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Agent.Controllers;
using UI;
using System;
using System.Linq;
using Agent.State;
namespace Agent
{

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

        private float inputVertical;
        private float inputHorizontal;

        private int action;

        private Vector3? seekValue = null;

        public AgentState agentState;

        public override void Initialize()
        {
            // Debug.Log(rootName + " Agent - Initialize");
            agentInputController = GetComponent<AgentInputController>();
            rBody = GetComponent<Rigidbody>();
            rootName = transform.root.gameObject.name;
        }

        public void SetState(AgentState state) {
            agentState = state;
        }
        public void EngineReset()
        {
            Debug.Log(rootName + "Agent - EngineReset - finalScore" + totalRewards);
            AgentReset();
            EndEpisode();
            
        }

        public void AgentReset()
        {
            totalRewards = 0;
            action = 0;
        }

        public void GoalReset(){
            agentState.Reset();
        }

        public void RewardGoal()
        {
            float reward = rewardGoal / (goalCount + 1);
            Debug.Log(string.Format("Agent({0}) - Reward: {1} for spot {2}/{3}",
            rootName, reward, goalCount, rewardGoal));
            AddReward(reward);
            totalRewards += reward;
            Debug.Log(rootName + " Agent - TOTAL REWARD" + totalRewards);
            GoalReset();
            EndEpisode();
        }

        public void UpdateGoalCount(float count)
        {
            Debug.Log(string.Format(" Agent({0}) - UpdateGoalCount {1}", rootName, count));
            goalCount = count;

        }

        public void IterationPenalty()
        {
            Debug.Log(rootName + " Agent - IterationPenalty" + iterationPenalty);
            AddReward(iterationPenalty);
            totalRewards += iterationPenalty;
        }

        public void RewardFinished()
        {
            Debug.Log(rootName + " Agent - RewardFinished" + rewardFinished);
            AddReward(rewardFinished);
            totalRewards += rewardFinished;
        }

        public void Dead()
        {
            Debug.Log(rootName + " Agent - Dead" + penaltyFall);
            AddReward(penaltyFall);
            totalRewards += penaltyFall;

        }

        private void TimeUp()
        {
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
            sensor.AddObservation(rBody.velocity);
            sensor.AddObservation(goalCount);
            sensor.AddObserVation(agentState.seek.value);
        }

        public void Update()
        {
            agentInputController.inputVertical = inputVertical;
            agentInputController.inputHorizontal = inputHorizontal;
            GeneralUI.x = agentInputController.inputHorizontal;
            GeneralUI.y = agentInputController.inputVertical;
            if(useSeek == 1) {
                agentState.UseSeek();
            }
        }
        public override void OnActionReceived(Unity.MLAgents.Actuators.ActionBuffers vectorAction)
        {
            Debug.Log(rootName + " Agent - action : vectorAction.ContinuousActions 0" + vectorAction.ContinuousActions[0]);
            Debug.Log(rootName + " Agent - action : vectorAction.ContinuousActions 1" + vectorAction.ContinuousActions[1]);
            // float[] y = { inputVertical, vectorAction.ContinuousActions[0] };
            // float[] x = { inputHorizontal, vectorAction.ContinuousActions[1] };
            // inputVertical = (float)1 / 2 * y.Sum();
            // inputHorizontal = (float)1 / 2 * x.Sum();
            useSeek = vectorAction.discreteActions[0];
            inputVertical = vectorAction.ContinuousActions[0];
            inputHorizontal = vectorAction.ContinuousActions[1];

            // agentInputController.inputVertical = inputVertical;
            // agentInputController.inputHorizontal = inputHorizontal;
        }

        public float getTotalRewards()
        {
            return totalRewards;
        }
    }

}
