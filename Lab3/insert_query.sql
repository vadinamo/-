INSERT INTO Roles VALUES (gen_random_uuid(), 'admin');
INSERT INTO Roles VALUES (gen_random_uuid(), 'client');

INSERT INTO Users VALUES (
	gen_random_uuid(),
	'admin',
	'admin@gmail.com',
	'admin@gmail.com',
	(SELECT id FROM Roles WHERE role_name = 'admin')
);
INSERT INTO Users VALUES (
	gen_random_uuid(),
	'client1',
	'client1@gmail.com',
	'client1@gmail.com',
	(SELECT id FROM Roles WHERE role_name = 'client')
);
INSERT INTO Users VALUES (
	gen_random_uuid(),
	'client2',
	'client2@gmail.com',
	'client2@gmail.com',
	(SELECT id FROM Roles WHERE role_name = 'client')
);
INSERT INTO Users VALUES (
	gen_random_uuid(),
	'client3',
	'client3@gmail.com',
	'client3@gmail.com',
	(SELECT id FROM Roles WHERE role_name = 'client')
);
INSERT INTO Users VALUES (
	gen_random_uuid(),
	'client4',
	'client4@gmail.com',
	'client4@gmail.com',
	(SELECT id FROM Roles WHERE role_name = 'client')
);

INSERT INTO Facilities VALUES (gen_random_uuid(), 'wi-fi');
INSERT INTO Facilities VALUES (gen_random_uuid(), 'parking');
INSERT INTO Facilities VALUES (gen_random_uuid(), 'kettle');
INSERT INTO Facilities VALUES (gen_random_uuid(), 'balcony');
INSERT INTO Facilities VALUES (gen_random_uuid(), 'washing machine');
INSERT INTO Facilities VALUES (gen_random_uuid(), 'breakfast');
INSERT INTO Facilities VALUES (gen_random_uuid(), 'airport transfer');

INSERT INTO Placement_types VALUES (gen_random_uuid(), 'appartments');
INSERT INTO Placement_types VALUES (gen_random_uuid(), 'hotel');
INSERT INTO Placement_types VALUES (gen_random_uuid(), 'motel');
INSERT INTO Placement_types VALUES (gen_random_uuid(), 'recreation complexes');
INSERT INTO Placement_types VALUES (gen_random_uuid(), 'homestay');

INSERT INTO Announcements VALUES (
	gen_random_uuid(),
	(SELECT id FROM Users WHERE username = 'client1'),
	'title1',
	'description1',
	'address1',
	1,
	'2001-01-01',
	(SELECT id FROM Placement_types WHERE placement_type = 'appartments'),
	1
);
INSERT INTO Announcements VALUES (
	gen_random_uuid(),
	(SELECT id FROM Users WHERE username = 'client2'),
	'title2',
	'description2',
	'address2',
	2,
	'2002-02-02',
	(SELECT id FROM Placement_types WHERE placement_type = 'hotel'),
	2
);
INSERT INTO Announcements VALUES (
	gen_random_uuid(),
	(SELECT id FROM Users WHERE username = 'client3'),
	'title3',
	'description3',
	'address3',
	3,
	'2003-03-03',
	(SELECT id FROM Placement_types WHERE placement_type = 'motel'),
	3
);
INSERT INTO Announcements VALUES (
	gen_random_uuid(),
	(SELECT id FROM Users WHERE username = 'client4'),
	'title4',
	'description4',
	'address4',
	4,
	'2004-04-04',
	(SELECT id FROM Placement_types WHERE placement_type = 'recreation complexes'),
	4
);
INSERT INTO Announcements VALUES (
	gen_random_uuid(),
	(SELECT id FROM Users WHERE username = 'client4'),
	'title5',
	'description5',
	'address5',
	5,
	'2005-05-05',
	(SELECT id FROM Placement_types WHERE placement_type = 'homestay'),
	5
);
INSERT INTO Announcements VALUES (
	gen_random_uuid(),
	NULL,
	'title6',
	'description6',
	'address6',
	6,
	'2006-06-06',
	(SELECT id FROM Placement_types WHERE placement_type = 'motel'),
	6
);

