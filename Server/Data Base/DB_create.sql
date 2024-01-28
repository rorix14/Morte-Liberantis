create database metroidvania;
use metroidvania;

create table user (
id int not null auto_increment,
name varchar(255) not null,
password varchar(150),
currently_playing BOOLEAN NOT NULL DEFAULT FALSE,
invaderId int default null,
CONSTRAINT playerID PRIMARY KEY (id)
);

create table upgrade (
id int not null auto_increment,
name varchar(32) not null,
CONSTRAINT upgradeID PRIMARY KEY (id)
);

create table ammo (
id int not null auto_increment,
name varchar(32) not null,
CONSTRAINT ammoID PRIMARY KEY (id)
);

create table savepoint(
id int not null auto_increment,
identifier varchar(16) not null,
scene varchar(16) not null,
CONSTRAINT UNIQUE KEY identifier_scene_unique (identifier,scene),
CONSTRAINT savepointID PRIMARY KEY (id)
);

 create table characters (
 id int not null auto_increment,
 name varchar(255) not null,
 image varchar(255) not null,
 CONSTRAINT charactersID PRIMARY KEY (id)
 );

create table text_adventure_message (
 id int not null auto_increment,
 identifier varchar(16) not null,
 text varchar(4096) not null,
 CONSTRAINT charactersID PRIMARY KEY (id)
 );

create table text_adventure_response (
 id int not null auto_increment,
 identifier varchar(16) not null,
 text varchar(4096) not null,
 CONSTRAINT responseID PRIMARY KEY (id)
 );
 
 create table daily_quest(
 id int not null auto_increment,
 description varchar(255),
  CONSTRAINT daily_questID PRIMARY KEY (id)
 );

create table user_upgrades(
id int not null auto_increment,
user_id int not null,
upgrade_id int not null,
active BOOLEAN NOT NULL DEFAULT FALSE,
CONSTRAINT user_upgradesID PRIMARY KEY (id),
CONSTRAINT UNIQUE KEY user_upgrade_unique (user_id,upgrade_id),
FOREIGN KEY (user_id)
	REFERENCES user (id),
FOREIGN KEY (upgrade_id)
	REFERENCES upgrade (id)
);

create table user_ammo(
id  int not null auto_increment,
user_id int not null unique,
ammo_id int not null,
amount int not null default 0,
CONSTRAINT user_ammoID PRIMARY KEY (id),
FOREIGN KEY (user_id)
	REFERENCES user (id),
FOREIGN KEY (ammo_id)
	REFERENCES ammo (id)
);

create table user_savepoint(
id int not null auto_increment,
user_id int not null unique,
savepoint_id int not null,
CONSTRAINT last_saved_pointID PRIMARY KEY (id),
FOREIGN KEY (user_id)
	REFERENCES user (id),
FOREIGN KEY (savepoint_id)
	REFERENCES savepoint (id)
);

create table user_characters (
id int not null auto_increment,
user_id int not null,
characters_id int not null,
text_adventure_done BOOLEAN NOT NULL DEFAULT FALSE,
CONSTRAINT characters_metID PRIMARY KEY (id),
CONSTRAINT UNIQUE KEY user_character_unique (user_id,characters_id),
FOREIGN KEY (user_id)
	REFERENCES user (id),
FOREIGN KEY (characters_id)
	REFERENCES characters (id)
);

create table character_text_adventure_message (
id  int not null auto_increment,
character_id int not null,
message_id int not null,
CONSTRAINT character_text_adventureID PRIMARY KEY (id),
FOREIGN KEY (character_id)
	REFERENCES characters (id),
FOREIGN KEY (message_id)
	REFERENCES text_adventure_message (id)
);

create table text_adventure_message_response (
id int not null auto_increment,
message_id int not null,
response_id int not null,
CONSTRAINT text_responsesID PRIMARY KEY (id),
FOREIGN KEY (message_id)
	REFERENCES text_adventure_message (id),
FOREIGN KEY (response_id)
	REFERENCES text_adventure_response (id)
);

create table text_adventure_reward (
id int not null auto_increment,
message_id int not null,
ammo_id int not null,
reward int not null,
CONSTRAINT text_adventure_rewardID PRIMARY KEY (id),
FOREIGN KEY (message_id)
	REFERENCES text_adventure_message (id),
FOREIGN KEY (ammo_id)
	REFERENCES ammo (id)
);

create table characters_daily_quest (
id int not null auto_increment,
characters_id int not null,
daily_quest_id int not null,
CONSTRAINT characters_questID PRIMARY KEY (id),
FOREIGN KEY (characters_id)
	REFERENCES characters (id),
FOREIGN KEY (daily_quest_id)
	REFERENCES daily_quest (id)
);

create table active_daily_quest (
id int not null auto_increment,
user_id int not null,
quest_id int not null,
app_compleated  BOOLEAN NOT NULL DEFAULT FALSE,
ingame_delivered  BOOLEAN NOT NULL DEFAULT FALSE,
quest_launch_date varchar(255),
CONSTRAINT active_daily_questID PRIMARY KEY (id),
FOREIGN KEY (user_id)
	REFERENCES user (id),
FOREIGN KEY (quest_id)
	REFERENCES daily_quest (id)
);

-- redundant??
-- create table response_outcome_text (
-- id  int not null auto_increment,
-- response_id int not null,
-- text_adventure_id int not null,
-- CONSTRAINT response_outcome_textID PRIMARY KEY (id),
-- FOREIGN KEY (response_id)
-- 	REFERENCES response (id),
-- FOREIGN KEY (text_adventure_id)
-- 	REFERENCES text_adventure (id)
-- );

/*
SELECT count(*) as result FROM `user` WHERE `name` = "trdd";

select * from user;
select count(*) as result from user where name = "test" and password = "pwd";
select user.name from user where user.id = 2;
select user.id from user where user.name = "test" and user.password = "pwd";*/
