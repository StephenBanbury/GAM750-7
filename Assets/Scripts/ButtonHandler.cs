using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ButtonHandler : MonoBehaviour
    {
        public void OnButtonClicked(Button button)
        {
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
                    gameController.OnJoinButtonClicked();
                }
                else if (button.name == "LeaveButton")
                {
                    gameController.OnLeaveButtonClicked();
                }
            }
        }
    }
}