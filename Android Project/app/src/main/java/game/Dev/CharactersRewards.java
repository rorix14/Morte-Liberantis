package game.Dev;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class CharactersRewards extends AppCompatActivity {

    ServerService serverService = new ServerService(this);

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_characters_rewards);
        getSupportActionBar().hide();

        Intent receiveIntent = getIntent();
        TextView[][] charactersTexts = {{findViewById(R.id.ecaReward1), findViewById(R.id.ecaReward2)},
                {findViewById(R.id.guerraReward1), findViewById(R.id.guerraReward2)},
                {findViewById(R.id.ramalhoReward1), findViewById(R.id.ramalhoReward2)}};

        TextView[] completedText = {findViewById(R.id.comlpeteEca), findViewById(R.id.comlpeteGuerra), findViewById(R.id.comlpeteRamalho)};
        LinearLayout[] linearLayouts = {findViewById(R.id.layoutEca), findViewById(R.id.layoutGuerra), findViewById(R.id.layoutRamalho)};

        String[] imageList = {"eca_de_queiros", "guerra_junqueiro", "ramalho_ortigao"};
        String userId = receiveIntent.getStringExtra("userId");
        //ArrayList<String> imageList = receiveIntent.getStringArrayListExtra("images");
        System.out.println("the available images are " + imageList);

        serverService.getCharacterRewards(imageList, jsonObject -> showRewards(jsonObject, imageList, charactersTexts, userId, completedText, linearLayouts),
                e -> System.out.println("ERROR: " + e));
    }

    // show character rewards in the containers
    private void showRewards(JSONObject response, String[] imageList, TextView[][] charactersTexts, String userId,
                             TextView[] completedText, LinearLayout[] linearLayouts) {
        try {
            setRewardText(response, imageList, charactersTexts);
        } catch (JSONException e) {
            e.printStackTrace();
        }
        serverService.getCharactersTextAdventureStatus(userId, jsonArray -> charctersTextAdventureDone(jsonArray, completedText, linearLayouts),
                e -> System.out.println("ERROR" + e));
    }

    private void setRewardText(JSONObject response, String[] imageList, TextView[][] charactersTexts) throws JSONException {
        JSONObject ecaRewards;

        for (int i = 0; i < imageList.length; i++) {
            ecaRewards = (JSONObject) response.get(imageList[i]);
            charactersTexts[i][0].setText("Value of the Reward:\n" + ecaRewards.get("reward1").toString() + " points\nIn  Item: " + ecaRewards.get("ammoId1"));
            charactersTexts[i][1].setText("Value of the Reward:\n" + ecaRewards.get("reward2").toString() + " points\nIn  Item: " + ecaRewards.get("ammoId2"));

        }
    }

    // see if character text adventure is done and show updated text
    private void charctersTextAdventureDone(JSONArray response, TextView[] completedText, LinearLayout[] linearLayouts) {
        JSONObject valueFromArry;

        try {
            for (int i = 0; i < response.length(); i++) {
                valueFromArry = (JSONObject) response.get(i);
                System.out.println("JSONObject: " + valueFromArry);
                if ("Eça de Queirós".equals(valueFromArry.get("characters").toString()))
                    showCompletedText(completedText, linearLayouts, 0);

                else if ("Abílio Junqueiro".equals(valueFromArry.get("characters").toString()))
                    showCompletedText(completedText, linearLayouts, 1);

                else if ("Ramalho Ortigão".equals(valueFromArry.get("characters").toString()))
                    showCompletedText(completedText, linearLayouts, 2);
            }
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private void showCompletedText(TextView[] completedText, LinearLayout[] linearLayouts, int index) {
        completedText[index].setVisibility(View.VISIBLE);
        linearLayouts[index].setAlpha(0.5f);
    }

    @Override
    protected void onStop() {
        super.onStop();
        finish();
    }
}
