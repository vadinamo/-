-- ANNOUNCEMENT CREATE
CREATE OR REPLACE FUNCTION log_announcement_created()
	RETURNS TRIGGER 
	AS $$
BEGIN
	INSERT INTO Announcement_log
	VALUES(gen_random_uuid(), 'CREATED', NEW.user_id, NEW.id);
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER announcement_created
AFTER INSERT ON Announcements
FOR EACH ROW 
EXECUTE PROCEDURE log_announcement_created();

-- ANNOUNCEMENT UPDATE
CREATE OR REPLACE FUNCTION log_announcement_updated()
	RETURNS TRIGGER 
	AS $$
BEGIN
	INSERT INTO Announcement_log
	VALUES(gen_random_uuid(), 'UPDATED', NEW.user_id, NEW.id);
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER announcement_updated
AFTER UPDATE ON Announcements
FOR EACH ROW 
EXECUTE PROCEDURE log_announcement_updated();


-- REVIEW CREATED
CREATE OR REPLACE FUNCTION log_review_created()
	RETURNS TRIGGER
	AS $$
BEGIN
	INSERT INTO Review_log
	VALUES(gen_random_uuid(), 'CREATED', NEW.user_id, NEW.id);
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER review_created
AFTER INSERT ON Reviews
FOR EACH ROW
EXECUTE PROCEDURE log_review_created();

-- REVIEW UPDATE
CREATE OR REPLACE FUNCTION log_review_updated()
	RETURNS TRIGGER
	AS $$
BEGIN
	INSERT INTO Review_log
	VALUES(gen_random_uuid(), 'UPDATED', NEW.user_id, NEW.id);
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER review_updated
AFTER UPDATE ON Reviews
FOR EACH ROW
EXECUTE PROCEDURE log_review_updated();