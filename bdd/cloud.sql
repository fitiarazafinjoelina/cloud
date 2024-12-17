CREATE TABLE "user_cloud"(
                             "id_user_cloud" SERIAL NOT NULL,
                             "email" VARCHAR(255) NOT NULL,
                             "username" VARCHAR(255) NOT NULL,
                             "password" VARCHAR(255) NOT NULL,
                             "nb_tentative" INTEGER NOT NULL
);
ALTER TABLE
    "user_cloud" ADD PRIMARY KEY("id_user_cloud");
CREATE TABLE "token"(
                        "id_token" SERIAL NOT NULL,
                        "token" VARCHAR(255) NOT NULL,
                        "date_debut" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
                        "date_fin" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
                        "id_user" INTEGER NOT NULL
);
ALTER TABLE
    "token" ADD PRIMARY KEY("id_token");
CREATE TABLE "pin"(
                      "id_pin" SERIAL NOT NULL,
                      "id_user" INTEGER NOT NULL,
                      "pin_number" INTEGER NOT NULL,
                      "date_debut" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
                      "date_fin" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL
);
ALTER TABLE
    "pin" ADD PRIMARY KEY("id_pin");
ALTER TABLE
    "token" ADD CONSTRAINT "token_id_user_foreign" FOREIGN KEY("id_user") REFERENCES "user_cloud"("id_user_cloud");
ALTER TABLE
    "pin" ADD CONSTRAINT "pin_id_user_foreign" FOREIGN KEY("id_user") REFERENCES "user_cloud"("id_user_cloud");

CREATE TABLE "temporary_token"(
                        "id_token" SERIAL NOT NULL,
                        "token" VARCHAR(255) NOT NULL,
                        "date_debut" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
                        "date_fin" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
                        "id_user" INTEGER NOT NULL
);
ALTER TABLE
    "temporary_token" ADD PRIMARY KEY("id_token");
ALTER TABLE
    "temporary_token" ADD CONSTRAINT "temp_token_id_user_foreign" FOREIGN KEY("id_user") REFERENCES "user_cloud"("id_user_cloud");