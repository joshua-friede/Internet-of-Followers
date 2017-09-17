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
        with open('data1.json', 'w') as fp:
            json.dump(adjacencyList, fp)
        return True
    
    else:
        for u in adjacencyList[user]:
            if u not in adjacencyList:
                generateFriendsGraph(u, d+1)
