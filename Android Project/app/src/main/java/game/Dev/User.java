package game.Dev;

import java.io.Serializable;

public class User implements Serializable {

    private String name;
    protected String id;

    public User(String id) {
        this.id = id;
    }
}
