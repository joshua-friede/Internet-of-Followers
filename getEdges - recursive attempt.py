import json
import tweepy
import time

###################### connect to twitter #########################

### Import Twitter API Credentials
consumer_key = open('TwitterAuth/consumer_key.txt', 'r').read()
consumer_secret = open('TwitterAuth/consumer_secret.txt', 'r').read()
access_token_key = open('TwitterAuth/access_token_key.txt', 'r').read()
access_token_secret = open('TwitterAuth/access_token_secret.txt', 'r').read()

### Connect to Twitter API

auth = tweepy.OAuthHandler(consumer_key, consumer_secret)
auth.set_access_token(access_token_key, access_token_secret)


api = tweepy.API(auth)

#####################################################################



def getFollowerHandleList(handle):
    followerHandles = []
    followerIDs = api.followers_ids(screen_name = handle)

    for id in followerIDs:
        followerHandles.append(getHandleFromID(id))
    return followerHandles


def getFollowerIDList(handle):
    return api.followers_ids(screen_name = handle)


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


    
    
    
    
    
    
    



