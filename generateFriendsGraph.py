from TwitterAuth import connectToTwitter
from friendsOf import friendsOf
import json



adjacencyList = {}
def generateFriendsGraph(user, d=0):
    global adjacencyList

    adjacencyList[user] = friendsOf(user)
    
    if d == 8:
        with open('data.json', 'w') as outfile:  
            json.dump(adjacencyList, outfile)
        return True
    
    else:
        for u in adjacencyList[user]:
            if u not in adjacencyList:
                generateFriendsGraph(u, d+1)
