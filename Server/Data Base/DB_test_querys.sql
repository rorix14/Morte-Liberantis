-- test querys for hybrid game save --------
INSERT INTO user (name,password)
SELECT * FROM (SELECT 'test' as name, 'test' as password) AS temp
WHERE NOT EXISTS (
	SELECT name from user WHERE name = 'test'  AND password = "test"
) LIMIT 1;

INSERT INTO component_psar (tbl_id, row_nr, col_1, col_2, col_3, col_4, col_5, col_6, unit, add_info, fsar_lock)
VALUES('2', '1', '1', '1', '1', '1', '1', '1', '1', '1', 'N')
ON DUPLICATE KEY UPDATE col_1 = VALUES(col_1), col_2 = VALUES(col_2), col_3 = VALUES(col_3), col_4 = VALUES(col_4), col_5 = VALUES(col_5), col_6 = VALUES(col_6), unit = VALUES(unit), add_info = VALUES(add_info), fsar_lock = VALUES(fsar_lock);

select * from user_ammo;

INSERT INTO user_ammo (user_id, ammo_id, amount)
VALUES (1, 1, 57)
ON DUPLICATE KEY UPDATE amount = VALUES(amount);

INSERT IGNORE INTO user_characters (user_id, characters_id)
VALUES (1, 2);

select * from user_characters where user_id = 1;

INSERT IGNORE INTO user_characters (user_id, characters_id)
VALUES (1, 1);


select * from user_upgrades;

INSERT INTO user_savepoint (user_id, savepoint_id)
VALUES (1, (select id from savepoint where identifier = 1 and scene = 1))
ON DUPLICATE KEY UPDATE savepoint_id = VALUES(savepoint_id);

select * from savepoint;
select * from user_savepoint;
select * from user_ammo;

INSERT INTO user_ammo (user_id, ammo_id, amount)
VALUES (1, 1, 575)
ON DUPLICATE KEY UPDATE amount = VALUES(amount);

select * from user where name = 'testx';
DELETE FROM user where id = 25; 
INSERT INTO user_upgrades (user_id, upgrade_id, active)
VALUES (1, (select id from upgrade where name = "double jump"), 1)
ON DUPLICATE KEY UPDATE active = true;

update user_upgrades set active = false where id = 1;

select * from user where id = 30;
insert into user(id ,name, password) values
("15","testing", "123456789");
select * from user;
select * from characters;
select * from upgrade;
select * from user_upgrades;
select * from user_savepoint;
select * from savepoint;
select * from upgrade;
DELETE FROM savepoint where id = 2; 

-- get all the text boxes related to that characters
select text_adventure_message.id as text_id
from characters, text_adventure_message, character_text_adventure_message where
characters.image = "eca_de_queiros" and
characters.id = character_text_adventure_message.character_id and
text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id ;

-- get player main game info
select upgrade.name, user_ammo.amount, savepoint.identifier, savepoint.scene from user, upgrade, savepoint, user_ammo, user_upgrades, user_savepoint where
user.id = 29 and user.id = user_ammo.user_id and user.id = user_upgrades.user_id and user.id = user_savepoint.user_id;

select user_ammo.amount, upgrade.name, savepoint.identifier, savepoint.scene 
    from user, user_ammo, savepoint, user_savepoint, upgrade, user_upgrades where
    user.id = ? and user_ammo.user_id = user.id
    and user_savepoint.user_id = user.id and user_savepoint.savepoint_id = savepoint.id and
    user_upgrades.user_id = user.id and user_upgrades.upgrade_id = upgrade.id;

-- final two querys to get player stats
select user_ammo.amount, savepoint.identifier, savepoint.scene 
from user, user_ammo, savepoint, user_savepoint, upgrade, user_upgrades where
user.id = 29 and user_ammo.user_id = user.id
and user_savepoint.user_id = user.id and user_savepoint.savepoint_id = savepoint.id;

select upgrade.name from upgrade, user, user_upgrades where
user.id = 40 and
user_upgrades.user_id = user.id and user_upgrades.upgrade_id = upgrade.id;

