using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubik.XR;
using Ubik.Messaging;
using Ubik.Samples;

public class NewFireworks : MonoBehaviour, IGraspable, IUseable, INetworkComponent, IFirework
{
    private Hand attached;

    public bool fired;
    private ParticleSystem particles;

    private float colourTimer;
    private Color current;
    private Color next;

    private NetworkContext context;
    private bool owner;

    [System.Serializable]
    public struct Message
    {
        public Color colour;
        public bool lit;
        public TransformMessage transformMsg;
    }

    public void Grasp(Hand controller)
    {
        attached = controller;
    }

    public void Release(Hand controller)
    {
        attached = null;
    }

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (attached != null)
        {
            transform.position = attached.transform.position;
            transform.rotation = attached.transform.rotation;
        }
        if (fired)
        {
            if (!particles.isPlaying)
            {
                particles.Play();
            }
        }

        if (Time.time > colourTimer)
        {
            colourTimer = Time.time + 1f;
            current = next;
            next = Random.ColorHSV();
        }

        // Guard: only owner can set color
        if (owner)
        {
            particles.startColor = Color.Lerp(current, next, (Time.time - colourTimer));
        }



        // Only owner send color information
        if (owner)
        {
            // Need to send multiple variables in one message
            context.SendJson<Message>(new Message() { 
                colour = particles.startColor,
                lit = fired,
                transformMsg = new TransformMessage(transform)
                
            }
            );
        }
        
    }

    public void Use(Hand controller)
    {
        fired = true;
    }

    public void UnUse(Hand controller)
    {
        
    }

    // Read colour message.
    // If you are not owner, you will override your local objects to match the online object.
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        particles.startColor = msg.colour;
        fired = msg.lit;
        transform.localPosition = msg.transformMsg.position;
        transform.localRotation = msg.transformMsg.rotation;
    }

    // Determine if its the owner
    public void Attach(Hand hand)
    {
        owner = true;
    }
}
