using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ButtonHandler : MonoBehaviour
    {
        public void onButtonClicked(Button button)
        {
            // which GameObject?
            GameObject go = GameObject.Find("GameController");
            if (go != null)
            {
                Home gameController = go.GetComponent<Home>();
                if (gameController == null)
                {
                    Debug.LogError("Missing game controller...");
                    return;
                }

                if (button.name == "JoinButton")
                {
                    gameController.onJoinButtonClicked();
                }
                else if (button.name == "LeaveButton")
                {
                    gameController.onLeaveButtonClicked();
                }
            }
        }
    }
}