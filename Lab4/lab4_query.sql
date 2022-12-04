-- 1.Составлен пул SQL запросов для сложной выборки из БД:
--		Запросы с несколькими условиями.
-- 		Запросы с вложенными конструкциями
-- 		Прочие сложные выборки, необходимые в вашем проекте.

-- Запросы с несколькими условиями.
SELECT * FROM Announcement_log 
	WHERE user_id = (SELECT id FROM Users WHERE username = 'client1')
	AND event LIKE '%1';

-- Запросы с вложенными конструкциями
SELECT * FROM Review_log WHERE review_id in (
	SELECT id FROM Reviews WHERE announcement_id IN (
		SELECT id FROM Announcements WHERE user_id IN (
			SELECT id FROM Users WHERE username = 'client1')));
			
			
-- 2.Составлен пул SQL запросов для получения представлений в БД:
-- 		JOIN-запросы различных видов (
-- 			INNER, 
-- 			OUTER, 
-- 			FULL, 
-- 			CROSS, 
-- 			SELF)

-- INNER
SELECT Announcement_log.event, Users.username, Announcements.title FROM Announcement_log 
	INNER JOIN Announcements ON Announcement_log.announcement_id = Announcements.id
	INNER JOIN Users ON Announcement_log.user_id = Users.id;

-- SELF
SELECT A.event FROM Announcement_log A, Announcement_log B
	WHERE A.user_id = B.user_id
	AND A.event <> B.event
	ORDER BY A.event;

-- LEFT
SELECT Users.username, Announcements.title FROM Announcements 
	LEFT JOIN Users ON Users.id = Announcements.user_id;

-- RIGHT
SELECT Users.username, Announcements.title FROM Users 
	RIGHT JOIN Announcements ON Users.id = Announcements.user_id;

-- FULL
SELECT Announcements.title, Users.username FROM Announcements
	FULL JOIN Users ON Users.id = Announcements.user_id;

-- CROSS
SELECT Announcements.title, Reviews.review, Reviews.rating FROM Announcements
	CROSS JOIN Reviews;
	

-- 3.Составлен пул SQL запросов для получения сгруппированных данных:
-- 		GROUP BY + агрегирующие функции
-- 		HAVING
-- 		UNION

-- GROUP BY + агрегирующие функции
SELECT COUNT(Announcement_log.id), Users.username FROM Announcement_log
	JOIN Users ON Announcement_log.user_id = Users.id
	GROUP BY Users.username;

-- HAVING
SELECT COUNT(Announcement_log.id), Users.username FROM Announcement_log
	JOIN Users ON Announcement_log.user_id = Users.id
	GROUP BY Users.username
	HAVING COUNT(Announcement_log.id) > 1;

-- UNION
SELECT user_id FROM Announcement_log 
UNION
SELECT user_id FROM Review_log;


-- 4.Составлен пул SQL запросов для сложных операций с данными:
-- 		EXISTS
-- 		INSERT INTO SELECT
-- 		CASE
-- 		EXPLAIN 

-- EXISTS
SELECT * FROM Users 
WHERE EXISTS (SELECT user_id FROM Announcements WHERE Announcements.user_id = Users.id);

-- INSERT INTO SELECT
-- INSERT INTO Announcement_log(id, event, user_id, announcement_id)
-- SELECT id, title, user_id, id FROM Announcements

-- CASE
SELECT title, price_per_day,
CASE
	WHEN price_per_day > 3 THEN 'mnogo'
	WHEN price_per_day = 3 THEN 'norm'
	ELSE 'malo'
END AS text
FROM Announcements;

-- EXPLAIN
EXPLAIN SELECT * FROM Users