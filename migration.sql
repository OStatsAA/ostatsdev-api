CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    CREATE TABLE "Projects" (
        "Id" uuid NOT NULL,
        "Title" character varying(256) NOT NULL,
        "Description" character varying(2048),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Projects" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "Name" character varying(128) NOT NULL,
        "Email" character varying(128) NOT NULL,
        "AuthIdentity" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    CREATE TABLE "Roles" (
        "Id" uuid NOT NULL,
        "ProjectId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "AccessLevel" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Roles" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Roles_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Roles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    CREATE INDEX "IX_Roles_ProjectId" ON "Roles" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    CREATE INDEX "IX_Roles_UserId" ON "Roles" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Users_AuthIdentity" ON "Users" ("AuthIdentity");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231110191241_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20231110191241_InitialCreate', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231129184429_AddDatasetConfiguration') THEN
    CREATE TABLE "DatasetsConfigurations" (
        "Id" uuid NOT NULL,
        "ProjectId" uuid NOT NULL,
        "Title" character varying(256) NOT NULL,
        "Source" character varying(512) NOT NULL,
        "Description" character varying(2048),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_DatasetsConfigurations" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_DatasetsConfigurations_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231129184429_AddDatasetConfiguration') THEN
    CREATE INDEX "IX_DatasetsConfigurations_ProjectId" ON "DatasetsConfigurations" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231129184429_AddDatasetConfiguration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20231129184429_AddDatasetConfiguration', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    DROP TABLE "DatasetsConfigurations";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE TABLE "Datasets" (
        "Id" uuid NOT NULL,
        "Title" character varying(256) NOT NULL,
        "Source" character varying(256) NOT NULL,
        "Description" character varying(4096),
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Datasets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE TABLE "DatasetsProjectsLinks" (
        "Id" uuid NOT NULL,
        "DatasetId" uuid NOT NULL,
        "ProjectId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_DatasetsProjectsLinks" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_DatasetsProjectsLinks_Datasets_DatasetId" FOREIGN KEY ("DatasetId") REFERENCES "Datasets" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_DatasetsProjectsLinks_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE TABLE "DatasetsUsersAccessLevels" (
        "Id" uuid NOT NULL,
        "DatasetId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "AccessLevel" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_DatasetsUsersAccessLevels" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_DatasetsUsersAccessLevels_Datasets_DatasetId" FOREIGN KEY ("DatasetId") REFERENCES "Datasets" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_DatasetsUsersAccessLevels_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE INDEX "IX_DatasetsProjectsLinks_DatasetId" ON "DatasetsProjectsLinks" ("DatasetId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE INDEX "IX_DatasetsProjectsLinks_ProjectId" ON "DatasetsProjectsLinks" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE INDEX "IX_DatasetsUsersAccessLevels_DatasetId" ON "DatasetsUsersAccessLevels" ("DatasetId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    CREATE INDEX "IX_DatasetsUsersAccessLevels_UserId" ON "DatasetsUsersAccessLevels" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231217210159_AddDatasets') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20231217210159_AddDatasets', '8.0.0');
    END IF;
END $EF$;
COMMIT;