select savepoint.identifier, savepoint.scene from user, savepoint, user_savepoint where user.id = 29 and user_savepoint.user_id = user.id and user_savepoint.savepoint_id = savepoint.id;
select upgrade.name from user, upgrade, user_upgrades where user.id = 29 and user_upgrades.user_id = user.id and user_upgrades.upgrade_id = upgrade.id;

-- ------------------------------ Get first text and responses from character --------------------------------
-- first option 
select text_adventure_message.identifier as text, text_adventure_response.identifier as responses 
from text_adventure_message, text_adventure_response, text_adventure_message_response where
text_adventure_message.id = text_adventure_message_response.message_id and 
text_adventure_response.id = text_adventure_message_response.response_id and
text_adventure_message.id in
(select min(text_adventure_message.id) as text_id
from characters, text_adventure_message, character_text_adventure_message where
characters.image = "eca_de_queiros" and
characters.id = character_text_adventure_message.character_id and
text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id);

-- second option
select text_adventure_message.identifier as text, text_adventure_response.identifier as responses 
from text_adventure_message, text_adventure_response, text_adventure_message_response, (select text_adventure_message.id as text_id
from characters, text_adventure_message, character_text_adventure_message where
characters.image = "eca_de_queiros" and
characters.id = character_text_adventure_message.character_id and
text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id asc LIMIT 1) as test where
text_adventure_message.id = text_adventure_message_response.message_id and 
text_adventure_response.id = text_adventure_message_response.response_id and
text_adventure_message.id = test.text_id;
-- -------------------------------------------------------- End ---------------------------------------------------------------------

-- get reward for message
select text_adventure_reward.reward, text_adventure_reward.ammo_id as ammoId 
from text_adventure_reward, ammo, text_adventure_message where
text_adventure_message.identifier = "S1" and
text_adventure_message.id = text_adventure_reward.message_id and
ammo.id = text_adventure_reward.ammo_id;

select * from user_ammo;

update user_ammo
set user_ammo.amount =  user_ammo.amount + (select text_adventure_reward.reward from text_adventure_reward, ammo, text_adventure_message where
text_adventure_message.identifier = "S2" and
text_adventure_message.id = text_adventure_reward.message_id and
ammo.id = text_adventure_reward.ammo_id)
where user_ammo.user_id = 1
and user_ammo.ammo_id = (select text_adventure_reward.ammo_id as ammoId from text_adventure_reward, ammo, text_adventure_message where
text_adventure_message.identifier = "S2" and
text_adventure_message.id = text_adventure_reward.message_id and
ammo.id = text_adventure_reward.ammo_id);


-- get possible answers to the text 
select text_adventure_message.identifier as text, text_adventure_response.identifier as responses 
from text_adventure_message, text_adventure_response, text_adventure_message_response where
text_adventure_message.identifier = "A9" and
text_adventure_message.id = text_adventure_message_response.message_id and 
text_adventure_response.id = text_adventure_message_response.response_id;

-- get all the characters a user has met
select characters.name 
from user_characters, user, characters where 
user.id = 40 and user_characters.text_adventure_done = false 
and user.id = user_characters.user_id and characters.id = user_characters.characters_id;

-- -------------------------------------- update text adventure to done ---------------------------------------------------
update user_characters 
set user_characters.text_adventure_done = false
where user_characters.user_id =1
and user_characters.characters_id = 1;
select user_characters.text_adventure_done from user_characters where
 user_characters.user_id =1
and user_characters.characters_id = 1;

select * from user_characters; 

-- character name if text adventure has compleated
select characters.name as characters
from user, characters, user_characters where
user.id = 1 and
user_characters.text_adventure_done = true and
user.id = user_characters.user_id and
characters.id = user_characters.characters_id;

-- -------------------------------------get characters rewards -------------------------------------------
select text_adventure_reward.reward as reward, text_adventure_reward.id as ammoId 
from text_adventure_reward, text_adventure_message, characters, character_text_adventure_message where
characters.name = "Eça de Queirós" and
characters.id = character_text_adventure_message.character_id and
text_adventure_message.id = 
text_adventure_message.id = text_adventure_reward.message_id;

