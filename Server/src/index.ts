import express from 'express';
import { Response, Request } from 'express';
import { debuglog } from 'util';

const bodyParser = require('body-parser')
const mysql = require('mysql2/promise');

const env = process.env;

let dbConnection: any;

(async () => {
    dbConnection = await mysql.createConnection({
        host: env.DB_HOST || 'localhost',
        database: env.DB_NAME || 'metroidvania',
        port: env.DB_PORT || 3306,
        user: env.DB_USER || 'root',
        password: env.DB_PASS || 'password'
    });

    console.log('connected to the db whit test!!')  
})();

const app = express();
const port = 8080; // default port to listen

app.use(bodyParser.urlencoded({
    extended: true
}));

app.use(bodyParser.json());

// define a route handler for the default home page
app.get('/', (req: Request, res: Response) => {
    res.send('Hello World!');
});

// register new user
app.post('/register', async (req: Request, res: Response) => {

    if (!req.body || !req.body.name || !req.body.password) {
        return respondBadRequest(res, "Required body param name or password not present");
    }

    const userName = req.body.name
    const userPassword = req.body.password
    console.log(`User register request: \nName: ${userName}, Password: ${userPassword}`);

    const verify: boolean = await isUserInDB(userName, userPassword);

    if (verify) {
        return respondBadRequest(res, 'User already exists');
    }

    const createUser: any = await query('INSERT INTO `user` (`name`,`password`) VALUES (?,?)', [userName, userPassword]);

    if (createUser.affectedRows !== 1) {
        return respondWithInternalServerError(res, 'Error saving user');
    }

    console.log(`User successfully registered: ${userName}`);
    respondWithOk(res, { id: createUser.insertId });
});
// const gameData = {
//     "user": {
//         "id": 1,
//         "name": "Pedro"
//     },
//     "ammo": {
//         "id": 1,
//         "amount": 60
//     },
//     "characterIds": [
//         1,
//         2,
//         3
//     ],
//     "upgradeNames": [
//         "double jump"
//     ],
//     "savePoint": {
//     	"identifier": 1,
//     	"scene": 1
//     }
// }
app.put('/save_point', async (req: Request, res: Response) => {

    const user = req.body.user;
    const ammo = req.body.ammo;
    const characterIds = req.body.characterIds;
    const upgradeNames = req.body.upgradeNames;
    const savePoint = req.body.savePoint;

    console.log("recived json: " + JSON.stringify(req.body))

    let userNameIndex = 1;

    if (!user.id || user.id.localeCompare("") == 0) {

        let userName = user.name;

        while (true) {

            const insertUserQuery: any = await query(`INSERT INTO user (name,password)
            SELECT * FROM (SELECT ? as name, ? as password) AS temp
            WHERE NOT EXISTS (
                SELECT name from user WHERE name = ? AND password = ?) LIMIT 1;`,
                [userName, userName, userName, userName]);

            if (insertUserQuery.affectedRows != 0) {
                user.id = insertUserQuery.insertId;
                break;
            }

            userName = userName + '' + userNameIndex;
            userNameIndex++;
        }
    }

    await query(`INSERT INTO user_ammo (user_id, ammo_id, amount)
    VALUES (?, ?, ?) ON DUPLICATE KEY UPDATE amount = VALUES(amount);`, [user.id, ammo.id, ammo.amount]);

    let values: any[] = [];

    if (characterIds.length != 0) {

        const updateCharactersQuery = characterIds.reduce((acc: string, cur: string, index: number) => {
            values.push(user.id, cur);
            return acc + "(?,?)" + (index === characterIds.length - 1 ? ';' : ',');

        }, `INSERT IGNORE INTO user_characters (user_id, characters_id) VALUES `);

        // console.log(updateCharactersQuery);
        await query(updateCharactersQuery, values);
    }


    values = [];
    if (upgradeNames.length != 0) {

        const updateUpgradeQuery = upgradeNames.reduce((acc: string, cur: string, index: number) => {
            values.push(user.id, cur);
            return acc + "(?,(select id from upgrade where name = ?),1)" + (index === upgradeNames.length - 1 ? '' : ',');
        }, `INSERT INTO user_upgrades (user_id, upgrade_id, active) VALUES `) + `ON DUPLICATE KEY UPDATE active = true;`;
        //console.log(updateUpgradeQuery);

        await query(updateUpgradeQuery, values);
    }

    await query(`INSERT INTO user_ammo (user_id, ammo_id, amount)
    VALUES (?, ?, ?) ON DUPLICATE KEY UPDATE amount = VALUES(amount);`, [user.id, ammo.id, ammo.amount]);

    await query(`INSERT INTO user_savepoint (user_id, savepoint_id) VALUES (?, (select id from savepoint where identifier = ? and scene = ?))
    ON DUPLICATE KEY UPDATE savepoint_id = VALUES(savepoint_id);`, [user.id, savePoint.identifier, savePoint.scene]);

    respondWithOk(res, { id: user.id });
});

