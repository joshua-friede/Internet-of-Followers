from TwitterAuth import connectToTwitter
from friendsOf import friendsOf
import json

adjacencyList = {}

api = connectToTwitter()

# example: generateFriendsGraph("katyperry")

done = []
edges = []
nodes = set()

def generateFriendsGraph(user, d=0):
    global done
    global edges
    global nodes
    # Reset done array if necessary
    if(d == 0):
        done = []

    friends = friendsOf(user, api, 5)
    nodes.add(user)

    if friends != None:
        for friend in friends:
            edges.append([friend, user])
            nodes.add(friend)

    print(json.dumps({"nodes": list(nodes), "edges": edges}))

    done.append(user)
    if d < 5 and friends != None:
        for u in friends:
            if u not in done:
                generateFriendsGraph(u, d+1)
                
    if d == 0:
        with open('data1.json', 'w') as fp:
            json.dump({"nodes": list(nodes), "edges": edges}, fp)
    else:
        return edges
