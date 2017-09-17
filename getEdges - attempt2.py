from TwitterAuth import connectToTwitter
import time
import tweepy
import json

api = connectToTwitter()

def friendsOf(handle):
    try:
        user = api.get_user(handle)
        friendsList = [f.screen_name for f in user.friends()]
        return friendsList
    except tweepy.error.RateLimitError:
        print("Twitter API limit reached, waiting 30 seconds...")
        time.sleep(30)
        return friendsOf(handle)

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
