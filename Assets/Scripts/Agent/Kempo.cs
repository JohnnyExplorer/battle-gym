using UnityEngine;
using Unity.MLAgents;
using Agent.Controllers;
namespace Agent {

    public class Kempo : Agent
    {

        public bool dead;
        public AgentInputController agentInputController;
        public TouchEngine engine;

        public float rewardGoal = 0.3f;
        public float rewardFinished = 1.0f;
        public float penaltyFall = -0.9f;
        public float penaltyWalk = -0.001f;
        public override void Initialize()
        {
            agentInputController = getComponent<AgentInputController>();
            engine = this.transform.parent.transform.Find("TouchEngine").gameObject;
        }
        public void EngineReset() {
            EndEpisode();
        }

        public void RewardGoal() {
            AddReward(rewardGoal);
        }

        public void RewardFinished() {
            AddReward(rewardFinished);
        }

        private void Dead() {
            AddReward(penaltyFall);
            EndEpisode();
        }
        public override void OnEpisodeBegin()
        {
            Globals.Episode += 1;
        }
        public override void OnActionReceived(float[] vectorAction)
        {
            this.transform.Translate(Vector3.right * vectorAction[0] * Movespeed * Time.deltaTime);
            this.transform.Translate(Vector3.forward * vectorAction[1] * Movespeed * Time.deltaTime);
            BoundCheck();
            Globals.ScreenText();
        }
        
}

    private void RandomPlaceTarget()
    {
        float rx = Random.Range(bndFloor.min.x, bndFloor.max.x);
        float rz = Random.Range(bndFloor.min.z, bndFloor.max.z);
        Target.transform.position = new Vector3(rx, 0, rz);
    }
    private void BoundCheck()
    {
        if (this.transform.position.x < bndFloor.min.x)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }
        else if (this.transform.position.x > bndFloor.max.x)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }

        if (this.transform.position.z < bndFloor.min.z)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }
        else if (this.transform.position.z > bndFloor.max.z)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }
    }
}