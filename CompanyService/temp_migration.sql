START TRANSACTION;
ALTER TABLE "Sales" ALTER COLUMN "ElectronicInvoiceId" DROP NOT NULL;

ALTER TABLE "Roles" ADD "Description" text NOT NULL DEFAULT '';

ALTER TABLE "Menus" ALTER COLUMN "ParentId" TYPE uuid;

ALTER TABLE "Menus" ALTER COLUMN "Id" TYPE uuid;
ALTER TABLE "Menus" ALTER COLUMN "Id" DROP IDENTITY;

ALTER TABLE "CompanyMenuConfigurations" ALTER COLUMN "MenuId" TYPE uuid;

ALTER TABLE "CompanyMenuConfigurations" ALTER COLUMN "Id" TYPE uuid;
ALTER TABLE "CompanyMenuConfigurations" ALTER COLUMN "Id" DROP IDENTITY;

ALTER TABLE "Approvals" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Approvals" ADD "LastModifiedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Approvals" ADD "RequestDate" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "ApprovalLevels" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "ApprovalLevels" ADD "LastModifiedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "ApprovalLevels" ADD "RequiresAllApprovers" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250922164418_MakeElectronicInvoiceIdNullable', '9.0.5');

COMMIT;

