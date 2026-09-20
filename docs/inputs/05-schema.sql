-- Current shape of the orders database, exported for reference.
-- SQLite. Times are stored as UTC ticks.

CREATE TABLE Customers (
    Id        TEXT    NOT NULL PRIMARY KEY,
    Reference TEXT    NOT NULL,
    FullName  TEXT    NOT NULL,
    Email     TEXT    NOT NULL,
    Phone     TEXT    NOT NULL
);
CREATE UNIQUE INDEX IX_Customers_Reference ON Customers (Reference);

CREATE TABLE Orders (
    Id          TEXT    NOT NULL PRIMARY KEY,
    Reference   TEXT    NOT NULL,
    CustomerId  TEXT    NOT NULL,
    PlacedUtc   INTEGER NOT NULL,   -- UTC ticks
    ShippedUtc  INTEGER NULL,       -- UTC ticks
    Status      INTEGER NOT NULL,   -- 0 Placed, 1 Shipped, 2 Delivered, 3 Cancelled
    TotalAmount TEXT    NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers (Id)
);
CREATE UNIQUE INDEX IX_Orders_Reference ON Orders (Reference);

CREATE TABLE OrderLines (
    Id          TEXT    NOT NULL PRIMARY KEY,
    OrderId     TEXT    NOT NULL,
    Sku         TEXT    NOT NULL,
    Description TEXT    NOT NULL,
    Quantity    INTEGER NOT NULL,
    UnitPrice   TEXT    NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE
);

-- Append only. Every state change on an order writes one of these.
CREATE TABLE AuditEntries (
    Id          TEXT    NOT NULL PRIMARY KEY,
    EntityType  TEXT    NOT NULL,
    EntityId    TEXT    NOT NULL,
    Action      TEXT    NOT NULL,
    Actor       TEXT    NOT NULL,
    OccurredUtc INTEGER NOT NULL,
    Reason      TEXT    NOT NULL
);
CREATE INDEX IX_AuditEntries_Entity ON AuditEntries (EntityType, EntityId);
