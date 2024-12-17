CREATE TABLE "user_cloud"(
    "id_user" SERIAL NOT NULL,
    "email" VARCHAR(255) NOT NULL,
    "username" VARCHAR(255) NOT NULL,
    "password" VARCHAR(255) NOT NULL,
    "id_role" INTEGER NOT NULL
);
ALTER TABLE
    "user_cloud" ADD PRIMARY KEY("id_user");
CREATE TABLE "role"(
    "id_role" SERIAL NOT NULL,
    "role" VARCHAR(255) NOT NULL
);
ALTER TABLE
    "role" ADD PRIMARY KEY("id_role");
CREATE TABLE "token"(
    "id_token" SERIAL NOT NULL,
    "token" VARCHAR(255) NOT NULL,
    "date_debut" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
    "date_fin" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
    "id_user" INTEGER NOT NULL
);
ALTER TABLE
    "token" ADD PRIMARY KEY("id_token");
ALTER TABLE
    "user_cloud" ADD CONSTRAINT "user_id_role_foreign" FOREIGN KEY("id_role") REFERENCES "role"("id_role");
ALTER TABLE
    "token" ADD CONSTRAINT "token_id_user_foreign" FOREIGN KEY("id_user") REFERENCES "user_cloud"("id_user");
    
ALTER TABLE "user_cloud" 
ALTER COLUMN "password" TYPE VARCHAR(300);