//login user
app.post('/login', async (req: Request, res: Response) => {
    const userName = req.body.name
    const userPassword = req.body.password

    const getUser: any = await query('select user.id as id from user where user.name = ? and user.password = ?', [userName, userPassword]);

    if (getUser.length === 0) {
        return respondBadRequest(res, 'User does not exist');
    }

    respondWithOk(res, { id: getUser[0].id });
});

//get the characters the user met
app.get('/user/:id/characters', async (req: Request, res: Response) => {

    const userId = req.params.id

    const getCharacters: any = await query(`select characters.name, characters.image, characters.id
    from user_characters, user, characters 
    where user.id = ? 
    and user.id = user_characters.user_id and characters.id = user_characters.characters_id`, [userId]);

    respondWithOk(res, getCharacters);
});

app.get('/characters/:image/text_adventure_message', async (req: Request, res: Response) => {

    const characterName = req.params.image
    ////// change for actual text
    const getCharacterQuestions: any = await query(`select text_adventure_message.identifier as text_identifier, text_adventure_message.text as message_text,
    text_adventure_response.identifier as response_identifier, text_adventure_response.text as response_text
    from text_adventure_message, text_adventure_response, text_adventure_message_response, (select text_adventure_message.id as text_id
    from characters, text_adventure_message, character_text_adventure_message where
    characters.image = ? and
    characters.id = character_text_adventure_message.character_id and
    text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id asc LIMIT 1) as test where
    text_adventure_message.id = text_adventure_message_response.message_id and 
    text_adventure_response.id = text_adventure_message_response.response_id and
    text_adventure_message.id = test.text_id;`, [characterName]);

    respondWithOk(res, getCharacterQuestions);
});

app.get('/text_adventure_response/:identifier', async (req: Request, res: Response) => {

    const responseIdentifier = req.params.identifier
    ////// change for actual text
    const getNextTextWithAnswers: any = await query(`select text_adventure_message.identifier as text_identifier, text_adventure_message.text as message_text,
    text_adventure_response.identifier as response_identifier, text_adventure_response.text as response_text
    from text_adventure_message, text_adventure_response, text_adventure_message_response where
    text_adventure_message.identifier = ? and
    text_adventure_message.id = text_adventure_message_response.message_id and 
    text_adventure_response.id = text_adventure_message_response.response_id`, [responseIdentifier]);

    respondWithOk(res, getNextTextWithAnswers);
});

app.get('/last_text/:identifier', async (req: Request, res: Response) => {

    const messageIdentifier = req.params.identifier

    const getLastMessage: any = await query(`select text_adventure_message.text
    from text_adventure_message where text_adventure_message.identifier = ?;`, [messageIdentifier]);

    respondWithOk(res, getLastMessage[0]);
});

app.patch('/user/:id/characters', async (req: Request, res: Response) => {
    const userId = req.params.id
    const characterId = req.body.characterId
    const messageIdentifier = req.body.identifier

    const updateCharacterTextAdventure: any = await query(`update user_characters 
    set user_characters.text_adventure_done = true
    where user_characters.user_id = ?
    and user_characters.characters_id = ?`, [userId, characterId]);

    const updateUserAmmoWithReward: any = await query(`update user_ammo
    set user_ammo.amount = user_ammo.amount + (select text_adventure_reward.reward 
    from text_adventure_reward, ammo, text_adventure_message where
    text_adventure_message.identifier = ? and
    text_adventure_message.id = text_adventure_reward.message_id and
    ammo.id = text_adventure_reward.ammo_id)
    where user_ammo.user_id = ?
    and user_ammo.ammo_id = (select text_adventure_reward.ammo_id as ammoId 
    from text_adventure_reward, ammo, text_adventure_message where
    text_adventure_message.identifier = ? and
    text_adventure_message.id = text_adventure_reward.message_id and
    ammo.id = text_adventure_reward.ammo_id)`, [messageIdentifier, userId, messageIdentifier]);

    respondWithOk(res, { textAdventureDone: updateCharacterTextAdventure.changedRows });
});

