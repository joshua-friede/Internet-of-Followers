from TwitterAuth import connectToTwitter
from friendsOf import friendsOf
import json



adjacencyList = {}
def generateFriendsGraph(user, d=0):
    global adjacencyList

    adjacencyList[user] = friendsOf(user)
    
    if d == 1:
        with open('data.json', 'w') as f:
            json.dumps(adjacencyList, f, sort_keys=False, indent=4, ensure_ascii=False)
        return True
    
    else:
        for u in adjacencyList[user]:
            if u not in adjacencyList:
                generateFriendsGraph(u, d+1)
