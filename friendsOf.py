import tweepy
import time

def friendsOf(handle, api, count=20):
    while True:
        print("Getting friends of " + handle + "...")
        try:
            u = api.get_user(id=handle)
            if not u.protected:
                return list(map(lambda f: f.screen_name, tweepy.Cursor(api.followers, id=handle).items(count)))
            else:
                return []
        except tweepy.error.RateLimitError:
            print("Twitter API limit reached, waiting 30 seconds...")
            time.sleep(30)
