package game.Dev;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.FragmentActivity;

import android.Manifest;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.util.Log;
import android.view.View;

import com.google.android.gms.common.internal.Constants;
import com.google.android.gms.location.Geofence;
import com.google.android.gms.location.Geofence.Builder;
import com.google.android.gms.location.GeofencingClient;
import com.google.android.gms.location.GeofencingRequest;
import com.google.android.gms.location.LocationServices;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.BitmapDescriptorFactory;
import com.google.android.gms.maps.model.CircleOptions;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.material.snackbar.Snackbar;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class MapsActivity extends FragmentActivity implements OnMapReadyCallback {

    private GoogleMap mMap;
    Marker userMarker;
    LocationManager locationManager;
    LocationListener locationListener;
    double locationLat;
    double locationLng;
    GeofencingClient locationGeofence;
    //ArrayList<Geofence> geofence;
    Geofence geofence;
    String userId;
    String date;
    int imageResourceId;
    String questGiverName;
    String placeName;
    View view;
    DailyQuest dailyQuest;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_maps);
        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);

        locationGeofence = LocationServices.getGeofencingClient(this);

        dailyQuest = new DailyQuest();

        Intent receiveIntent = getIntent();
        userId = receiveIntent.getStringExtra("userId");
        date = receiveIntent.getStringExtra("date");
        imageResourceId = receiveIntent.getIntExtra("character image", 0);
        questGiverName = receiveIntent.getStringExtra("character name");
        placeName = receiveIntent.getStringExtra("location name");
    }

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    @Override
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap;
        //mMap.setMapType(GoogleMap.MAP_TYPE_HYBRID);
        Intent receiveIntent = getIntent();

        locationLat = receiveIntent.getDoubleExtra("location latitude", 0);
        locationLng = receiveIntent.getDoubleExtra("location Longitude", 0);
        LatLng chosenPlace = new LatLng(locationLat, locationLng);
        mMap.addMarker(new MarkerOptions().position(chosenPlace).title("Destination"));

        locationManager = (LocationManager) this.getSystemService(Context.LOCATION_SERVICE);
        locationListener = new LocationListener() {
            @Override
            public void onLocationChanged(Location location) {
                Log.i("User location", location.toString());
                userMarker.remove();
                LatLng userLocation = new LatLng(location.getLatitude(), location.getLongitude());
                userMarker = mMap.addMarker(new MarkerOptions().position(userLocation).title("You Location").icon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_AZURE)));
                mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(userLocation, 15));
                float[] results = new float[1];
                Location.distanceBetween(userLocation.latitude, userLocation.longitude, locationLat, locationLng, results);

                view = findViewById(R.id.map);
                Snackbar.make(view, ((int) results[0]) + " meters to " + placeName, Snackbar.LENGTH_INDEFINITE)
                        .setAction("Close", v -> {
                        })
                        .setAnimationMode(Snackbar.ANIMATION_MODE_SLIDE)
                        .setActionTextColor(getResources().getColor(R.color.colorPrimary))
                        .show();
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

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            //ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION}, 1);
            // } else {
            Location lastKnownLocation;
            if (dailyQuest.isEmulator()) {
                // USE FOR RUNNING IN THE EMULATOR
                locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000, 30, locationListener);
                lastKnownLocation = locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
            } else {
                // USE FOR RUNNING IN AN ACTUAL PHONE
                locationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 1000, 30, locationListener);
                lastKnownLocation = locationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
            }

            if (lastKnownLocation != null) {
                LatLng userLocation = new LatLng(lastKnownLocation.getLatitude(), lastKnownLocation.getLongitude());
                userMarker = mMap.addMarker(new MarkerOptions().position(userLocation).title("You Location").icon(BitmapDescriptorFactory.defaultMarker(BitmapDescriptorFactory.HUE_AZURE)));
                mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(userLocation, 15));
            }
        }
        geofence = buildGeofence();

        locationGeofence.addGeofences(getGeofencingRequest(), getGeofencePendingIntent())
                .addOnSuccessListener(this, aVoid -> {
                    // Geofence added
                    System.out.println("Geofence Success");

                })
                .addOnFailureListener(this, e -> {
                    // Failed to add geofences
                    System.out.println("Geofence Failure " + getGeofencingRequest().getGeofences().get(0));
                });

    }

    private Geofence buildGeofence() {
        System.out.println("test geofence");
        int radius = 50;

        CircleOptions circleOptions = new CircleOptions()
                .center(new LatLng(locationLat, locationLng))
                .radius(radius)
                .fillColor(0x40ff0000)
                .strokeColor(Color.BLACK)
                .strokeWidth(10);

        mMap.addCircle(circleOptions);

        return (new Builder()).setRequestId("location Geofence")
                .setCircularRegion(locationLat, locationLng, (float) radius)
                .setTransitionTypes(Geofence.GEOFENCE_TRANSITION_ENTER |
                        Geofence.GEOFENCE_TRANSITION_EXIT)
                .setExpirationDuration(Geofence.NEVER_EXPIRE).build();
    }

    private GeofencingRequest getGeofencingRequest() {

        GeofencingRequest.Builder builder = new GeofencingRequest.Builder();
        builder.setInitialTrigger(GeofencingRequest.INITIAL_TRIGGER_ENTER);
        builder.addGeofence(geofence);
        return builder.build();
    }

    private PendingIntent getGeofencePendingIntent() {
        // Reuse the PendingIntent if we already have it.
        PendingIntent geofencePendingIntent = null;
        if (geofencePendingIntent != null) {
            return geofencePendingIntent;
        }
        Intent intent = new Intent(this, GeofenceBroadcastReceiver.class);
        intent.putExtra("userId", userId);
        intent.putExtra("date", date);
        intent.putExtra("character image", imageResourceId);
        intent.putExtra("character name", questGiverName);
        // We use FLAG_UPDATE_CURRENT so that we get the same pending intent back when
        // calling addGeofences() and removeGeofences().
        geofencePendingIntent = PendingIntent.getBroadcast(this, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        return geofencePendingIntent;
    }

    @Override
    protected void onStop() {
        super.onStop();
        finish();
    }
}
