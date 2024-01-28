"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var express_1 = __importDefault(require("express"));
var bodyParser = require('body-parser');
var mysql = require('mysql2/promise');
var env = process.env;
var dbConnection;
(function () { return __awaiter(void 0, void 0, void 0, function () {
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, mysql.createConnection({
                    host: env.DB_HOST || 'localhost',
                    database: env.DB_NAME || 'metroidvania',
                    port: env.DB_PORT || 3306,
                    user: env.DB_USER || 'root',
                    password: env.DB_PASS || 'password'
                })];
            case 1:
                dbConnection = _a.sent();
                console.log('connected to the db');
                return [2 /*return*/];
        }
    });
}); })();
var app = express_1.default();
var port = 8080; // default port to listen
app.use(bodyParser.urlencoded({
    extended: true
}));
app.use(bodyParser.json());
// define a route handler for the default home page
app.get('/', function (req, res) {
    res.send('Hello World!');
});
// define a route handler for the default home page
// register new user
app.post('/register', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userName, userPassword, verify, createUser;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                if (!req.body || !req.body.name || !req.body.password) {
                    return [2 /*return*/, respondBadRequest(res, "Required body param name or password not present")];
                }
                userName = req.body.name;
                userPassword = req.body.password;
                console.log("User register request: \nName: " + userName + ", Password: " + userPassword);
                return [4 /*yield*/, isUserInDB(userName, userPassword)];
            case 1:
                verify = _a.sent();
                if (verify) {
                    return [2 /*return*/, respondBadRequest(res, 'User already exists')];
                }
                return [4 /*yield*/, query('INSERT INTO `user` (`name`,`password`) VALUES (?,?)', [userName, userPassword])];
            case 2:
                createUser = _a.sent();
                if (createUser.affectedRows !== 1) {
                    return [2 /*return*/, respondWithInternalServerError(res, 'Error saving user')];
                }
                console.log("User successfully registered: " + userName);
                respondWithOk(res, { id: createUser.insertId });
                return [2 /*return*/];
        }
    });
}); });
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
app.put('/save_point', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var user, ammo, characterIds, upgradeNames, savePoint, userNameIndex, userName, insertUserQuery, values, updateCharactersQuery, updateUpgradeQuery;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                user = req.body.user;
                ammo = req.body.ammo;
                characterIds = req.body.characterIds;
                upgradeNames = req.body.upgradeNames;
                savePoint = req.body.savePoint;
                console.log("recived json: " + JSON.stringify(req.body));
                userNameIndex = 1;
                if (!(!user.id || user.id.localeCompare("") == 0)) return [3 /*break*/, 3];
                userName = user.name;
                _a.label = 1;
            case 1:
                if (!true) return [3 /*break*/, 3];
                return [4 /*yield*/, query("INSERT INTO user (name,password)\n            SELECT * FROM (SELECT ? as name, ? as password) AS temp\n            WHERE NOT EXISTS (\n                SELECT name from user WHERE name = ? AND password = ?) LIMIT 1;", [userName, userName, userName, userName])];
            case 2:
                insertUserQuery = _a.sent();
                if (insertUserQuery.affectedRows != 0) {
                    user.id = insertUserQuery.insertId;
                    return [3 /*break*/, 3];
                }
                userName = userName + '' + userNameIndex;
                userNameIndex++;
                return [3 /*break*/, 1];
            case 3: return [4 /*yield*/, query("INSERT INTO user_ammo (user_id, ammo_id, amount)\n    VALUES (?, ?, ?) ON DUPLICATE KEY UPDATE amount = VALUES(amount);", [user.id, ammo.id, ammo.amount])];
            case 4:
                _a.sent();
                values = [];
                if (!(characterIds.length != 0)) return [3 /*break*/, 6];
                updateCharactersQuery = characterIds.reduce(function (acc, cur, index) {
                    values.push(user.id, cur);
                    return acc + "(?,?)" + (index === characterIds.length - 1 ? ';' : ',');
                }, "INSERT IGNORE INTO user_characters (user_id, characters_id) VALUES ");
                // console.log(updateCharactersQuery);
                return [4 /*yield*/, query(updateCharactersQuery, values)];
            case 5:
                // console.log(updateCharactersQuery);
                _a.sent();
                _a.label = 6;
            case 6:
                values = [];
                if (!(upgradeNames.length != 0)) return [3 /*break*/, 8];
                updateUpgradeQuery = upgradeNames.reduce(function (acc, cur, index) {
                    values.push(user.id, cur);
                    return acc + "(?,(select id from upgrade where name = ?),1)" + (index === upgradeNames.length - 1 ? '' : ',');
                }, "INSERT INTO user_upgrades (user_id, upgrade_id, active) VALUES ") + "ON DUPLICATE KEY UPDATE active = true;";
                //console.log(updateUpgradeQuery);
                return [4 /*yield*/, query(updateUpgradeQuery, values)];
            case 7:
                //console.log(updateUpgradeQuery);
                _a.sent();
                _a.label = 8;
            case 8: return [4 /*yield*/, query("INSERT INTO user_ammo (user_id, ammo_id, amount)\n    VALUES (?, ?, ?) ON DUPLICATE KEY UPDATE amount = VALUES(amount);", [user.id, ammo.id, ammo.amount])];
            case 9:
                _a.sent();
                return [4 /*yield*/, query("INSERT INTO user_savepoint (user_id, savepoint_id) VALUES (?, (select id from savepoint where identifier = ? and scene = ?))\n    ON DUPLICATE KEY UPDATE savepoint_id = VALUES(savepoint_id);", [user.id, savePoint.identifier, savePoint.scene])];
            case 10:
                _a.sent();
                respondWithOk(res, { id: user.id });
                return [2 /*return*/];
        }
    });
}); });
//login user
app.post('/login', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userName, userPassword, getUser;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userName = req.body.name;
                userPassword = req.body.password;
                return [4 /*yield*/, query('select user.id as id from user where user.name = ? and user.password = ?', [userName, userPassword])];
            case 1:
                getUser = _a.sent();
                if (getUser.length === 0) {
                    return [2 /*return*/, respondBadRequest(res, 'User does not exist')];
                }
                respondWithOk(res, { id: getUser[0].id });
                return [2 /*return*/];
        }
    });
}); });
//get the characters the user met
app.get('/user/:id/characters', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, getCharacters;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                return [4 /*yield*/, query("select characters.name, characters.image, characters.id\n    from user_characters, user, characters \n    where user.id = ? \n    and user.id = user_characters.user_id and characters.id = user_characters.characters_id", [userId])];
            case 1:
                getCharacters = _a.sent();
                respondWithOk(res, getCharacters);
                return [2 /*return*/];
        }
    });
}); });
app.get('/characters/:image/text_adventure_message', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var characterName, getCharacterQuestions;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                characterName = req.params.image;
                return [4 /*yield*/, query("select text_adventure_message.identifier as text_identifier, text_adventure_message.text as message_text,\n    text_adventure_response.identifier as response_identifier, text_adventure_response.text as response_text\n    from text_adventure_message, text_adventure_response, text_adventure_message_response, (select text_adventure_message.id as text_id\n    from characters, text_adventure_message, character_text_adventure_message where\n    characters.image = ? and\n    characters.id = character_text_adventure_message.character_id and\n    text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id asc LIMIT 1) as test where\n    text_adventure_message.id = text_adventure_message_response.message_id and \n    text_adventure_response.id = text_adventure_message_response.response_id and\n    text_adventure_message.id = test.text_id;", [characterName])];
            case 1:
                getCharacterQuestions = _a.sent();
                respondWithOk(res, getCharacterQuestions);
                return [2 /*return*/];
        }
    });
}); });
app.get('/text_adventure_response/:identifier', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var responseIdentifier, getNextTextWithAnswers;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                responseIdentifier = req.params.identifier;
                return [4 /*yield*/, query("select text_adventure_message.identifier as text_identifier, text_adventure_message.text as message_text,\n    text_adventure_response.identifier as response_identifier, text_adventure_response.text as response_text\n    from text_adventure_message, text_adventure_response, text_adventure_message_response where\n    text_adventure_message.identifier = ? and\n    text_adventure_message.id = text_adventure_message_response.message_id and \n    text_adventure_response.id = text_adventure_message_response.response_id", [responseIdentifier])];
            case 1:
                getNextTextWithAnswers = _a.sent();
                respondWithOk(res, getNextTextWithAnswers);
                return [2 /*return*/];
        }
    });
}); });
app.get('/last_text/:identifier', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var messageIdentifier, getLastMessage;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                messageIdentifier = req.params.identifier;
                return [4 /*yield*/, query("select text_adventure_message.text\n    from text_adventure_message where text_adventure_message.identifier = ?;", [messageIdentifier])];
            case 1:
                getLastMessage = _a.sent();
                respondWithOk(res, getLastMessage[0]);
                return [2 /*return*/];
        }
    });
}); });
app.patch('/user/:id/characters', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, characterId, messageIdentifier, updateCharacterTextAdventure, updateUserAmmoWithReward;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                characterId = req.body.characterId;
                messageIdentifier = req.body.identifier;
                return [4 /*yield*/, query("update user_characters \n    set user_characters.text_adventure_done = true\n    where user_characters.user_id = ?\n    and user_characters.characters_id = ?", [userId, characterId])];
            case 1:
                updateCharacterTextAdventure = _a.sent();
                return [4 /*yield*/, query("update user_ammo\n    set user_ammo.amount = user_ammo.amount + (select text_adventure_reward.reward \n    from text_adventure_reward, ammo, text_adventure_message where\n    text_adventure_message.identifier = ? and\n    text_adventure_message.id = text_adventure_reward.message_id and\n    ammo.id = text_adventure_reward.ammo_id)\n    where user_ammo.user_id = ?\n    and user_ammo.ammo_id = (select text_adventure_reward.ammo_id as ammoId \n    from text_adventure_reward, ammo, text_adventure_message where\n    text_adventure_message.identifier = ? and\n    text_adventure_message.id = text_adventure_reward.message_id and\n    ammo.id = text_adventure_reward.ammo_id)", [messageIdentifier, userId, messageIdentifier])];
            case 2:
                updateUserAmmoWithReward = _a.sent();
                respondWithOk(res, { textAdventureDone: updateCharacterTextAdventure.changedRows });
                return [2 /*return*/];
        }
    });
}); });
app.get('/user/:id/characters/:characterId', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, characterId, getCharacterTextAdventure;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                characterId = req.params.characterId;
                return [4 /*yield*/, query("select user_characters.text_adventure_done as textAdventureDone \n    from user_characters where\n    user_characters.user_id =?\n    and user_characters.characters_id = ?", [userId, characterId])];
            case 1:
                getCharacterTextAdventure = _a.sent();
                respondWithOk(res, getCharacterTextAdventure[0]);
                return [2 /*return*/];
        }
    });
}); });
app.post('/characters/text_adventure_reward', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var charactersNames, characterRewards, test1, test2, characters, i, characterName, getCharacterRewards, j, elemet;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                charactersNames = req.body.images;
                characterRewards = {};
                test1 = {};
                test2 = {};
                characters = {};
                test1.reward1 = "reward1.1";
                test1.ammoId1 = "ammoId1.1";
                test1.reward2 = "reward2.1";
                test1.ammoId2 = "ammoId2.1";
                test2.reward1 = "reward1.2";
                test2.ammoId1 = "ammoId1.2";
                test2.reward2 = "reward2.2";
                test2.ammoId2 = "ammoId2.2";
                i = 0;
                _a.label = 1;
            case 1:
                if (!(i < charactersNames.length)) return [3 /*break*/, 4];
                characterName = charactersNames[i];
                return [4 /*yield*/, query("select text_adventure_reward.reward as reward, text_adventure_reward.ammo_id as ammoId \n        from text_adventure_reward, text_adventure_message where\n        text_adventure_message.id = text_adventure_reward.message_id and\n        text_adventure_message.id in(\n        select text_adventure_message.id as text_id\n        from characters, text_adventure_message, character_text_adventure_message where\n        characters.image = ? and\n        characters.id = character_text_adventure_message.character_id and\n        text_adventure_message.id = character_text_adventure_message.message_id ORDER BY text_adventure_message.id)", [characterName])];
            case 2:
                getCharacterRewards = _a.sent();
                for (j = 0; j < getCharacterRewards.length; j++) {
                    elemet = getCharacterRewards[j];
                    switch (j) {
                        case 0: {
                            characterRewards.reward1 = elemet.reward;
                            characterRewards.ammoId1 = elemet.ammoId;
                            break;
                        }
                        case 1: {
                            characterRewards.reward2 = elemet.reward;
                            characterRewards.ammoId2 = elemet.ammoId;
                            break;
                        }
                        default: {
                            break;
                        }
                    }
                    switch (i) {
                        case 0: {
                            characters.eca_de_queiros = characterRewards;
                            break;
                        }
                        case 1: {
                            characters.guerra_junqueiro = characterRewards;
                            break;
                        }
                        case 2: {
                            characters.ramalho_ortigao = characterRewards;
                            break;
                        }
                        default: {
                            break;
                        }
                    }
                }
                _a.label = 3;
            case 3:
                i++;
                return [3 /*break*/, 1];
            case 4:
                characters.guerra_junqueiro = test1;
                characters.ramalho_ortigao = test2;
                //x = [characters,characters1,characters2]
                respondWithOk(res, characters);
                return [2 /*return*/];
        }
    });
}); });
app.get('/user/:id/user_characters', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, getCharacterName;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                return [4 /*yield*/, query("select characters.name as characters\n    from user, characters, user_characters where\n    user.id = ? and\n    user_characters.text_adventure_done = true and\n    user.id = user_characters.user_id and\n    characters.id = user_characters.characters_id", [userId])];
            case 1:
                getCharacterName = _a.sent();
                respondWithOk(res, getCharacterName);
                return [2 /*return*/];
        }
    });
}); });
app.get('/text_adventure_message/:identifier/text_adventure_reward', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var textIdentifier, getTextReward;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                textIdentifier = req.params.identifier;
                return [4 /*yield*/, query("select text_adventure_reward.reward, text_adventure_reward.ammo_id as ammoId \n    from text_adventure_reward, ammo, text_adventure_message where\n    text_adventure_message.identifier = ? and\n    text_adventure_message.id = text_adventure_reward.message_id and\n    ammo.id = text_adventure_reward.ammo_id;", [textIdentifier])];
            case 1:
                getTextReward = _a.sent();
                respondWithOk(res, getTextReward[0]);
                return [2 /*return*/];
        }
    });
}); });
app.post('/user_characters', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, charactersId, insertCharacter;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.body.userId;
                charactersId = req.body.charactersId;
                if (!req.body || !req.body.userId || !req.body.charactersId) {
                    return [2 /*return*/, respondBadRequest(res, "Required body param userId or charactersId not present")];
                }
                return [4 /*yield*/, query("insert into user_characters(user_id,characters_id) values\n    (?,?)", [userId, charactersId])];
            case 1:
                insertCharacter = _a.sent();
                respondWithOk(res, { affectedRows: insertCharacter.affectedRows, insertId: insertCharacter.insertId });
                return [2 /*return*/];
        }
    });
}); });
// daily quest methods 
// prosses daily quest in the App
app.get('/user/:userId/active_daily_quest/:date', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, currentdate, randomNumber, dailyQuestQuery, getDailyQuest, possibleQuests, insertNewDailyQuest, getCharacterAndQuest;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.userId;
                currentdate = req.params.date;
                dailyQuestQuery = "select characters.image, characters.name, daily_quest.description, id_and_compleated.compleated\n    from characters, daily_quest, characters_daily_quest, (select daily_quest.id as questId, active_daily_quest.app_compleated as compleated\n    from daily_quest, active_daily_quest, user where \n    user.id = ? and\n    user.id = active_daily_quest.user_id and\n    active_daily_quest.quest_launch_date = ? and\n    active_daily_quest.quest_id = daily_quest.id) as id_and_compleated where \n    daily_quest.id = id_and_compleated.questId and\n    daily_quest.id = characters_daily_quest.daily_quest_id and \n    characters.id = characters_daily_quest.characters_id;";
                return [4 /*yield*/, query(dailyQuestQuery, [userId, currentdate])];
            case 1:
                getDailyQuest = _a.sent();
                console.log("first daily quest: " + JSON.stringify(getDailyQuest));
                if (!(getDailyQuest.length == 0)) return [3 /*break*/, 5];
                return [4 /*yield*/, query("select daily_quest.id from daily_quest, characters, characters_daily_quest where \n        characters.id = characters_daily_quest.characters_id and \n        daily_quest.id = characters_daily_quest.daily_quest_id and \n        characters.id in (select characters.id from user_characters, user, characters where \n        user.id = ? and\n        user.id = user_characters.user_id and \n        characters.id = user_characters.characters_id);", [userId])];
            case 2:
                possibleQuests = _a.sent();
                randomNumber = getRandomIntInclusive(0, (possibleQuests.length - 1));
                console.log("random number: " + randomNumber + " ,value: " + JSON.stringify(possibleQuests[randomNumber]));
                return [4 /*yield*/, query("insert into active_daily_quest (user_id,quest_id,quest_launch_date) values\n        (?,?,?);", [userId, possibleQuests[randomNumber].id, currentdate])];
            case 3:
                insertNewDailyQuest = _a.sent();
                return [4 /*yield*/, query(dailyQuestQuery, [userId, currentdate])];
            case 4:
                getCharacterAndQuest = _a.sent();
                respondWithOk(res, getCharacterAndQuest[0]);
                return [3 /*break*/, 6];
            case 5:
                respondWithOk(res, getDailyQuest[0]);
                _a.label = 6;
            case 6: return [2 /*return*/];
        }
    });
}); });
// update daily quest if completed in the App
app.patch('/user/:id/active_daily_quest', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, currentdate, updateDailyQuest;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                currentdate = req.body.date;
                console.log("id: " + userId);
                console.log("date: " + currentdate);
                return [4 /*yield*/, query("update active_daily_quest\n    set active_daily_quest.app_compleated = true\n    where active_daily_quest.user_id = ? and active_daily_quest.quest_launch_date = ?;", [userId, currentdate])];
            case 1:
                updateDailyQuest = _a.sent();
                respondWithOk(res, { updatedQuest: updateDailyQuest.changedRows });
                return [2 /*return*/];
        }
    });
}); });
// process daily quest in the main game
app.post('/user/:id/:date', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, currentdate, characterId, getDailyQuestStatus, updateDailyQuest;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                currentdate = req.params.date;
                characterId = req.body.characterId;
                return [4 /*yield*/, query("select current_quest.app_compleated, current_quest.ingame_delivered from characters_daily_quest, daily_quest, \n    (select active_daily_quest.app_compleated as app_compleated, active_daily_quest.ingame_delivered as ingame_delivered, active_daily_quest.quest_id as quest_id\n    from active_daily_quest, user where \n    active_daily_quest.user_id = ? and active_daily_quest.quest_launch_date =  ? and\n    user.id = active_daily_quest.user_id) as current_quest where\n    daily_quest.id = current_quest.quest_id and\n    characters_daily_quest.characters_id = ? and\n    daily_quest.id = characters_daily_quest.daily_quest_id;", [userId, currentdate, characterId])];
            case 1:
                getDailyQuestStatus = _a.sent();
                if (getDailyQuestStatus.length == 0) {
                    return [2 /*return*/, respondWithOk(res, { dailyQuestStatus: "No daily quest for this chacter" })];
                }
                if (!(getDailyQuestStatus[0].app_compleated == 0)) return [3 /*break*/, 2];
                respondWithOk(res, { dailyQuestStatus: "Character has daily quest but it is not completed in the app" });
                return [3 /*break*/, 5];
            case 2:
                if (!(getDailyQuestStatus[0].app_compleated == 1 && getDailyQuestStatus[0].ingame_delivered == 1)) return [3 /*break*/, 3];
                respondWithOk(res, { dailyQuestStatus: "Daily quest for this character has already been completed" });
                return [3 /*break*/, 5];
            case 3:
                if (!(getDailyQuestStatus[0].app_compleated == 1 && getDailyQuestStatus[0].ingame_delivered == 0)) return [3 /*break*/, 5];
                return [4 /*yield*/, query("update active_daily_quest\n        set active_daily_quest.ingame_delivered = true\n        where active_daily_quest.user_id = ? and active_daily_quest.quest_launch_date = ?;", [userId, currentdate])];
            case 4:
                updateDailyQuest = _a.sent();
                switch (updateDailyQuest.changedRows) {
                    case 1: {
                        respondWithOk(res, { dailyQuestStatus: "completed" });
                        break;
                    }
                    case 0: {
                        respondWithOk(res, { dailyQuestStatus: "update failed" });
                        break;
                    }
                }
                _a.label = 5;
            case 5: return [2 /*return*/];
        }
    });
}); });
// get player stats in the main game
app.get('/user/:id/stats', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userid, getUserStats, getUserUpgrades, values, index, element;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userid = req.params.id;
                return [4 /*yield*/, query("select user_ammo.amount, savepoint.identifier, savepoint.scene \n    from user, user_ammo, savepoint, user_savepoint, upgrade, user_upgrades where\n    user.id = ? and user_ammo.user_id = user.id\n    and user_savepoint.user_id = user.id and user_savepoint.savepoint_id = savepoint.id;", [userid])];
            case 1:
                getUserStats = _a.sent();
                return [4 /*yield*/, query("select upgrade.name from upgrade, user, user_upgrades where\n    user.id = ? and\n    user_upgrades.user_id = user.id and user_upgrades.upgrade_id = upgrade.id;", [userid])];
            case 2:
                getUserUpgrades = _a.sent();
                values = [];
                if (getUserStats.length == 0 && getUserUpgrades.length == 0) {
                    return [2 /*return*/, respondWithOk(res, {
                            ammoAmount: 0, upgradeNames: values,
                            savePointIdentifier: 0, savePointScene: 7
                        })];
                }
                for (index = 0; index < getUserUpgrades.length; index++) {
                    element = getUserUpgrades[index];
                    values.push(element.name);
                }
                respondWithOk(res, {
                    ammoAmount: getUserStats[0].amount, upgradeNames: values,
                    savePointIdentifier: getUserStats[0].identifier, savePointScene: getUserStats[0].scene
                });
                return [2 /*return*/];
        }
    });
}); });
// get all currently playing users
app.get('/currently_palying', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var playingUserIds, idValues;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, query("select user.id from user where user.currently_playing = true;", [])];
            case 1:
                playingUserIds = _a.sent();
                idValues = [];
                playingUserIds.forEach(function (element) {
                    idValues.push(element.id);
                });
                respondWithOk(res, { ids: idValues });
                return [2 /*return*/];
        }
    });
}); });
app.put('/currently_palying/:id/:status', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userid, userStatus, updatedPlayingStatus;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userid = req.params.id;
                userStatus = req.params.status;
                return [4 /*yield*/, query("update user\n    set user.currently_playing = ?\n    where user.id = ?;", [userStatus, userid])];
            case 1:
                updatedPlayingStatus = _a.sent();
                respondWithOk(res, { playingStatus: updatedPlayingStatus.changedRows });
                return [2 /*return*/];
        }
    });
}); });
app.get('/invader_id/:id', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var userId, playingUserIds;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                userId = req.params.id;
                return [4 /*yield*/, query("select user.invaderId from user where user.id = ?;", [userId])];
            case 1:
                playingUserIds = _a.sent();
                respondWithOk(res, { id: playingUserIds[0].invaderId });
                return [2 /*return*/];
        }
    });
}); });
app.post('/update_invader_id', function (req, res) { return __awaiter(void 0, void 0, void 0, function () {
    var playerInvaded, invaderId, updateInvaderId;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                playerInvaded = req.body.id == 0 ? null : req.body.id;
                invaderId = req.body.invaderId;
                return [4 /*yield*/, query("update user \n    set user.invaderId = ?\n    where user.id = ?;", [invaderId, playerInvaded])];
            case 1:
                updateInvaderId = _a.sent();
                respondWithOk(res, { changedRows: updateInvaderId.changedRows });
                return [2 /*return*/];
        }
    });
}); });
// respond methods
var respondWithOk = function (res, body) {
    respond(res, 200, body);
};
var respondWithInternalServerError = function (res, message) {
    respond(res, 500, { error: message });
};
var respondBadRequest = function (res, message) {
    respond(res, 400, { error: message });
};
var respond = function (res, status, body) {
    if (body === void 0) { body = undefined; }
    var dateTime = new Date();
    console.log("Response status " + status + " with body " + JSON.stringify(body) + ", " + dateTime);
    res.status(status).send(JSON.stringify(body));
};
var query = function (query, params) { return __awaiter(void 0, void 0, void 0, function () {
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, dbConnection.query(query, params)];
            case 1: return [2 /*return*/, (_a.sent())[0]];
        }
    });
}); };
// start the Express server
app.listen(port, function () {
    console.log("server started at http://localhost:" + port);
});
var isUserInDB = function (name, passsword) { return __awaiter(void 0, void 0, void 0, function () {
    var results, existsUserName;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, query('SELECT count(*) as result FROM `user` WHERE `name` = ? and `password` = ?', [name, passsword])];
            case 1:
                results = _a.sent();
                existsUserName = results[0].result !== 0;
                return [2 /*return*/, existsUserName];
        }
    });
}); };
function getRandomIntInclusive(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
//# sourceMappingURL=index.js.map