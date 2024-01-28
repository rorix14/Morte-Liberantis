package game.Dev;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.MessageFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class DailyQuest extends AppCompatActivity {

    TextView currentDate;
    TextView dailyQuestText;
    ImageView questGiverImage;
    String date;
    String userId;
    int imageResourceId;
    String questGiverName;

    ServerService serverService;

    // Manage nearby locations
    String result;
    DownLoadTask task;
    ListView listView;
    ArrayAdapter<String> arrayAdapter;

    ArrayList<Double> locationLatitude = new ArrayList<>();
    ArrayList<Double> locationLongitude = new ArrayList<>();
    ArrayList<String> locationName = new ArrayList<>();
    ArrayList<String> vicinity = new ArrayList<>();
    ArrayList<String> formattedAddress = new ArrayList<>();

    // manage current user location
    LocationManager locationManager;
    LocationListener locationListener;
    TextView userLocationText;
    Double myLat;
    Double myLng;

    // count reaming time to end of day
    CountDownTimer countDownTimer;
    int hours;
    int minutes;
    int seconds;
    long remainingMilliseconds;
    long totalCurrentDaySeconds;

    public class DownLoadTask extends AsyncTask<String, Void, String> {

        @Override
        protected String doInBackground(String... urls) {

            String result = "";
            URL url;
            HttpURLConnection urlConnection = null;

            try {
                url = new URL(urls[0]);
                urlConnection = (HttpURLConnection) url.openConnection();
                InputStream in = urlConnection.getInputStream();
                InputStreamReader reader = new InputStreamReader(in);
                int data = reader.read();

                while (data != -1) {
                    char current = (char) data;
                    result += current;
                    data = reader.read();
                }
                System.out.println(result);
                return result;

            } catch (Exception e) {
                e.printStackTrace();
                return null;
            }
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_daily_quest);
        getSupportActionBar().hide();


        task = new DownLoadTask();
        listView = findViewById(R.id.myList);
        userLocationText = findViewById(R.id.userLocationText);
        currentDate = findViewById(R.id.currentDateText);
        dailyQuestText = findViewById(R.id.questText);
        questGiverImage = findViewById(R.id.questGiverImage);

        Intent receiveIntent = getIntent();
        userId = receiveIntent.getStringExtra("userId");
        date = receiveIntent.getStringExtra("date");
        //currentDate.setText(date);

        totalCurrentDaySeconds = calculateTotalCurrentDaySeconds();
        countDownTimer = new CountDownTimer(86400000, 1000) {
            @Override
            public void onTick(long millisUntilFinished) {
                remainingMilliseconds = (millisUntilFinished - (totalCurrentDaySeconds * 1000));
                currentDate.setText(MessageFormat.format("Time remaining: {0}", formatHHMMSS((remainingMilliseconds / 1000))));
            }

            @Override
            public void onFinish() {
                System.out.println("finished");
            }
        };

        serverService = new ServerService(this);

        serverService.getDailyQuest(userId, date, jsonObject -> processDailyQuestUI(dailyQuestText, questGiverImage, jsonObject),
                e -> System.out.println(e));

        locationManager = (LocationManager) this.getSystemService(Context.LOCATION_SERVICE);
        locationListener = new LocationListener() {
            @Override
            public void onLocationChanged(Location location) {
                UpdateLocationInfo(location);
            }

            @Override
            public void onStatusChanged(String provider, int status, Bundle extras) {

            }

            @Override
            public void onProviderEnabled(String provider) {

            }

            @Override
            public void onProviderDisabled(String provider) {

            }
        };

        // CHECK FOR USER PERMISSION
        if (android.os.Build.VERSION.SDK_INT < android.os.Build.VERSION_CODES.Q && ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        } else if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.Q && (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED || ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_BACKGROUND_LOCATION) != PackageManager.PERMISSION_GRANTED)) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_BACKGROUND_LOCATION}, 1);
        } else {

            Location lastKnownLocation;
            if (isEmulator()) {
                // USE FOR RUNNING IN THE EMULATOR
                System.out.println("isEmulator");
                locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000, 30, locationListener);
                lastKnownLocation = locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
            } else {
                // USE FOR RUNNING IN AN ACTUAL PHONE
                locationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 1000, 30, locationListener);
                lastKnownLocation = locationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
            }

            if (lastKnownLocation != null) {
                UpdateLocationInfo(lastKnownLocation);
                nearByPlaces(lastKnownLocation.getLatitude(), lastKnownLocation.getLongitude());
            }
        }
    }

    boolean isEmulator() {
        return Build.FINGERPRINT.startsWith("generic")
                || Build.FINGERPRINT.startsWith("unknown")
                || Build.MODEL.contains("google_sdk")
                || Build.MODEL.contains("Emulator")
                || Build.MODEL.contains("Android SDK built for x86")
                || Build.MANUFACTURER.contains("Genymotion")
                || (Build.BRAND.startsWith("generic") && Build.DEVICE.startsWith("generic"))
                || "google_sdk".equals(Build.PRODUCT);
    }

    private void nearByPlaces(double lat, double lng) {
        // Google Key 1: AIzaSyBGOyxIbgcd6-hJ-bDGpQbnOthDYGC2lII
        // Google Key 2: AIzaSyDmGQo-EYYNNyf3Gi4kG1_fp4OhEjcvFAE
        try {
            result = task.execute("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat + "," + lng + "&radius=200&type=restaurant&fields=geometry,name&key=AIzaSyBGOyxIbgcd6-hJ-bDGpQbnOthDYGC2lII" + "")
                    .get();

            JSONObject jsonObject = new JSONObject(result);
            JSONArray jsonArray = jsonObject.getJSONArray("results");
            JSONObject jsonPart;

            for (int i = 0; i < jsonArray.length(); i++) {
                jsonPart = jsonArray.getJSONObject(i);

                locationName.add(i, jsonPart.getString("name"));
                vicinity.add(i, jsonPart.getString("vicinity"));
                locationLatitude.add(i, jsonPart.getJSONObject("geometry").getJSONObject("location").getDouble("lat"));
                locationLongitude.add(i, jsonPart.getJSONObject("geometry").getJSONObject("location").getDouble("lng"));

                formattedAddress.add(i, "'" + locationName.get(i) + "'" + "\n" + vicinity.get(i));
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
        arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, formattedAddress);
        listView.setAdapter(arrayAdapter);

        listView.setOnItemClickListener((parent, view, position, id) -> {
            System.out.println("position:" + position);
            goToMaps(position);
        });
    }

    private void goToMaps(int i) {
        Intent intent = new Intent(this, MapsActivity.class);

        intent.putExtra("location name", locationName.get(i));
        intent.putExtra("location latitude", locationLatitude.get(i));
        intent.putExtra("location Longitude", locationLongitude.get(i));
        intent.putExtra("date", date);
        intent.putExtra("userId", userId);
        intent.putExtra("character image", imageResourceId);
        intent.putExtra("character name", questGiverName);

        startActivity(intent);
    }

    public void UpdateLocationInfo(Location location) {
        System.out.println("User location: " + location.toString());
        System.out.println("User Latitude: " + location.getLatitude());
        System.out.println("User Longitude: " + location.getLongitude());
        myLat = location.getLatitude();
        myLng = location.getLongitude();

        String add = "Could not find address :(";
        Geocoder geocoder = new Geocoder(this, Locale.getDefault());
        try {
            List<Address> addressList = geocoder.getFromLocation(location.getLatitude(), location.getLongitude(), 1);
            System.out.println("Stuff from user location: " + addressList);

            if (addressList != null && addressList.size() > 0) {
                add = "Current Address:\n";
                if (addressList.get(0).getThoroughfare() != null) {
                    add += addressList.get(0).getThoroughfare() + "\n";
                }
                if (addressList.get(0).getLocality() != null) {
                    add += addressList.get(0).getLocality() + " ";
                }
                if (addressList.get(0).getPostalCode() != null) {
                    add += addressList.get(0).getPostalCode() + " ";
                }
                if (addressList.get(0).getAdminArea() != null) {
                    add += addressList.get(0).getAdminArea();
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        //userLocationText.setText(add);
        // getNearPlaces();
    }

    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 1) {
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                StartListening();
            }
        }
    }

    public void StartListening() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {

            if (isEmulator()) {
                // USE FOR RUNNING IN THE EMULATOR
                locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000, 30, locationListener);
            } else {
                // USE FOR RUNNING IN AN ACTUAL PHONE
                locationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 0, 0, locationListener);
            }
        }
    }

    private void processDailyQuestUI(TextView dailyQuestText, ImageView questGiverImage, JSONObject jsonObject) {
        try {
            if (jsonObject.getInt("compleated") == 0) {

                dailyQuestText.setText(jsonObject.getString("description"));
                questGiverName = jsonObject.getString("name");
                imageResourceId = this.getResources().getIdentifier(jsonObject.getString("image"), "drawable",
                        this.getPackageName());
                questGiverImage.setImageResource(imageResourceId);
                countDownTimer.start();
            } else {
                dailyQuestText.setText("No more quests for today... \nWait for tomorrow!");
                countDownTimer.cancel();
                currentDate.setText(date);
            }

        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    public long calculateTotalCurrentDaySeconds() {
        Date today = Calendar.getInstance().getTime();//getting date
        SimpleDateFormat formattedToHours = new SimpleDateFormat("HH");//formatting according to my need
        SimpleDateFormat formattedToMinutes = new SimpleDateFormat("mm");
        SimpleDateFormat formattedToSeconds = new SimpleDateFormat("ss");

        hours = Integer.parseInt(formattedToHours.format(today));
        minutes = Integer.parseInt(formattedToMinutes.format(today));
        seconds = Integer.parseInt(formattedToSeconds.format(today));

        return (hours * 60 * 60) + (minutes * 60) + seconds;
    }

    public String formatHHMMSS(long secondsCount) {
        //Calculate the seconds to display:
        long seconds = (secondsCount % 60);
        secondsCount -= seconds;
        //Calculate the minutes:
        long minutesCount = secondsCount / 60;
        long minutes = minutesCount % 60;
        minutesCount -= minutes;
        //Calculate the hours:
        long hoursCount = minutesCount / 60;
        return "" + hoursCount + ":" + minutes + ":" + seconds;
    }

    @Override
    protected void onStop() {
        super.onStop();
        countDownTimer.cancel();
    }

    @Override
    protected void onRestart() {
        super.onRestart();

        totalCurrentDaySeconds = calculateTotalCurrentDaySeconds();

        serverService.getDailyQuest(userId, date, jsonObject -> processDailyQuestUI(dailyQuestText, questGiverImage, jsonObject),
                e -> System.out.println(e));
    }
}