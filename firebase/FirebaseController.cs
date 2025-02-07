using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace cloud.firebase;

[ApiController]
[Route("/api/firebase")]
public class FirebaseController
{
    private readonly FirebaseService _firebaseService;

    public FirebaseController(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<List<ExportedUserRecord>> GetFirebaseUsers()
    {
        return await _firebaseService.GetUsersAsync();
    }
}