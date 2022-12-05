CREATE OR REPLACE PROCEDURE register_client(
	username varchar(50),
	email varchar(50),
	password varchar(50)
)
LANGUAGE PLPGSQL
AS $$

BEGIN
	INSERT INTO Users VALUES (gen_random_uuid(),
							  username,
							  email,
							  password,
						      (SELECT id FROM Roles WHERE role_name = 'client'));
END;$$;


CREATE OR REPLACE PROCEDURE add_review(
	rating int,
	review varchar(1000),
	announcement_id uuid,
	user_id uuid
)
LANGUAGE PLPGSQL
AS $$

BEGIN
	INSERT INTO Reviews VALUES (gen_random_uuid(),
							  rating,
								review,
								NOW(),
								announcement_id,
								user_id);
END;$$;