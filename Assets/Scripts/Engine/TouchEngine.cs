using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agent;
using Agent.Controllers;
using Agent.Tools;
using UI;
using RPGCharacterAnimsFREE;

namespace Engine {

    public class TouchEngine : MonoBehaviour
    {
        //GameObject to spawn for spot touching
        [SerializeField] public GameObject gameObjectSpot;
        [SerializeField] public GameObject gameObjectAgent;
        [SerializeField] public GameObject gameObjectField;
        //Spot controls
        public int configSpotSpawnCount;
        public int configSpawnLocationDivider;
        public int configMaxEpisodeLength = 1000;
        public int activeSpots = 0;
        private Dictionary<int,GameObject?> spotInstance;
        private List<int> spotFound = new List<int>();
        private GameObject agentInstance;
        private Kempo kempoAgent;
        
        private bool spawningLock = false;
        private (float,float) gameArea;
        public int currentFrame = 0;
        
        public TouchEngine() {
            spotInstance = new Dictionary<int,GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            SetupLessonParameters();
            // Debug.Log("ENGINE - Spot Engine Start");
            HingeJoint hingeInactive = this.GetComponentInChildren(typeof(HingeJoint), true) as HingeJoint;

            gameArea = GetFieldArea(transform.Find("Plane").gameObject.GetComponent<Renderer>());
            InitialSpotSpawn(configSpotSpawnCount);
            SetupAgent(gameObjectAgent);
            GeneralUI.possible = activeSpots;

            

        }

        private void SetupLessonParameters() {
            configSpotSpawnCount = Academy.Instance.EnvironmentParameters.GetWithDefault("spotSpawnCount", configSpotSpawnCount);
            configSpawnLocationDivider = Academy.Instance.EnvironmentParameters.GetWithDefault("dropLocationDivider", configSpawnLocationDivider);
            configMaxEpisodeLength = Academy.Instance.EnvironmentParameters.GetWithDefault("maxframe", configMaxEpisodeLength);
        }

        private void SetupAgent(GameObject agent)
        {
            if(agentInstance)
            Destroy(agentInstance);
            agentInstance = Instantiate(agent, new Vector3(
                                                    RandomLoc(gameArea.Item1/configSpawnLocationDivider),
                                                    10.0f,
                                                    RandomLoc(gameArea.Item2/configSpawnLocationDivider)
                                                    ), Quaternion.identity,this.GetComponent<Transform>());
            agentInstance.transform.localPosition = new Vector3(
                                                    RandomLoc(gameArea.Item1/configSpawnLocationDivider),
                                                    10.0f,
                                                    RandomLoc(gameArea.Item2/configSpawnLocationDivider));

            //setup floor
            RPGCharacterController agentRpgController = agentInstance.GetComponentInParent<RPGCharacterController>();
            agentRpgController.floor = gameObjectField.GetComponent<Transform>();
            agentRpgController.Init();
            kempoAgent = agentInstance.GetComponent<Kempo>();

            
            //spotInstance[index] 
            if(activeSpots > 0) {
                UpdateAgentGoalCount();
            }
           
        }

        private void InitialSpotSpawn(int count) {
            if(!spawningLock) {
                spawningLock = true;
                // Debug.Log("ENGINE SPAWNING!!" + count);
                foreach (int index in Enumerable.Range(0, count))
                {

                    spotInstance[index] = Instantiate(gameObjectSpot, new Vector3(
                                                                                    RandomLoc(gameArea.Item1/configSpawnLocationDivider),
                                                                                    10.0f,
                                                                                    RandomLoc(gameArea.Item2/configSpawnLocationDivider)
                                                                                    ), Quaternion.identity,GetComponent<Transform>());
                    //spotInstance[index].transform.parent = GetComponent<Transform>().root.GetComponent<Transform>();
                    spotInstance[index].transform.localPosition = new Vector3(
                                                    RandomLoc(gameArea.Item1/configSpawnLocationDivider),
                                                    10.0f,
                                                    RandomLoc(gameArea.Item2/configSpawnLocationDivider));
                    var spotScript = spotInstance[index].GetComponent<Spot>();
                    spotScript.SetEngine(this,index);
                    // Debug.Log("ENGINE - Spawning Spot " + index);
                }
                activeSpots = count;
                spawningLock = false;
            }
        }

