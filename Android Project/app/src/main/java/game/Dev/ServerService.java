package game.Dev;

import android.content.Context;
import android.os.Message;

import androidx.arch.core.util.Function;

import com.android.volley.DefaultRetryPolicy;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.JsonRequest;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;
import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.function.Consumer;

public class ServerService {

    //private String host = "http://10.72.99.22:8080";
    private String host = "http://10.0.2.2:8080";
    private Context context;

    public ServerService(Context context) {
        this.context = context;
    }

    public void register(String name, String password, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        postToEndpoint("register", name, password, onSuccess, onFailure);
    }

    public void login(String name, String password, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        postToEndpoint("login", name, password, onSuccess, onFailure);
    }

    public void getUserCharacters(String userId, final Consumer<JSONArray> onSuccess, final Consumer<Exception> onFailure) {
        doGetArray(MessageFormat.format("/user/{0}/characters", userId), onSuccess, onFailure);
    }

    public void getCharacterQuestion(String image, final Consumer<JSONArray> onSuccess, final Consumer<Exception> onFailure) {
        doGetArray(MessageFormat.format("/characters/{0}/text_adventure_message", image), onSuccess, onFailure);
    }

    public void getTextWithAnwsers(String identifier, final Consumer<JSONArray> onSuccess, final Consumer<Exception> onFailure) {
        doGetArray(MessageFormat.format("/text_adventure_response/{0}", identifier), onSuccess, onFailure);
    }

    public void getLastMessageText(String identifier, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        doGet(MessageFormat.format("/last_text/{0}", identifier), onSuccess, onFailure);
    }

    public void getTextStatos(int userId, int characterId, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        doGet(MessageFormat.format("/user/{0}/characters/{1}", userId, characterId), onSuccess, onFailure);
    }

    public void getCharactersTextAdventureStatus(String identifier, final Consumer<JSONArray> onSuccess, final Consumer<Exception> onFailure) {
        doGetArray(MessageFormat.format("/user/{0}/user_characters", identifier), onSuccess, onFailure);
    }

    public void getTextReward(String textIdentifier, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        doGet(MessageFormat.format("/text_adventure_message/{0}/text_adventure_reward", textIdentifier), onSuccess, onFailure);
    }

    public void patchCharacterTextAdventure(int userId, int characterId, String identifier, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        Map<String, String> credentials = new HashMap<>();
        credentials.put("characterId", Integer.toString(characterId));
        credentials.put("identifier", identifier);

        doPatch(MessageFormat.format("/user/{0}/characters", userId), credentials, onSuccess, onFailure);
    }

    public void getCharacterRewards(String[] imageList, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        Map<String, String[]> credentials = new HashMap<>();
        credentials.put("images", imageList);
        doGetWhithBodyArray("/characters/text_adventure_reward", credentials, onSuccess, onFailure);
    }

    public void getDailyQuest(String userId, String currentDate, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        doGet(MessageFormat.format("/user/{0}/active_daily_quest/{1}", userId, currentDate), onSuccess, onFailure);
    }

    public void updateDailyQuest(String userId, String currentDate, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        Map<String, String> credentials = new HashMap<>();
        credentials.put("date", currentDate);
        doPatch(MessageFormat.format("/user/{0}/active_daily_quest", userId), credentials, onSuccess, onFailure);
    }

    private void postToEndpoint(String endpoint, String name, String password, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        Map<String, String> credentials = new HashMap<>();
        credentials.put("name", name);
        credentials.put("password", password);

        doPost("/" + endpoint, credentials, onSuccess, onFailure);
    }

