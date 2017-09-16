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

    public GraphNode graph;
    public Dictionary<string, GraphNode> nodes;

    int x = 0;
    int y = 0;
    int z = 0;

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
	}

    void GenerateNode(GraphNode g)
    {
        GameObject n = GameObject.CreatePrimitive(PrimitiveType.Cube);
        n.transform.position = new Vector3(x * 4, y * 4, z * 4);
        z++;
        if(z > y + 1)
        {
            z = 0;
            y++;
        }
        if(y > x + 1)
        {
            y = 0;
            x++;
        }
        g.model = n;
        nodes.Add(g.Username, g);
        foreach(GraphNode gg in g.Connections)
        {
            if(!nodes.ContainsKey(gg.Username))
            {
                GenerateNode(gg);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach(var element in nodes)
        {
            GraphNode g1 = element.Value;
            foreach(GraphNode g2 in g1.Connections)
            {
                Debug.DrawLine(g1.model.transform.position, g2.model.transform.position, Color.white, 0, true);
            }
        }
    }
}
