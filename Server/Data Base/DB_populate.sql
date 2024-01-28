insert into user(name, password) values
("test", "123");

insert into characters(name, image) values
("Eça de Queirós", "eca_de_queiros"),
("Abílio Junqueiro","guerra_junqueiro"),
("Ramalho Ortigão","ramalho_ortigao");

insert into ammo(name) value
("test");

insert into text_adventure_message(identifier,text) values
( "A1","Hi there Antero, I was thinking about some of the subjects for our future casino conferences. Are you willing to discuss some right now?"),
( "A2","Well you seem to be fairly enthusiastic. But what kind of revolution are we talking about!"),
( "A3","I thought about giving a conference about Realism as a new form of art expression."),
( "A4","Do tell your ideas?"),
( "A5","Yes I totally agree, but on what specifically should we focus on in this introduction?"),
( "A6","I could not have said it better myself! So who do you think should be the first person to give a talk?"),
( "A7","Nice joke Antero!!! But now for real, who do you think should go first?"),
( "A8","So what should be the main theme?"),
( "A9","Umm... I not saying I disagree with that line of thought, but if we start like that, we will be immediately shut down."),
( "A10","Yes that will create a great foundation for us to build upon, maybe that alone will be enough to make some question the status quo."),
( "A11","But before I get to exited, can you go in to a little bit more detail on what that subject entails?"),
( "B1","For a second you had me worried… but now I see we share the same base idea."),
( "B2","So, you want to bring chaos and violence to the country..."),
( "B3","Not only us but also our likeminded friends, I am sure that their contribution to these talks will very fruitful!"),
( "B4","Well for a moment there I thought you were starting to become one of those fanatic revolutionaries!"),
( "B5","In these times of great revolutions, we might be the first to be successful in starting a peaceful revolution!"),
( "B6","Let’s get back on the topic of the themes of the casino conferences."),
( "X1","Maybe we should talk some other time Antero..."),
( "S1","That is perfect Antero!!! I cannot wait for the casino conferences to begin!! We will make History I am sure."),
( "S2","Well you still have time to think, but I'm sure these conferences will change History!!!");
select * from  text_adventure_message;
select * from text_adventure_message;
update text_adventure_message 
set text = "Well you still have time to think, but I'm sure these conferences will change History!!!"
where id = 20;

insert into text_adventure_response(identifier,text) values
( "A2","Revolution Eça, that is what we will be talking about!!!"),
( "A3","Of course Eça I would gladly go over some of those topics with you."),
( "A4","That is extremely interesting and very different to what I was thinking of talking about."),
( "A5","Think we should open with an introduction that explains why we are doing these talks."),
( "A6","The decadence of the Iberic people in face of a changing Europe"),
( "A7","We should invite António Feliciano de Castilho as our first speaker."),
( "A8","Maybe I could go first, because I already have an idea of what I want to talk about."),
( "A9","How the pretentions monarchy and the decrepit catholic church are filling the mind of society with obsolete ideas."),
( "A10","We should look to Iberic people has a whole, its history as an entity, judge it socially, morally and politically."),
( "A11","Lets hope my dear friend."),
( "B1","A revolution of the mind, one where we discuss politics, science, art and culture in order to make the populous more mindful of the real issues."),
( "B2","One where we rile the populous to rise against the monarchy and the old guard and we bring revolution by sheer might"),
( "B3","Of course we do! That is why we are making these conferences."),
( "B4","I don’t understand, why you were worried?"),
( "B5","What!! never!! You know I believe violence to be a barbaric thing!"),
( "B6","Lets hope so Eça..."),
( "X1","A means to an end my dear friend."),
( "S1","I would discuss its decadence. How the changes in Christianity have atrophied individual conscience, how absolute monarchy made the Iberian race blind and submissive, among some other themes."),
( "S2","Well I haven’t thought to much about those yet…"),

( "A4","Uhm.. I have so many ideas for them..."),
( "A5","First I think we should open with an introduction that explains why we are doing these talks."),
( "A5","Of course, in the first talk we should do an introduction that explains the reasons for these conferences and their future themes. "),
( "A6","Revolution, as a noble concept, based on philosophy  and modern science."),
( "B5","Indeed we are lucky to have such a talented cast of friends."),
( "B6","Yes indeed my friend, these talks will be of great benefit to our country."),
( "X1","I was not joking..."),
( "X1","That is the price we must pay for our freedom.");
-- 27
-- select * from text_adventure_response;
insert into user_characters(user_id, characters_id) values
(1,1),
(1,3);

insert into user_ammo(user_id,ammo_id) values
(1, 1);

 insert into character_text_adventure_message(character_id, message_id) values
 (1,1),
 (1,2),
 (1,3),
 (1,4),
 (1,5),
 (1,6),
 (1,7),
 (1,8),
 (1,9),
 (1,10),
 (1,11),
 (1,12),
 (1,13),
 (1,14),
 (1,15),
 (1,16),
 (1,17),
 (1,18),
 (1,19),
 (1,20);
insert into text_adventure_message_response(message_id,response_id) values
(1,2),
(1,1),
(2,12),
(2,11),
(3,4),
(3,3),
(4,21),
(5,5),
(5,23),
(6,7),
(6,6),
(7,26),
(8,9),
(8,8),
(9,27),
(10,10),
(11,18),
(11,19),
(12,13),
(12,14),
(13,17),
(14,25),
(14,24),
(15,15),
(16,16),
(17,22),
(17,20);
/*select * from  text_adventure_message_response;
 select * from text_adventure_response;*/
insert into text_adventure_reward (ammo_id, message_id, reward) values 
(1,19,5),
(1,20,2);

insert into daily_quest (description) values
("Eça quest 1"),
("Eça quest 2"),
("Abílio quest 1"),
("Abílio quest 2"),
("Ramalho quest 1"),
("Ramalho quest 2");

insert into characters_daily_quest (characters_id,daily_quest_id) values 
(1,1),
(1,2),
(2,3),
(2,4),
(3,5),
(3,6);

insert into upgrade (name) values ("double jump");
insert into upgrade (name) values ("dash");
-- insert into user_upgrades (user_id, upgrade_id, active) values (29,2,1);
-- insert into savepoint (identifier, scene) values(1,1);
-- insert into savepoint (identifier, scene) values (2,1);

insert into savepoint (identifier, scene) values
(1,7),
(1,8),
(2,8),
(1,9),
(1,10);