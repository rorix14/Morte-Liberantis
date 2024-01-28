package game.Dev;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.app.NotificationCompat;
import androidx.core.content.ContextCompat;

import android.Manifest;
import android.animation.ArgbEvaluator;
import android.animation.ObjectAnimator;
import android.animation.ValueAnimator;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.os.Parcelable;
import android.view.animation.Animation;
import android.widget.TextView;

import com.google.android.material.snackbar.Snackbar;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Objects;

public class MainActivity extends AppCompatActivity {

    public TextView startGametext;
    LocationManager locationManager;
    LocationListener locationListener;
    NotificationManager mNotificationManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Objects.requireNonNull(getSupportActionBar()).hide();
//       getSupportActionBar().setDisplayShowTitleEnabled(false);//hide title
//       getSupportActionBar().setDisplayShowHomeEnabled(false);//hide icon

        locationManager = (LocationManager) this.getSystemService(Context.LOCATION_SERVICE);

        // CHECK FOR USER PERMISSION
//        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED || ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_BACKGROUND_LOCATION) != PackageManager.PERMISSION_GRANTED) {
//            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_BACKGROUND_LOCATION}, 1);
//        }
        if (android.os.Build.VERSION.SDK_INT < android.os.Build.VERSION_CODES.Q && ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        } else if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.Q && (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED || ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_BACKGROUND_LOCATION) != PackageManager.PERMISSION_GRANTED)) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_BACKGROUND_LOCATION}, 1);
        }

        startGametext = findViewById(R.id.textView3);
       /* Snackbar.make(startGametext, "Test this", Snackbar.LENGTH_LONG)
                .show();*/
        manageBlinkEffect();
        startGametext.setOnClickListener(v -> OpenActivity());
    }

    private void manageBlinkEffect() {
        ObjectAnimator animator = ObjectAnimator.ofInt(startGametext, "textColor",
                Color.LTGRAY, Color.BLACK);
        animator.setDuration(1500);
        animator.setEvaluator(new ArgbEvaluator());
        animator.setRepeatMode(ValueAnimator.REVERSE);
        animator.setRepeatCount(Animation.INFINITE);
        animator.start();
    }

    public void OpenActivity() {
        Intent intent = new Intent(this, LoginActivity.class);
        //Intent intent = new Intent(this, friends.class);
        //Intent intent = new Intent(this, DailyQuest.class);
        //Intent intent = new Intent(this, Menu.class);
        intent.putExtra("NotificationManager", (Parcelable) mNotificationManager);
        startActivity(intent);
    }

    @Override
    public void onBackPressed() {
        System.out.println("back was pressed!!!");
        finish();
    }
    /*NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(getApplicationContext(), "YOUR_CHANNEL_ID")
                .setSmallIcon(R.drawable.logo) // notification icon
                .setLargeIcon(BitmapFactory.decodeResource(getResources(), R.drawable.eca_de_queiros))// set image
                .setContentTitle(title) // title for notification
                .setContentText(message)// message for notification
                .setAutoCancel(true); // clear notification after click
        Intent intent = new Intent(getApplicationContext(),  LoginActivity.class);
        PendingIntent pi = PendingIntent.getActivity(this, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        mBuilder.setContentIntent(pi);
        mNotificationManager.notify(0, mBuilder.build());
    }*/

    /*public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 1) {
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                StartListening();
            }
        }
    }

    public void StartListening() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            // USE FOR RUNNING IN THE EMULATOR
            locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000, 30, locationListener);
            // USE FOR RUNNING IN AN ACTUAL PHONE
            //locationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 0, 0, locationListener);
        }
    }*/
}
