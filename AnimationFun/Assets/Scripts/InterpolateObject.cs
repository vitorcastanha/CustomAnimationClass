using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TinyAnimation))]
public class InterpolateObject : MonoBehaviour {

    private TinyAnimation anim;
    public TinyAnimation.InterpolationMode mode;

    private void Start()
    {
        anim = GetComponent<TinyAnimation>();
        ProgrammaticallySetAnimation(anim);
        anim.SetInterpolationMode(mode);
        anim.Play(true);

    }

    private void ProgrammaticallySetAnimation(TinyAnimation anim)
    {
        anim.AddKeyFrame(0, Vector2.one, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(2f, Vector2.one * 5f, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(4f, Vector2.one * 10f, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(5f, Vector2.one * 2f, TinyAnimation.TransformMode.POSITION);
        anim.AddKeyFrame(8f, Vector2.one, TinyAnimation.TransformMode.POSITION);
    }
}