        private void SpotSpawn(int count) {
            if(spawningLock == false) {
                spawningLock = true;
                // Debug.Log("ENGINE SPAWNING!!" + count);
                foreach (KeyValuePair<int, GameObject> spot in spotInstance)
                {
                    spot.Value.transform.localPosition = new Vector3(RandomLoc(gameArea.Item1/configSpawnLocationDivider), 10.1f, RandomLoc(gameArea.Item2/configSpawnLocationDivider));
                    spot.Value.GetComponent<Rigidbody>().useGravity = true;
                }
                activeSpots = count;
                spawningLock = false;
            }
        }

        private (float,float) GetFieldArea(Renderer field) {
            return ((int)System.Math.Round(field.bounds.size.x)/2,(int)System.Math.Round(field.bounds.size.z)/2);
        }

        private float Invert(float i) {
            return i * -1;
        }

        private float RandomLoc(float i){
            return Random.Range(this.Invert(i), i);
        }

        private void FixedUpdate() {
            if(activeSpots <= 0 && spawningLock == false) {
                if(spotFound.Count > 1) {
                    RewardAgentRewardFinished();
                }
                Resetboard();
            }
            if(IsAgentDead(agentInstance) && spawningLock == false) {
                PenalizeAgentDead();
                Resetboard();
            }
            
        }
        private void Update() {
            GeneralUI.ScreenText();
            currentFrame++;

            if(currentFrame >= configMaxEpisodeLength) {
                PenalizeAgentTime();
                Resetboard();
            } else {
                PenalizeAgentforiteration();
            }
        }
        private void RewardAgentRewardFinished() {
            GeneralUI.success += 1;
            GeneralUI.points += 0;
            SignalAgent("RewardFinished");
        }
        private void SignalAgentEngineReset() {
            GeneralUI.episode += 1;
            SignalAgent("EngineReset");
        }
        private bool IsAgentDead(GameObject agentObject) {
            return (agentObject.GetComponent<Transform>().position.y < 0);
        }

        private void RewardAgentGoal() {
            GeneralUI.points += 1;
            SignalAgent("RewardGoal");
        }

        private void PenalizeAgentDead() {
            GeneralUI.fail += 1;
            SignalAgent("Dead");
        }

        private void PenalizeAgentTime() {
            GeneralUI.fail += 1;
            SignalAgent("TimeUp");
        }

        private void PenalizeAgentforiteration() {
            SignalAgent("IterationPenalty");
        }
        
        private void UpdateAgentGoalCount() {
            SignalAgent("UpdateGoalCount",activeSpots/configSpotSpawnCount);

        }

        private void SignalAgent(string signal, int? payLoad = null) {
            if(payLoad == null) {
               kempoAgent.SendMessage(signal);
               return;
            }
            kempoAgent.SendMessage(signal,payLoad);
            
        }

        public void SpotRemove(int index) {
            // Debug.Log("ENGINE - Removing Spot " + index);
            if(activeSpots > 0)
                activeSpots = activeSpots - 1;
            spotInstance[index].transform.localPosition = new Vector3((index * 5) +35, 10.1f, 55);
            spotInstance[index].GetComponent<Rigidbody>().useGravity = false;
            spotInstance[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
            UpdateAgentGoalCount();
            // Debug.Log("ENGINE - Count " + activeSpots);
        }

        public void SpotFound(int index) {
            // Debug.Log("ENGINE - FoundSignal From " + index);
            if(!spotFound.Contains(index)) {
                SpotRemove(index);
                spotFound.Add(index);
                RewardAgentGoal();
            }
        }

        public void SpotLost(int index) {
            // Debug.Log("ENGINE - LostSignal From " + index);
            if(spotInstance.ContainsKey(index)) {
                SpotRemove(index);
                UpdateAgentGoalCount();
            }
        }


        private void ResetAgent() {
            agentInstance.transform.localPosition = new Vector3(
                RandomLoc(gameArea.Item1/configSpawnLocationDivider),
                0.1f,
                RandomLoc(gameArea.Item2/configSpawnLocationDivider));

        }        

        private void Resetboard() {
             SetupLessonParameters();
             SignalAgentEngineReset();
             ResetAgent();
             SpotSpawn(configSpotSpawnCount);
             
             currentFrame = 0;
             spotFound.Clear();
             
             var agentTotalReward = kempoAgent.getTotalRewards();
             GeneralUI.reward = agentTotalReward; 

        }
    }
}
