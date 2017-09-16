using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ModelLoader : MonoBehaviour {

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
            other._connections.Add(this);
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

    public GraphNode graph;
    public Dictionary<string, GraphNode> nodes;
    Dictionary<Pair<string, string>, GameObject> connections;

    int x = 0;
    int y = 0;
    int z = 0;

    public float nodeScale = 0.3f;
    public float edgeWidth = 0.05f;

    public int iterations = 1000;

	// Use this for initialization
	void Start () {
        graph = new GraphNode("aszecsei");

        GraphNode last = null;

        for(int i=0; i<10; i++)
        {
            GraphNode temp = new GraphNode("User " + (i + 1));

            for(int j=0; j<10; j++)
            {
                GraphNode temp2 = new GraphNode("User " + (i + 1) + " " + (j + 1));
                temp.connect(temp2);
                if(Random.Range(0, 2) > 0)
                {
                    temp2.connect(graph);
                }
                if(last != null && Random.Range(0, 2) > 0)
                {
                    temp2.connect(last);
                }
                last = temp2;
            }

            graph.connect(temp);
        }

        // Generate the physical representation of the nodes!
        nodes = new Dictionary<string, GraphNode>();
        GenerateNode(graph);

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
        n.transform.position = new Vector3(x * 4, y * 4, z * 4);
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
        foreach (GraphNode gg in g.Connections)
        {
            if (!nodes.ContainsKey(gg.Username))
            {
                GenerateNode(gg);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        // TODO: Calculate the force on each vertex
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
            elem.Value.model.transform.position += force;
        }

        // Fix edges
        foreach (var edge in connections)
        {
            edge.Value.GetComponent<LineRenderer>().SetPositions(new Vector3[] { nodes[edge.Key.First].model.transform.position, nodes[edge.Key.Second].model.transform.position });
        }
    }
}
