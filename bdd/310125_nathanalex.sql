


ALTER TABLE "user_cloud" ADD COLUMN "uid" VARCHAR(255);
ALTER TABLE "user_cloud" ADD COLUMN "verified" BOOLEAN DEFAULT FALSE;
-- ALTER TABLE "user_validation" ADD COLUMN "uid" VARCHAR(255);
ALTER TABLE "user_cloud" ADD COLUMN "token" TEXT;



    