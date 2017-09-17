using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ModelLoader : MonoBehaviour {

    public class Data : System.Object
    {
        public string[] nodes;
        public string[,] edges;

        public Data(string json)
        {
            JSONObject obj = new JSONObject(json);
            JSONObject nodeObj = obj.GetField("nodes");
            JSONObject edgesObj = obj.GetField("edges");
            Debug.Log(nodeObj);
            Debug.Log(edgesObj);
            nodes = new string[nodeObj.list.Count];
            for(int i=0; i<nodeObj.list.Count; i++)
            {
                nodes[i] = nodeObj.list[i].str;
            }
            edges = new string[edgesObj.list.Count, 2];
            for (int i = 0; i < edgesObj.list.Count; i++)
            {
                edges[i, 0] = edgesObj.list[i].list[0].str;
                edges[i, 1] = edgesObj.list[i].list[1].str;
            }

            Debug.Log(this.ToString());
        }

        public List<GraphNode> parse()
        {
            Dictionary<string, GraphNode> nodesDict = new Dictionary<string, GraphNode>();
            foreach(string username in nodes)
            {
                nodesDict.Add(username, new GraphNode(username));
            }

            for(int i=0; i<edges.GetLength(0); i++)
            {
                Debug.Log("Connecting " + edges[i, 0] + " and " + edges[i, 1]);
                nodesDict[edges[i,1]].connect(nodesDict[edges[i,0]]);
            }

            return nodesDict.Values.ToList();
        }

        public override string ToString()
        {
            return string.Join(",", nodes);
        }
    }

    [System.Serializable]
    public class GraphNode : System.Object
    {
        private string _username;
        private List<GraphNode> _connections;
        public GameObject model;

        public GraphNode(string username)
        {
            this._username = username;
            this._connections = new List<GraphNode>();
        }

        public void connect(GraphNode other)
        {
            this._connections.Add(other);
        }
        
        public GraphNode[] Connections
        {
            get { return _connections.ToArray(); }
        }
        
        public string Username
        {
            get { return _username;  }
        }

        public override string ToString()
        {
            return _username + " | " + _connections.Aggregate("", (x, y) => { return x + y._username + ", "; });
        }
    }

    [System.Serializable]
    public class Pair<T1, T2> 
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Pair<T1, T2> p = obj as Pair<T1, T2>;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.First.Equals(p.First) && this.Second.Equals(p.Second);
        }

        public bool Equals(Pair<T1, T2> second)
        {
            // If parameter is null return false:
            if ((object)second == null)
            {
                return false;
            }

            return this.First.Equals(second.First) && this.Second.Equals(second.Second);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + First.ToString() + ", " + Second.ToString() + ")";
        }
    }

    public List<GraphNode> graphNodes;
    public Dictionary<string, GraphNode> nodes;
    Dictionary<Pair<string, string>, GameObject> connections;

    int x = 0;
    int y = 0;
    int z = 0;

    public float nodeScale = 0.3f;
    public float edgeWidth = 0.05f;

    public int iterations = 1000;

    public GameObject NameplatePrefab;

    private string dataFileName = "data.json";

    // Use this for initialization
    void Start () {
        // Load graph
        TextAsset jsonObj = Resources.Load("data") as TextAsset;
        Debug.Log(jsonObj.text);
        graphNodes = new Data(jsonObj.text).parse();

        if(graphNodes == null)
        {
            Application.Quit();
        }

        // Generate the physical representation of the nodes!
        nodes = new Dictionary<string, GraphNode>();
        foreach(GraphNode g in graphNodes)
        {
            GenerateNode(g);
        }

        connections = new Dictionary<Pair<string, string>, GameObject>();

        foreach (var element in nodes)
        {
            GraphNode g1 = element.Value;
            foreach (GraphNode g2 in g1.Connections)
            {
                if(!(
                    connections.ContainsKey(new Pair<string, string>(g1.Username, g2.Username)) || 
                    connections.ContainsKey(new Pair<string, string>(g2.Username, g1.Username))
                    ))
                {
                    GameObject conn = new GameObject("Connection: " + g1.Username + ", " + g2.Username);
                    LineRenderer lr = conn.AddComponent<LineRenderer>();
                    conn.transform.parent = g1.model.transform;
                    lr.SetPositions(new Vector3[] { g1.model.transform.position, g2.model.transform.position });
                    lr.startWidth = edgeWidth;
                    lr.endWidth = edgeWidth;
                    if (g1.Connections.Contains(g2) && g2.Connections.Contains(g1))
                        lr.startColor = Color.red;
                    else
                        lr.startColor = Color.white;
                    lr.endColor = Color.red;
                    lr.material = new Material(Shader.Find("Particles/Additive"));
                    connections.Add(new Pair<string, string>(g1.Username, g2.Username), conn);
                }
                

                // Debug.DrawLine(g1.model.transform.position, g2.model.transform.position, Color.white, 0, true);
            }
        }
    }

    void GenerateNode(GraphNode g)
    {
        GameObject n = GameObject.CreatePrimitive(PrimitiveType.Cube);
        n.transform.localScale = new Vector3(nodeScale, nodeScale, nodeScale);
        n.transform.position = new Vector3(x * 4, y * 4, z * 4 + 10);
        z++;
        if (z > y + 1)
        {
            z = 0;
            y++;
        }
        if (y > x + 1)
        {
            y = 0;
            x++;
        }
        g.model = n;
        nodes.Add(g.Username, g);

        GameObject nameplate = Instantiate<GameObject>(NameplatePrefab);
        nameplate.name = g.Username;
        UnityEngine.UI.Text t = nameplate.GetComponentInChildren<UnityEngine.UI.Text>();
        t.text = g.Username;
        nameplate.transform.SetParent(n.transform, true);
        nameplate.transform.localPosition = new Vector3(0, -1 / nodeScale, 0);
        nameplate.transform.localScale = Vector3.one;
    }
	
	// Update is called once per frame
	void Update () {
        // Calculate the force on each vertex
        float c1 = 2f;
        float c2 = 1f;
        float c3 = 1f;
        float c4 = 0.1f;

        foreach (var elem in nodes)
        {
            // Calculate the force on each node
            Vector3 force = Vector3.zero;

            foreach (var other in nodes)
            {
                if (!elem.Key.Equals(other.Key))
                {
                    if (connections.ContainsKey(new Pair<string, string>(elem.Key, other.Key)) || connections.ContainsKey(new Pair<string, string>(other.Key, elem.Key)))
                    {
                        force += (other.Value.model.transform.position - elem.Value.model.transform.position).normalized * c1 * Mathf.Log10(Vector3.Distance(elem.Value.model.transform.position, other.Value.model.transform.position) / c2);
                    }
                    else
                    {
                        force -= (other.Value.model.transform.position - elem.Value.model.transform.position).normalized * c3 / (elem.Value.model.transform.position - other.Value.model.transform.position).sqrMagnitude;
                    }
                }
            }

            // move the vertex c4 * (force on vertex)
            force *= c4;
            elem.Value.model.transform.position += Time.deltaTime * 10f * force;
        }

        // Fix edges
        foreach (var edge in connections)
        {
            edge.Value.GetComponent<LineRenderer>().SetPositions(new Vector3[] { nodes[edge.Key.First].model.transform.position, nodes[edge.Key.Second].model.transform.position });
        }
    }
}
