DROP TABLE IF EXISTS Roles, Users, Facilities, Placement_types, Announcements, Announcement_has_Facility, Reservations, Reviews, Review_log, Announcement_log;

CREATE TABLE Roles (
	id uuid PRIMARY KEY,
	role_name varchar(50) NOT NULL
);

CREATE TABLE Users (
	id uuid PRIMARY KEY,
	username varchar(50) NOT NULL,
	email varchar(50) NOT NULL,
	password varchar(50) NOT NULL,
	role_id uuid REFERENCES Roles(id) NOT NULL
);

CREATE INDEX user_id_index ON Users(id);

CREATE TABLE Facilities (
	id uuid PRIMARY KEY,
	facility_name varchar(50) NOT NULL
);

CREATE TABLE Placement_types (
	id uuid PRIMARY KEY,
	placement_type varchar(50) NOT NULL
);

CREATE TABLE Announcements (
	id uuid PRIMARY KEY,
	user_id uuid REFERENCES Users(id) NOT NULL,
	title varchar(100) NOT NULL,
	description varchar(1000) NOT NULL,
	address varchar(100) NOT NULL,
	room_count int NOT NULL,
	post_date date NOT NULL,
	placement_type_id uuid REFERENCES Placement_types(id) NOT NULL,
	price_per_day int NOT NULL
);

CREATE INDEX announcement_id ON Announcements(id);

CREATE TABLE Announcement_has_Facility (
	announcement_id uuid REFERENCES Announcements(id) NOT NULL,
	facility_id uuid REFERENCES Facilities(id) NOT NULL
);

CREATE TABLE Reservations (
	id uuid PRIMARY KEY,
	from_date date NOT NULL,
	till_date date NOT NULL,
	user_id uuid REFERENCES Users(id) NOT NULL,
	announcement_id uuid REFERENCES Announcements(id) NOT NULL
);

CREATE TABLE Reviews (
	id uuid PRIMARY KEY,
	rating int NOT NULL,
	review varchar(1000) NOT NULL,
	post_date date NOT NULL,
	announcement_id uuid REFERENCES Announcements(id) NOT NULL,
	user_id uuid REFERENCES Users(id) NOT NULL
);

CREATE TABLE Review_log (
	id uuid PRIMARY KEY,
	event varchar(100) NOT NULL,
	user_id uuid REFERENCES Users(id),
	review_id uuid REFERENCES Reviews(id)
);

CREATE TABLE Announcement_log (
	id uuid PRIMARY KEY,
	event varchar(100) NOT NULL,
	user_id uuid REFERENCES Users(id),
	announcement_id uuid REFERENCES Announcements(id)
);