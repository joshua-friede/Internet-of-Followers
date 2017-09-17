from TwitterAuth import connectToTwitter
import tweepy
import time

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
