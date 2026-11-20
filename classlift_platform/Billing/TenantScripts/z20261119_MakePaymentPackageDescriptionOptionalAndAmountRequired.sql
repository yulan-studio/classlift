-- Payment Package schema correction
-- Description: nullable
-- Amount: required


ALTER TABLE payment_package
    MODIFY COLUMN Description LONGTEXT NULL,
    MODIFY COLUMN Amount DECIMAL(10,2) NOT NULL;