    private void doPatch(String endpoint, Map<String, String> body, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {
        RequestQueue queue = Volley.newRequestQueue(context);

        JsonObjectRequest request = new JsonObjectRequest(Request.Method.PATCH, host + endpoint, new JSONObject(body),
                response -> {
                    System.out.println(response);
                    onSuccess.accept(response);
                }, error -> {
            System.out.println("Request failure");
            onFailure.accept(error);
        });

        setRetryPolicy(request);

        // Add the request to the RequestQueue.
        queue.add(request);
    }

    private void doGetArray(String endpoint, final Consumer<JSONArray> onSuccess, final Consumer<Exception> onFailure) {

        RequestQueue queue = Volley.newRequestQueue(context);

        // Request a string response from the provided URL.
        JsonArrayRequest request = new JsonArrayRequest(Request.Method.GET, host + endpoint, null,
                response -> {
                    System.out.println(MessageFormat.format("Request succeed with {0}", response));
                    onSuccess.accept(response);
                }, error -> {
            System.out.println(MessageFormat.format("Request failure with {0}", error));
            onFailure.accept(error);
        });

        setRetryPolicy(request);

        // Add the request to the RequestQueue.
        queue.add(request);
    }

    private void doGet(String endpoint, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {

        RequestQueue queue = Volley.newRequestQueue(context);

//        StringRequest request = new StringRequest(Request.Method.GET, host + endpoint,
//                response -> {
//                    System.out.println(MessageFormat.format("Request succeed with {0}", response));
//                    onSuccess.accept(parse(response));
//                }, error -> {
//            System.out.println(MessageFormat.format("Request failure with {0}", error));
//            onFailure.accept(error);
//        });
//
//        // Add the request to the RequestQueue.
//        queue.add(request);

//        // Request a string response from the provided URL.
        JsonObjectRequest request = new JsonObjectRequest(Request.Method.GET, host + endpoint, null,
                response -> {
                    System.out.println(MessageFormat.format("Request succeed with {0}", response));
                    onSuccess.accept(response);
                }, error -> {
            System.out.println(MessageFormat.format("Request failure with {0}", error));
            onFailure.accept(error);
        });

        setRetryPolicy(request);

        // Add the request to the RequestQueue.
        queue.add(request);
    }

    private void doPost(String endpoint, Map<String, String> body, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {

        RequestQueue queue = Volley.newRequestQueue(context);

//        StringRequest request = new StringRequest(Request.Method.POST, host + endpoint,
//                response -> {
//                    System.out.println(MessageFormat.format("Request succeed with {0}", response));
//                    onSuccess.accept(response);
//                }, error -> {
//                    System.out.println(MessageFormat.format("Request failure with {0}", error));
//                    onFailure.accept(error);
//                }){
//            @Override
//            protected Map<String,String> getParams(){
//                return body;
//            }
//        };

        // Request a string response from the provided URL.
        JsonObjectRequest request = new JsonObjectRequest(Request.Method.POST, host + endpoint, new JSONObject(body),
                response -> {
                    System.out.println(response);
                    onSuccess.accept(response);
                }, error -> {
            System.out.println("Request failure");
            onFailure.accept(error);
        });

        setRetryPolicy(request);

        // Add the request to the RequestQueue.
        queue.add(request);
    }

    private void doGetWhithBodyArray(String endpoint, Map<String, String[]> body, final Consumer<JSONObject> onSuccess, final Consumer<Exception> onFailure) {

        RequestQueue queue = Volley.newRequestQueue(context);

        // Request a string response from the provided URL.
        JsonObjectRequest request = new JsonObjectRequest(Request.Method.POST, host + endpoint, new JSONObject(body),
                response -> {
                    System.out.println(response);
                    onSuccess.accept(response);
                }, error -> {
            System.out.println("Request failure");
            onFailure.accept(error);
        });

        setRetryPolicy(request);

        // Add the request to the RequestQueue.
        queue.add(request);
    }

    private <T> void setRetryPolicy(JsonRequest<T> request) {
        request.setRetryPolicy(new DefaultRetryPolicy(
                100,
                DefaultRetryPolicy.DEFAULT_MAX_RETRIES,
                DefaultRetryPolicy.DEFAULT_BACKOFF_MULT));
    }

}
//    private <T> T parse(String json) {
//        ObjectMapper mapper = new ObjectMapper();
//        try {
//            return mapper.readValue(json, new TypeReference<T>() {});
//        } catch (IOException e) {
//            e.printStackTrace();
//        }
//        return null;
//    }