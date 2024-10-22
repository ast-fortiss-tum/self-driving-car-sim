using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashTimer : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (LoadMenuScene ());
	}

	IEnumerator LoadMenuScene ()
	{
		yield return new WaitForSeconds (0.1f);
		// Dictionary<string, string> argsDict = new Dictionary<string, string>();
		//SceneManager.LoadScene ("LakeTrackAutonomousDay");

		string sceneName = "MenuScene";

		string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains("--track"))
            {
				Debug.Log(args[i] + " " + args[i+1]);
				sceneName = args[i+1];
            }
        }
		SceneManager.LoadScene(sceneName);
	}
}
