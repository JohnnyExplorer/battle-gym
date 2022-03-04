using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public static class GeneralUI
    {
        public static int episode = 0;
        public static int success = 0;
        public static int fail = 0;
        public static float reward = 0;

        public static int points = 0;
        public static int possible = 0;
        private static TextMesh GameObjectGeneralUI = null;
        public static string fpsText;
        public static float deltaTime;
        public static float x;
        public static float y;
    

        public static void getFPS() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText = Mathf.Ceil (fps).ToString ();
        }
        public static void ScreenText()        
        {
            // Debug.Log("UI  - Start"+ GameObject.Find("GeneralUI"));
            if (GameObjectGeneralUI == null)
            {
                // Debug.Log("UI - Getting UI");
                GameObjectGeneralUI = GameObject.Find("GeneralUI").GetComponent<TextMesh>();
            }
            float successPercent = (success / (float)(success + fail)) * 100;
            if (GameObjectGeneralUI != null)
            {
                // Debug.Log("UI - Updating UI");
                getFPS();
                GameObjectGeneralUI.text = string.Format("x={6},y={7} FPS = {5}, Episode={0}, Success={1}, Fail={2}  Win%{3} -- Reward {4} "
                    , episode
                    , success
                    , fail
                    , successPercent.ToString("0")
                    , reward
                    , fpsText
                    , x
                    , y
                
                    
                    );
            } else {
                // Debug.Log("UI - NO UI found");
            }

        }
    }
}