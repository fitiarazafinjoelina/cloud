using System.Runtime.InteropServices.JavaScript;
using cloud.user;

namespace cloud.helper;

public class EmailHelper {
    public static string GetValidationEmail(int idUser) {
        return $@"
            <!DOCTYPE html>
<html lang=""fr"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Validation d'Email</title>
    <style>
      /* Style pour les clients d'email */
      body {{
        font-family: Arial, sans-serif;
        line-height: 1.6;
        color: #333333;
        margin: 0;
        padding: 0;
      }}
      .container {{
        max-width: 600px;
        margin: 0 auto;
        padding: 20px;
        background-color: #f9f9f9;
        border: 1px solid #dddddd;
      }}
      .header {{
        text-align: center;
        padding: 10px 0;
      }}
      .header img {{
        max-width: 150px;
      }}
      .content {{
        padding: 20px;
        text-align: center;
      }}
      .btn {{
        display: inline-block;
        margin-top: 20px;
        padding: 12px 25px;
        font-size: 16px;
        text-decoration: none;
        color: #ffffff;
        background-color: #28a745;
        border-radius: 5px;
      }}
      .footer {{
        text-align: center;
        font-size: 12px;
        color: #777777;
        margin-top: 20px;
      }}
    </style>
  </head>
  <body>
    <div class=""container"">
      <!-- En-tête -->
      <div class=""header"">
        <img src=""https://via.placeholder.com/150"" alt=""Logo de l'entreprise"" />
      </div>
      <!-- Contenu principal -->
      <div class=""content"">
        <h1>Confirmez votre adresse email</h1>
        <p>
          Merci de vous être inscrit ! Veuillez confirmer votre adresse email
          en cliquant sur le bouton ci-dessous.
        </p>
        <a href=""users/valider?id={idUser}"" class=""btn""
          >Confirmer mon email</a
        >
        <p>
          Si vous n'avez pas demandé cette vérification, ignorez simplement ce
          message.
        </p>
      </div>
      <!-- Pied de page -->
      <div class=""footer"">
        <p>&copy; 2024 Nom de l'Entreprise. Tous droits réservés.</p>
        <p>
          <a href=""https://example.com/conditions"">Conditions d'utilisation</a>
          | <a href=""https://example.com/politique"">Politique de confidentialité</a>
        </p>
      </div>
    </div>
  </body>
</html>

        ";
    }
}