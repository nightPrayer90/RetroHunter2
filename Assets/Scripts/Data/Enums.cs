

namespace RetroHunter2 {
    public enum SoundIndexKey {
        MouseHover,
        MouseKlick,
        MouseNo,
        playerShoot,
        playerReload,
        playerAmmoEmpty,
        multiplierUp,
        multiplierFail,
        levelUp,
        upgradeGet,
        doublePointsGet,
        openShopSound, 
        enemy01Die,
        enemyEscapeDie,
        none, 
        albertHit,
        albertDie,
        kelvinHit,
        kelvinDie,
        getScoreGoal,
        kelvinSelect,
        albertSelect,
        kelvinHover,
        albertHover,
        islandToggle,
        KelvinShooting,
        albertCookieThrow,
        cookieCrumble,
        cookieExplosion1,
        cookieExplosion2,
        yellowCookieCrumble,
        blueCookieCrumble,
        bulletEjectSound,
        openSnorblesUI,
        bigEnemyExplosion,
        playerShootWithNoAmmo,
        pointerExitBtn,
        pointerKlick,
        paperSound,
        pinSound
    }

    public enum GameState {
        play,
        upgradeScreen,
        gameOver, 
        cinematic,
        snorbleScreen,
        breakHud
    }

    public enum UpgradeType {
        ReloadSpeed,
        MaxAmmo,
        ComboFill,
        NoAmmoChance,
        ComboDrain,
        WaveTime,
        EnemySpawnRate,
        ScoreBonus,
        DoubleScoreChance,
        DoubleExpChance
    }

    public enum SceneKey {
        MainMenu,
        Game,
        StoryWindow
    }

    public enum SnorbleType {
        Kelvin,
        Albert
    }

    public enum CookieType {
        Red,
        Blue,
        Yellow
    }

    public enum EnemyType {
        Enemy_01,
        Enemy_02,
        Enemy_03
    }

    public enum MainMenuState {
        Menu,
        Credits
    }
}


