USE master;
GO

DROP DATABASE IF EXISTS CollegeBackend;
GO

CREATE DATABASE CollegeBackend;
GO

USE CollegeBackend;
GO

CREATE TABLE [active]
(
    [active_id]            int      NOT NULL IDENTITY (1,1),
    [station_id]           int      NOT NULL,
    [start_date_time]      datetime NOT NULL,
    [main_direction_id]    int      NOT NULL,
    [train_id]             int      NOT NULL,
    [main_start_date_time] datetime NOT NULL,
    CONSTRAINT [_copy_2] PRIMARY KEY CLUSTERED ([active_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [carriages]
(
    [carriage_id]      int     NOT NULL IDENTITY (1, 1),
    [index]            char(3) NOT NULL,
    [related_train_id] int     NOT NULL,
    CONSTRAINT [_copy_5] PRIMARY KEY CLUSTERED ([carriage_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [directions]
(
    [direction_id]     int           NOT NULL IDENTITY (1,1),
    [name]             nvarchar(255) NOT NULL,
    [start_station_id] int           NOT NULL,
    [end_station_id]   int           NOT NULL,
    CONSTRAINT [_copy_1] PRIMARY KEY CLUSTERED ([direction_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [sitting]
(
    [sit_id]              int          NOT NULL IDENTITY (1,1),
    [index]               char(3)      NOT NULL,
    [related_carriage_id] int          NOT NULL,
    [sit_type]            nvarchar(10) NOT NULL DEFAULT N'Плацкарт',
    [price]               int          NOT NULL DEFAULT 5000,
    CONSTRAINT [_copy_6] PRIMARY KEY CLUSTERED ([sit_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [stations]
(
    [station_id]        int           NOT NULL IDENTITY (1,1),
    [related_direction] int           NOT NULL,
    [name]              nvarchar(255) NOT NULL,
    CONSTRAINT [_copy_4] PRIMARY KEY CLUSTERED ([station_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [tickets]
(
    [ticket_id]            int  NOT NULL IDENTITY (1,1),
    [related_direction_id] int  NOT NULL,
    [related_active_id]    int  NOT NULL,
    [start_date]           date NOT NULL,
    [passport_id]          int  NULL,
    [end_station_id]       int  NOT NULL,
    [sitting_id]           int  NOT NULL UNIQUE,
    CONSTRAINT [_copy_3] PRIMARY KEY CLUSTERED ([ticket_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

CREATE TABLE [trains]
(
    [train_id] int           NOT NULL IDENTITY (1,1),
    [name]     nvarchar(255) NOT NULL,
    CONSTRAINT [_copy_7] PRIMARY KEY CLUSTERED ([train_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) 
GO

CREATE TABLE [users]
(
    [passport_id] int           NOT NULL,
    [first_name]  nvarchar(255) NOT NULL,
    [second_name] nvarchar(255) NOT NULL,
    [patronymic]  nvarchar(255) NOT NULL,
    [role]        nvarchar(255) NOT NULL,
    [username]    nvarchar(max) NOT NULL,
    [password]    nvarchar(max) NOT NULL,
    PRIMARY KEY CLUSTERED ([passport_id])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO

ALTER TABLE [active]
    ADD CONSTRAINT [fk_active_stations_1] FOREIGN KEY ([station_id]) REFERENCES [stations] ([station_id])
GO
ALTER TABLE [active]
    ADD CONSTRAINT [fk_active_directions_1] FOREIGN KEY ([main_direction_id]) REFERENCES [directions] ([direction_id])
GO
ALTER TABLE [active]
    ADD CONSTRAINT [fk_active_trains_1] FOREIGN KEY ([train_id]) REFERENCES [trains] ([train_id])
    GO
ALTER TABLE [carriages]
    ADD CONSTRAINT [fk_carriages_trains_1] FOREIGN KEY ([related_train_id]) REFERENCES [trains] ([train_id])
GO
ALTER TABLE [sitting]
    ADD CONSTRAINT [fk_sitting_carriages_1] FOREIGN KEY ([related_carriage_id]) REFERENCES [carriages] ([carriage_id])
GO
ALTER TABLE [tickets]
    ADD CONSTRAINT [fk_sitting_tickets_1] FOREIGN KEY ([sitting_id]) REFERENCES [sitting] ([sit_id])
GO
ALTER TABLE [stations]
    ADD CONSTRAINT [fk_stations_directions_1] FOREIGN KEY ([related_direction]) REFERENCES [directions] ([direction_id])
GO
ALTER TABLE [tickets]
    ADD CONSTRAINT [fk_tickets_directions_1] FOREIGN KEY ([related_direction_id]) REFERENCES [directions] ([direction_id])
GO
ALTER TABLE [tickets]
    ADD CONSTRAINT [fk_tickets_active_1] FOREIGN KEY ([related_active_id]) REFERENCES [active] ([active_id])
GO
ALTER TABLE [tickets]
    ADD CONSTRAINT [fk_tickets_stations_1] FOREIGN KEY ([end_station_id]) REFERENCES [stations] ([station_id])
GO
ALTER TABLE [tickets]
    ADD CONSTRAINT [fk_users_tickets_1] FOREIGN KEY ([passport_id]) REFERENCES [users] ([passport_id])
GO
    
INSERT INTO trains (name)
VALUES (N'Ингушетия'),
    (N'Италмас'),
    (N'Пассажирский'),
    (N'Красная стрела'),
    (N'Скорый');

INSERT INTO carriages ([index], related_train_id)
VALUES ('A1', 1),
       ('A2', 1),
       ('A3', 1),
       ('A4', 1),
       ('A5', 1),
       ('A1', 2),
       ('A2', 2),
       ('A3', 2),
       ('A4', 2),
       ('A5', 2),
       ('A1', 3),
       ('A2', 3),
       ('A3', 3),
       ('A4', 3),
       ('A5', 3),
       ('A1', 4),
       ('A2', 4),
       ('A3', 4),
       ('A4', 4),
       ('A5', 4),
       ('A1', 5),
       ('A2', 5),
       ('A3', 5),
       ('A4', 5),
       ('A5', 5);

INSERT INTO sitting ([index], related_carriage_id, sit_type, price)
VALUES ('A1', 1, N'Купе', 4000),
       ('A2', 1, N'Купе', 3000),
       ('B3', 1, N'Купе', 4000),
       ('A4', 1, N'Купе', 2000),
       ('B5', 1, N'Плацкарт', 4000),
       ('A6', 1, N'Плацкарт', 3000),
       ('C7', 1, N'Плацкарт', 4000),
       ('C8', 1, N'Плацкарт', 4000),
       ('B9', 1, N'Купе', 1000),
       ('A10', 1, N'Купе', 4000),
       ('A1', 2, N'Купе', 4000),
       ('C2', 2, N'Плацкарт', 3000),
       ('C3', 2, N'Купе', 3000),
       ('B4', 2, N'Плацкарт', 1000),
       ('A5', 2, N'Плацкарт', 2000),
       ('A6', 2, N'Плацкарт', 2000),
       ('A7', 2, N'Купе', 2000),
       ('B8', 2, N'Плацкарт', 1000),
       ('C9', 2, N'Плацкарт', 4000),
       ('C10', 2, N'Плацкарт', 1000),
       ('C1', 3, N'Плацкарт', 3000),
       ('C2', 3, N'Плацкарт', 2000),
       ('A3', 3, N'Плацкарт', 4000),
       ('B4', 3, N'Купе', 2000),
       ('C5', 3, N'Купе', 4000),
       ('B6', 3, N'Купе', 2000),
       ('B7', 3, N'Купе', 2000),
       ('C8', 3, N'Купе', 1000),
       ('C9', 3, N'Купе', 3000),
       ('A10', 3, N'Купе', 4000),
       ('C1', 4, N'Купе', 4000),
       ('A2', 4, N'Купе', 2000),
       ('A3', 4, N'Плацкарт', 1000),
       ('C4', 4, N'Плацкарт', 3000),
       ('A5', 4, N'Купе', 1000),
       ('B6', 4, N'Плацкарт', 4000),
       ('C7', 4, N'Плацкарт', 4000),
       ('B8', 4, N'Купе', 3000),
       ('A9', 4, N'Купе', 1000),
       ('A10', 4, N'Плацкарт', 4000),
       ('A1', 5, N'Купе', 2000),
       ('A2', 5, N'Купе', 3000),
       ('B3', 5, N'Купе', 2000),
       ('A4', 5, N'Плацкарт', 1000),
       ('B5', 5, N'Купе', 4000),
       ('C6', 5, N'Плацкарт', 4000),
       ('C7', 5, N'Плацкарт', 2000),
       ('B8', 5, N'Купе', 4000),
       ('A9', 5, N'Купе', 2000),
       ('A10', 5, N'Купе', 4000),
       ('B1', 6, N'Плацкарт', 3000),
       ('C2', 6, N'Плацкарт', 2000),
       ('B3', 6, N'Купе', 2000),
       ('C4', 6, N'Купе', 2000),
       ('A5', 6, N'Купе', 4000),
       ('B6', 6, N'Купе', 4000),
       ('A7', 6, N'Купе', 3000),
       ('A8', 6, N'Купе', 1000),
       ('C9', 6, N'Купе', 1000),
       ('A10', 6, N'Купе', 1000),
       ('A1', 7, N'Плацкарт', 2000),
       ('A2', 7, N'Купе', 2000),
       ('B3', 7, N'Плацкарт', 3000),
       ('A4', 7, N'Купе', 4000),
       ('A5', 7, N'Плацкарт', 3000),
       ('A6', 7, N'Купе', 4000),
       ('A7', 7, N'Плацкарт', 2000),
       ('C8', 7, N'Купе', 1000),
       ('C9', 7, N'Плацкарт', 2000),
       ('A10', 7, N'Плацкарт', 3000),
       ('C1', 8, N'Плацкарт', 2000),
       ('C2', 8, N'Купе', 2000),
       ('C3', 8, N'Плацкарт', 4000),
       ('A4', 8, N'Плацкарт', 1000),
       ('B5', 8, N'Плацкарт', 1000),
       ('C6', 8, N'Плацкарт', 3000),
       ('B7', 8, N'Купе', 3000),
       ('B8', 8, N'Плацкарт', 4000),
       ('C9', 8, N'Купе', 1000),
       ('B10', 8, N'Плацкарт', 4000),
       ('C1', 9, N'Плацкарт', 2000),
       ('B2', 9, N'Плацкарт', 1000),
       ('A3', 9, N'Купе', 3000),
       ('C4', 9, N'Плацкарт', 2000),
       ('A5', 9, N'Купе', 4000),
       ('A6', 9, N'Плацкарт', 2000),
       ('C7', 9, N'Купе', 3000),
       ('B8', 9, N'Купе', 2000),
       ('C9', 9, N'Купе', 1000),
       ('A10', 9, N'Плацкарт', 4000),
       ('C1', 10, N'Плацкарт', 2000),
       ('B2', 10, N'Купе', 2000),
       ('A3', 10, N'Плацкарт', 4000),
       ('B4', 10, N'Купе', 3000),
       ('B5', 10, N'Плацкарт', 3000),
       ('A6', 10, N'Плацкарт', 4000),
       ('B7', 10, N'Плацкарт', 1000),
       ('A8', 10, N'Купе', 3000),
       ('B9', 10, N'Купе', 1000),
       ('C10', 10, N'Плацкарт', 4000),
       ('B1', 11, N'Купе', 4000),
       ('B2', 11, N'Купе', 2000),
       ('B3', 11, N'Купе', 1000),
       ('B4', 11, N'Плацкарт', 2000),
       ('A5', 11, N'Плацкарт', 1000),
       ('B6', 11, N'Купе', 2000),
       ('C7', 11, N'Плацкарт', 3000),
       ('A8', 11, N'Купе', 3000),
       ('B9', 11, N'Купе', 1000),
       ('C10', 11, N'Купе', 1000),
       ('B1', 12, N'Плацкарт', 2000),
       ('C2', 12, N'Плацкарт', 1000),
       ('C3', 12, N'Плацкарт', 4000),
       ('A4', 12, N'Купе', 4000),
       ('C5', 12, N'Купе', 1000),
       ('C6', 12, N'Плацкарт', 4000),
       ('C7', 12, N'Плацкарт', 1000),
       ('A8', 12, N'Плацкарт', 1000),
       ('A9', 12, N'Плацкарт', 2000),
       ('C10', 12, N'Купе', 1000),
       ('A1', 13, N'Плацкарт', 4000),
       ('C2', 13, N'Купе', 1000),
       ('C3', 13, N'Купе', 3000),
       ('B4', 13, N'Плацкарт', 3000),
       ('C5', 13, N'Купе', 1000),
       ('B6', 13, N'Купе', 3000),
       ('A7', 13, N'Плацкарт', 1000),
       ('A8', 13, N'Купе', 1000),
       ('A9', 13, N'Плацкарт', 2000),
       ('A10', 13, N'Плацкарт', 4000),
       ('A1', 14, N'Купе', 2000),
       ('C2', 14, N'Купе', 2000),
       ('A3', 14, N'Купе', 2000),
       ('A4', 14, N'Плацкарт', 1000),
       ('C5', 14, N'Плацкарт', 4000),
       ('C6', 14, N'Плацкарт', 3000),
       ('C7', 14, N'Плацкарт', 3000),
       ('A8', 14, N'Плацкарт', 4000),
       ('A9', 14, N'Купе', 3000),
       ('B10', 14, N'Плацкарт', 3000),
       ('C1', 15, N'Плацкарт', 4000),
       ('B2', 15, N'Купе', 4000),
       ('C3', 15, N'Плацкарт', 3000),
       ('B4', 15, N'Плацкарт', 3000),
       ('C5', 15, N'Купе', 4000),
       ('A6', 15, N'Плацкарт', 2000),
       ('A7', 15, N'Купе', 4000),
       ('A8', 15, N'Плацкарт', 2000),
       ('A9', 15, N'Плацкарт', 1000),
       ('A10', 15, N'Купе', 3000),
       ('B1', 16, N'Плацкарт', 1000),
       ('A2', 16, N'Купе', 3000),
       ('A3', 16, N'Плацкарт', 2000),
       ('B4', 16, N'Плацкарт', 1000),
       ('C5', 16, N'Купе', 1000),
       ('C6', 16, N'Купе', 4000),
       ('A7', 16, N'Купе', 4000),
       ('B8', 16, N'Купе', 3000),
       ('A9', 16, N'Купе', 4000),
       ('B10', 16, N'Плацкарт', 3000),
       ('C1', 17, N'Плацкарт', 2000),
       ('A2', 17, N'Купе', 3000),
       ('C3', 17, N'Плацкарт', 4000),
       ('B4', 17, N'Плацкарт', 1000),
       ('B5', 17, N'Плацкарт', 4000),
       ('C6', 17, N'Плацкарт', 1000),
       ('C7', 17, N'Купе', 2000),
       ('C8', 17, N'Купе', 2000),
       ('C9', 17, N'Плацкарт', 1000),
       ('A10', 17, N'Купе', 3000),
       ('A1', 18, N'Плацкарт', 3000),
       ('C2', 18, N'Купе', 1000),
       ('A3', 18, N'Купе', 2000),
       ('C4', 18, N'Купе', 3000),
       ('C5', 18, N'Плацкарт', 4000),
       ('C6', 18, N'Плацкарт', 1000),
       ('C7', 18, N'Плацкарт', 2000),
       ('C8', 18, N'Купе', 2000),
       ('B9', 18, N'Плацкарт', 4000),
       ('B10', 18, N'Купе', 1000),
       ('A1', 19, N'Купе', 3000),
       ('A2', 19, N'Купе', 1000),
       ('A3', 19, N'Купе', 2000),
       ('C4', 19, N'Плацкарт', 3000),
       ('B5', 19, N'Купе', 1000),
       ('C6', 19, N'Плацкарт', 3000),
       ('C7', 19, N'Плацкарт', 2000),
       ('B8', 19, N'Купе', 3000),
       ('C9', 19, N'Купе', 2000),
       ('B10', 19, N'Купе', 4000),
       ('B1', 20, N'Плацкарт', 3000),
       ('A2', 20, N'Плацкарт', 3000),
       ('A3', 20, N'Купе', 1000),
       ('A4', 20, N'Плацкарт', 3000),
       ('A5', 20, N'Купе', 1000),
       ('B6', 20, N'Плацкарт', 3000),
       ('B7', 20, N'Купе', 2000),
       ('B8', 20, N'Купе', 3000),
       ('A9', 20, N'Плацкарт', 4000),
       ('C10', 20, N'Плацкарт', 2000),
       ('A1', 21, N'Плацкарт', 3000),
       ('A2', 21, N'Плацкарт', 3000),
       ('A3', 21, N'Плацкарт', 3000),
       ('A4', 21, N'Плацкарт', 2000),
       ('C5', 21, N'Купе', 4000),
       ('C6', 21, N'Плацкарт', 4000),
       ('C7', 21, N'Купе', 4000),
       ('A8', 21, N'Купе', 1000),
       ('C9', 21, N'Купе', 3000),
       ('A10', 21, N'Плацкарт', 4000),
       ('B1', 22, N'Купе', 3000),
       ('A2', 22, N'Купе', 2000),
       ('A3', 22, N'Купе', 4000),
       ('B4', 22, N'Купе', 3000),
       ('A5', 22, N'Купе', 3000),
       ('C6', 22, N'Плацкарт', 3000),
       ('A7', 22, N'Купе', 3000),
       ('B8', 22, N'Плацкарт', 2000),
       ('C9', 22, N'Купе', 1000),
       ('C10', 22, N'Плацкарт', 2000),
       ('B1', 23, N'Купе', 1000),
       ('A2', 23, N'Купе', 1000),
       ('A3', 23, N'Плацкарт', 4000),
       ('C4', 23, N'Купе', 1000),
       ('A5', 23, N'Купе', 3000),
       ('C6', 23, N'Плацкарт', 1000),
       ('A7', 23, N'Купе', 4000),
       ('A8', 23, N'Купе', 4000),
       ('B9', 23, N'Купе', 3000),
       ('C10', 23, N'Купе', 3000),
       ('B1', 24, N'Купе', 3000),
       ('A2', 24, N'Купе', 2000),
       ('B3', 24, N'Плацкарт', 3000),
       ('C4', 24, N'Купе', 2000),
       ('A5', 24, N'Плацкарт', 2000),
       ('C6', 24, N'Плацкарт', 4000),
       ('B7', 24, N'Плацкарт', 3000),
       ('C8', 24, N'Купе', 2000),
       ('C9', 24, N'Плацкарт', 4000),
       ('B10', 24, N'Плацкарт', 1000),
       ('C1', 25, N'Плацкарт', 4000),
       ('C2', 25, N'Купе', 3000),
       ('B3', 25, N'Купе', 3000),
       ('B4', 25, N'Купе', 1000),
       ('B5', 25, N'Купе', 4000),
       ('B6', 25, N'Плацкарт', 4000),
       ('A7', 25, N'Плацкарт', 1000),
       ('B8', 25, N'Купе', 2000),
       ('A9', 25, N'Плацкарт', 1000),
       ('A10', 25, N'Плацкарт', 2000);

INSERT INTO directions (name, start_station_id, end_station_id)
VALUES (N'Москва - Санкт-Питербург', 1, 6),
       (N'Воронеж - Калининград', 7, 12),
       (N'Самара - Нижний Новгород', 13, 18),
       (N'Саратов - Саров', 19, 24),
       (N'Анапа - Екатеринбург', 25, 30);

INSERT INTO stations (related_direction, name)
VALUES (1, N'Москва'),
       (1, N'Торжок'),
       (1, N'Бологое'),
       (1, N'Окуловка'),
       (1, N'Любань'),
       (1, N'Санкт-Питербург'),

       (2, N'Воронеж'),
       (2, N'Ступино'),
       (2, N'Домодедово'),
       (2, N'Кашира'),
       (2, N'Венёв'),
       (2, N'Калининград'),

       (3, N'Самара'),
       (3, N'Ядрин'),
       (3, N'Канаш'),
       (3, N'Ульяновск'),
       (3, N'Димитровград'),
       (3, N'Нижний Новгород'),

       (4, N'Саратов'),
       (4, N'Инсар'),
       (4, N'Ковылкино'),
       (4, N'Краснослободск'),
       (4, N'Пенза'),
       (4, N'Саров'),

       (5, N'Анапа'),
       (5, N'Колпино'),
       (5, N'Пушкин'),
       (5, N'Любань'),
       (5, N'Окуловка'),
       (5, N'Екатеринбург');

INSERT INTO active (station_id, start_date_time, main_direction_id, train_id, main_start_date_time)
VALUES (1, '01-01-2000 00:10:00', 1, 1, '01-01-2000 00:00:00'),
       (2, '02-01-2000 00:10:00', 1, 1, '01-01-2000 00:00:00'),
       (3, '03-01-2000 00:10:00', 1, 1, '01-01-2000 00:00:00'),
       (4, '04-01-2000 00:10:00', 1, 1, '01-01-2000 00:00:00'),
       (5, '05-01-2000 00:10:00', 1, 1, '01-01-2000 00:00:00'),
       (6, '06-01-2000 00:10:00', 1, 1, '01-01-2000 00:00:00'),

       (7, '01-01-2000 00:10:00', 2, 2, '01-01-2000 00:00:00'),
       (8, '02-01-2000 00:10:00', 2, 2, '01-01-2000 00:00:00'),
       (9, '03-01-2000 00:10:00', 2, 2, '01-01-2000 00:00:00'),
       (10, '04-01-2000 00:10:00', 2, 2, '01-01-2000 00:00:00'),
       (11, '05-01-2000 00:10:00', 2, 2, '01-01-2000 00:00:00'),
       (12, '06-01-2000 00:10:00', 2, 2, '01-01-2000 00:00:00'),

       (13, '01-01-2000 00:10:00', 3, 3, '01-01-2000 00:00:00'),
       (14, '02-01-2000 00:10:00', 3, 3, '01-01-2000 00:00:00'),
       (15, '03-01-2000 00:10:00', 3, 3, '01-01-2000 00:00:00'),
       (16, '04-01-2000 00:10:00', 3, 3, '01-01-2000 00:00:00'),
       (17, '05-01-2000 00:10:00', 3, 3, '01-01-2000 00:00:00'),
       (18, '06-01-2000 00:10:00', 3, 3, '01-01-2000 00:00:00'),

       (19, '01-01-2000 00:10:00', 4, 4, '01-01-2000 00:00:00'),
       (20, '02-01-2000 00:10:00', 4, 4, '01-01-2000 00:00:00'),
       (21, '03-01-2000 00:10:00', 4, 4, '01-01-2000 00:00:00'),
       (22, '04-01-2000 00:10:00', 4, 4, '01-01-2000 00:00:00'),
       (23, '05-01-2000 00:10:00', 4, 4, '01-01-2000 00:00:00'),
       (24, '06-01-2000 00:10:00', 4, 4, '01-01-2000 00:00:00'),

       (25, '01-01-2000 00:10:00', 5, 5, '01-01-2000 00:00:00'),
       (26, '02-01-2000 00:10:00', 5, 5, '01-01-2000 00:00:00'),
       (27, '03-01-2000 00:10:00', 5, 5, '01-01-2000 00:00:00'),
       (28, '04-01-2000 00:10:00', 5, 5, '01-01-2000 00:00:00'),
       (29, '05-01-2000 00:10:00', 5, 5, '01-01-2000 00:00:00'),
       (30, '06-01-2000 00:10:00', 5, 5, '01-01-2000 00:00:00')

-- default users
INSERT INTO [users]
VALUES (0, 'admin', 'admin', 'admin', 'Administrator', 'admin',
    'AQAAAAEAACcQAAAAEHXqg4aVPN8q9c4JLMGDhl2g09SVHUtWB/ZJXjAjml4tn559tlzQ3RSA+3g3JzC4FQ=='), -- admin - password = admin (hashed)
    (1, 'moderator', 'moderator', 'moderator', 'Moderator', 'moderator',
    'AQAAAAEAACcQAAAAEF3jw4jSnLt3n0Hu1VYYUCJsChDyNdX8+s/Nic7J+YmFnEcfHiK6+K/jbyrLW31l6w==') -- moderator - password = moderator (hashed)