package game.Dev;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.MessageFormat;
import java.util.HashMap;
import java.util.Map;

public class ConversationPage extends AppCompatActivity {

    private ServerService serverService = new ServerService(this);
    Map<String, String> question = new HashMap<>();
    int isTextAdventureDone = 0;
    TextView rewardsText;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_conversation_page);
        getSupportActionBar().hide();

        rewardsText = findViewById(R.id.textRewardText);
        ImageView characterImage = findViewById(R.id.imageView3);
        TextView npcDialog = findViewById(R.id.npcDialog);
        Button option1 = findViewById(R.id.option1);
        Button option2 = findViewById(R.id.option2);

        Intent receiveIntent = getIntent();
        String image = receiveIntent.getStringExtra("image");
        int userId = Integer.parseInt(receiveIntent.getStringExtra("user id"));
        int characterId = Integer.parseInt(receiveIntent.getStringExtra("character id"));
        System.out.println("userID: " + userId + ", characterID: " + characterId);

        characterImage.setTranslationX(500);

        int resourceId = this.getResources().getIdentifier(image, "drawable",
                this.getPackageName());

        characterImage.setImageResource(resourceId);
        characterImage.animate().translationXBy(-500).rotation(360).alpha(1).setDuration(1000);

        serverService.getTextStatos(userId, characterId, jsonObject -> setextAdventureStatos(jsonObject, image, npcDialog, option1, option2,
                userId, characterId),
                exception -> System.out.println(exception));
    }


    // see if text adventure is competed and actualize scene accordingly
    public void setextAdventureStatos(JSONObject jsonObject, String image, TextView npcDialog, Button option1, Button option2,
                                      int userId, int characterId) {
        try {
            isTextAdventureDone = Integer.parseInt(jsonObject.get("textAdventureDone").toString());
        } catch (JSONException e) {
            e.printStackTrace();
        }

        System.out.println("Text Adventure Status: " + isTextAdventureDone);

        // if text adventure is not done get the first question and the answers that corresponds to it
        // and responses clickable
        if (isTextAdventureDone == 0) {
            serverService.getCharacterQuestion(image,
                    jsonArray -> getQuestion(jsonArray, npcDialog, option1, option2, question, null,
                            userId, characterId), exception -> System.out.println(exception));

            option1.setOnClickListener(view -> serverService.getTextWithAnwsers(question.get("option 1"),
                    jsonArray -> getQuestion(jsonArray, npcDialog, option1, option2, question, question.get("option 1"),
                            userId, characterId), exception -> System.out.println(exception)));

            option2.setOnClickListener(view -> serverService.getTextWithAnwsers(question.get("option 2"),
                    jsonArray -> getQuestion(jsonArray, npcDialog, option1, option2, question, question.get("option 2"),
                            userId, characterId), exception -> System.out.println(exception)));
        } else npcDialog.setText("You Have compleated my Text Adventure!");
    }

    private void getQuestion(JSONArray response, TextView npcDialog, Button option1, Button option2,
                             Map<String, String> question, String chosenOption, int userId, int characterId) {
        JSONObject valueFromArry;
        option1.setVisibility(option1.INVISIBLE);
        option2.setVisibility(option1.INVISIBLE);

        try {
            for (int i = 0; i < response.length(); i++) {
                valueFromArry = (JSONObject) response.get(i);
                question.put("text", valueFromArry.get("text_identifier").toString());
                question.put("messageText", valueFromArry.getString("message_text"));// To show character text
                switch (i) {
                    case 0:
                        setOptionText(question, valueFromArry, option1, "option 1", "response text 1", "1:");
                        break;
                    case 1:
                        setOptionText(question, valueFromArry, option2, "option 2", "response text 2", "2:");
                        break;
                }
            }
        } catch (JSONException e) {
            e.printStackTrace();
        }
        if (response.length() == 0) {
            //set last text
            serverService.getLastMessageText(chosenOption, jsonObject -> {
                try {
                    npcDialog.setText(jsonObject.getString("text"));
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }, System.out::println);
            // update text adventure status
            serverService.patchCharacterTextAdventure(userId, characterId, chosenOption, jsonObject -> getReward(chosenOption),
                    exception -> System.out.println(exception));
        } else npcDialog.setText(question.get("messageText")); // final change
    }

    private void setOptionText(Map<String, String> question, JSONObject valueFromArry, Button option, String key, String responceText, String questionNuber) throws JSONException {
        question.put(key, valueFromArry.get("response_identifier").toString());
        question.put(responceText,valueFromArry.getString("response_text"));
        option.setText(question.get(responceText));
        option.setVisibility(option.VISIBLE);
    }

    // show test reward if textAdventure is done
    private void getReward(String chosenOption) {
        serverService.getTextReward(chosenOption, jsonObject -> {
            rewardsText.setVisibility(View.VISIBLE);
            try {
                rewardsText.setText(MessageFormat.format("You gained {0} ammo from item {1}",
                        new String[]{jsonObject.getString("reward"), jsonObject.getString("ammoId")}));
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }, e -> System.out.println(e));
    }

    @Override
    protected void onStop() {
        super.onStop();
        finish();
    }
}
//    private void getQuestion(JSONArray response, TextView npcDialog, Button option1, Button option2,
//                             ArrayList<String> text, ArrayList<String> optionIdentifier, ServerService serverService) {
//        JSONObject valueFromArry;
//        try {
//            for (int i = 0; i < response.length(); i++) {
//                valueFromArry = (JSONObject) response.get(i);
//                text.add(valueFromArry.get("text_identifier").toString());
//                optionIdentifier.add(valueFromArry.get("response_identifier").toString());
//            }
//        } catch (JSONException e) {
//            e.printStackTrace();
//        }
//        npcDialog.setText(text.get(0));
//        option1.setText(optionIdentifier.get(0));
//        option2.setText(optionIdentifier.get(1));
//    }

/*
try {
            question.put("id", response.get("id").toString());
            question.put("identifier", response.get("identifier").toString());
        } catch (JSONException e) {
            e.printStackTrace();
        }
        npcDialog.setText(question.get("text"));

        serverService.getTextWithAnwsers(question.get("identifier"), jsonArray -> System.out.println(jsonArray), exception -> System.out.println(exception));
 */