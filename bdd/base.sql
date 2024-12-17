INSERT INTO "user_cloud" ("email", "username", "password", "nb_tentative") VALUES
   ('alice@example.com', 'alice', 'password123', 0),
   ('bob@example.com', 'bob', 'securePass456', 0),
   ('charlie@example.com', 'charlie', 'myP@ssw0rd', 0),
   ('david@example.com', 'david', 'Password987', 0),
   ('emma@example.com', 'emma', 'em12345password', 0);

INSERT INTO "token" ("token", "date_debut", "date_fin", "id_user") VALUES
   ('abcd1234efgh5678ijkl9012mnop3456', '2024-12-17 10:00:00', '2024-12-18 10:00:00', 1),
   ('pqrs7890uvwx1234yzab5678cdef9012', '2024-12-17 11:00:00', '2024-12-18 11:00:00', 2),
   ('ijkl4567mnop8901qrst2345uvwx6789', '2024-12-17 12:00:00', '2024-12-18 12:00:00', 3),
   ('mnop1234qrst5678uvwx9012yzab3456', '2024-12-17 13:00:00', '2024-12-18 13:00:00', 4),
   ('abcd6789efgh0123ijkl3456mnop7890', '2024-12-17 14:00:00', '2024-12-18 14:00:00', 5);