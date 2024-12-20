# Cloud

## Demarrer l'environnement et application ASP.NET
```docker compose up --build```

## Pour eteindre l'application
```docker compose down -v```

## Scénario
    -Inscription
    -Login renvoie un token temporaire à utiliser dans Validation pin
    -Validation pin renvoie le token que l'utilisateur doit utiliser
    -Réinitialisation d'email :
        .Doit envoyer une requête de réinitialisation par email
    -Une fois fait aller dans le lien unique fourni par la réinitialisation de l'email
    -Modification de nom d'utilisateur nécessite le token actuel (provenance validation pin)
    -Modification de mot de passe nécessite le token actuel (provenance validation pin) 