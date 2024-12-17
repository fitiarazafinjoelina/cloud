using cloud.Database;
using cloud.user;

namespace cloud.userValidation;

public class UserValidationService {
    private readonly AppDbContext _context;

    public UserValidationService(AppDbContext context) {
        _context = context;
    }

    public User ValidateUser(int id) {
        UserValidation? userValidation = _context.UserValidations.FirstOrDefault(u => u.Id == id);
        if (userValidation == null) {
            throw new Exception($"Aucun utilisateur en cours de validation trouvee pour id: {id}");
        }

        User user = new User {
            Email = userValidation.Email,
            Username = userValidation.Username,
            Password = userValidation.Password,
            NbTentative = 0
        };

        _context.Users.Add(user);
        _context.UserValidations.Remove(userValidation);
        _context.SaveChanges();
        return user;
    }
}