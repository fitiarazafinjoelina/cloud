using cloud.Database;
using cloud.email;
using cloud.helper;
using cloud.userValidation;

namespace cloud.user;

public class UserService {
    private readonly AppDbContext _context;

    public UserService(AppDbContext context) {
        _context = context;
    }

    public UserValidation SignUpUser(UserInscriptionDTO userInscriptionDto) {
        UserValidation userValidation = new UserValidation {
            Username = userInscriptionDto.Username,
            Email = userInscriptionDto.Email,
            Password = PasswordHelper.HashPassword(userInscriptionDto.Password)
        };

        _context.UserValidations.Add(userValidation);
        _context.SaveChanges();

        return userValidation;
    }
}