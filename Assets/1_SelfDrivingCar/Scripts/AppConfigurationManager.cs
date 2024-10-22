using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using System;
// For files
using System.IO;
using System.Collections;
using System.Collections.Generic;
public class AppConfigurationManager : MonoBehaviour
{

    public AppConfiguration conf = new AppConfiguration();
    private GameObject _app;
    private SocketIOComponent _socket;

    public void Awake() 
    {
        // TODO: change "__app" with a constant
        GameObject _app = GameObject.Find("__app");

        // Default settings
        conf.fps = 30;
        conf.port = 4567;

        // Read settings from config file
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 1; i < args.Length - 1; i++) {
            if (args[i] == "--config") {
                var configFilename = args[i+1];
                this.conf = JsonUtility.FromJson<AppConfiguration>(File.ReadAllText(configFilename));
            }
        }

        // Read settings from command line
        for (int i = 1; i < args.Length - 1; i++) {
            if (args[i] == "--fps") {
                conf.fps = int.Parse(args[i+1]);
            }
            if (args[i] == "--port") {
                conf.port = int.Parse(args[i+1]);
            }
        }

        Debug.Log("Application started with the following configuration: " + JsonUtility.ToJson(conf));

        // Set frame rate
        Application.targetFrameRate = conf.fps;

        // Move to somewhere else
        SceneManager.LoadScene(1);
    }
}


[System.Serializable]
public class AppConfiguration
{
    // Game information
     [SerializeField]
    public int fps = 30;

    // Socket IO configuration
     [SerializeField]
    public int port = 4567;

    // Minimum amount of time (in millis) to wait before sending car telemetry
    [SerializeField]
    public int telemetryMinInterval = 33; // ~33 = 1000 /30


}