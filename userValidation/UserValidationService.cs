using cloud.Database;
using cloud.firebase;
using cloud.user;
using Microsoft.EntityFrameworkCore;

namespace cloud.userValidation;

public class UserValidationService {
    private readonly AppDbContext _context;
    private readonly FirebaseService _firebaseService;

    public UserValidationService(AppDbContext context, FirebaseService firebaseService) {
        _context = context;
        _firebaseService = firebaseService;
    }

    public async Task<User> ValidateUser(int id) {
        await using var transaction = _context.Database.BeginTransaction();

        try
        {
            UserValidation? userValidation = _context.UserValidations.FirstOrDefault(u => u.Id == id);
            if (userValidation == null)
            {
                throw new Exception($"Aucun utilisateur en cours de validation trouvee pour uid: {id}");
            }

            User user = new User
            {
                Email = userValidation.Email,
                Username = userValidation.Username,
                Password = userValidation.Password,
                NbTentative = 0,
                Uid = "",
                Verified = true
            };
            

            _context.Users.Add(user);
            _context.UserValidations.Remove(userValidation);
            _context.SaveChanges();
            transaction.Commit();
            // _firebaseService.SetEmailVerified(uid);
            return user;

        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw e;
        }

    }
}