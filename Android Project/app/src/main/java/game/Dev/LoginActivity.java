package game.Dev;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.EditText;

import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.NotificationCompat;

import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;
import com.google.android.material.textfield.TextInputLayout;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Objects;

public class LoginActivity extends AppCompatActivity {

    private ServerService service;
    private Button loginBtn;
    private Button registerBtn;
    private TextInputLayout nameInput;
    private TextInputLayout passwordInput;
    private TextView feedbackText;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        Objects.requireNonNull(getSupportActionBar()).hide();


        this.service = new ServerService(this);

        loginBtn = findViewById(R.id.loginButton);
        registerBtn = findViewById(R.id.registerButton);

        nameInput = findViewById(R.id.nameInput);
        passwordInput = findViewById(R.id.passwordInput);
        feedbackText = findViewById(R.id.textView2);

        loginBtn.setOnClickListener(view -> login());
        registerBtn.setOnClickListener(view -> register());
    }

    private void startCenacle(JSONObject response) {
        Intent intent = new Intent(this, Menu.class);

        try {
            intent.putExtra("userId", response.get("id").toString());
            intent.putExtra("user name", nameInput.getEditText().getText().toString());
        } catch (JSONException e) {
            e.printStackTrace();
        }
        startActivity(intent);
    }

    private void register() {
        service.register(nameInput.getEditText().getText().toString(), passwordInput.getEditText().getText().toString(),
                (JSONObject response) -> startCenacle(response),
                (Exception err) -> feedbackText.setText("Failed to register user"));
    }

    private void login() {
        service.login(nameInput.getEditText().getText().toString(), passwordInput.getEditText().getText().toString(),
                (JSONObject response) -> startCenacle(response),
                (Exception err) -> feedbackText.setText("Invalid username or password"));
    }

    @Override
    protected void onStop() {
        super.onStop();
        finish();
    }

    /*void showNotification(String title, String message) {
        NotificationManager mNotificationManager =
                (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel("YOUR_CHANNEL_ID",
                    "YOUR_CHANNEL_NAME",
                    NotificationManager.IMPORTANCE_DEFAULT);
            channel.setLockscreenVisibility(Notification.VISIBILITY_PUBLIC);
            channel.setDescription("YOUR_NOTIFICATION_CHANNEL_DISCRIPTION");
            mNotificationManager.createNotificationChannel(channel);
        }
        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(getApplicationContext(), "YOUR_CHANNEL_ID")
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
}

//        final LoginActivity storedThis = this;

//        RequestQueue queue = Volley.newRequestQueue(storedThis);
//        String url = "http://10.0.2.2:8080/login";

//        Map<String, String> credentials = new HashMap<>();
//        credentials.put("name", nameInput.getText().toString());
//        credentials.put("password", passwordInput.getText().toString());

//        // Request a string response from the provided URL.
//        JsonObjectRequest stringRequest = new JsonObjectRequest(Request.Method.POST, url, new JSONObject(credentials),
//                new Response.Listener<JSONObject>() {
//                    @Override
//                    public void onResponse(JSONObject response) {
//                        System.out.println(response);
//                        OpenActivity();
//                    }
//                }, new Response.ErrorListener() {
//            @Override
//            public void onErrorResponse(VolleyError error) {
//                System.out.println("Login error");
//                feedbackText.setText("Invalid username or password");
//            }
//        });
//
//        // Add the request to the RequestQueue.
//        queue.add(stringRequest);

