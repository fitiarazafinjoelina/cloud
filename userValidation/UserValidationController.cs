using cloud.Database;
using cloud.user;
using cloud.Utils;
using Microsoft.AspNetCore.Mvc;

namespace cloud.userValidation;

[Controller]
[Route("/api/[controller]")]
public class UserValidationController {
    private readonly AppDbContext _context;

    public UserValidationController(AppDbContext context) {
        _context = context;
    }


    [HttpGet("/{id}")]
    public async Task<ResponseBody> Validate(int id) {
        ResponseBody response = new ResponseBody();
        try {
            UserValidation? userValidation = _context.UserValidations.FirstOrDefault(u => u.Id == id, null);
            if (userValidation == null) {
                throw new Exception($"Aucun utilisateur en cours de validation trouvee pour id: {id}");
            }

            User user = new User() {
                Email = userValidation.Email,
                Username = userValidation.Username,
                Password = userValidation.Password,
                NbTentative = 0
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            response.StatusCode = 200;
            response.Data = new { IdUser = id };
            response.Message = $"Cher {user.Username},  Votre Compte a ete bien confirme";
        }
        catch (Exception e) {
            response.StatusCode = 500;
            response.Message = e.Message;
        }

        return response;
    }
}