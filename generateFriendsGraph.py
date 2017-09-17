from TwitterAuth import connectToTwitter
from friendsOf import friendsOf
import json

adjacencyList = {}

api = connectToTwitter()

# example: generateFriendsGraph("katyperry")

done = []

def generateFriendsGraph(user, d=0):
    global done
    # Reset done array if necessary
    if(d == 0):
        done = []

    friends = friendsOf(user, api)

    edges = []
    if friends != None:
        for friend in friends:
            edges.append([friend, user])

    print("Friends of " + user + ": " + str(edges))

    done.append(user)
    if d < 8 and friends != None:
        for u in friends:
            if u not in done:
                edges += generateFriendsGraph(u, d+1)
                
    if d == 0:
        # todo: generate nodes
        nodes = set()
        for conn in edges:
            nodes.add(conn[0])
            nodes.add(conn[1])
        with open('data1.json', 'w') as fp:
            json.dump({nodes: nodes, edges: edges}, fp)
    else:
        return edges
