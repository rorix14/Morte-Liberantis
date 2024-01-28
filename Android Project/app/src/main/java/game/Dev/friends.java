package game.Dev;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;

public class friends extends AppCompatActivity {

    ServerService serverService = new ServerService(this);
    ArrayList<String> charactersList = new ArrayList<>();
    ArrayList<String> imageList = new ArrayList<>();
    ArrayList<String> idList = new ArrayList<>();
    ArrayList<ArrayList<String>> lists = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_friends);
        getSupportActionBar().hide();

        Button rewardsBtn = findViewById(R.id.rewardsBtn);

        lists.add(0, charactersList);
        lists.add(1, imageList);
        lists.add(2, idList);

        ArrayList<View> charactersContainerList = new ArrayList<>();

        View ramalhoContainer = findViewById(R.id.ramalhoOrtigaoConteiner);
        View ecaContainer = findViewById(R.id.ecaCoiteiner);
        View guerraCotainer = findViewById(R.id.guerraConteiner);
        charactersContainerList.add(ramalhoContainer);
        charactersContainerList.add(ecaContainer);
        charactersContainerList.add(guerraCotainer);

        Intent receiveIntent = getIntent();
        String userId = receiveIntent.getStringExtra("userId");
        System.out.println("The user id is " + userId);


        rewardsBtn.setOnClickListener(v -> gotoCharactersRewards(userId));

        serverService.getUserCharacters(userId, jsonArray -> userCharactersJSON(jsonArray, charactersContainerList, userId, lists),
                exception -> System.out.println(exception));
    }

    // get the characters the player has met in the main game and make them clickable
    // get jsonObject from jsonArray and add values to the correct lists
    private void userCharactersJSON(JSONArray response, ArrayList<View> containerList, String userId, ArrayList<ArrayList<String>> lists) {
        JSONObject valueFromArry;
        try {
            for (int i = 0; i < response.length(); i++) {
                valueFromArry = (JSONObject) response.get(i);
                lists.get(0).add(valueFromArry.get("name").toString());
                lists.get(1).add(valueFromArry.get("image").toString());
                lists.get(2).add(valueFromArry.get("id").toString());
            }
        } catch (JSONException e) {
            e.printStackTrace();
        }
        System.out.println("Characters: " + lists.get(0) + ", Images: " + lists.get(1) + ", ID: " + lists.get(2));

        highlightContainer(containerList, userId, lists);
    }

    // highlight the characters containers and make them clickable
    public void highlightContainer(ArrayList<View> containerList, String userId, ArrayList<ArrayList<String>> lists) {

        for (int i = 0; i < lists.get(0).size(); i++) {
            if (lists.get(0).get(i).equals("Eça de Queirós"))
                activateOnClick(containerList.get(1), i, userId, lists);

            else if (lists.get(0).get(i).equals("Abílio Junqueiro"))
                activateOnClick(containerList.get(2), i, userId, lists);

            else if (lists.get(0).get(i).equals("Ramalho Ortigão"))
                activateOnClick(containerList.get(0), i, userId, lists);
        }
        System.out.println("This is all my lists: " + lists);
    }

    public void activateOnClick(View containerList, int finalI, String userId, ArrayList<ArrayList<String>> lists) {
        containerList.animate().alpha(1);

        containerList.setOnClickListener(view -> {
            Intent intentNew = new Intent(getApplicationContext(), ConversationPage.class);
            intentNew.putExtra("image", lists.get(1).get(finalI));
            intentNew.putExtra("user id", userId);
            intentNew.putExtra("character id", lists.get(2).get(finalI));
            System.out.println("Image: " + lists.get(1).get(finalI) + ", Character id: " + lists.get(2).get(finalI));
            startActivity(intentNew);
        });
    }

    // go to reward scene
    private void gotoCharactersRewards(String userId) {
        Intent intentNew = new Intent(getApplicationContext(), CharactersRewards.class);
        intentNew.putExtra("images", lists.get(1));
        intentNew.putExtra("userId", userId);
        startActivity(intentNew);
    }
}
