create table "user_validation" (
    "id" SERIAL primary key not null ,
    "username" varchar(255) not null ,
    "email" varchar(255) not null ,
    "password" varchar(255) not null
);

