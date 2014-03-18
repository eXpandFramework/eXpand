using System.Threading;

namespace ProcessAsUser {
    public class RDCClient {
        public static bool Connect(string userName, string password){
            var done = new ManualResetEventSlim();
            bool connect = false;
            var processCreationThread = new Thread(() => {
                var form = new Form1();
                connect = form.Connect(userName, password);
                done.Set();
            });
            processCreationThread.SetApartmentState(ApartmentState.STA);
            processCreationThread.Start();
            done.Wait();
            return connect;
        }
    }
}
