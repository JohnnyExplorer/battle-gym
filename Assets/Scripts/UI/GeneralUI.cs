using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public static class GeneralUI
    {
        public static int episode = 0;
        public static int success = 0;
        public static int fail = 0;

        public static int points = 0;
        public static int possible = 0;
        private static TextMesh GameObjectGeneralUI = null;
    
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
                GameObjectGeneralUI.text = string.Format("Episode={0}, Success={1}, Fail={2} %{3} | Current Points {4} / {5}"
                    , episode
                    , success
                    , fail
                    , successPercent.ToString("0")
                    , points
                    , possible
                    );
            } else {
                // Debug.Log("UI - NO UI found");
            }

        }
    }
}