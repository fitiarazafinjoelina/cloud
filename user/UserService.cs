using cloud.Database;
using cloud.email;
using cloud.firebase;
using cloud.helper;
using cloud.userValidation;
using FirebaseAdmin.Auth;

namespace cloud.user;

public class UserService {
    private readonly AppDbContext _context;
    private readonly FirebaseService _firebaseService;

    public UserService(AppDbContext context, FirebaseService firebaseService) {
        _context = context;
        _firebaseService = firebaseService;
    }

    public async Task<UserValidation> SignUpUser(UserInscriptionDTO userInscriptionDto)
    {
        await using var transaction = _context.Database.BeginTransaction();
        try
        {
            // UserRecord userRecord = await _firebaseService.AddUserToFirebase(userInscriptionDto);
            UserValidation userValidation = new UserValidation
            {
                Username = userInscriptionDto.Username,
                Email = userInscriptionDto.Email,
                Password = PasswordHelper.HashPassword(userInscriptionDto.Password),
                Uid = ""
            };

            _context.UserValidations.Add(userValidation);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return userValidation;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw e;
        }

    }
    
    // public async updateToke
}