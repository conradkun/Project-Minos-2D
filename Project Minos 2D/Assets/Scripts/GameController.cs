using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;

public class GameController : MonoBehaviour
{
    public Transform PlayerPrefab;
    public Transform King;
    public Transform Servant;
    public Transform Minotaur;
    public string server_url;
    private string myID;
    private WebSocket ws;
    private int counter;
    public List<Transform> players = new List<Transform>();

    public int mazeWidth;
    public int mazeHeight;
    public string mazeSeed;

    private string playerTag;

    public Sprite floorSprite;
    public Sprite roofSprite;
    public Sprite wallSprite;
    public Sprite wallCornerSprite;

    public MazeSprite mazeSpritePrefab;

    System.Random mazeRG;

    Maze maze;

    void DrawMaze()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 position = new Vector3(x, y);

                if (maze.Grid[x, y] == 1)
                {
                    CreateMazeSprite(position, floorSprite, transform, 0, mazeRG.Next(0, 3) * 90, false);
                }
                else
                {
                    CreateMazeSprite(position, roofSprite, transform, 0, 0, false);

                    DrawWalls(x, y);
                }
            }
        }
    }

    void DrawWalls(int x, int y)
    {
        bool top = GetMazeGridCell(x, y + 1);
        bool bottom = GetMazeGridCell(x, y - 1);
        bool right = GetMazeGridCell(x + 1, y);
        bool left = GetMazeGridCell(x - 1, y);

        Vector3 position = new Vector3(x, y);

        if (top)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 0, true);
        }

        if (left)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 90, true);
        }

        if (bottom)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 180, true);
        }

        if (right)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 270, true);
        }

        if (!left && !top && x > 0 && y < mazeHeight - 1)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 0, true);
        }

        if (!left && !bottom && x > 0 && y > 0)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 90, true);
        }

        if (!right && !bottom && x < mazeWidth - 1 && y > 0)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 180, true);
        }

        if (!right && !top && x < mazeWidth - 1 && y < mazeHeight - 1)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 270, true);
        }
    }

    public bool GetMazeGridCell(int x, int y)
    {
        if (x >= mazeWidth || x < 0 || y >= mazeHeight || y <= 0)
        {
            return false;
        }

        return maze.Grid[x, y] == 1;
    }

    void CreateMazeSprite(Vector3 position, Sprite sprite, Transform parent, int sortingOrder, float rotation, bool collider)
    {
        MazeSprite mazeSprite = Instantiate(mazeSpritePrefab, position, Quaternion.identity) as MazeSprite;
        mazeSprite.SetSprite(sprite, sortingOrder);
        mazeSprite.transform.SetParent(parent);
        if (collider) { mazeSprite.addCollider(); }  
        mazeSprite.transform.Rotate(0, 0, rotation);
    }


