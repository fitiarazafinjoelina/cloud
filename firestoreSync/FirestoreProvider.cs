using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace cloud.firestoreSync;

public class FirestoreProvider
{
    public FirestoreDb CreateFirestore()
    {
        string projectId = "your-project-id";
        string jsonPath = "service-account.json";
        GoogleCredential credential = GoogleCredential.FromFile(jsonPath);
        FirestoreDbBuilder builder = new FirestoreDbBuilder
        {
            ProjectId = projectId,
            Credential = credential
        };
        return builder.Build();
    }
}