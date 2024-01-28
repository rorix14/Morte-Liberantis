package game.Dev;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;

import com.google.android.material.dialog.MaterialAlertDialogBuilder;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Objects;

public class Menu extends AppCompatActivity {

    Button cenaculoBtn;
    Button dailyQuestBtn;
    Button rewardsBtn;
    String date;
    String userName;
    static boolean firstLog;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_menu);
        Objects.requireNonNull(getSupportActionBar()).hide();

        cenaculoBtn = findViewById(R.id.cenaculoBtn);
        dailyQuestBtn = findViewById(R.id.dailyQuestBtn);
        rewardsBtn = findViewById(R.id.rewardsBtn);

        Intent receiveIntent = getIntent();
        String userId = receiveIntent.getStringExtra("userId");
        userName = receiveIntent.getStringExtra("user name");

        processNewIntent(userId);
    }

    private void gotoClickedPlace(String userId, Class gotoClass) {
        Intent intentNew = new Intent(getApplicationContext(), gotoClass);
        intentNew.putExtra("userId", userId);
        intentNew.putExtra("date", date);
        startActivity(intentNew);
    }

    public void processNewIntent(String userId) {
        Date today = Calendar.getInstance().getTime();//getting date
        SimpleDateFormat formatter = new SimpleDateFormat("dd_MM_yyyy");//formatting according to my need
        date = formatter.format(today);
        System.out.println("test date:" + date);

        cenaculoBtn.setOnClickListener(v -> gotoClickedPlace(userId, friends.class));
        dailyQuestBtn.setOnClickListener(v -> gotoClickedPlace(userId, DailyQuest.class));
        rewardsBtn.setOnClickListener(v -> gotoClickedPlace(userId, CharactersRewards.class));
    }

    void greatNewUser() {
        new MaterialAlertDialogBuilder(Menu.this, R.style.ThemeOverlay_MaterialComponents_MaterialAlertDialog)
                .setIcon(R.drawable.logo)
                .setTitle("Welcome " + userName + "!\nThe app highlights are:")
                .setMessage("Cenaculo: Talk with the famous poets that you have meet!\n" +
                        "\nDaily Quest: Go on real adventures with your favorite poets!\n" +
                        "\nRewards: See the rewards the poets from Cenaculo can give you!")
                .setPositiveButton("Got it!", (dialog, which) -> firstLog = true).show();
    }

    @Override
    protected void onResume() {
        super.onResume();

        if (!firstLog) {
            greatNewUser();
        }
    }
}
