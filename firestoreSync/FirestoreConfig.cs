using FirebaseAdmin;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using Grpc.Net.Client;

namespace cloud.firestoreSync;
public class FirestoreConfig
{
    public static async Task<FirestoreDb> GetFirestoreDbAsync()
    {
        // const string emulatorHost = "172.23.160.1:8082";
        // Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", emulatorHost);

        FirestoreDb firestoreDb;
        if (Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_HOST") != null)
        {
            Console.WriteLine($"Using Firestore Emulator on {Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_HOST")}... ");
            firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = "demo",
                EmulatorDetection = EmulatorDetection.EmulatorOnly,
            }.Build();
        }
        else
        {
            Console.WriteLine("Using real Firestore...");
            
            // var defaultApp = FirebaseApp.Create(new AppOptions()
            // {
            //     Credential = GoogleCredential.GetApplicationDefault(),
            //     ProjectId = "cloud-syncing"
            // });
            firestoreDb = FirestoreDb.Create("cloud-syncing");
        }

        return firestoreDb;
    }
}