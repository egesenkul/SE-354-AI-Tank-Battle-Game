using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class KayaNode 
{
    public KayaNode(Vector3 d)
    {
        x = (int)d.x; z = (int)d.z;
    }
    public int x;
    public int z;
    public Vector3 getValue()
    {
        return new Vector3(x, 0, z);
    }
}

class KayaEdge
{
    public KayaNode from;
    public KayaNode to;
    public float getCost()
    {
        return (from.getValue() - to.getValue()).magnitude;
    }
}

class KayaNodeRecord
{
    public KayaNode node;
    public KayaEdge connection;
    public float costSoFar;
    public float estimatedTotalCost;
}

class KayaHeuristic
{
    public KayaHeuristic(Vector3 target)
    {
        this.target = target;
    }
    private Vector3 target;
    public float estimate(Vector3 t)
    {
        return (target - t).magnitude;
    }
    public float estimate(KayaNode t)
    {
        return (target - t.getValue()).magnitude;
    }
}

class KayaStar {
	public int[,] alan;
	public Vector3 source;
    public Vector3 target;
	public ArrayList targets;
	
	public KayaStar(int [,] a) {
		alan = a;
	}
	
	List<KayaEdge> getConnections(KayaNode node)
    {
        List<KayaEdge> connections = new List<KayaEdge>();

        int px = node.x; int pz = node.z;
        for (int i = px - 1; i <= px + 1; i++)
        {
            for (int j = pz - 1; j <= pz + 1; j++)
            {
                if (i == px && j == pz) continue;
                if (i >= 0 && i < 50 && j >= 0 && j < 50 && alan[i, j] != 1 && alan[i, j] != 5)
                {
                    KayaEdge edge = new KayaEdge();
                    edge.from = node;
                    edge.to = new KayaNode(new Vector3(i, 0, j));
                    connections.Add(edge);
                }
            }
        }
        return connections;
    }
	
	KayaNodeRecord findSmallest(List<KayaNodeRecord> list) 
	{
		if(list.Count == 1) return list[0];
		int index = 0;
		float s = list[0].estimatedTotalCost;
		for(int i=1;i<list.Count;i++)
		{
			if(list[i].estimatedTotalCost < s) 
			{
				index = i; 
				s = list[i].estimatedTotalCost;
			}
		}
		return list[index];
	}
	
	KayaNodeRecord FindRecordInList(List<KayaNodeRecord> list, KayaNode n)
    {
        foreach (KayaNodeRecord r in list)
        {
            if (r.node.getValue() == n.getValue()) return r;
        }
        return null;
    }
	
	public List<KayaEdge> aStar(Vector3 start, Vector3 end)
    {
        KayaHeuristic heuristic = new KayaHeuristic(end);
        KayaNodeRecord startRecord = new KayaNodeRecord();
        startRecord.node = new KayaNode(start);
        startRecord.connection = null;
        startRecord.costSoFar = 0;
        startRecord.estimatedTotalCost = heuristic.estimate(start);

        List<KayaNodeRecord> open = new List<KayaNodeRecord>();
        open.Add(startRecord);
        List<KayaNodeRecord> closed = new List<KayaNodeRecord>();
        KayaNodeRecord current = null;
        while (open.Count > 0)
        {
            //open.Sort(); 
            //current = open[0];
			current = findSmallest(open);
            // target is the closest node on the list
            // break out of the while loop
            if (current.node.getValue() == end) break;

            // NodeRecord endNodeRecord = null; // new NodeRecord();

            List<KayaEdge> connections = getConnections(current.node);
            foreach (KayaEdge connection in connections)
            {
                KayaNode endNode = connection.to;
                float endNodeCost = current.costSoFar + connection.getCost();
                float endNodeHeuristic = float.MaxValue;
                KayaNodeRecord endNodeRecord = FindRecordInList(closed, endNode);
                if (endNodeRecord != null)
                {
                    if (endNodeRecord.costSoFar <= endNodeCost)
                        continue;
                    closed.Remove(endNodeRecord);
                    endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                }
                else if (FindRecordInList(open, endNode) != null)
                {
                    endNodeRecord = FindRecordInList(open, endNode);
                    if (endNodeRecord.costSoFar <= endNodeCost)
                        continue;
                    endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                }
                else
                {
                    endNodeRecord = new KayaNodeRecord();
                    endNodeRecord.node = endNode;
                    endNodeHeuristic = heuristic.estimate(endNode);
                }
                endNodeRecord.costSoFar = endNodeCost;
                endNodeRecord.connection = connection;
                endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                if (FindRecordInList(open, endNode) == null)
                {
                    open.Add(endNodeRecord);
                }
            }
            open.Remove(current);
            closed.Add(current);
        }
        if (current.node.getValue() != end) return null; // null;
        else
        {
            List<KayaEdge> path = new List<KayaEdge>();
            while (current.node.getValue() != start)
            {
                path.Add(current.connection);
                current = FindRecordInList(closed, current.connection.from);
            }
            path.Reverse();
            return path;
        }
    }
}

