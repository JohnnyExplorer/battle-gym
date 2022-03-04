        using UnityEngine;
 using System.Collections;
 
namespace Battle_Gym.Assets.Scripts.Tools
{
    public class MarksmanH : MonoBehaviour {
        [SerializeField] public GameObject gameObjectTarget;
        public Transform center;
        public Vector3 axis = Vector3.up;
        public Vector3 desiredPosition;
        public float radius = 2.0f;
        public float radiusSpeed = 0.5f;
        public float rotationSpeed = 80.0f;
    
        void Start () {
            center = gameObjectTarget.transform;
            transform.position = (transform.position - center.position).normalized * radius + center.position;
            radius = 2.0f;
        }
        
        void Update () {
            orbit();
        }

        void orbit() {
            transform.RotateAround (center.position, axis, rotationSpeed * Time.deltaTime);
            desiredPosition = (transform.position - center.position).normalized * radius + center.position;
            transform.position = Vector3.MoveTowards(desiredPosition,transform.position, Time.deltaTime * radiusSpeed);
            

        }
    }
 }