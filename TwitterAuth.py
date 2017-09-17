import tweepy

### Connect to Twitter API
def connectToTwitter():

    print("Reading Twitter auth tokens...")

    ### Import Twitter API Credentials
    consumer_key = open('TwitterAuth/consumer_key.txt', 'r').read()
    consumer_secret = open('TwitterAuth/consumer_secret.txt', 'r').read()
    access_token_key = open('TwitterAuth/access_token_key.txt', 'r').read()
    access_token_secret = open('TwitterAuth/access_token_secret.txt', 'r').read()

    print("Creating auth tokens...")

    auth = tweepy.OAuthHandler(consumer_key, consumer_secret)
    auth.set_access_token(access_token_key, access_token_secret)

    print("Initializing Twitter API...")
    
    return tweepy.API(auth, wait_on_rate_limit=True, retry_count=5, retry_delay=10, retry_errors=set([401, 404, 500, 503]))