INSERT INTO Announcement_has_Facility VALUES (
	(SELECT id FROM Announcements WHERE title = 'title1'),
	(SELECT id FROM Facilities WHERE facility_name = 'wi-fi')
);
INSERT INTO Announcement_has_Facility VALUES (
	(SELECT id FROM Announcements WHERE title = 'title2'),
	(SELECT id FROM Facilities WHERE facility_name = 'parking')
);
INSERT INTO Announcement_has_Facility VALUES (
	(SELECT id FROM Announcements WHERE title = 'title3'),
	(SELECT id FROM Facilities WHERE facility_name = 'kettle')
);
INSERT INTO Announcement_has_Facility VALUES (
	(SELECT id FROM Announcements WHERE title = 'title4'),
	(SELECT id FROM Facilities WHERE facility_name = 'balcony')
);
INSERT INTO Announcement_has_Facility VALUES (
	(SELECT id FROM Announcements WHERE title = 'title5'),
	(SELECT id FROM Facilities WHERE facility_name = 'washing machine')
);

INSERT INTO Reservations VALUES (
	gen_random_uuid(),
	'2001-01-01',
	'2002-02-02',
	(SELECT id FROM Users WHERE username = 'client1'),
	(SELECT id FROM Announcements WHERE title = 'title1')
);
INSERT INTO Reservations VALUES (
	gen_random_uuid(),
	'2002-02-02',
	'2003-03-03',
	(SELECT id FROM Users WHERE username = 'client2'),
	(SELECT id FROM Announcements WHERE title = 'title2')
);
INSERT INTO Reservations VALUES (
	gen_random_uuid(),
	'2003-03-03',
	'2004-04-04',
	(SELECT id FROM Users WHERE username = 'client3'),
	(SELECT id FROM Announcements WHERE title = 'title3')
);
INSERT INTO Reservations VALUES (
	gen_random_uuid(),
	'2004-04-04',
	'2005-05-05',
	(SELECT id FROM Users WHERE username = 'client4'),
	(SELECT id FROM Announcements WHERE title = 'title4')
);

INSERT INTO Reviews VALUES (
	gen_random_uuid(),
	1,
	'review1',
	'2001-01-01',
	(SELECT id FROM Announcements WHERE title = 'title1'),
	(SELECT id FROM Users WHERE username = 'client1')
);
INSERT INTO Reviews VALUES (
	gen_random_uuid(),
	2,
	'review2',
	'2002-02-02',
	(SELECT id FROM Announcements WHERE title = 'title2'),
	(SELECT id FROM Users WHERE username = 'client2')
);
INSERT INTO Reviews VALUES (
	gen_random_uuid(),
	3,
	'review3',
	'2003-03-03',
	(SELECT id FROM Announcements WHERE title = 'title3'),
	(SELECT id FROM Users WHERE username = 'client3')
);
INSERT INTO Reviews VALUES (
	gen_random_uuid(),
	4,
	'review4',
	'2004-04-04',
	(SELECT id FROM Announcements WHERE title = 'title4'),
	(SELECT id FROM Users WHERE username = 'client4')
);

INSERT INTO Review_log VALUES (
	gen_random_uuid(),
	'event1',
	(SELECT id FROM Users WHERE username = 'client1'),
	(SELECT id FROM Reviews WHERE review = 'review1')
);
INSERT INTO Review_log VALUES (
	gen_random_uuid(),
	'event2',
	(SELECT id FROM Users WHERE username = 'client2'),
	(SELECT id FROM Reviews WHERE review = 'review2')
);
INSERT INTO Review_log VALUES (
	gen_random_uuid(),
	'event3',
	(SELECT id FROM Users WHERE username = 'client3'),
	(SELECT id FROM Reviews WHERE review = 'review3')
);
INSERT INTO Review_log VALUES (
	gen_random_uuid(),
	'event4',
	(SELECT id FROM Users WHERE username = 'client4'),
	(SELECT id FROM Reviews WHERE review = 'review4')
);

INSERT INTO Announcement_log VALUES (
	gen_random_uuid(),
	'event1',
	(SELECT id FROM Users WHERE username = 'client1'),
	(SELECT id FROM Announcements WHERE title = 'title1')
);
INSERT INTO Announcement_log VALUES (
	gen_random_uuid(),
	'event0',
	(SELECT id FROM Users WHERE username = 'client1'),
	(SELECT id FROM Announcements WHERE title = 'title5')
);
INSERT INTO Announcement_log VALUES (
	gen_random_uuid(),
	'event2',
	(SELECT id FROM Users WHERE username = 'client2'),
	(SELECT id FROM Announcements WHERE title = 'title2')
);
INSERT INTO Announcement_log VALUES (
	gen_random_uuid(),
	'event3',
	(SELECT id FROM Users WHERE username = 'client3'),
	(SELECT id FROM Announcements WHERE title = 'title3')
);
INSERT INTO Announcement_log VALUES (
	gen_random_uuid(),
	'event4',
	(SELECT id FROM Users WHERE username = 'client4'),
	(SELECT id FROM Announcements WHERE title = 'title4')
);
