using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;

public class GameControllerOLD : MonoBehaviour
{
    public Transform WallPrefab;
    public Transform PlayerPrefab;
    public bool c;
    public string server_url;
    private WebSocket ws;
    private int counter;
    // Use this for initialization
    void Start()
    {
        ws = new WebSocket(server_url);
        SetupServer();
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        if(counter < 10)
        {
            return;
        }
        counter = 0;
        Debug.Log("Sended");
        ws.Send("{ \"Type\":\"pos\", \"Payload\":{ \"X\":" + transform.position.x.ToString() + ",\"Z\":" + transform.position.z.ToString() + "} }");
    }
    /**
    void CreateMaze2(JSONObject maze)
    {
        //x and z
        int i = 0;
        foreach (JSONObject obj in maze.list)
        {
            int j = 0;
            foreach (((int)cell.n) in obj.list)
            {
                
                if (cell.n == 1)
                {
                    Vector3 p = new Vector3(i, 0, j);
                    Instantiate(WallPrefab, p, Quaternion.identity);
                }
                j++;
            }
            i++;
        }
    }
    **/
    IEnumerator CreateMaze(int[,] maze)

    {

        Debug.Log("Creating");
        bool workDone = false;

        while (!workDone)
        {
            // Let the engine run for a frame.
            yield return null;
            for (int k = 0; k < maze.GetLength(0); k++)
            {

                for (int l = 0; l < maze.GetLength(1); l++)
                {
                    if (maze[k, l] == 1)
                    {

                        Vector3 p = new Vector3(k, 0, l);

                        Instantiate(WallPrefab, p, Quaternion.identity);
                    }

                }
            }
            workDone = true;

        }
       
    }
    IEnumerator CreatePlayer(float x,float z)

    {

        Debug.Log("Creating PLayer");
        bool workDone = false;

        while (!workDone)
        {
            // Let the engine run for a frame.
            yield return null;
            Vector3 p = new Vector3(x, 1, z);
            var playerGlobal = GameObject.Find("OVRPlayerController").transform;
            var playerLocal = playerGlobal.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
            playerGlobal.transform.position = p;
            workDone = true;

        }

    }
    void SetupServer()
    {
        ws.Connect();
        ws.Send("{ \"Type\":\"ident\", \"Payload\":\"oculus\"}");
        ws.OnMessage += (sender, e) =>
        {
            var preDict = (Dictionary<string, object>)MiniJSON.Deserialize(e.Data);
            if (preDict["Type"].ToString() == "map")
            {
                Debug.Log("hey");
                var dict = (Dictionary<string, object>)MiniJSON.Deserialize(e.Data);
                
                var payload = (Dictionary<string, object>)dict["Payload"];
                var width = MapMessage.CreateFromJSON(e.Data).Payload.Width;
                var height = MapMessage.CreateFromJSON(e.Data).Payload.Height;
                Debug.Log(width);
                Debug.Log(height);
                var cells = (List<object>)payload["Cells"];
                int[,] map = new int[width,height];
                for (int row = 0; row < width; row++)
                {
                    var items = (List<object>)cells[row];
                    for (int col = 0; col < height; col++)
                    {
                        map[row,col] = Convert.ToInt32(items[col]);
                    }
                }
                UnityMainThreadDispatcher.Instance().Enqueue(CreateMaze(map));
            }
            if (preDict["Type"].ToString() == "registered")
            {
                JSONObject obj = new JSONObject(e.Data);
                var character = obj["Payload"]["Character"];
                var x_spawn = obj["Payload"]["Position"]["X"].n;
                var z_spawn = obj["Payload"]["Position"]["Z"].n;
    
                UnityMainThreadDispatcher.Instance().Enqueue(CreatePlayer(x_spawn,z_spawn));
    
            }

        };
    }
}