app.get('/user/:id/characters/:characterId', async (req: Request, res: Response) => {

    const userId = req.params.id
    const characterId = req.params.characterId

    const getCharacterTextAdventure: any = await query(`select user_characters.text_adventure_done as textAdventureDone 
    from user_characters where
    user_characters.user_id =?
    and user_characters.characters_id = ?`, [userId, characterId]);

    respondWithOk(res, getCharacterTextAdventure[0]);
});

app.post('/characters/text_adventure_reward', async (req: Request, res: Response) => {

    const charactersNames = req.body.images

    interface Mapping {
        reward1?: string;
        ammoId1?: string;
        reward2?: string;
        ammoId2?: string;
    }

    interface character {
        eca_de_queiros?: Mapping;
        guerra_junqueiro?: Mapping;
        ramalho_ortigao?: Mapping;
    }

    var characterRewards: Mapping = {};
    var test1: Mapping = {};
    var test2: Mapping = {};
    var characters: character = {};

    test1.reward1 = "reward1.1"
    test1.ammoId1 = "ammoId1.1"
    test1.reward2 = "reward2.1"
    test1.ammoId2 = "ammoId2.1"

    test2.reward1 = "reward1.2"
    test2.ammoId1 = "ammoId1.2"
    test2.reward2 = "reward2.2"
    test2.ammoId2 = "ammoId2.2"

    //var characters1: character = {};
    //var characters2: character = {};

    // var x: Array<character>;

    for (let i = 0; i < charactersNames.length; i++) {
        const characterName = charactersNames[i];

        let getCharacterRewards: any = await query(`select text_adventure_reward.reward as reward, text_adventure_reward.ammo_id as ammoId 
        from text_adventure_reward, text_adventure_message where
        text_adventure_message.id = text_adventure_reward.message_id and
        text_adventure_message.id in(
        select text_adventure_message.id as text_id
        from characters, text_adventure_message, character_text_adventure_message where
        characters.image = ? and
        characters.id = character_text_adventure_message.character_id and
        text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id)`, [characterName]);

        for (let j = 0; j < getCharacterRewards.length; j++) {
            const elemet = getCharacterRewards[j];
            switch (j) {
                case 0: {
                    characterRewards.reward1 = elemet.reward
                    characterRewards.ammoId1 = elemet.ammoId
                    break;
                }
                case 1: {
                    characterRewards.reward2 = elemet.reward
                    characterRewards.ammoId2 = elemet.ammoId
                    break;
                }
                default: {
                    break;
                }
            }
            switch (i) {
                case 0: {
                    characters.eca_de_queiros = characterRewards
                    break;
                }
                case 1: {
                    characters.guerra_junqueiro = characterRewards
                    break;
                }
                case 2: {
                    characters.ramalho_ortigao = characterRewards
                    break;
                }
                default: {
                    break;
                }
            }
        }
    }

    characters.guerra_junqueiro = test1
    characters.ramalho_ortigao = test2

    //x = [characters,characters1,characters2]

    respondWithOk(res, characters);
});

app.get('/user/:id/user_characters', async (req: Request, res: Response) => {

    const userId = req.params.id

    const getCharacterName: any = await query(`select characters.name as characters
    from user, characters, user_characters where
    user.id = ? and
    user_characters.text_adventure_done = true and
    user.id = user_characters.user_id and
    characters.id = user_characters.characters_id`, [userId]);

    respondWithOk(res, getCharacterName);
});

app.get('/text_adventure_message/:identifier/text_adventure_reward', async (req: Request, res: Response) => {

    const textIdentifier = req.params.identifier

    const getTextReward: any = await query(`select text_adventure_reward.reward, text_adventure_reward.ammo_id as ammoId 
    from text_adventure_reward, ammo, text_adventure_message where
    text_adventure_message.identifier = ? and
    text_adventure_message.id = text_adventure_reward.message_id and
    ammo.id = text_adventure_reward.ammo_id;`, [textIdentifier]);

    respondWithOk(res, getTextReward[0]);
});

app.post('/user_characters', async (req: Request, res: Response) => {

    const userId = req.body.userId
    const charactersId = req.body.charactersId

    if (!req.body || !req.body.userId || !req.body.charactersId) {
        return respondBadRequest(res, "Required body param userId or charactersId not present");
    }
    /* const results: any = await query(`select count(*) as result from user_characters where 
     user_characters.user_id = ? and  user_characters.characters_id = ?`, [userId, charactersId]);
 
     if (results[0].result !== 0) {
         return respondBadRequest(res, 'Charecter already met');
     }*/

    const insertCharacter: any = await query(`insert into user_characters(user_id,characters_id) values
    (?,?)`, [userId, charactersId]);

    respondWithOk(res, { affectedRows: insertCharacter.affectedRows, insertId: insertCharacter.insertId });
});

