using System.IO;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    int S = 20;
    [SerializeField] GameObject BaseTile;
    [SerializeField] GameObject Map;
    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    public int numOPoint = 5;
    private GameObject[,] Mainmap;
    private System.Tuple<int, int>[] PathWay;

    // Start is called before the first frame update
    void Start()
    {
        PathWay = new System.Tuple<int, int>[numOPoint + 2];
        Mainmap = new GameObject[S, S];
        makeMap();
        RandomPoint(1, 2, S, "start");
        RandomPoint(S - 2, S - 1, S, "end");
        PathPoint();
        //MoveThere(12, 1, 12, 8);
        //MoveThere(12, 19, 4, 5);
        //MoveThere(18,19, 18, 4);
        Pathing();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            Debug.Log("Saved");
            //writeTxtFile();
        }
    }
    void makeMap()
    {
        for (int i = 0; i < S; i++)
        {
            for (int j = 0; j < S; j++)
            {
                GameObject tile = Instantiate(BaseTile, transform.position + new Vector3(j * 1.2f, -i * 1.2f, 0), transform.rotation);
                tile.name = "Tile" + i + " " + j;
                tile.transform.parent = Map.transform;
                Mainmap[i, j] = tile;
            }
        }
    }
    void RandomPoint(int x, int y, int z, string type)
    {
        int a = Random.Range(x, y);
        int b = Random.Range(0, z);
        if (type == "start")
        {
            Destroy(Mainmap[a, b]);
            GameObject ok = Instantiate(start, Mainmap[a, b].transform.position, Mainmap[a, b].transform.rotation);
            Mainmap[a, b] = ok;
            ok.name = "Start";
            ok.transform.parent = Map.transform;
            PathWay[0] = new System.Tuple<int, int>(a, b);
        }
        else if (type == "end")
        {
            Destroy(Mainmap[a, b]);
            GameObject ok = Instantiate(end, Mainmap[a, b].transform.position, Mainmap[a, b].transform.rotation);
            Mainmap[a, b] = ok;
            ok.name = "End";
            ok.transform.parent = Map.transform;
            PathWay[numOPoint + 1] = new System.Tuple<int, int>(a, b);
        }
    }
    void ShowBob()
    {
        for (int i = 0; i < S; i++)
        {
            for (int j = 0; j < S; j++)
            {

            }
        }
    }
    void Pathing()
    {
        System.Array.Sort(PathWay);
        for (int i = 0; i < PathWay.Length - 1; i++)
        {
            if (Mainmap[PathWay[i].Item1, PathWay[i].Item2].activeSelf == false)
            {
                Mainmap[PathWay[i].Item1, PathWay[i].Item2].SetActive(true);
                Debug.Log(PathWay[i].Item1 + " " + PathWay[i].Item2);

            }
            MoveThere(PathWay[i].Item1, PathWay[i].Item2, PathWay[i + 1].Item1, PathWay[i + 1].Item2);
        }
    }
    void PathPoint()
    {
        int a, b, ohgodstop = 0, i = 0;
        while (i != numOPoint)
        {
            ohgodstop++;
            a = Random.Range(2, S - 2);
            b = Random.Range(2, S);
            if (ohgodstop > 100000)
            {
                Debug.Log("You Fucked up!");
                break;
            }
            else if (a + 1 <= S - 1 && b + 1 <= S - 2 && a - 1 >= 2 && b - 1 > 2
                && Mainmap[a + 1, b + 1].activeSelf == true && Mainmap[a - 1, b - 1].activeSelf == true && Mainmap[a + 1, b].activeSelf == true && Mainmap[a, b + 1].activeSelf == true && Mainmap[a - 1, b].activeSelf == true && Mainmap[a, b - 1].activeSelf == true && Mainmap[a - 1, b + 1].activeSelf == true && Mainmap[a + 1, b - 1].activeSelf == true
                && CheckRow(a, b) == true && Mainmap[a, b].tag != "Start" && Mainmap[a, b].tag != "End"
                && Mainmap[a, b] != null && !Mainmap[a, b].Equals(null) && Mainmap[a, b].activeSelf == true)
            {
                //Debug.Log(a + " " + b);
                Mainmap[a, b].SetActive(false);
                PathWay[i + 1] = new System.Tuple<int, int>(a, b);
                var ok = Mainmap[a, b].GetComponent<Renderer>();
                ok.material.color = new Color(0.2f, 0, 0);
                i++;

            }
            else
            {
                //Debug.Log("no" + a + " " + b);
            }
        }
        Debug.Log(ohgodstop);
    }
    bool CheckRow(int c, int r)
    {
        for (int i = 0; i < S; i++)
        {
            if (Mainmap[c, i].activeSelf != true || Mainmap[c + 1, i].activeSelf != true || Mainmap[c - 1, i].activeSelf != true)
            {
                return false;
            }
            else if (Mainmap[c, i].name == "Start" || Mainmap[c, i].name == "End")
            {
                return false;
            }
        }
        for (int i = 0; i < S; i++)
        {
            if (Mainmap[i, r].activeSelf != true || Mainmap[i, r + 1].activeSelf != true || Mainmap[i, r - 1].activeSelf != true)
            {
                return false;
            }
            else if (Mainmap[i, r].name == "Start" || Mainmap[i, r].name == "End")
            {
                return false;
            }
        }
        return true;
    }
    void MoveThere(int x, int y, int a, int b)
    {
        int LoopDestroyer = 0;
        while (1 > 0)
        {
            LoopDestroyer++;
            if (LoopDestroyer == 100000)
            {
                Debug.Log("y:DIDN`T work!");
                return;
            }
            else if (y != b && y > b)
            {
                y--;
                var ok = Mainmap[x, y].GetComponent<Renderer>();
                ok.material.color = new Color(0, 0, 0);
                if (y == b)
                {
                    LoopDestroyer *= 0;
                    break;
                }
            }
            else if (y != b && y < b)
            {
                y++;
                var ok = Mainmap[x, y].GetComponent<Renderer>();
                ok.material.color = new Color(0, 0, 0);
                if (y == b)
                {
                    LoopDestroyer *= 0;
                    break;
                }
            }
        }
        while (1 > 0)
        {
            LoopDestroyer++;
            if (LoopDestroyer == 100000)
            {
                Debug.Log("x:DIDN`T work!");
                return;
            }
            else if (x != a && x > a)
            {
                x--;
                if (x == a)
                {
                    LoopDestroyer *= 0;
                    break;
                }
                var ok = Mainmap[x, y].GetComponent<Renderer>();
                ok.material.color = new Color(0, 0, 0);

            }
            else if (x != a && x < a)
            {
                x++;
                if (x == a)
                {
                    LoopDestroyer *= 0;
                    break;
                }
                var ok = Mainmap[x, y].GetComponent<Renderer>();
                ok.material.color = new Color(0, 0, 0);

            }
        }
    }
    void writeTxtFile()
    {
        string path = "Assets/txtfile/MapsIdea.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Map size:" + S + " " + S);

        writer.Close();
    }
    void encoding()
    {
        string Finalencode = "", Line = "";
        int black = 0, white = 0;
        for (int i = 0; i < Mainmap.Length; i++)
        {
            for (int j = 0; j < Mainmap.Length; j++)
            {

                var col = Mainmap[i, j].GetComponent<Renderer>();
                Color PathTile = new Color(0, 0, 0), BaseTile = new Color(1, 1, 1);
                if (col.material.color.Equals(PathTile))
                {

                }
                else if (col.material.color.Equals(BaseTile))
                {

                }
                else if (Mainmap[i, j].tag == "Start")
                {

                }
                else if (Mainmap[i, j].tag == "End")
                {

                }
            }
            Finalencode += Line + "\n";
            black *= 0;
            white *= 0;
            Line = "";
        }
    }
    void decoding()
    {

    }
}
