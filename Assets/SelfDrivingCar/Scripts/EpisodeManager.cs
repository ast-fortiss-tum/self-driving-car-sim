using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class EpisodeManager : MonoBehaviour
{
    public List<EpisodeEvent> eventRecords;
    public EpisodeMetrics metrics;
    private GameObject _app;
    private SocketIOComponent _socket;

    public void Update() {
        // TODO: remove and option in the menu
        if (Input.GetKey (KeyCode.Z)) {
			ResetTrack(Track.RoadGenerator);
		}
    }

    public void Awake()
    {
        eventRecords = new List<EpisodeEvent>();
        metrics = new EpisodeMetrics();
        _app = GameObject.Find("__app");
        _socket = _app.GetComponent<SocketIOComponent> ();

        // External Network API
        // Pause simulation
		_socket.On("pause_sim", PauseSim);
        // Resume simulation
		_socket.On("resume_sim", ResumeSim);
        // Initialize the episode
		_socket.On("end_episode", EndEpisode);
		_socket.On("start_episode", StartEpisode);
    } 

    public void ResetTrack(Track track)
    {
        // TODO: some stuff here are hard coded. Find better way to map track to sceneName
        switch (track) {
            case Track.Lake:
                SceneManager.LoadScene("LakeTrackAutonomousDay");
                break;
            case Track.Jungle:
                SceneManager.LoadScene("JungleTrackAutonomousDay");
                break;
            case Track.Mountain:
                SceneManager.LoadScene("MountainTrackAutonomousDay");
                break;
            case Track.RoadGenerator:
                SceneManager.LoadScene("GeneratedTrack");
                break;
        }
    }

    public void AddEvent(EpisodeEvent e)
    {
        switch (e.key)
        {
            case "collision":
                metrics.collisionCount += 1;
                break;
            case "out_of_track":
                metrics.outOfTrackCount += 1;
                break;
        }
        eventRecords.Add(e);
        _socket.Emit("episode_event", JsonUtility.ToJson(e));
    }

    public void AddEvent(string key, string value)
    {
        this.AddEvent(new EpisodeEvent(key, value));
    }

    private void PauseSim (SocketIOEvent obj)
	{
        Time.timeScale = 0;
        _socket.Emit("sim_paused", new JSONObject ());
	}

	private void ResumeSim (SocketIOEvent obj)
	{
        Time.timeScale = 1;
        _socket.Emit("sim_resumed", new JSONObject ());
	}

    private void EndEpisode (SocketIOEvent obj)
    {
        // send back the episode metrics and events
        Time.timeScale = 0;
        _socket.Emit("episode_metrics", JsonUtility.ToJson(metrics));
        _socket.Emit("episode_events", JsonUtility.ToJson(eventRecords));
        _socket.Emit("episode_ended", new JSONObject ());
    }

    private void StartEpisode (SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
		string trackName = jsonObject.GetField("track_name").str;
        this.ResetTrack(this.TrackFromString(trackName));
        // reset episode metrics and events
        eventRecords = new List<EpisodeEvent>();
        metrics = new EpisodeMetrics();
        Time.timeScale = 1;
        _socket.Emit("episode_started", new JSONObject ());
    }


    // TODO: Helper function, should probably be removed from here
    private Track TrackFromString(string name) {
        switch (name) {
            case "lake":
                return Track.Lake;
            case "jungle":
                return Track.Jungle;
            case "mountain":
                return Track.Mountain;
            case "road_generator":
                return Track.RoadGenerator;
            default:
                Debug.Log("Track {0} not recognized, returning Track Lake.");
                return Track.Lake;
        }
    }

}


[System.Serializable]
public class EpisodeMetrics
{
    public int collisionCount = 0;
    public int outOfTrackCount = 0;
}

[System.Serializable]
public class EpisodeEvent
{

    // Unix time in milliseconds
    public string timestamp;
    public string key;
    public string value;

    public EpisodeEvent(string key, string value) {
        this.timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
        this.key = key;
        this.value = value;
    }
}

// TODO: move this class somewhere else
public enum Track
{
    Lake,
    Jungle,
    Mountain,
    RoadGenerator,
}

public enum Weather
{
    Sunny,
    Rainy,
    Snowy,
    Foggy,
}

public enum DayTime
{
    Day,
    DayNightCycle,
}