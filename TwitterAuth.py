import tweepy

### Connect to Twitter API
def connectToTwitter():

    ### Import Twitter API Credentials
    consumer_key = open('TwitterAuth/consumer_key.txt', 'r').read()
    consumer_secret = open('TwitterAuth/consumer_secret.txt', 'r').read()
    access_token_key = open('TwitterAuth/access_token_key.txt', 'r').read()
    access_token_secret = open('TwitterAuth/access_token_secret.txt', 'r').read()

    auth = tweepy.OAuthHandler(consumer_key, consumer_secret)
    auth.set_access_token(access_token_key, access_token_secret)
    
    return tweepy.API(auth)
