using cloud.Database;
using cloud.helper;
using cloud.user;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace cloud.firebase;

public class FirebaseService
{
    private readonly FirebaseAuth _auth;
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;

    public FirebaseService(HttpClient httpClient, AppDbContext context)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("./serviceAccount.json")
            });
        }

        _auth = FirebaseAuth.DefaultInstance;
        _httpClient = httpClient;
        _context = context;
    }
    
    public async Task<List<ExportedUserRecord>> GetUsersAsync()
    {
        var users = new List<ExportedUserRecord>();
        var pagedEnumerable = _auth.ListUsersAsync(null);

        await foreach (var user in pagedEnumerable)
        {
            users.Add(user);
        }

        return users;
    }

    public async Task<UserRecord> AddUserToFirebase(UserInscriptionDTO userInscriptionDto)
    {
        var userArgs = new UserRecordArgs()
        {
            Email = userInscriptionDto.Email,
            EmailVerified = false,
            Password = userInscriptionDto.Password,
            DisplayName = userInscriptionDto.Email,
            Disabled = false,
        };
        
        return await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
    }

    public async Task<UserRecord> SetEmailVerified(string uid)
    {
        return await _auth.UpdateUserAsync(new UserRecordArgs()
        {
            Uid = uid,
            EmailVerified = true
        });
    }

    public async Task<User> registerUserByUID(string uid, string password)
    {
        // FirebaseToken firebaseToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        User user = new()
        {
            Uid = userRecord.Uid,
            Username = userRecord.DisplayName,
            Password = PasswordHelper.HashPassword(password),
            Email = userRecord.Email,
            NbTentative = 0,
            Verified = userRecord.EmailVerified,
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<FirebaseToken> VerifyAndDecode(string token)
    {
        return await _auth.VerifyIdTokenAsync(token);
    }

    // public async Task<string> SignInWithEmailAndPassword(string email, string password)
    // {
    //     var request = new
    //     {
    //         email = email,
    //         password = password,
    //         returnSecureToken = true
    //     };
    //
    //     var response = await _httpClient.PostAsJsonAsync(
    //         $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}",
    //         request);
    //
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         throw new Exception("Invalid login credentials.");
    //     }
    //
    //     var result = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
    //     return result.IdToken;
    // }
    

    // public async Task<ExportedUserRecord> GetUserByEmailAndPassword(string email, string password)
    // {
    //     _auth.
    // }

}