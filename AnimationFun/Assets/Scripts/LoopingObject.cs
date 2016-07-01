using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TinyAnimation))]
public class LoopingObject : MonoBehaviour 
{

    private TinyAnimation anim;
    private bool looping = true;

    private void Start()
    {
        anim = GetComponent<TinyAnimation>();
        ProgrammaticallySetAnimation(anim);
        anim.Play(looping);

    }

    private void ProgrammaticallySetAnimation(TinyAnimation anim)
    {
        anim.AddKeyFrame(0, Vector3.zero, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(1f, Vector3.one * 1.3f, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(2.3f, Vector3.one * 3.75f, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(3f, Vector3.zero, TinyAnimation.TransformMode.POSITION);

        anim.AddKeyFrame(0, Vector3.zero, TinyAnimation.TransformMode.ROTATION);
        anim.AddKeyFrame(1f, Vector3.one * 50f, TinyAnimation.TransformMode.ROTATION);
        anim.AddKeyFrame(2.3f, Vector3.one * 130f, TinyAnimation.TransformMode.ROTATION);
        anim.AddKeyFrame(3f, Vector3.zero, TinyAnimation.TransformMode.ROTATION);

        anim.AddKeyFrame(3f, Vector3.one, TinyAnimation.TransformMode.SCALE);
        anim.AddKeyFrame(3.5f, Vector3.one * 3f, TinyAnimation.TransformMode.SCALE);
        anim.AddKeyFrame(4f, Vector3.one, TinyAnimation.TransformMode.SCALE);
    }
}
