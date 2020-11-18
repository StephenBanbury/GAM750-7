using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using System.Collections;
using UnityEngine.Android;
#endif

namespace Assets.Scripts
{
    /// <summary>
    ///    Serves a game controller object for this application.
    /// </summary>
    public class Home : MonoBehaviour
    {

        // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
        static Perform app = null;

        private string HomeSceneName = "Home";

        private string PlaySceneName = "Perform";

        [SerializeField] private string AppID = "54f15673a8fd43318b10d4e42f8dd781";

        void Awake()
        {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);
#endif

            // keep this alive across scenes
            DontDestroyOnLoad(this.gameObject);
        }
        
        void Start()
        {
            CheckAppId();
        }

        void Update()
        {
            CheckPermissions();
        }

        private void CheckAppId()
        {
            Debug.Assert(AppID.Length > 10, "Please fill in your AppId first on Game Controller object.");
        }

        /// <summary>
        ///   Checks for platform dependent permissions.
        /// </summary>
        private void CheckPermissions()
        {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
        }

        public void OnJoinButtonClicked()
        {
            // get parameters (channel name, channel profile, etc.)
            //GameObject go = GameObject.Find("ChannelName");
            //InputField field = go.GetComponent<InputField>();

            GameObject go = GameObject.Find("CallerID");
            InputField field = go.GetComponent<InputField>();
            
            uint uuid;
            bool isInt = uint.TryParse(field.text, out uuid);

            if (isInt)
            {
                // create app if nonexistent
                if (ReferenceEquals(app, null))
                {
                    app = new Perform(); // create app
                    app.LoadEngine(AppID); // load engine
                }

                // join channel and jump to next scene
                app.Join("NotNearEnough", uuid);

                SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
                SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Single);
            }

        }

        public void OnLeaveButtonClicked()
        {
            if (!ReferenceEquals(app, null))
            {
                app.Leave(); // leave channel
                app.UnloadEngine(); // delete engine
                app = null; // delete app
                SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);
            }

            Destroy(gameObject);
        }

        public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == PlaySceneName)
            {
                if (!ReferenceEquals(app, null))
                {
                    app.OnScenePerformLoaded(); // call this after scene is loaded
                }

                SceneManager.sceneLoaded -= OnLevelFinishedLoading;
            }
        }

        void OnApplicationPause(bool paused)
        {
            if (!ReferenceEquals(app, null))
            {
                app.EnableVideo(paused);
            }
        }

        void OnApplicationQuit()
        {
            if (!ReferenceEquals(app, null))
            {
                app.UnloadEngine();
            }
        }
    }
}