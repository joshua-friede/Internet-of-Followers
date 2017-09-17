from TwitterAuth import connectToTwitter
from friendsOf import friendsOf
import json

adjacencyList = {}

# example: generateFriendsGraph("katyperry")

def generateFriendsGraph(user, d=0):
    global adjacencyList

    adjacencyList[user] = friendsOf(user)
    
    if d == 8:
        # write adjacencyList to json file
        file = open("data1.json", "w")
        file.write(simplejson.dumps(simplejson.loads(output), indent=4, sort_keys=False))
        file.close()

        return True
    
    else:
        for u in adjacencyList[user]:
            if u not in adjacencyList:
                generateFriendsGraph(u, d+1)