// daily quest methods 
// prosses daily quest in the App
app.get('/user/:userId/active_daily_quest/:date', async (req: Request, res: Response) => {

    const userId = req.params.userId
    const currentdate = req.params.date
    let randomNumber: any
    const dailyQuestQuery = `select characters.image, characters.name, daily_quest.description, id_and_compleated.compleated
    from characters, daily_quest, characters_daily_quest, (select daily_quest.id as questId, active_daily_quest.app_compleated as compleated
    from daily_quest, active_daily_quest, user where 
    user.id = ? and
    user.id = active_daily_quest.user_id and
    active_daily_quest.quest_launch_date = ? and
    active_daily_quest.quest_id = daily_quest.id) as id_and_compleated where 
    daily_quest.id = id_and_compleated.questId and
    daily_quest.id = characters_daily_quest.daily_quest_id and 
    characters.id = characters_daily_quest.characters_id;`;

    const getDailyQuest: any = await query(dailyQuestQuery, [userId, currentdate]);

    console.log("first daily quest: " + JSON.stringify(getDailyQuest));

    if (getDailyQuest.length == 0) {

        const possibleQuests: any = await query(`select daily_quest.id from daily_quest, characters, characters_daily_quest where 
        characters.id = characters_daily_quest.characters_id and 
        daily_quest.id = characters_daily_quest.daily_quest_id and 
        characters.id in (select characters.id from user_characters, user, characters where 
        user.id = ? and
        user.id = user_characters.user_id and 
        characters.id = user_characters.characters_id);`, [userId]);

        randomNumber = getRandomIntInclusive(0, (possibleQuests.length - 1))
        console.log("random number: " + randomNumber + " ,value: " + JSON.stringify(possibleQuests[randomNumber]));

        const insertNewDailyQuest: any = await query(`insert into active_daily_quest (user_id,quest_id,quest_launch_date) values
        (?,?,?);`, [userId, possibleQuests[randomNumber].id, currentdate]);

        const getCharacterAndQuest: any = await query(dailyQuestQuery, [userId, currentdate]);

        respondWithOk(res, getCharacterAndQuest[0]);
    } else {
        respondWithOk(res, getDailyQuest[0]);
    }
});

// update daily quest if completed in the App
app.patch('/user/:id/active_daily_quest', async (req: Request, res: Response) => {
    const userId = req.params.id
    const currentdate = req.body.date

    console.log("id: " + userId)
    console.log("date: " + currentdate)

    const updateDailyQuest: any = await query(`update active_daily_quest
    set active_daily_quest.app_compleated = true
    where active_daily_quest.user_id = ? and active_daily_quest.quest_launch_date = ?;`, [userId, currentdate]);

    respondWithOk(res, { updatedQuest: updateDailyQuest.changedRows });
});