// Use this for initialization
    void Start()
    {
        Physics2D.gravity = Vector2.zero;
        mazeRG = new System.Random(mazeSeed.GetHashCode());

    if (mazeWidth % 2 == 0)
        mazeWidth++;

    if (mazeHeight % 2 == 0)
    {
        mazeHeight++;
    }

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
        Debug.Log("Updating Pos");
        Debug.Log(GameObject.Find(playerTag).transform.position.x);
        Debug.Log(GameObject.Find(playerTag).transform.position.y);
        Debug.Log(myID);
        ws.Send("{ \"Type\":\"pos\", \"Payload\":{ \"X\":" + GameObject.Find(playerTag).transform.position.x + ",\"Z\":" + GameObject.Find(playerTag).transform.position.y + "} }");
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
    IEnumerator CreateMaze(int[,] mazeMap)

    {

        Debug.Log("Creating");
        bool workDone = false;

        while (!workDone)
        {
            // Let the engine run for a frame.
            yield return null;
            maze.Generate(mazeMap);

            DrawMaze();
            workDone = true;

        }
       
    }
    IEnumerator CreatePlayer(float x,float y,bool isKing)

    {

        Debug.Log("Creating PLayer");
        bool workDone = false;

        while (!workDone)
        {
            // Let the engine run for a frame.
            yield return null;
            Vector3 p = new Vector3(x, y, 0);
            var king = GameObject.Find("PlayerKing").transform;
            var servant = GameObject.Find("PlayerServant").transform;
            if (isKing) {
                Destroy(servant.gameObject);
                playerTag = "PlayerKing";
                king.transform.position = p;
            }
            else {
                Destroy(king.gameObject);
                playerTag = "PlayerServant";
                servant.transform.position = p;
            }
            
            workDone = true;

        }

    }
    IEnumerator CreateOtherPlayer(string id, float x, float y, bool king, bool minotaur)

    {

        Debug.Log("Creating Other Player");
        bool workDone = false;

        while (!workDone)
        {
            // Let the engine run for a frame.
            yield return null;
            Vector3 p = new Vector3(x, y, 0);
            if (king)
            {
                Transform t = King;
                t.GetComponent<Player>().ID = id;
                t.GetComponent<Player>().isKing = true;
                t.GetComponent<Player>().isMinotaur = false;
                t.GetComponent<Player>().lastPosition = p;
                players.Add(Instantiate(t , p, Quaternion.identity));
            }
            else if (minotaur)
            {
                Debug.Log("Minautor");
                Transform t = Minotaur;
                t.GetComponent<Player>().ID = id;
                t.GetComponent<Player>().isKing = false;
                t.GetComponent<Player>().isMinotaur = true;
                t.GetComponent<Player>().lastPosition = p;
                players.Add(Instantiate(t, p, Quaternion.identity));
            }
            else
            {
                Transform t = Servant;
                t.GetComponent<Player>().ID = id;
                t.GetComponent<Player>().isKing = false;
                t.GetComponent<Player>().isMinotaur = false;
                t.GetComponent<Player>().lastPosition = p;
                players.Add(Instantiate(t, p, Quaternion.identity));
            }
            workDone = true;

        }

    }
    IEnumerator UpdatePosition(string id, float x, float z)

    {

        Debug.Log("Updating position of other player");
        bool workDone = false;

        while (!workDone)
        {
            yield return null;
            foreach (Transform t in players)
            {
                if (t.GetComponent<Player>().ID == id)
                {
                    Debug.Log("Position of " + id + " updated");
                    Vector3 p = new Vector3(x, z, 0);
                    t.GetComponent<Mover>().Move(new Vector2(x, z));
                    t.position = p;
                    t.rotation = Quaternion.identity;
                }
            }
            workDone = true;

        }

    }
    IEnumerator KillPlayer(string id)

    {

        Debug.Log("Killing player " + id);
        bool workDone = false;

        while (!workDone)
        {
            yield return null;
            foreach (Transform t in players)
            {
                if (t.GetComponent<Player>().ID == id)
                {
                    Destroy(t.gameObject);
                }
            }
            workDone = true;

        }

    }
    IEnumerator KillMe()

    {

        Debug.Log("Killing me");
        bool workDone = false;

        while (!workDone)
        {
            yield return null;
            
            workDone = true;
            Destroy(GameObject.Find(playerTag));
        }

    }
    void SetupServer()
    {
        ws.Connect();
        ws.Send("{ \"Type\":\"ident\", \"Payload\":\"player\"}");
        ws.OnMessage += (sender, e) =>
        {
            var preDict = (Dictionary<string, object>)MiniJSON.Deserialize(e.Data);
            if (preDict["Type"].ToString() == "map")
            {
                Debug.Log("Map");

                var dict = (Dictionary<string, object>)MiniJSON.Deserialize(e.Data);
                
                var payload = (Dictionary<string, object>)dict["Payload"];
                var width = MapMessage.CreateFromJSON(e.Data).Payload.Width;
                var height = MapMessage.CreateFromJSON(e.Data).Payload.Height;
                Debug.Log(width);
                Debug.Log(height);
                mazeWidth = width;
                mazeHeight = height;
                maze = new Maze(width, height);
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
                Debug.Log(e.Data);
                var character = obj["Payload"]["You"]["Character"].n;
                myID = obj["Payload"]["You"]["ID"].str;
                Debug.Log("JOINING ID + " + obj["Payload"]["You"]["ID"].str);
                Debug.Log("Char " + character.ToString());
                var x_spawn = obj["Payload"]["You"]["Position"]["X"].n;
                var z_spawn = obj["Payload"]["You"]["Position"]["Z"].n;
                Debug.Log("registered on the server");
                Debug.Log(x_spawn);
                Debug.Log(z_spawn);
                UnityMainThreadDispatcher.Instance().Enqueue(CreatePlayer(x_spawn, z_spawn, character == 2));
                for (int m = 0; m < 10; m++)
                {
                    if (!obj["Payload"]["Players"][m].IsNull && obj["Payload"]["Players"][m]["ID"].str != myID)
                    {
                        string id = obj["Payload"]["Players"][m]["ID"].str;
                        x_spawn = obj["Payload"]["Players"][m]["Position"]["X"].n;
                        z_spawn = obj["Payload"]["Players"][m]["Position"]["Z"].n;
                        UnityMainThreadDispatcher.Instance().Enqueue(CreateOtherPlayer(id, x_spawn, z_spawn, obj["Payload"]["Players"][m]["Character"].n == 2, obj["Payload"]["Players"][m]["Character"].n == 1));
                    }
                }


               
    
            }
            if (preDict["Type"].ToString() == "joined")
            {
                JSONObject obj = new JSONObject(e.Data);
                var character = obj["Payload"]["Character"].n;
                Debug.Log("NEW USER DETECTED " + character.ToString());
                var x_spawn = obj["Payload"]["Position"]["X"].n;
                var z_spawn = obj["Payload"]["Position"]["Z"].n;
                string id = obj["Payload"]["ID"].str;
                Debug.Log("joined on the server");
                Debug.Log(x_spawn);
                Debug.Log(z_spawn);
                UnityMainThreadDispatcher.Instance().Enqueue(CreateOtherPlayer(id, x_spawn, z_spawn,character == 2, character == 1));
          
            }
            if (preDict["Type"].ToString() == "player")
            {
                JSONObject obj = new JSONObject(e.Data);
               
                Debug.Log("Position of " + obj["Payload"]["ID"].str + " updated");
                string id = obj["Payload"]["ID"].str;
                float x_update = obj["Payload"]["Position"]["X"].n;
                float z_update = obj["Payload"]["Position"]["Z"].n;
                UnityMainThreadDispatcher.Instance().Enqueue(UpdatePosition(id, x_update, z_update));
            }
            if (preDict["Type"].ToString() == "dead")
            {
                JSONObject obj = new JSONObject(e.Data);

                Debug.Log("PLayer " + obj["Payload"]["ID"].str + " is dead");
                string id = obj["Payload"]["ID"].str;
                UnityMainThreadDispatcher.Instance().Enqueue(KillPlayer(id));
                if(id == myID) { Debug.Log("Its me "); UnityMainThreadDispatcher.Instance().Enqueue(KillMe()); }
            }


            //Write join

        };
    }
}

