import time
from TwitterAuth import connectToTwitter

api = connectToTwitter()

def getFollowerHandleList(handle):
    followerHandles = []
    followerIDs = api.followers_ids(screen_name = handle)

    for id in followerIDs:
        followerHandles.append(getHandleFromID(id))
    return followerHandles

def getFollowerIDList(handle):
    return api.friends_ids(screen_name = handle)

def getFriendIDList(handle):
    return api.friends_ids(screen_name = handle)

def getHandleFromID(id):

    handle = ""

    while handle == "":
        try:
            user = api.get_user(id)
            return user.screen_name
        except tweepy.error.RateLimitError:
            print("Twitter API Limit Reached, waiting 30 seconds")
            time.sleep(30)
        
    return handle

done = []
def followers(user, depth):
    global done
    edges = [] # tuple (user, follower)

    for follower in getFollowerIDList(user):
        edges.append( (user, follower) )
        done.append(user)

    if depth == 1:
        
        return edges
    
    else:
        for u in getFollowerIDList(user):
            if u not in done:
                edges.extend(followers(u, depth+1))
        return edges


    
    
    
    
    
    
    



