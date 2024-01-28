package game.Dev;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.PersistableBundle;

public class TestActivity extends AppCompatActivity {

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState, @Nullable PersistableBundle persistentState) {
        super.onCreate(savedInstanceState, persistentState);
        System.out.println("thisisanothertest");
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        System.out.println("thisisanothertest");
        super.onCreate(savedInstanceState);
//        setContentView(R.layout.activity_test);
    }
}
