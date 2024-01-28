package game.Dev;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.util.Log;
import android.widget.Toast;

import androidx.core.app.NotificationCompat;

import com.google.android.gms.location.Geofence;
import com.google.android.gms.location.GeofencingEvent;

import java.util.List;

import static android.content.ContentValues.TAG;
import static android.provider.Settings.System.getString;

public class GeofenceBroadcastReceiver extends BroadcastReceiver {

    @Override
    public void onReceive(Context context, Intent intent) {

        GeofencingEvent geofencingEvent = GeofencingEvent.fromIntent(intent);
        if (geofencingEvent.hasError()) {

            System.out.println(" Gefence error: " + geofencingEvent.hasError());
            return;
        }

        String userId = intent.getStringExtra("userId");
        String date = intent.getStringExtra("date");
        System.out.println("userId: " + userId + " ,date: " + date);

        ServerService serverService = new ServerService(context);
        // Get the transition type.
        int geofenceTransition = geofencingEvent.getGeofenceTransition();

        // Test that the reported transition was of interest.
        if (geofenceTransition == Geofence.GEOFENCE_TRANSITION_ENTER ||
                geofenceTransition == Geofence.GEOFENCE_TRANSITION_EXIT) {

            serverService.updateDailyQuest(userId, date, jsonObject -> System.out.println("result: " + jsonObject), System.out::println);
            // Get the geofences that were triggered. A single event can trigger
            // multiple geofences.
            List<Geofence> triggeringGeofences = geofencingEvent.getTriggeringGeofences();

            // Get the transition details as a String.
            String geofenceTransitionDetails = getGeofenceTransitionDetails(
                    this,
                    geofenceTransition,
                    triggeringGeofences
            );

           /* Toast toast = Toast.makeText(context, "Quest Complected", Toast.LENGTH_SHORT);
            toast.show();*/
            // Send notification and log the transition details.
            System.out.println("Transition details: " + geofenceTransitionDetails);
            showNotification(intent.getStringExtra("character name") + ":", "Thank you Antero!", context, intent.getIntExtra("character image", 0));
        } else {
            // Log the error.
            System.out.println("Transition Error: " + geofenceTransition);
        }
    }

    private String getGeofenceTransitionDetails(GeofenceBroadcastReceiver geofenceBroadcastReceiver, int geofenceTransition, List<Geofence> triggeringGeofences) {
        return "1: " + geofenceBroadcastReceiver + ",2: " + geofenceTransition + ",3: " + triggeringGeofences;
    }

    void showNotification(String title, String message, Context context, int imageResource) {
        NotificationManager mNotificationManager =
                (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel("YOUR_CHANNEL_ID",
                    "YOUR_CHANNEL_NAME",
                    NotificationManager.IMPORTANCE_DEFAULT);
            channel.setLockscreenVisibility(Notification.VISIBILITY_PUBLIC);
            channel.setDescription("YOUR_NOTIFICATION_CHANNEL_DISCRIPTION");
            mNotificationManager.createNotificationChannel(channel);
        }

        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(context, "YOUR_CHANNEL_ID")
                .setSmallIcon(R.drawable.ml_icon) // notification icon
                .setLargeIcon(BitmapFactory.decodeResource(context.getResources(), imageResource))// set image
                .setContentTitle(title) // title for notification
                .setContentText(message)// message for notification
                .setAutoCancel(true); // clear notification after click
        Intent intent = new Intent(context, DailyQuest.class);
        PendingIntent pi = PendingIntent.getActivity(context, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        mBuilder.setContentIntent(pi);
        mNotificationManager.notify(0, mBuilder.build());
    }
}
