using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TinyAnimation))]
public class SampleObject : MonoBehaviour 
{

    private TinyAnimation anim;
    private bool looping = true;

    [Header ("Select a time stamp with the game playing")] [Range (0, 3f)] public float sampleTime;

    private void Start()
    {
        anim = GetComponent<TinyAnimation>();
        ProgrammaticallySetAnimation(anim);
        anim.Play(looping);
    }
        
    private void Update()
    {
        anim.Sample(sampleTime);
        anim.StopAnimation();
    }

    private void ProgrammaticallySetAnimation(TinyAnimation anim)
    {
        anim.AddKeyFrame(0, Vector3.zero, TinyAnimation.TransformMode.ROTATION);
        anim.AddKeyFrame(1f, Vector3.one * 50f, TinyAnimation.TransformMode.ROTATION);
        anim.AddKeyFrame(2.3f, Vector3.one * 130f, TinyAnimation.TransformMode.ROTATION);
        anim.AddKeyFrame(3.5f, Vector3.zero, TinyAnimation.TransformMode.ROTATION);

        //add and remove frame
        anim.AddKeyFrame(4f, Vector3.zero, TinyAnimation.TransformMode.ROTATION);
        anim.RemoveKeyFrame(anim.GetCurveCount(TinyAnimation.TransformMode.ROTATION) - 1, TinyAnimation.TransformMode.ROTATION);

        anim.AddKeyFrame(0f, Vector3.one, TinyAnimation.TransformMode.SCALE);
        anim.AddKeyFrame(2f, Vector3.one * 3f, TinyAnimation.TransformMode.SCALE);
        anim.AddKeyFrame(3f, Vector3.one, TinyAnimation.TransformMode.SCALE);

        Debug.Log(this.name + " scale channel animation duration: " + anim.GetCurveDuration(TinyAnimation.TransformMode.SCALE));
        Debug.Log(this.name + " total animation duration: " + anim.GetAnimationDuration());
    }
}
