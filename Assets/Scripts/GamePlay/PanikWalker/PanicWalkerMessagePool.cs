using UnityEngine;

public class PanicWalkerMessagePool : MonoBehaviour {
    [Header("Hit Reactions")]
    [SerializeField]
    private string[] hitMessages = {
        "Ouch!", "Why me?", "Hey!", "Ow!", "Not again!", "Bruh!", "Yikes!"
    };

    [Header("Throw Reactions")]
    [SerializeField]
    private string[] throwMessages = {
        "Take this!", "Catch!", "Heads up!", "Surprise!", "Yeet!"
    };

    [Header("Death Reactions")]
    [SerializeField]
    private string[] deathMessages = {
        "I'm out!", "Tell my story!", "Noooo!", "I'm melting...", "Goodbye, cruel world!", "Blip!"
    };

    [Header("Intro Messages")]
    [SerializeField]
    private string[] introMessages = {
    "Happy hunting – bring back some heads!",
    "Don’t slip on the guts.",
    "Paint the walls red.",
    "No mercy. No survivors.",
    "Make it rain… with intestines.",
    "Shoot first. Reload never.",
    "Time to mop the floor with brain matter.",
    "They scream. You score.",
    "Slice ‘em thin!",
    "Their blood is your paint.",
    "Aim for the squishy bits.",
    "Wipe the floor with their organs.",
    "One shot – one splatter.",
    "Their death, your upgrade.",
    "Turn the battlefield into a butcher shop.",
    "Send ‘em back in pieces.",
    "Don’t stop ‘til the guts hit the ground.",
    "Every corpse is progress.",
    "More meat for the grinder.",
    "The screams mean you're doing it right."
};

    [Header("narrative Death Messages")]
    [SerializeField]
    private string[] narrativeDeathMessages = {
    "{name} exploded in a shower of pixelated gore.",
    "{name} generously redecorated the island with blood.",
    "{name} finally found peace... in pieces.",
    "{name} was processed into digital soup.",
    "{name} set their last bit.",
    "{name} vanished in a puff of teeth and smoke.",
    "{name} became one with the floor.",
    "{name} was erased without backup.",
    "{name} ragequit... permanently.",
    "{name} turned into abstract art.",
    "{name} got uploaded straight to hell.",
    "{name} has been successfully recycled.",
    "{name} decorated the walls with regret.",
    "{name} went splat – artistically.",
    "{name} now exists only in patch notes.",
    "{name} tripped over a bullet and died.",
    "{name} mistook their spleen for ammo.",
    "{name} lost an argument with physics.",
    "{name} found out gravity still works.",
    "{name} died the way they lived – poorly."
};

    [Header("Game Over Messages")]
    [SerializeField]
    private string[] top10Messages = {
    "Top 10? The galaxy trembles at your name.",
    "A legend among bounty hunters. Respect.",
    "Snorbles' extinction is your legacy.",
    "Your trigger finger wrote history.",
    "They'll sing songs of your carnage."
};

    [SerializeField]
    private string[] rank11to50Messages = {
    "A respectable score. The Snorbles won't forget.",
    "You've made your mark, hunter.",
    "Not bad. Not legendary, but not bad.",
    "The galaxy acknowledges your efforts.",
    "You've earned your credits today."
};

    [SerializeField]
    private string[] rank51PlusMessages = {
    "Every Snorble counts... or counted.",
    "A humble start to a notorious career.",
    "Keep hunting. Glory awaits.",
    "The path to infamy is paved with Snorbles.",
    "Not the worst. Not the best. Yet."
};

    [SerializeField]
    private string[] noRankMessages = {
    "The Snorbles died in vain.",
    "No rank. No glory. Just carnage.",
    "Your efforts were... unnoticed.",
    "The galaxy remains unimpressed.",
    "Try harder. The Snorbles demand it."
};

    public string GetRandomHitMessage() {
        return hitMessages[Random.Range(0, hitMessages.Length)];
    }

    public string GetRandomThrowMessage() {
        return throwMessages[Random.Range(0, throwMessages.Length)];
    }

    public string GetRandomDeathMessage() {
        return deathMessages[Random.Range(0, deathMessages.Length)];
    }

    public string GetRandomIntroMessage() {
        return introMessages[Random.Range(0, introMessages.Length)];
    }

    public string GetRandomNarrativeDeathMessage(string walkerName) {
        string raw = narrativeDeathMessages[Random.Range(0, narrativeDeathMessages.Length)];
        return raw.Replace("{name}", walkerName);
    }

    public string GetTop10Message() {
        return top10Messages[Random.Range(0, top10Messages.Length)];
    }

    public string GetRank11to50Message() {
        return rank11to50Messages[Random.Range(0, rank11to50Messages.Length)];
    }

    public string GetRank51PlusMessage() {
        return rank51PlusMessages[Random.Range(0, rank51PlusMessages.Length)];
    }

    public string GetNoRankMessage() {
        return noRankMessages[Random.Range(0, noRankMessages.Length)];
    }
}