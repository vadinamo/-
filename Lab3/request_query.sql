-- LIKE - С УЧЕТОМ РЕГИСТРА
-- ILIKE - БЕЗ УЧЕТА РЕГИСТРА
-- 

SELECT * FROM Users WHERE username ILIKE 'CLIENT_';

SELECT * FROM Announcements 
WHERE user_id = (SELECT id FROM Users WHERE username = 'client4')
ORDER BY price_per_day DESC;

DELETE FROM Announcement_has_facility
WHERE announcement_id = (SELECT id FROM Announcements WHERE title = 'title1')
AND facility_id = (SELECT id FROM Facilities WHERE facility_name = 'wi-fi');

UPDATE Reviews SET review = 'asd' WHERE user_id = (SELECT id FROM Users WHERE username = 'client4');