class KayaAlign : MonoBehaviour {

    public float target;
    public float speed;

    public int maxAngularAcceleration = 60;
    public int maxRotationSpeed = 60;
    public float targetRadius = 0.91f;
    public float slowRadius = 0.91f;
    public float timeToTarget = 0.91f;

    // Use this for initialization
	void Start () {
        target = transform.rotation.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
        float angle = transform.eulerAngles.y;
        float rotation = Mathf.DeltaAngle(angle, target);
        float rotationSize = Mathf.Abs(rotation);
        if (rotationSize < targetRadius) return;
        
        float targetRotation = 0.0f;
        if (rotationSize > slowRadius) targetRotation = maxRotationSpeed;
        else targetRotation = maxRotationSpeed * rotationSize / slowRadius;
        targetRotation *= rotation / rotationSize;
        float angular = targetRotation - speed;
        angular /= timeToTarget;

        float angularAcceleration = Mathf.Abs(angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            angular /= angularAcceleration;
            angular *= maxAngularAcceleration;
        }
        speed += angular;
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}
}

class KayaArrive : MonoBehaviour {

    public Vector3 target;
    public Vector3 velocity;

    public int maxAcceleration = 12;
    public int maxSpeed = 1;
    public float targetRadius = 0.1f;
    public float slowRadius = 0.1f;
    public float timeToTarget = 0.1f;
	public bool arrived = false;

	// Use this for initialization
	void Start () {
        target = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        if (distance < targetRadius) {
			arrived = true;
			return;
		} else {arrived = false; }
            

        float targetSpeed = 0;
        if (distance > slowRadius)
            targetSpeed = maxSpeed;
        else
            targetSpeed = maxSpeed * distance / slowRadius;

        Vector3 targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        Vector3 linear = targetVelocity - velocity;
        linear /= timeToTarget;

        if (linear.magnitude > maxAcceleration)
        {
            linear.Normalize();
            linear *= maxAcceleration;
        }

        transform.position += velocity *Time.deltaTime;
        velocity += linear * Time.deltaTime;

        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
            
	}
}

public class Tank : MonoBehaviour {
	private State currentState;
	Level level;
	int msize;
	KayaStar astar;
	bool test = true;
	int idx = 0;
	List<KayaEdge> edges;
	KayaAlign align;
	KayaArrive arrive;
	int riza;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<AITankScript>().playername = "Kaya";
		level = GameObject.Find("Level").GetComponent<Level>();
		align = (KayaAlign)gameObject.AddComponent(typeof(KayaAlign));
		arrive = (KayaArrive)gameObject.AddComponent(typeof(KayaArrive));
		astar = new KayaStar(level.getMap());
		this.changeState (new Search ());
		Debug.Log (currentState.ToString ());
		riza = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentState != null) {
			currentState.Execute (this);
		}
	}
	public interface State{
		void Enter (Tank tnk);
		void Execute (Tank tnk);
		void Exit (Tank tnk);
	}
	public class Search:State{
		public void Enter(Tank tnk){
			Debug.Log (tnk.currentState.ToString ());
			Debug.Log (tnk.riza.ToString ());

				tnk.changeState (new Move (new Vector3 (12, 0, 10)));

			}
		public void Execute(Tank tnk){

		}
		public void Exit(Tank tnk){

		}
	}
	public class Move:State{
		public Vector3 ptrash= Vector3.zero;
		public Move (Vector3 tp){
			ptrash=tp;
		}
		public void Enter(Tank tnk){
			tnk.test = true;
		}
		public void Execute(Tank tnk){
			Debug.Log (tnk.currentState.ToString ());
			if (tnk.test) {
				tnk.test = false;
				Debug.Log (ptrash);
				tnk.edges = tnk.astar.aStar(tnk.gameObject.transform.position, ptrash);
				tnk.arrive.target = tnk.edges[0].to.getValue ();
			}
			if(tnk.arrive.arrived) tnk.arrive.target = tnk.edges[++tnk.idx].to.getValue();
			tnk.align.target = Mathf.Atan2(tnk.arrive.velocity.x, tnk.arrive.velocity.z) * Mathf.Rad2Deg;

			if ((tnk.transform.position - ptrash).magnitude < 0.25f) {
				tnk.changeState (new Third ());
				Debug.Log("vardi");
			}
		}
		public void Exit(Tank tnk){

		}

	}
	public class Third:State{
		public void Enter(Tank tnk){
			Debug.Log (tnk.currentState.ToString ());
			tnk.changeState (new Move (new Vector3 (15, 0, 10)));
		}
		public void Execute(Tank tnk){

		}
		public void Exit(Tank tnk){

		}
	}

	public void changeState(State s){
		if (currentState != null) {
			currentState.Exit (this);
		}
		currentState = s;
		s.Enter (this);
	}
}