select text_adventure_reward.reward as reward, text_adventure_reward.ammo_id as ammoId 
from text_adventure_reward, text_adventure_message where
text_adventure_message.id = text_adventure_reward.message_id and
text_adventure_message.id in(
select text_adventure_message.id as text_id
from characters, text_adventure_message, character_text_adventure_message where
characters.image = "eca_de_queiros" and
characters.id = character_text_adventure_message.character_id and
text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id);

select * from text_adventure_reward;
select * from text_adventure_reward;

-- ---------------meet characters in the game------------------------
select user_characters.characters_id from user_characters where user_characters.user_id = 1;

select count(*) as result from user_characters where user_characters.user_id = 1 and  user_characters.characters_id = 2;

insert into user_characters(user_id,characters_id) values
(1,2);

select * from user_characters;
select * from user;


-- ---- daily quests related querys --------------------------------
select daily_quest.description, active_daily_quest.app_compleated from daily_quest, active_daily_quest, user where 
user.id = 1 and
active_daily_quest.quest_launch_date = "23_11__2019" and
active_daily_quest.quest_id = daily_quest.id;

-- if there is no quest get one and insert it into DB
select daily_quest.id from daily_quest, characters, characters_daily_quest where 
characters.id = characters_daily_quest.characters_id and 
daily_quest.id = characters_daily_quest.daily_quest_id and 
characters.id in (select characters.id from user_characters, user, characters where 
user.id = 1 and
user.id = user_characters.user_id and 
characters.id = user_characters.characters_id);

insert into active_daily_quest (user_id,quest_id,quest_launch_date) values
(1,6,"16/11/2019");

select characters.image, daily_quest.description from characters, daily_quest, characters_daily_quest where 
daily_quest.id = 6 and
daily_quest.id = characters_daily_quest.daily_quest_id and 
characters.id = characters_daily_quest.characters_id;

-- if there is a current daily quest 
select characters.image, daily_quest.description from characters, daily_quest, characters_daily_quest where 
daily_quest.description = "Ramalho quest 2" and
daily_quest.id = characters_daily_quest.daily_quest_id and 
characters.id = characters_daily_quest.characters_id;

-- final
select characters.image, daily_quest.description, id_and_compleated.compleated
from characters, daily_quest, characters_daily_quest, (select daily_quest.id as questId, active_daily_quest.app_compleated as compleated
from daily_quest, active_daily_quest, user where 
user.id = 1 and
user.id = active_daily_quest.user_id and
active_daily_quest.quest_launch_date = "17_11_2019" and
active_daily_quest.quest_id = daily_quest.id) as id_and_compleated where 
daily_quest.id = id_and_compleated.questId and
daily_quest.id = characters_daily_quest.daily_quest_id and 
characters.id = characters_daily_quest.characters_id;

-- on app quest completed 
update active_daily_quest
set active_daily_quest.app_compleated = false
where active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "03_12_2019";

select * from active_daily_quest;
select * from active_daily_quest where active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "17_11_2019";

-- main game daily quest related querys
select active_daily_quest.app_compleated, active_daily_quest.ingame_delivered, active_daily_quest.quest_id
from active_daily_quest, user where 
active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "17_11_2019"and
user.id = active_daily_quest.user_id;

select characters_daily_quest.characters_id, current_quest.app_compleated, current_quest.ingame_delivered from characters_daily_quest, daily_quest, 
(select active_daily_quest.app_compleated as app_compleated, active_daily_quest.ingame_delivered as ingame_delivered, active_daily_quest.quest_id as quest_id
from active_daily_quest, user where 
active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "17_11_2019"and
user.id = active_daily_quest.user_id) as current_quest where
daily_quest.id = current_quest.quest_id and
daily_quest.id = characters_daily_quest.daily_quest_id;
-- final
select current_quest.app_compleated, current_quest.ingame_delivered from characters_daily_quest, daily_quest, 
(select active_daily_quest.app_compleated as app_compleated, active_daily_quest.ingame_delivered as ingame_delivered, active_daily_quest.quest_id as quest_id
from active_daily_quest, user where 
active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "17_11_2019"and
user.id = active_daily_quest.user_id) as current_quest where
daily_quest.id = current_quest.quest_id and
characters_daily_quest.characters_id = 1 and
daily_quest.id = characters_daily_quest.daily_quest_id;

