using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dialogue;

public class NPC : Interactable
{
    private DialogueManager dialogueManager;
    
    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public override void Interact()
    {
        dialogueManager.StartDialogue(Conversation());
    }

    private DialogueSection Conversation()
    {
        string localName = "FireKeeper";

        Monologue yes = new Monologue(localName, "Good choice. Rest your legs a bit.");
        Monologue no = new Monologue(localName, "Just a heads-up, there's a big hole ahead on the path. Tread carefully around it.");

        Choices b = new Choices(localName, "Care to share the warmth of the fire?", ChoiceList(Choice("Yes, thank you. It's been a long journey.", yes), Choice("No, Do you know what lies ahead?", no)));
        Monologue a = new Monologue(localName, "Evening, traveler. These woods get cold, don't they?", b);

        return a;
    }
}
