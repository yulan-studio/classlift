-- ClassLift migration: add default Unknown province and city.
-- MySQL / MariaDB

START TRANSACTION;

INSERT INTO provinces (Name, CreatedBy, CreatedDate)
SELECT 'Unknown', MIN(Id), UTC_TIMESTAMP()
FROM users
WHERE NOT EXISTS (
    SELECT 1 FROM provinces WHERE LOWER(TRIM(Name)) = 'unknown'
)
HAVING MIN(Id) IS NOT NULL;

INSERT INTO cities (Name, ProvinceID, CreatedBy, CreatedDate)
SELECT 'Unknown', p.ProvinceID, MIN(u.Id), UTC_TIMESTAMP()
FROM users u
INNER JOIN provinces p ON LOWER(TRIM(p.Name)) = 'unknown'
WHERE NOT EXISTS (
    SELECT 1 FROM cities WHERE LOWER(TRIM(Name)) = 'unknown'
)
GROUP BY p.ProvinceID;

COMMIT;

-- Rollback (run separately only when the records are no longer referenced):
-- START TRANSACTION;
-- DELETE FROM cities
-- WHERE LOWER(TRIM(Name)) = 'unknown'
--   AND NOT EXISTS (SELECT 1 FROM coaches WHERE coaches.CityID = cities.CityID)
--   AND NOT EXISTS (SELECT 1 FROM children WHERE children.CityID = cities.CityID);
-- DELETE FROM provinces
-- WHERE LOWER(TRIM(Name)) = 'unknown'
--   AND NOT EXISTS (SELECT 1 FROM cities WHERE cities.ProvinceID = provinces.ProvinceID);
-- COMMIT;