-- update daily quest in game
update active_daily_quest
set active_daily_quest.ingame_delivered = false
where active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "03_12_2019"; 

-- use to delete any mistake 
DELETE FROM user_characters where id = 56;
select * from user_characters where user_characters.user_id = 1;
-- test daily quest in main game
select * from active_daily_quest;

update active_daily_quest
set active_daily_quest.ingame_delivered = false
where active_daily_quest.user_id = 1 and active_daily_quest.quest_launch_date = "19_11_2019"; 

insert into active_daily_quest (user_id, quest_id, app_compleated,quest_launch_date) values
(1, 4,true,"19_11_2019");
-- CAUTION !!!
DELETE FROM active_daily_quest where id = 21;

select * from characters_daily_quest;
select * from user;

-- user playing status for invasion purposses
select * from user where user.currently_playing = false;
update user
set user.currently_playing = true
where user.id = 7;
select * from user where user.id = 7;
select * from user where user.id = 79;

select * from user where name = "test";
update user_characters
set user_characters.text_adventure_done = false
where
user_characters.user_id = 1 and user_characters.characters_id = 1;

update user 
set user.invaderId = null
where user.id = 79;

-- experiments
select text_adventure_message.identifier as text_identifier, text_adventure_response.identifier as response_identifier
from characters, text_adventure_message, character_text_adventure_message, text_adventure_message_response, text_adventure_response where
characters.image = "eca_de_queiros" and text_adventure_message.id = 4 and
characters.id = character_text_adventure_message.character_id and
text_adventure_message.id = character_text_adventure_message.message_id and
text_adventure_message.id = text_adventure_message_response.message_id and 
text_adventure_response.id = text_adventure_message_response.response_id;

select text_adventure_message.identifier as text_identifier, text_adventure_message.text,
 text_adventure_response.identifier as response_identifier, text_adventure_response.text
    from text_adventure_message, text_adventure_response, text_adventure_message_response, (select text_adventure_message.id as text_id
    from characters, text_adventure_message, character_text_adventure_message where
    characters.image = "eca_de_queiros" and
    characters.id = character_text_adventure_message.character_id and
    text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id asc LIMIT 1) as test where
    text_adventure_message.id = text_adventure_message_response.message_id and 
    text_adventure_response.id = text_adventure_message_response.response_id and
    text_adventure_message.id = test.text_id;
    
    select text_adventure_message.identifier as text_identifier, text_adventure_message.text,
    text_adventure_response.identifier as response_identifier, text_adventure_response.text
    from text_adventure_message, text_adventure_response, text_adventure_message_response where
    text_adventure_message.identifier = "S1" and
    text_adventure_message.id = text_adventure_message_response.message_id and 
    text_adventure_response.id = text_adventure_message_response.response_id;
    
    select text_adventure_message.text from text_adventure_message where text_adventure_message.identifier = "X1";
    
create table user_credentials(ID INTEGER PRIMARY KEY auto_increment,PASSWORD TEXT);
 create table user_passwords(ID INTEGER PRIMARY KEY auto_increment,DESCRIPTION TEXT, PASSWORD TEXT ); 
 create table user_articles(ID INTEGER PRIMARY KEY auto_increment,URL TEXT ) ;
DROP TABLE IF EXISTS user_credentials, user_passwords, user_articles;
 delete from user_articles where (URL = ? and Title = ?) limit 1;
 
 create table user_passwords(
 ID INTEGER PRIMARY KEY auto_increment,
 DESCRIPTION Text ,
 PASSWORD text
 );
 
 insert into user_passwords(DESCRIPTION, PASSWORD) value
 ("oldDES", "oldPAs");
 
  update user_passwords set user_passwords.DESCRIPTION = "new des", PASSWORD = "new pas" where
  ID = (Select ID where DESCRIPTION = "newDES" and PASSWORD = "oldPAs" limit 1);
  select * from user_passwords;
  Select ID from user_passwords  where DESCRIPTION = "newDES" and PASSWORD = "oldPAs";