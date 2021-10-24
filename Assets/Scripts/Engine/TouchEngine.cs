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
        public int spotPoolSize = 10;
        public float configSpotSpawnCount;
        public float configSpawnLocationDivider;
        public float configMaxEpisodeLength = 1000;
        public float activeSpots = 0;
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
            InitialSpotSpawn(spotPoolSize);
            SpotSpawn(configSpotSpawnCount);
            SetupAgent(gameObjectAgent);
            GeneralUI.possible = (int)activeSpots;

            

        }

        private void SetupLessonParameters() {

            Debug.Log("Got me some values configSpotSpawnCount " + (int) Academy.Instance.EnvironmentParameters.GetWithDefault("configSpotSpawnCount", configSpotSpawnCount));
            Debug.Log("Got me some values configSpawnLocationDivider " + Academy.Instance.EnvironmentParameters.GetWithDefault("configSpawnLocationDivider", configSpawnLocationDivider));
            Debug.Log("Got me some values configMaxEpisodeLength " + (int) Academy.Instance.EnvironmentParameters.GetWithDefault("configMaxEpisodeLength", configMaxEpisodeLength));
            configSpotSpawnCount = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("configSpotSpawnCount", configSpotSpawnCount);
            configSpawnLocationDivider = Academy.Instance.EnvironmentParameters.GetWithDefault("configSpawnLocationDivider", configSpawnLocationDivider);
            configMaxEpisodeLength =  (int) Academy.Instance.EnvironmentParameters.GetWithDefault("configMaxEpisodeLength", configMaxEpisodeLength);
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

        private void InitialSpotSpawn(float spotPoolSize) {
            // Debug.Log("ENGINE SPAWNING!!" + count);
            foreach (int index in Enumerable.Range(0, (int) spotPoolSize))
            {

                spotInstance[index] = Instantiate(gameObjectSpot, new Vector3(
                                                                                RandomLoc(gameArea.Item1/configSpawnLocationDivider),
                                                                                10.0f,
                                                                                RandomLoc(gameArea.Item2/configSpawnLocationDivider)
                                                                                ), Quaternion.identity,GetComponent<Transform>());
                SpotPool(index);
                var spotScript = spotInstance[index].GetComponent<Spot>();
                spotScript.SetEngine(this,index);
            }
            spawningLock = false;
        }

        private void SpotSpawn(float count) {
            if(spawningLock == false) {
                spawningLock = true;
                // Debug.Log("ENGINE SPAWNING!!" + count);
                foreach (int index in Enumerable.Range(0, (int) count))
                {
                    spotInstance[index].transform.localPosition = new Vector3(RandomLoc(gameArea.Item1/configSpawnLocationDivider), 10.1f, RandomLoc(gameArea.Item2/configSpawnLocationDivider));
                    spotInstance[index].GetComponent<Rigidbody>().useGravity = true;
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

        private void SignalAgent(string signal, float? payLoad = null) {
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
            SpotPool(index);
            UpdateAgentGoalCount();
            // Debug.Log("ENGINE - Count " + activeSpots);
        }

        public void SpotPool(int index) {
            spotInstance[index].transform.localPosition = new Vector3((index * -5) +35, 10.1f, -35);
            spotInstance[index].GetComponent<Rigidbody>().useGravity = false;
            spotInstance[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
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

        private void ResetSpot() {
            foreach (int index in Enumerable.Range(0, (int) spotPoolSize))
            {
                SpotRemove(index);
            }
        }    

        private void Resetboard() {
             SetupLessonParameters();
             SignalAgentEngineReset();
             ResetAgent();
             ResetSpot();
             SpotSpawn(configSpotSpawnCount);
             
             currentFrame = 0;
             spotFound.Clear();
             
             var agentTotalReward = kempoAgent.getTotalRewards();
             GeneralUI.reward = agentTotalReward; 

        }
    }
}