// process daily quest in the main game
app.post('/user/:id/:date', async (req: Request, res: Response) => {

    const userId = req.params.id
    const currentdate = req.params.date
    const characterId = req.body.characterId

    const getDailyQuestStatus: any = await query(`select current_quest.app_compleated, current_quest.ingame_delivered from characters_daily_quest, daily_quest, 
    (select active_daily_quest.app_compleated as app_compleated, active_daily_quest.ingame_delivered as ingame_delivered, active_daily_quest.quest_id as quest_id
    from active_daily_quest, user where 
    active_daily_quest.user_id = ? and active_daily_quest.quest_launch_date =  ? and
    user.id = active_daily_quest.user_id) as current_quest where
    daily_quest.id = current_quest.quest_id and
    characters_daily_quest.characters_id = ? and
    daily_quest.id = characters_daily_quest.daily_quest_id;`, [userId, currentdate, characterId]);

    if (getDailyQuestStatus.length == 0) {

        return respondWithOk(res, { dailyQuestStatus: "No daily quest for this chacter" })
    }
    if (getDailyQuestStatus[0].app_compleated == 0) {

        respondWithOk(res, { dailyQuestStatus: "Character has daily quest but it is not completed in the app" })

    } else if (getDailyQuestStatus[0].app_compleated == 1 && getDailyQuestStatus[0].ingame_delivered == 1) {

        respondWithOk(res, { dailyQuestStatus: "Daily quest for this character has already been completed" })

    } else if (getDailyQuestStatus[0].app_compleated == 1 && getDailyQuestStatus[0].ingame_delivered == 0) {

        const updateDailyQuest: any = await query(`update active_daily_quest
        set active_daily_quest.ingame_delivered = true
        where active_daily_quest.user_id = ? and active_daily_quest.quest_launch_date = ?;`, [userId, currentdate]);

        switch (updateDailyQuest.changedRows) {
            case 1: {
                respondWithOk(res, { dailyQuestStatus: "completed" })
                break;
            }
            case 0: {
                respondWithOk(res, { dailyQuestStatus: "update failed" })
                break;
            }
        }
    }
});
// get player stats in the main game
app.get('/user/:id/stats', async (req: Request, res: Response) => {

    const userid = req.params.id

    const getUserStats: any = await query(`select user_ammo.amount, savepoint.identifier, savepoint.scene 
    from user, user_ammo, savepoint, user_savepoint, upgrade, user_upgrades where
    user.id = ? and user_ammo.user_id = user.id
    and user_savepoint.user_id = user.id and user_savepoint.savepoint_id = savepoint.id;`, [userid]);

    const getUserUpgrades: any = await query(`select upgrade.name from upgrade, user, user_upgrades where
    user.id = ? and
    user_upgrades.user_id = user.id and user_upgrades.upgrade_id = upgrade.id;`, [userid]);

    let values: any[] = [];

    if (getUserStats.length == 0 && getUserUpgrades.length == 0) {
        return respondWithOk(res, {
            ammoAmount: 0, upgradeNames: values,
            savePointIdentifier: 0, savePointScene: 7
        });
    }

    for (let index = 0; index < getUserUpgrades.length; index++) {
        const element = getUserUpgrades[index];
        values.push(element.name);
    }

    respondWithOk(res, {
        ammoAmount: getUserStats[0].amount, upgradeNames: values,
        savePointIdentifier: getUserStats[0].identifier, savePointScene: getUserStats[0].scene
    });
});

// get all currently playing users
app.get('/currently_palying', async (req: Request, res: Response) => {

    const playingUserIds: any = await query(`select user.id from user where user.currently_playing = true;`, []);

    let idValues: any[] = [];

    playingUserIds.forEach((element: any) => {
        idValues.push(element.id);
    });

    respondWithOk(res, {ids: idValues});
});

app.put('/currently_palying/:id/:status', async (req: Request, res: Response) => {
    const userid = req.params.id
    const userStatus = req.params.status

    const updatedPlayingStatus: any = await query(`update user
    set user.currently_playing = ?
    where user.id = ?;`, [userStatus, userid]);

    respondWithOk(res, { playingStatus: updatedPlayingStatus.changedRows });
})

app.get('/invader_id/:id', async (req: Request, res: Response) => {

    const userId = req.params.id;
    const playingUserIds: any = await query(`select user.invaderId from user where user.id = ?;`, [userId]);

    respondWithOk(res, {id: playingUserIds[0].invaderId});
});

app.post('/update_invader_id', async (req: Request, res: Response) => {

    const playerInvaded = req.body.id == 0 ? null : req.body.id;
    const invaderId = req.body.invaderId

    const updateInvaderId: any = await query(`update user 
    set user.invaderId = ?
    where user.id = ?;`, [invaderId, playerInvaded]);

    respondWithOk(res, { changedRows: updateInvaderId.changedRows });
})


// respond methods
const respondWithOk = (res: any, body: any) => {
    respond(res, 200, body);
};

const respondWithInternalServerError = (res: Response, message: string) => {
    respond(res, 500, { error: message });
};

const respondBadRequest = (res: Response, message: string) => {
    respond(res, 400, { error: message });
};

const respond = (res: Response, status: number, body: any = undefined) => {
    let dateTime = new Date()
    console.log(`Response status ${status} with body ${JSON.stringify(body)}, ${dateTime}`);
    res.status(status).send(JSON.stringify(body));
};

const query = async (query: string, params: string[]): Promise<any[]> => {
    return (await dbConnection.query(query, params))[0];
}

// start the Express server
app.listen(port, () => {
    console.log(`server started at http://localhost:${port}`);
});

const isUserInDB = async (name: string, passsword: string) => {

    const results = await query('SELECT count(*) as result FROM `user` WHERE `name` = ? and `password` = ?', [name, passsword]);
    const existsUserName: boolean = results[0].result !== 0;

    return existsUserName;
}
function getRandomIntInclusive(min: any, max: any) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}