using LinqToTwitter;

namespace XTwitter {
    class Program {

        static void Main(string[] args) {
            var auth = new SingleUserAuthorizer {
                Credentials = new SingleUserInMemoryCredentials {
                    ConsumerKey = args[0],
                    ConsumerSecret = args[1],
                    TwitterAccessToken = args[2],
                    TwitterAccessTokenSecret = args[3]
                }
            };

            var twitterCtx = new TwitterContext(auth);
            twitterCtx.UpdateStatus(args[4]);
        }
    }
}
